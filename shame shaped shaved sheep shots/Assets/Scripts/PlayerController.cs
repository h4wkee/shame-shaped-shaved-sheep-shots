using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public enum PlayerType
    {
        player1,
        player2,
        computer
    };

    public PlayerType playerType;
    public float moveSpeed;
    public float jumpForce;
    public float punchCooldown;
    public float kickCooldown;
    public LayerMask whatIsGround;
    public AudioClip walkSound;

    public CharacterPhysics Physics { get; set; }
    public float HorizontalMove { get; set; }
    public bool JumpTrigger { get; set; }
    public bool CrouchTrigger { get; set; }
    public bool CrouchUpTrigger { get; set; }
    public bool PunchTrigger { get; set; }
    public bool KickTrigger { get; set; }
    public bool BlockTrigger { get; set; }
    public bool BlockUpTrigger { get; set; }
    public bool BlockActive { get; set; }
    public bool SoundEffects { get; set; }
    public bool FacingRight { get; private set; }
    public bool Crouch { get; private set; }

    private Rigidbody2D rb;
    private bool grounded;
    private bool jumpState;
    private Animator anim;
    private float groundRadius;
    private float punchTimer;
    private float kickTimer;
    private bool hit;

    public void Flip()
    {
        FacingRight = !FacingRight;
        Vector2 newScale = this.transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    public void WalkSound()
    {
        if (SoundEffects && grounded)
            GetComponent<AudioSource>().PlayOneShot(walkSound, 0.6F);
    }

    void Start()
    {
        Physics = GetComponent<CharacterPhysics>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        grounded = false;
        groundRadius = 0.01f;
        punchTimer = kickTimer = 0;
        hit = false;
        BlockActive = false;

        DeterminePlayerType();
    }

    void FixedUpdate()
    {
        MoveControll();   
    }

    void Update()
    {
        BehaviorControll();
    }

    void DeterminePlayerType()
    {
        if (playerType == PlayerType.player1)
        {
            FacingRight = true;
            GetComponent<VirtualController>().InitiateController(playerType);
        }
        else if (playerType == PlayerType.player2)
        {
            FacingRight = false;
            switch (PlayerPrefs.GetString("Mode"))
            {
                case "pvp":
                    GetComponent<VirtualController>().enabled = true;
                    GetComponent<VirtualController>().InitiateController(playerType);
                    break;
                case "pvcEasy":
                    GetComponent<AIHandler>().enabled = true;
                    GetComponent<AIHandler>().Level = AIHandler.DifficultyLevel.easy;
                    break;
                case "pvcMedium":
                    GetComponent<AIHandler>().enabled = true;
                    GetComponent<AIHandler>().Level = AIHandler.DifficultyLevel.medium;
                    break;
                case "pvcDeath":
                    GetComponent<AIHandler>().enabled = true;
                    GetComponent<AIHandler>().Level = AIHandler.DifficultyLevel.death;
                    break;

            }
        }
    }

    void MoveControll()
    {
        if (Physics.Living)
        {
            grounded = Physics2D.OverlapCircle(this.transform.position, groundRadius, whatIsGround);
            anim.SetBool("onGround", grounded);

            anim.SetFloat("Speed", Mathf.Abs(HorizontalMove));
            anim.SetFloat("vSpeed", rb.velocity.y);

            if (HorizontalMove > 0 && !FacingRight)
                Flip();
            else if (HorizontalMove < 0 && FacingRight)
                Flip();

            if (!Crouch && !BlockActive && HorizontalMove != 0)
                rb.velocity = new Vector2(HorizontalMove * moveSpeed, rb.velocity.y);
        }
    }

    void BehaviorControll()
    {
        if (Physics.Living)
        {
            punchTimer += Time.deltaTime;
            kickTimer += Time.deltaTime;

            if (grounded && JumpTrigger && !jumpState)
            {
                jumpState = true;
                StartCoroutine(Jump());
            }
            if (CrouchTrigger)
            {
                Crouch = true;
                anim.SetTrigger("Crouch");
                anim.SetBool("Stand", false);
            }
            if (Crouch && (CrouchUpTrigger || JumpTrigger))
            {
                Crouch = false;
                anim.SetBool("Stand", true);
            }
            if (hit)
            {
                Hit();
            }
            if (PunchTrigger && punchTimer >= punchCooldown && !BlockActive)
            {
                Punch();
            }
            if (KickTrigger && kickTimer >= kickCooldown && !BlockActive)
            {
                Kick();
            }
            if (BlockTrigger)
            {
                Block();
            }
            if (BlockUpTrigger && BlockActive)
            {
                BlockEnd();
            }
        }
    }

    void Hit()
    {
        transform.FindChild("PunchRange").gameObject.SetActive(false);
        transform.FindChild("KickRange").gameObject.SetActive(false);
        transform.FindChild("CrouchPunchRange").gameObject.SetActive(false);
        transform.FindChild("CrouchKickRange").gameObject.SetActive(false);
        BlockActive = false;
        hit = false;
    }

    void Punch()
    {
        if (Crouch)
        {
            anim.CrossFade("CrouchPunch", 0.2f, 0);
            transform.FindChild("CrouchPunchRange").gameObject.SetActive(true);
        }
        else
        {
            anim.CrossFade("Punch", 0.2f, 0);
            transform.FindChild("PunchRange").gameObject.SetActive(true);
        }
        punchTimer = 0;
        hit = true;
    }

    void Kick()
    {
        if (Crouch)
        {
            anim.CrossFade("CrouchKick", 0.2f, 0);
            transform.FindChild("CrouchKickRange").gameObject.SetActive(true);
        }
        else
        {
            anim.CrossFade("Kick", 0.2f, 0);
            transform.FindChild("KickRange").gameObject.SetActive(true);
        }
        kickTimer = 0;
        hit = true;
    }

    void Block()
    {
        if (Crouch)
            anim.CrossFade("CrouchBlock", 0.2f, 0);
        else
            anim.CrossFade("Block", 0.2f, 0);
        BlockActive = true;
    }

    void BlockEnd()
    {
        if (Crouch)
            anim.CrossFade("Crouch", 0.2f, 0);
        else
            anim.CrossFade("Idle", 0.2f, 0);
        BlockActive = false;
    }

    IEnumerator Jump()
    {
        anim.SetBool("onGround", false);
        anim.SetTrigger("Jump");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        rb.AddForce(new Vector2(0, jumpForce));
        jumpState = false;
    }
}
