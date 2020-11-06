/*
---CHANGES TO DO AND UPDATES---
    Is an elder man, so in order to read the things, u are constatly smashing a button to allow to see.
    When are in free move, autoenfoque with depth of field too.
*/
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Camera cam;
    private InputManager inputs;


    private Vector3 movement;
    private float x_rotation;

    private float timer = 0;
    
    private float defaultPosY = 0;

    [Range(1, 10), SerializeField]
    private int movementSpeed = 0;
    [Range(10, 50), SerializeField]
    private int verticalRotationSpeed = 0;
    [Range(10, 100), SerializeField]
    private int horizontalRotationSpeed = 0;

    [Range(0, 0.1f), SerializeField]
    private float bobbingAmount = 0;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputs = GetComponent<InputManager>();
        movement = Vector3.zero;
        cam = Camera.main;

        defaultPosY = cam.gameObject.transform.position.y;

    }
    private void Update()
    {
        if (!ViewController.isObjectGrabbed)
        {
            move();
            rotate();
            applyHeadBouncing();
        } 
    }

    private void move()
    {
        movement = new Vector3(inputs.i_movement.x, 0f, inputs.i_movement.y);
        movement = transform.TransformDirection(movement);

        controller.Move(movement * Time.deltaTime * movementSpeed);
    }

    private void rotate()
    {
        if (inputs.i_rotate.x != 0)
        {
            transform.Rotate(new Vector3(0, inputs.i_rotate.x * Time.deltaTime * horizontalRotationSpeed, 0));
        }
        if (inputs.i_rotate.y != 0)
        {

            x_rotation = cam.gameObject.transform.eulerAngles.x;
            if (x_rotation > 180)
            {
                x_rotation -= 360f;
            }

            if ((x_rotation < 60 && inputs.i_rotate.y < 0) || (x_rotation > -60 && inputs.i_rotate.y > 0))
            {
                applyRotation();
            }

        }
    }

    private void applyRotation()
    {
        cam.gameObject.transform.Rotate(new Vector3(-inputs.i_rotate.y * Time.deltaTime * verticalRotationSpeed, 0, 0));
    }


    //base of the code extract from: https://sharpcoderblog.com/blog/head-bobbing-effect-in-unity-3d / Author - NSDG.
    private void applyHeadBouncing()
    {
        if (Mathf.Abs(controller.velocity.x) > 0.1f || Mathf.Abs(controller.velocity.z) > 0.1f)
        {
            //Player is moving
            timer += Time.deltaTime * controller.velocity.magnitude * 2;
            cam.gameObject.transform.localPosition = new Vector3(cam.gameObject.transform.localPosition.x, 
                                                                defaultPosY + Mathf.Sin(timer) * bobbingAmount, 
                                                                cam.gameObject.transform.localPosition.z);
        }
        else
        {
            //Idle
            timer = 0;
            cam.gameObject.transform.localPosition = new Vector3(cam.gameObject.transform.localPosition.x, 
                                                                Mathf.Lerp(cam.gameObject.transform.localPosition.y, defaultPosY, Time.deltaTime * controller.velocity.magnitude * 2), 
                                                                cam.gameObject.transform.localPosition.z);
        }
    }


}
