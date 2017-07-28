using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarHandler : MonoBehaviour {

    public CharacterPhysics player;

    private Image hpContent;

	void Start ()
    {
        hpContent = transform.FindChild("Content").GetComponent<Image>();
	}
	
	void Update ()
    {
        hpContent.fillAmount = player.HP / 100f;
    }
}
