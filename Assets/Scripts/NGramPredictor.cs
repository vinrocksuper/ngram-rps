using RPS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class KeyDataRecord
{
    //Total count of all values we've ever seen for this key
    public int total;

    //A map of the value -> count of how many times we've seen this value with this key
    public Dictionary<RPSMove, int> counts = new Dictionary<RPSMove, int>()
    {
        { RPSMove.Rock, 0 },
        { RPSMove.Paper, 0 },
        { RPSMove.Scissors, 0 }
    };
}

public class NGramPredictor : MonoBehaviour
{
    public static Dictionary<List<RPSMove>, KeyDataRecord> data = new Dictionary<List<RPSMove>, KeyDataRecord>();

    // size of window +1
    public int nValue = 3;

    public void registerSequence(RPSMove[] actions)
    {
        List<RPSMove> key = new List<RPSMove>();

        RPSMove value = actions[nValue - 1];

        for (int i = 0; i < nValue-1; i++)
        { 
            key.Add(actions[i]);
        }

        if (!data.ContainsKey(key))
        {
            KeyDataRecord keyData = new KeyDataRecord();
            data.Add(key, keyData);
        }

        KeyDataRecord dataRecord = (KeyDataRecord)data[key];
        dataRecord.total++;
        if (!dataRecord.counts.ContainsKey(value))
        {
            dataRecord.counts.Add(value, 1);
        }
        else
        {
            dataRecord.counts[value]++;
        }
    }

    public RPSMove getMostLikely(RPSMove[] actions)
    {
        // Get the key from the actions array
        List<RPSMove> key = actions.Take(nValue - 1).ToList();
        bool found = false;
        KeyDataRecord keyData = null;
        foreach (KeyValuePair<List<RPSMove>,KeyDataRecord> pair in data)
        {
            found = Enumerable.SequenceEqual(key, pair.Key);

            if (found)
            {
                keyData = pair.Value;
                break;
            }
        }

        if (keyData == null || !found)
        {
            return RPSMove.Rock;
        }

        // Find the highest probability value
        int highestValue = 0;
        RPSMove bestAction = RPSMove.Rock; 

        foreach (KeyValuePair<RPSMove,int> actionCount in keyData.counts)
        {
            if (actionCount.Value > highestValue)
            {
                highestValue = actionCount.Value;
                bestAction = actionCount.Key;
            }
        }

        return bestAction;
    }
}
