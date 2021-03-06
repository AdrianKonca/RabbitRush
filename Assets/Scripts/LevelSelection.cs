using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    public Text Text;
    private Dictionary<string, string> _nameToLevelMap;
    private List<string> _names;
    private int _currentLevel;

    private void Awake()
    {
        string startLevelName = "Riverway";
        _nameToLevelMap = new Dictionary<string, string> {
            { startLevelName , "Level1Coop" },
            { "Riverway Old", "Level1" },
            { "Canyon", "Level2" },
            { "River", "Level3" },
        };
        _names = _nameToLevelMap.Keys.ToList();
        _currentLevel = _names.IndexOf(startLevelName);
        Text.text = startLevelName;
    }

    public void NextLevel()
    {
        _currentLevel = (_currentLevel + 1) % _names.Count;
        ReloadMenu();
    }

    public void PreviousLevel()
    {
        _currentLevel = (_currentLevel - 1);
        if (_currentLevel < 0)
            _currentLevel = _names.Count - 1;
        ReloadMenu();
    }

    private void ReloadMenu()
    {
        //TODO: Load image;
        Text.text = _names[_currentLevel];

    }
    public string GetSelectedSceneName => _nameToLevelMap[_names[_currentLevel]];

}
