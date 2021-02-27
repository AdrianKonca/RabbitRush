using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerSelectionManager : MonoBehaviour
{
    // Start is called before the first frame update
    private List<PlayerConfiguration> playerConfigs;
    [SerializeField]
    private int MaxPlayers = 4;
    private bool levelLoaded = false;
    public static PlayerSelectionManager Instance { get; private set; }

    void Update()
    {
        if (levelLoaded)
            GetComponent<PlayerInputManager>().splitScreen = true;
    }
    void Awake()
    {
        if (Instance != null)
            Debug.LogWarning("Trying to create another singleton!");
        Instance = this;
        DontDestroyOnLoad(Instance);
        playerConfigs = new List<PlayerConfiguration>();
    }

    public void SetPlayerColor(int index, Material material)
    {
        playerConfigs[index].PlayerMaterial = material;
    }
    public void SetPlayerReady(int index, bool value)
    {
        playerConfigs[index].IsReady = !playerConfigs[index].IsReady;
        AreAllPlayersReady();
    }
    public bool GetPlayerReady(int index)
    {
        return playerConfigs[index].IsReady;
    }
    public void AreAllPlayersReady()
    {
        if (playerConfigs.Count <= MaxPlayers && playerConfigs.All(p => p.IsReady))
            OnAllPlayerReady();
    }
    public void OnAllPlayerReady()
    {
        var piManager = GetComponent<PlayerInputManager>();
        //piManager.joiningEnabled = false;
        piManager.DisableJoining();
        SceneManager.LoadScene("Level1Coop");
        levelLoaded = true;
    }
    public void OnPlayerJoinLocal(PlayerInput input)
    {
        Debug.Log("Player joined " + input.playerIndex);
        Debug.Log(input);

        if (!playerConfigs.Any(p => p.PlayerIndex == input.playerIndex))
        {
            input.transform.SetParent(transform);
            playerConfigs.Add(new PlayerConfiguration(input));
        }
    }
    public void GetPlayerIndex(PlayerInput input)
    {
        //foreach (var config in playerConfigs)
        //{
        //    if config == input;
        //}
    }
    public List<PlayerConfiguration> GetPlayerConfigurations() => playerConfigs;
}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput input)
    {
        Input = input;
    }
    public int PlayerIndex { get; set; }
    public PlayerInput Input { get; set; }
    public bool IsReady { get; set; }
    public Material PlayerMaterial { get; set; }
}