using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Image Sound;
    public Sprite MaxSound;
    public Sprite MediumSound;
    public Sprite MinSound;
    public Sprite NoSound;
    public AudioMixer Mixer;
    public GameObject Options;
    public Animator OptionsAnimator;
    public GameObject FirstButton;
    public GameObject menu;
    public GameObject options;
    Resolution[] resolutions;
    public Dropdown resolutionDropdown;
    public GameObject OptionsButton;

    public void SetOptionsButton()
    {
        menu.GetComponent<Transform>().localScale = new Vector3(0, 0, 0);
        options.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        EventSystem.current.SetSelectedGameObject(FirstButton);
    }
    public void SetButton()
    {
        menu.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        options.GetComponent<Transform>().localScale = new Vector3(0, 0, 0);
        EventSystem.current.SetSelectedGameObject(OptionsButton);
    }
    private void Start()
    {
        options.GetComponent<Transform>().localScale = new Vector3(0, 0, 0);
        resolutions = Screen.resolutions;
        int currentResolution = 0;
        resolutionDropdown.ClearOptions();

        List<string> reso = new List<string>();
        string tmp;

        for (int i=0; i < resolutions.Length; i++)
        {
            tmp = resolutions[i].width + " x " + resolutions[i].height;
            reso.Add(tmp);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolution = i;
            }
        }

        resolutionDropdown.AddOptions(reso);
        resolutionDropdown.value = currentResolution;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetVolume(float volume)
    {
        Mixer.SetFloat("MainMenuVolume", volume);
        if (volume > -20.0)
            SetImage(MaxSound);
        else if (volume <= -20.0 && volume >= -60.0)
            SetImage(MediumSound);
        else if (volume == -80.0)
            SetImage(NoSound);
        else
            SetImage(MinSound);
    }

    private void SetImage(Sprite newImg)
    {
        Sound.sprite = newImg;
    }

    public void SetGraphics(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetFullscreen(bool isfullscreen)
    {
        Screen.fullScreen = isfullscreen;
    }

    public void SetResolution(int index)
    {
        Screen.SetResolution(resolutions[index].width,resolutions[index].height, Screen.fullScreen);
    }

}
