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
    private float movementSpeed = 0;
    float walkingDefaultSpeed;
    [Range(10, 50), SerializeField]
    private int verticalRotationSpeed = 0;
    [Range(10, 100), SerializeField]
    private int horizontalRotationSpeed = 0;

    [Range(0, 0.1f), SerializeField]
    private float bobbingAmount = 0;

    public float footStepSoundFactor = 0.28f;
    const float headBobSpeedFactor = 1f;
    
    bool walking = false;
    float footStepCount;
    const float footStepMax = 0.1f;

    float defaultHeight = 0.65f;
    
    float m_headBobSpeedFrequency;    
    float m_footStepSoundFrequency;    

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputs = GetComponent<InputManager>();
        movement = Vector3.zero;
        cam = Camera.main;

        defaultPosY = cam.gameObject.transform.position.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        footStepCount = footStepMax;
        walkingDefaultSpeed = movementSpeed;
        Run(false);
    }
    private void Update()
    {                        
        rotate();
        move();
        applyHeadBouncing();

        if (transform.position.y != defaultHeight)
        {
            //controller.Move(-Vector3.up * Time.deltaTime * 1f);
            transform.position = new Vector3(transform.position.x, defaultHeight, transform.position.z);
        }

        
        if (walking)
        {            
            footStepCount -= m_footStepSoundFrequency * Time.deltaTime;
            if (footStepCount <= 0)
            {
                footStepCount = footStepMax;
                AudioManager.instance.ReproduceFootsteps();
            }
        }
    }

    /// <summary>
    /// Set to true to run, modifing footsteps sound and headbobbing
    /// </summary>
    /// <param name="state"></param>
    public void Run(bool state)
    {
        m_headBobSpeedFrequency = (state) ? headBobSpeedFactor * 1.5f: headBobSpeedFactor;
        m_footStepSoundFrequency = (state) ? footStepSoundFactor * 1.5f: footStepSoundFactor;
        movementSpeed = (state) ? walkingDefaultSpeed * 1.5f: walkingDefaultSpeed;        
    }

    private void move()
    {
        movement = new Vector3(inputs.i_movement.x, 0f, inputs.i_movement.y);
        movement = transform.TransformDirection(movement);

        controller.Move(movement * Time.deltaTime * movementSpeed);

        //sound      
        walking = (controller.velocity.magnitude > 0.0f);        
    }

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;

    private void rotate()
    {
        /*if (inputs.i_rotate.x != 0)
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

        }*/

        float rotationX = transform.eulerAngles.y + inputs.i_rotate.x * sensitivityX;

        rotationY += inputs.i_rotate.y * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        cam.gameObject.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
        transform.localEulerAngles = new Vector3(0, rotationX, 0);

    
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
            timer += m_headBobSpeedFrequency * Time.deltaTime * controller.velocity.magnitude * 2;
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
