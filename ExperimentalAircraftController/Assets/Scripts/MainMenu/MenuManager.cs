using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject SettingsMenu;
    public GameObject AboutMenu;
    public Slider Volume;

    public void ChangeVolume()
    {
        AudioListener.volume = Volume.value;
    }

    private void Update()
    {
        ChangeVolume();
    }

    public void SaveMenuSettings()
    {
        //volume
        PlayerPrefs.SetFloat("Volume", Volume.value);
    }

    public void OpenSettings()
    {
        Volume.value = PlayerPrefs.GetFloat("Volume");
        SettingsMenu.SetActive(true);
        MainMenu.SetActive(false);
        AboutMenu.SetActive(false);  
    }

    public void OpenAbout()
    {
        AboutMenu.SetActive(true);
        SettingsMenu.SetActive(false);
        MainMenu.SetActive(false);
    }

    public void CloseAbout()
    {
        MainMenu.SetActive(true);
        AboutMenu.SetActive(false);
        SettingsMenu.SetActive(false);
    }

    public void CloseSettings()
    {
        MainMenu.SetActive(true);
        SettingsMenu.SetActive(false);
        AboutMenu.SetActive(false);
        SaveMenuSettings();
    }

    public void PlayGame(string LevelName)
    {
        SceneManager.LoadScene(LevelName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenChanel()
    {
        Application.OpenURL("https://www.youtube.com/channel/UCJiwQf6qD9UKvnjoqlUqG8Q");
    }

}
