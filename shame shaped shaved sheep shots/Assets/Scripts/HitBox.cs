using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {


    public float punchHpLoss;
    public float kickHpLoss;
    public float punchRecoilForce;
    public float kickRecoilForce;

    public float HPLoss { get; set; }
    public Vector2 Recoil { get; private set; }

    private Vector2 collisionVelocity;

    private void OnCollisionEnter2D(Collision2D collision)
    { 
        collisionVelocity = collision.relativeVelocity;
        CalculateLoss(collision.collider.name);
    }

    void CalculateLoss(string hitType)
    {
        switch(hitType)
        {
           case "PunchRange":
           case "CrouchPunchRange":
                HPLoss += punchHpLoss;
                if (collisionVelocity.x >= 0)
                    Recoil = new Vector2(collisionVelocity.x, collisionVelocity.x) * punchRecoilForce;
                else
                    Recoil = new Vector2(collisionVelocity.x, -collisionVelocity.x) * punchRecoilForce;
                break;
            case "KickRange":
            case "CrouchKickRange":
                HPLoss += kickHpLoss;
                if (collisionVelocity.x >= 0)
                    Recoil = new Vector2(collisionVelocity.x, collisionVelocity.x) * kickRecoilForce;
                else
                    Recoil = new Vector2(collisionVelocity.x, -collisionVelocity.x) * kickRecoilForce;
                break;
        }
    }

}
