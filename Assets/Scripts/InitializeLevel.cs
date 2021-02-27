using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeLevel : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    void Start()
    {
        try
        {
            var playerConfigs = PlayerSelectionManager.Instance.GetPlayerConfigurations().ToArray();
            for (int i = 0; i < playerConfigs.Length; i++)
            {
                var spawnPoint = transform.GetChild(i);
                var player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation, gameObject.transform);
                player.GetComponent<CharacterMovement>().InitializePlayer(playerConfigs[i]);
            }
        }
        catch (System.Exception)
        {
            Debug.LogWarning("No Player configuration could be found - launch game from main menu!");
            throw;
        }
        
    }
}
