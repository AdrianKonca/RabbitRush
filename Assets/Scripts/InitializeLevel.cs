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
            Destroy(GameObject.Find("TESTING_OBJECT"));
            var debugCharacter = GameObject.Find("CHARACTER_DEBUG");
            if (debugCharacter != null)
            {
                Debug.LogError("CHARACTER_DEBUG is active in this scene - please disable it!");
                debugCharacter.SetActive(false);
            }
        }
        catch (System.Exception)
        {
            Debug.LogWarning("No Player configuration could be found - launch game from main menu!");
        }
        
    }
}
