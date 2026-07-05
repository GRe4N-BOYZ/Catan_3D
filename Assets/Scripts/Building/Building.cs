using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public Player owner;
    public abstract int VictoryPoint
    {
        get;
    }
}
