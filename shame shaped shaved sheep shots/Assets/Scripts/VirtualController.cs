using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualController : MonoBehaviour {

    private PlayerController controller;
    private string moveAxisName;
    private UnityEngine.KeyCode jumpCode;
    private UnityEngine.KeyCode crouchCode;
    private UnityEngine.KeyCode punchCode;
    private UnityEngine.KeyCode kickCode;
    private UnityEngine.KeyCode blockCode;

    public void InitiateController(PlayerController.PlayerType playerType)
    {
        switch (playerType)
        {
            case PlayerController.PlayerType.player1:
                moveAxisName = "P1_Horizontal";
                jumpCode = KeyCode.W;
                crouchCode = KeyCode.S;
                punchCode = KeyCode.I;
                kickCode = KeyCode.O;
                blockCode = KeyCode.P;
                break;
            case PlayerController.PlayerType.player2:
                moveAxisName = "P2_Horizontal";
                jumpCode = KeyCode.UpArrow;
                crouchCode = KeyCode.DownArrow;
                punchCode = KeyCode.Keypad7;
                kickCode = KeyCode.Keypad8;
                blockCode = KeyCode.Keypad9;
                break;
        }
    }

    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        UpdateTriggers();
    }

    void UpdateTriggers()
    {
        controller.HorizontalMove = Input.GetAxis(moveAxisName);
        controller.JumpTrigger = Input.GetKeyDown(jumpCode);
        controller.CrouchTrigger = Input.GetKeyDown(crouchCode);
        controller.CrouchUpTrigger = Input.GetKeyUp(crouchCode);
        controller.PunchTrigger = Input.GetKeyDown(punchCode);
        controller.KickTrigger = Input.GetKeyDown(kickCode);
        controller.BlockTrigger = Input.GetKeyDown(blockCode);
        controller.BlockUpTrigger = Input.GetKeyUp(blockCode);
    }
}
