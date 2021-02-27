using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private AudioSource effectsSource;

    [SerializeField]
    public AudioClip winSound;
    [SerializeField]
    public AudioClip carrotPickupSound;
    [SerializeField]
    public AudioClip movementSound;
    [SerializeField]
    public AudioClip deathSound;

    public static AudioManager Instance;
    public enum Sounds
    {
        Win,
        Death,
        Movement,
        CarrotPickup
    }

    public class SoundDefinition
    {
        public AudioClip AudioClip;
        public float Volume;
    }
    public Dictionary<Sounds, SoundDefinition> soundMap;
    void Awake()
    {
        if (Instance != null)
            Debug.LogWarning("Trying to create another singleton!");
        Instance = this;
        soundMap = new Dictionary<Sounds, SoundDefinition>
        {
            { Sounds.Win, new SoundDefinition { AudioClip = winSound, Volume = 1f } },
            { Sounds.Death, new SoundDefinition { AudioClip = deathSound, Volume = 1f } },
            { Sounds.Movement, new SoundDefinition { AudioClip = movementSound, Volume = 0.8f } },
            { Sounds.CarrotPickup, new SoundDefinition { AudioClip = carrotPickupSound, Volume = 0.5f } },
        };
    }

    // Update is called once per frame
    public void PlaySound(Sounds sound)
    {
        effectsSource.PlayOneShot(soundMap[sound].AudioClip, soundMap[sound].Volume);
    }
}
