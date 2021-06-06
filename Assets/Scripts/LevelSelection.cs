using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelection : MonoBehaviour
{
    public TextMeshProUGUI text;
    private Dictionary<string, string> _nameToLevelMap;
    private List<string> _names;
    private int _currentLevel;
    public Image img;
    public Sprite lvl1;
    public Sprite lvl2;
    public Sprite lvl3;


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
        img.sprite = lvl1;
        text.text = startLevelName;
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
        if (_names[_currentLevel] == "Canyon")
        {
            img.sprite = lvl2;
        }
        else if (_names[_currentLevel] == "River") 
        {
            img.sprite = lvl3;
        }
        else if (_names[_currentLevel] == "Riverway Old")
        {
            img.sprite = lvl1;
        }
        else if (_names[_currentLevel] == "Riverway")
        {
            img.sprite = lvl1;
        }

        text.text = _names[_currentLevel];

    }
    public string GetSelectedSceneName => _nameToLevelMap[_names[_currentLevel]];

}
