using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class ViewController : MonoBehaviour
{
    //-----------------------------------------
    //Public Variables
    //-----------------------------------------
    public float focusSpeed;
    public PostProcessVolume volume;
    public Texture freeMoveMask;
    public Texture wardrobeMask;

    //-----------------------------------------
    //Private Variables
    //-----------------------------------------
    private DepthOfField depthOf;
    private Vignette vignette;
    [SerializeField] private float rangeOfGrab;
    private GameObject wardrobeActive;
    private Transform transformBeforeWardrobe;
    private InputManager inputs;
    private Camera cam;

    void Start()
    {
        inputs = GetComponent<InputManager>();
        cam = Camera.main;

        volume.profile.TryGetSettings(out depthOf);
        volume.profile.TryGetSettings(out vignette);
        depthOf.enabled.value = true;
        //isObjectGrabbed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(inputs.actionValue){
            if(inputs.playerInput.currentActionMap.name.Equals("Wardrobe")){
                ExitTheWardrobe();
            }
            else{
                Interact();
            }
            inputs.actionValue = false;
        }
        calculateDistanceToObject();

    }

    private void calculateDistanceToObject()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity))
        {
            depthOf.focusDistance.value = Mathf.Lerp(depthOf.focusDistance.value, hit.distance, Time.deltaTime * focusSpeed);
        }
    }

    private void Interact(){
        //RICARDO
        //Aqui va lo de coger la pila
        //FERCHUS
        //Aqui va lo de abrir las puertas

        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if(Physics.Raycast(ray, out hit, rangeOfGrab)){
            if(hit.collider.CompareTag("Battery")){
                //aqui el codigo de la pila.
            }
            else if(hit.collider.CompareTag("Wardrobe")){
                if(inputs.playerInput.currentActionMap.name.Equals("FreeMove")){
                    EnterTheWardrobe(hit);
                }

            }
            else if(hit.collider.CompareTag("Door")){
                print("Puerta");
                Animator anim = hit.collider.gameObject.GetComponent<Animator>();
                if(anim.GetBool("Open")){
                    anim.SetBool("Open", false);         
                }
                else{
                    anim.SetBool("Open", true);
                }
                
            }
        }
    }

    private void EnterTheWardrobe(RaycastHit hit){
        transformBeforeWardrobe = this.transform;
        wardrobeActive = hit.collider.gameObject;
        wardrobeActive.SetActive(false);

        transform.position = wardrobeActive.transform.position;
        transform.rotation = wardrobeActive.transform.rotation;
        transform.Rotate(0, 180, 0);
        vignette.mask.value = wardrobeMask;
        inputs.playerInput.SwitchCurrentActionMap("Wardrobe");
    }

    private void ExitTheWardrobe(){
        transform.position = transformBeforeWardrobe.position + transform.forward;
        transform.rotation = transformBeforeWardrobe.rotation;
        wardrobeActive.SetActive(true);
        vignette.mask.value = freeMoveMask;
        inputs.playerInput.SwitchCurrentActionMap("FreeMove");
    }
}
