using System.Collections.Generic;
using UnityEngine;

public static class ChanceUtility
{
    public static T Roll<T>(List<(T item, int weight)> table)
    {
        int totalWeight = 0;

        for (int i = 0; i < table.Count; i++)
            totalWeight += table[i].weight;

        int roll = Random.Range(0, totalWeight);

        int cumulative = 0;

        for (int i = 0; i < table.Count; i++)
        {
            cumulative += table[i].weight;
            if (roll < cumulative)
                return table[i].item;
        }

        return default;
    }
}