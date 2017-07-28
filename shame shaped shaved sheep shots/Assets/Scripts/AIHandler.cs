using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AIHandler : MonoBehaviour {

    public enum DifficultyLevel
    {
        easy,
        medium,
        death
    }

    public PlayerController controller;
    public PlayerController opponent;

    public DifficultyLevel Level { get; set; }

    private enum ActionType
    {
        punch,
        kick,
        block,
    }

    private ActionType action;
    private System.Array caps;
    private System.Random random;
    private float difficultyModifier;
    private bool actionState;

	void Start () {
        caps = System.Enum.GetValues(typeof(ActionType));
        random = new System.Random();
        PickDifficulty();
    }
	
	void Update ()
    {
        ManageAIBehavior();
	}

    void PickDifficulty()
    {
        switch (Level)
        {
            case DifficultyLevel.easy:
                difficultyModifier = 3f;
                break;
            case DifficultyLevel.medium:
                difficultyModifier = 2f;
                break;
            case DifficultyLevel.death:
                difficultyModifier = 1f;
                break;
        }
    }

    void ManageAIBehavior()
    {
        if (opponent.Physics.Living == true)
        {
            if (controller.transform.position.x > (opponent.transform.position.x + 5))
            {
                controller.HorizontalMove = -1;
            }
            else if (controller.transform.position.x < (opponent.transform.position.x - 5))
            {
                controller.HorizontalMove = 1;
            }
            else
            {
                if ((controller.transform.position.x > opponent.transform.position.x && controller.FacingRight) || (controller.transform.position.x < opponent.transform.position.x && !controller.FacingRight))
                    controller.Flip();
                if (Level == DifficultyLevel.easy || Level == DifficultyLevel.medium)
                {
                    if (opponent.Crouch)
                    {
                        controller.CrouchUpTrigger = false;
                        controller.CrouchTrigger = true;
                    }
                    else
                    {
                        controller.CrouchUpTrigger = true;
                        controller.CrouchTrigger = false;
                    }
                }
                controller.HorizontalMove = 0;
                if(!actionState)
                    StartCoroutine(PickAction());
            }
        }
    }

    IEnumerator PickAction()
    {
        actionState = true;
        if (Level == DifficultyLevel.death)
            action = (ActionType)caps.GetValue(random.Next(0, caps.Length));
        else
            action = (ActionType)caps.GetValue(random.Next(0, caps.Length - 1));

        switch (action)
        {
            case ActionType.punch:
                controller.PunchTrigger = true;
                yield return new WaitForSeconds(difficultyModifier);
                controller.PunchTrigger = false;
                break;
            case ActionType.kick:
                controller.KickTrigger = true;
                yield return new WaitForSeconds(difficultyModifier);
                controller.KickTrigger = false;
                break;
            case ActionType.block:
                controller.BlockUpTrigger = false;
                controller.BlockTrigger = true;
                yield return new WaitForSeconds(difficultyModifier);
                controller.BlockUpTrigger = true;
                controller.BlockTrigger = false;
                break;
        }
        actionState = false;
    }
}
