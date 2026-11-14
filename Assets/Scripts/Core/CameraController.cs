using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Rig References")]
    [Tooltip("子オブジェクトのYawPivotを指定")]
    public Transform yawPivot;
    [Tooltip("孫オブジェクトのPitchPivotを指定")]
    public Transform pitchPivot;
    [Tooltip("ひ孫のMain Cameraを指定")]
    public Transform cameraTransform;

    [Header("Rotation")]
    public float rotateSpeed = 0.2f;
    private float yaw = 0f;
    private float pitch = 45f;
    public float minPitch = 20f;
    public float maxPitch = 150f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float minZoom = 1f;
    public float maxZoom = 40f;
    private float currentZoomDistance = 15; // 初期距離

    [Header("Pan")]
    public float panSpeed = 0.01f;

    [Header("Move")]
    public float moveSpeed = 15f;
    private Vector3 boardCenterPosition = Vector3.zero;

    private void Start()
    {
        // もしInspectorで未設定なら、自動で子階層から取得を試みる
        if (yawPivot == null) yawPivot = transform.Find("YawPivot");
        if (pitchPivot == null && yawPivot != null) pitchPivot = yawPivot.Find("PitchPivot");
        if (cameraTransform == null) cameraTransform = Camera.main?.transform; 
        // 【修正】Startでの位置合わせを消して、初期の回転・ズームの適用だけに絞る
        ApplyRigTransform();
    }
    private void Update()
    {
        HandleReset();
        RotateCamera();
        ZoomCamera();
        PanCamera();
        MoveCamera();
    }
    private void HandleReset()
    {
        // Unity 6のInput SystemでRキーが押された瞬間を検知
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            // 位置・角度・ズームを初期状態（ボードの中央基準）に戻す
            transform.position = boardCenterPosition; // ボードの中央 (0,0,0)
            yaw = 0f;
            pitch = 45f;
            currentZoomDistance = 15f;

            ApplyRigTransform();
        }
    }
    // 【追加】HexBoardから盤面完成時に呼び出される、初期位置確定メソッド
    public void SetToBoardCenter()
    {
        HexBoard board = FindFirstObjectByType<HexBoard>();
        if (board != null)
        {
            boardCenterPosition = board.GetBoardCenter();
        }
        else
        {
            boardCenterPosition = new Vector3(0f, 0f, 3.0f);
        }

        // カメラリグの位置を更新して適用
        transform.position = boardCenterPosition;
        ApplyRigTransform();
    }

    private void ApplyRigTransform()
    {
        if (yawPivot) yawPivot.localRotation = Quaternion.Euler(0f, yaw, 0f);
        if (pitchPivot) pitchPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        if (cameraTransform)
        {
            Vector3 localPos = cameraTransform.localPosition;
            localPos.z = -currentZoomDistance;
            cameraTransform.localPosition = localPos;
        }
    }
    private void RotateCamera()
    {
        if (yawPivot == null || pitchPivot == null) return;
        if (!Mouse.current.rightButton.isPressed) return;

        Vector2 mouseDelta =
            Mouse.current.delta.ReadValue();

        // 回転の計算
        yaw += mouseDelta.x * rotateSpeed;
        pitch -= mouseDelta.y * rotateSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 左右の回転はYawPivot（Y軸のみ）に適用
        yawPivot.localRotation = Quaternion.Euler(0f, yaw, 0f);
        // 上下の回転はPitchPivot（X軸のみ）に適用
        pitchPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void ZoomCamera()
    {
        if (cameraTransform == null || Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) < 0.01f) return;

        scroll = Mathf.Clamp(scroll, -1f, 1f);
        currentZoomDistance -= scroll * zoomSpeed;
        currentZoomDistance = Mathf.Clamp(currentZoomDistance, minZoom, maxZoom);

        Vector3 localPosition = cameraTransform.localPosition;
        localPosition.z = -currentZoomDistance; // 常にカメラの後ろ方向に引く
        cameraTransform.localPosition = localPosition;
    }

    private void PanCamera()
    {
        if (!Mouse.current.middleButton.isPressed) return;
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // 【ここがポイント】移動方向の基準を「YawPivot（カメラの水平の向き）」にする
        // これにより、カメラがどれだけ上下に傾いていても、画面の並行移動が狂わなくなる
        Vector3 right = yawPivot.right;
        Vector3 forward = yawPivot.forward;

        Vector3 move =
        (
            -right * mouseDelta.x -forward * mouseDelta.y
        ) * panSpeed;

        transform.position += move;
    }

    private void MoveCamera()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            input.y += 1;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1;

        if (input == Vector2.zero)
            return;

        // キーボード移動も「YawPivot」の水平な前方を基準にする
        Vector3 forward = yawPivot.forward;
        Vector3 right = yawPivot.right;
        forward.y = 0f;
        forward.Normalize();

        Vector3 move = (forward * input.y + right * input.x).normalized;

        // カメラ自身ではなく、中心点である「CameraRig（this）」の座標を動かす
        transform.position += move * moveSpeed * Time.deltaTime;
    }
}