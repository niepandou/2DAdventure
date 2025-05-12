using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;


public class Sign : MonoBehaviour
{
    public Transform PlayerTransform;
    public PlayerInputControl playerInput;
    private Animator anim;
    public GameObject signSprite;
    private IInteractable targetItem;
    private bool canPress;

    private void Awake()
    {
        //anim = GetComponentInChildren<Animator>();
        anim = signSprite.GetComponent<Animator>();

        playerInput = new PlayerInputControl();
        playerInput.Enable();
    }

    private void OnEnable()
    { 
        InputSystem.onActionChange += OnActionChange;
        playerInput.Gameplay.Confirm.started += OnConfirm;
    }

    private void OnDisable()
    {
        canPress = false;
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
    }

    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if (actionChange == InputActionChange.ActionStarted)
        {
            var device = ((InputAction)obj).activeControl.device;

            switch (device.device)
            {
                case Keyboard:
                    anim.Play("keyboard");
                    break;
                case XInputController:
                    anim.Play("xbox");
                    break;
                default:
                    anim.Play("keyboard");
                    break;
            }
        }
    }

    private void Update()
    {
        signSprite.transform.localScale = PlayerTransform.localScale; 
    }

    private void OnTriggerStay2D(Collider2D other)
    { 
        if (other.CompareTag("Interactable"))
        {
            canPress = true;
            targetItem = other.GetComponent<IInteractable>();
            signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        canPress = false;
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        
    }
    
    private void OnConfirm(InputAction.CallbackContext obj)
    {
        if (canPress)
        {
            targetItem.TriggerAction();
            GetComponent<AudioDefination>()?.playAudioClip();
        }
    }
}
