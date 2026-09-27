using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public static int LevelsBeaten = 0;
    [SerializeField] private GameObject ContinueButton;
    private GameObject ActiveScreen;
    [SerializeField] private GameObject[] Screens;
    [SerializeField] private GameObject[] LevelLocks;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(LevelsBeaten > 0)
        {
            ContinueButton.SetActive(true);
        }
        else
        {
            ContinueButton.SetActive(false);
        }
        ActiveScreen = Screens[0];
        for(int i = 0; i < Screens.Length; i++)
        {
            Screens[i].SetActive(false);
        }
        ActiveScreen.SetActive(true);
        for(int i = 0; i < LevelLocks.Length; i++)
        {
            if(LevelsBeaten > i)
            {
                LevelLocks[i].SetActive(false);
            }
            else
            {
                LevelLocks[i].SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Continue()
    {
        if(LevelsBeaten < 6)
        {
            LoadLevel(LevelsBeaten + 1);
        }
    }
    public void LoadLevel(int level)
    {
        if (level - 1 <= LevelsBeaten)
        {
            SceneManager.LoadScene(level);
        }
    }
    public void OpenMain()
    {
        ActiveScreen.SetActive(false);
        ActiveScreen = Screens[0];
        ActiveScreen.SetActive(true);
    }
    public void OpenLevelSelect()
    {
        ActiveScreen.SetActive(false);
        ActiveScreen = Screens[1];
        ActiveScreen.SetActive(true);
    }
    public void OpenCredits()
    {
        ActiveScreen.SetActive(false);
        ActiveScreen = Screens[2];
        ActiveScreen.SetActive(true);
    }
    public void OpenSettings()
    {
        ActiveScreen.SetActive(false);
        ActiveScreen = Screens[3];
        ActiveScreen.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
