using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ViewController : MonoBehaviour
{
    private InputManager inputs;
    private Camera cam;

    public float focusSpeed;
    public PostProcessVolume volume;
    private DepthOfField depthOf;
    [SerializeField] private float rangeOfGrab;
    private GameObject wardrobeActive;
    private Transform transformBeforeWardrobe;

    void Start()
    {
        inputs = GetComponent<InputManager>();
        cam = Camera.main;

        volume.profile.TryGetSettings(out depthOf);
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
                //Aqui el codigo para abrir la puerta.
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
        inputs.playerInput.SwitchCurrentActionMap("Wardrobe");
    }

    private void ExitTheWardrobe(){
        transform.position = transformBeforeWardrobe.position + transform.forward;
        transform.rotation = transformBeforeWardrobe.rotation;
        wardrobeActive.SetActive(true);
        inputs.playerInput.SwitchCurrentActionMap("FreeMove");
    }
}
