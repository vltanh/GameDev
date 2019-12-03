using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject menu;
    public GameObject help;
    public GameObject about;

    public void SwitchScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void About()
    {
        menu.SetActive(false);
        about.SetActive(true);
    }

    public void Help()
    {
        menu.SetActive(false);
        help.SetActive(true);
    }

    public void BackFromAbout()
    {
        menu.SetActive(true);
        about.SetActive(false);
    }

    public void BackFromHelp()
    {
        menu.SetActive(true);
        help.SetActive(false);
    }
}