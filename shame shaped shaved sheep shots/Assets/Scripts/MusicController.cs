using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

	void Update () {
        CheckMusicPrefs();  
    }

    void CheckMusicPrefs()
    {
        if (PlayerPrefs.GetInt("Music") == 1 && !GetComponent<AudioSource>().isActiveAndEnabled)
            GetComponent<AudioSource>().enabled = true;
        else if (PlayerPrefs.GetInt("Music") == 0 && GetComponent<AudioSource>().isActiveAndEnabled)
            GetComponent<AudioSource>().enabled = false;
    }
}
