using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour {

    public bool SoundEffects { get; private set; }
    public bool Music { get; private set; }

    public void PvP()
    {
        PlayerPrefs.SetString("Mode", "pvp");
        StartBattle();
    }

    public void PvCEasy()
    {
        PlayerPrefs.SetString("Mode", "pvcEasy");
        StartBattle();
    }

    public void PvCMedium()
    {
        PlayerPrefs.SetString("Mode", "pvcMedium");
        StartBattle();
    }

    public void PvCDeath()
    {
        PlayerPrefs.SetString("Mode", "pvcDeath");
        StartBattle();
    }

    public void ShowDifficulties()
    {
        transform.FindChild("PvC").FindChild("DifficultyMenu").gameObject.SetActive(true);
    }

    public void GotoControlls()
    {
        PlayerPrefs.SetInt("PreviousScene", SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene("Controlls");
    }

    public void ChangeSoundEffects()
    {
        if (SoundEffects)
            transform.FindChild("SoundEffects").GetChild(0).GetComponent<Text>().text = "enable sound effects";
        else
            transform.FindChild("SoundEffects").GetChild(0).GetComponent<Text>().text = "disable sound effects";
        SoundEffects = !SoundEffects;
    }

    public void ChangeMusic()
    {
        if (Music)
            transform.FindChild("Music").GetChild(0).GetComponent<Text>().text = "enable music";
        else
            transform.FindChild("Music").GetChild(0).GetComponent<Text>().text = "disable music";
        Music = !Music;
        PlayerPrefs.SetInt("Music", Music ? 1 : 0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartBattle()
    {
        PlayerPrefs.SetInt("SoundEffects", SoundEffects ? 1 : 0);
        SceneManager.LoadScene("Battleground");
    }

    void Start()
    {
        Cursor.visible = true;
        CheckAudioPrefs();
        ManageAudioButtons();
    }

    void CheckAudioPrefs()
    {
        if (!PlayerPrefs.HasKey("SoundEffects"))
            SoundEffects = true;
        else
            SoundEffects = PlayerPrefs.GetInt("SoundEffects") != 0;

        if (!PlayerPrefs.HasKey("Music"))
        {
            Music = true;
            PlayerPrefs.SetInt("Music", 1);
        }
        else
            Music = PlayerPrefs.GetInt("Music") != 0;
    }

    void ManageAudioButtons()
    {
        if (SoundEffects)
            transform.FindChild("SoundEffects").GetChild(0).GetComponent<Text>().text = "disable sound effects";
        else
            transform.FindChild("SoundEffects").GetChild(0).GetComponent<Text>().text = "enable sound effects";

        if (Music)
            transform.FindChild("Music").GetChild(0).GetComponent<Text>().text = "disable music";
        else
            transform.FindChild("Music").GetChild(0).GetComponent<Text>().text = "enable music";
    }
}
