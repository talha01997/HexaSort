using System.Collections.Generic;
using UnityEngine;

public class UniqueSequenceGenerator : MonoBehaviour
{
    public static UniqueSequenceGenerator instance;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

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

        return uniqueSequence;
    }

    // Example usage
    void Start()
    {
        int minRange = 1;
        int maxRange = 10;
        int listLength = 10;

        List<int> sequence = GenerateUniqueSequence(minRange, maxRange, listLength);

        foreach (int number in sequence)
        {
            Debug.Log(number);
        }
    }
}