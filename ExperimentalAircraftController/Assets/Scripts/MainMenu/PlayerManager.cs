using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject MainUI;
    public GameObject PauseMenu;
    public float Volume;
    public static bool Pause;

    private void Start()
    {
        LoadSettings();
    }

    private void Update()
    {
        ChangeSettings();

        if(Pause && Input.GetKeyDown(KeyCode.Escape))
        {
            Pause = false;
        }
        
        else if (!Pause && Input.GetKeyDown(KeyCode.Escape))
        {
            Pause = true;
        }

        if (Pause)
        {
            MainUI.SetActive(false);
            PauseMenu.SetActive(true);
            Time.timeScale = 0;
            AudioListener.volume = 0;
        }
        else
        {
            MainUI.SetActive(true);
            PauseMenu.SetActive(false);
            Time.timeScale = 1;
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        }
    }

    public void UnPause()
    {
        Pause = false;
    }

    public void LoadSettings()
    {
        Volume = PlayerPrefs.GetFloat("Volume");
    }

    public void ChangeSettings()
    {
        AudioListener.volume = Volume;
    }

    public void ReterunToMenu(string Menu)
    {
        Pause = false;
        SceneManager.LoadScene(Menu);
    }
}
