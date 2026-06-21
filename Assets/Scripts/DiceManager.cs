using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public int RollDice()
    {
        int dice1 = Random.Range(1, 7);
        int dice2 = Random.Range(1, 7);

        int total = dice1 + dice2;

        Debug.Log("Dice : " + total);

        return total;
    }
}