using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem
{
    static private Dictionary<string, int> _basePoints = new Dictionary<string, int> { 
        { "Level1", 300 }, { "Level2", 600 } 
    };
    static private Dictionary<(int, int), string> _grades = new Dictionary<(int, int), string>() {
        {(80, int.MaxValue), "S"},
        {(65, 80), "A"},
        {(60, 65), "B"},
        {(55, 60), "C"},
        {(40, 55), "D"},
        {(0, 40), "E"},
        {(int.MinValue, 0), "F" }
    };

    static public string GetGrade(int playerCount, float time, int deathCount, string sceneName)
    {

        float points = playerCount * _basePoints[sceneName] - time - (10 * deathCount);
        foreach (var key in _grades.Keys)
        {
            if(points >= key.Item1 && points < key.Item2)
            {
                return _grades[key];
            }

        }
        return "NA";

    }

}
