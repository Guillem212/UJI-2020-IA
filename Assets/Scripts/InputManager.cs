using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    [HideInInspector] public Vector2 i_movement;
    [HideInInspector] public Vector2 i_rotate;
    [HideInInspector] public bool laternValue;

    public PlayerInput playerInput;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void OnMove(InputValue value)
    {
        i_movement = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        i_rotate = value.Get<Vector2>();
    }

    public void OnLantern(InputValue value)
    {
        AudioManager.instance.Play("Flashlight");
        if(laternValue){
            laternValue = false;
        }
        else{
            laternValue = true;
        }
    }

    public void OnAction(InputValue value){
        
    }
}
