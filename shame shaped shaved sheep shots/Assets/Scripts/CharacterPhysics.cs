using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPhysics : MonoBehaviour {
    
    public AudioClip faceShot;
    public AudioClip bodyShot;
    public AudioClip blockShot;
    public AudioClip deadSound;

    public bool SoundEffects { get; set; }
    public bool Living { get; set; }
    public float HP { get; set; }

    private PlayerController controller;
    private HitBox head;
    private HitBox body;
    private Vector2 recoil;
    private Animator anim;  

    void Start () {
        anim = GetComponent<Animator>();
        anim.SetBool("Living", true);
        HP = 100f;
        head = transform.FindChild("HeadBox").GetComponent<Collider2D>().GetComponent<HitBox>();
        body = transform.FindChild("BodyBox").GetComponent<Collider2D>().GetComponent<HitBox>();
        controller = GetComponent<PlayerController>();
        head.HPLoss = 0;
        body.HPLoss = 0;
    }
	
	void Update () {
        HealthCheck();
    }

    void HealthCheck()
    {
        if (Living)
        {
            if (HP <= 0 && Living)
                Die();
            else if (head.HPLoss > 0)
                HeadDamage();
            else if (body.HPLoss > 0)
                BodyDamage();
        }
    }

    void Die()
    {
        Living = false;
        anim.SetBool("Living", false);
        if (SoundEffects)
            GetComponent<AudioSource>().PlayOneShot(deadSound);
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Block"))
            anim.Play("Idle");
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("CrouchBlock"))
            anim.Play("Crouch");
    }

    void HeadDamage()
    {
        recoil = transform.FindChild("HeadBox").GetComponent<Collider2D>().GetComponent<HitBox>().Recoil;
        if (!controller.BlockActive || (controller.BlockActive && controller.Crouch))
        {
            transform.FindChild("HeadBox").GetChild(0).GetComponent<ParticleSystem>().Play();
            if (SoundEffects)
                GetComponent<AudioSource>().PlayOneShot(faceShot);
        }
        else if (SoundEffects)
            GetComponent<AudioSource>().PlayOneShot(blockShot);
        HP -= (controller.BlockActive && !controller.Crouch) ? head.HPLoss * 0.1f : head.HPLoss;
        body.HPLoss = 0;
        StartCoroutine(Beaten());
    }

    void BodyDamage()
    {
        recoil = transform.FindChild("BodyBox").GetComponent<Collider2D>().GetComponent<HitBox>().Recoil;
        if (SoundEffects && (!controller.BlockActive || (controller.BlockActive && !controller.Crouch)))
            GetComponent<AudioSource>().PlayOneShot(bodyShot);
        else if (SoundEffects)
            GetComponent<AudioSource>().PlayOneShot(blockShot);
        HP -= (controller.BlockActive && controller.Crouch) ? body.HPLoss * 0.05f : body.HPLoss;
        head.HPLoss = 0;
        StartCoroutine(Beaten());
    }

    IEnumerator Beaten()
    {
        if(!controller.BlockActive || (controller.BlockActive && ((controller.Crouch && head.HPLoss > 0) || (!controller.Crouch && body.HPLoss > 0))))
        {
            if (controller.Crouch)
                anim.CrossFade("CrouchDamage", 0.2f, 0);
            else
                anim.CrossFade("Damage", 0.2f, 0);
            controller.BlockActive = false;
        }
        
        controller.GetComponent<Rigidbody2D>().AddForce(recoil);
        head.HPLoss = 0;
        body.HPLoss = 0;
        yield return new WaitForSeconds(0.5f);
        if (!controller.BlockActive)
        {
            if (controller.Crouch)
                anim.CrossFade("Crouch", 0.2f, 0);
            else
                anim.CrossFade("Idle", 0.2f, 0);
        }
    }
}
