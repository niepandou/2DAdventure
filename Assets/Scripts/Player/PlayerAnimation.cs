using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PhySicsCheck phySicsCheck;
    private PlayerController playerController;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        phySicsCheck = GetComponent<PhySicsCheck>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        setAnimation();  
    }

    public void setAnimation()
    {
        anim.SetFloat("velocityX",Mathf.Abs(rb.velocity.x));
        anim.SetFloat("velocityY",rb.velocity.y);
        anim.SetBool("isGround",phySicsCheck.isGround);
        anim.SetBool("isGuard", playerController.isGuard);
        anim.SetBool("isDead",playerController.isDead);  
        anim.SetBool("isAttack",playerController.isAttack);
        anim.SetBool("onWall",phySicsCheck.onWall);
        anim.SetBool("isSlide",playerController.isSlide);
        anim.SetBool("isAntiAttack",playerController.couldAntiAttack);
    }

    public void PlayHurt()
    {
        anim.SetTrigger("hurt");
    }

    public void PlayAttack()
    {
        anim.SetTrigger("attack");
    }
}
