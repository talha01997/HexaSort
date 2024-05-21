using System.Collections.Generic;
using UnityEngine;

public class UniqueSequenceGenerator : MonoBehaviour
{
    public static UniqueSequenceGenerator instance;
    public int minRange = 1;
    public int maxRange = 10;
    public int listLength = 10;
    public List<int> sequence;

    public List<int> GenerateUniqueSequence(int minRange, int maxRange, int listLength)
    {
        List<int> uniqueSequence = new List<int>();
        HashSet<int> usedNumbers = new HashSet<int>();
        int lastNumber = -1;
        int count = 0;

        while (uniqueSequence.Count < listLength)
        {
            int newNumber = Random.Range(minRange, maxRange);

            if (newNumber == lastNumber)
            {
                if (count < 2)
                {
                    uniqueSequence.Add(newNumber);
                    count++;
                }
            }
            else
            {
                if (!usedNumbers.Contains(newNumber))
                {
                    uniqueSequence.Add(newNumber);
                    usedNumbers.Add(newNumber);
                    lastNumber = newNumber;
                    count = 1;
                }
            }
        }
        sequence = uniqueSequence;
        return uniqueSequence;
    }

    // Example usage
    void Start()
    {
        if (!instance)
            instance = this;
        //int minRange = 1;
        //int maxRange = 10;
        //int listLength = 10;

        sequence = GenerateUniqueSequence(minRange, maxRange, listLength);

        //foreach (int number in sequence)
        //{
        //    Debug.Log(number);
        //}
    }
}