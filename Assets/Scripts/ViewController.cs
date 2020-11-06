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
        Ray ray = Camera.main.ScreenPointToRay(inputs.i_rotate);
        if(Physics.Raycast(ray, out hit, rangeOfGrab)){
            if(hit.collider.CompareTag("Battery")){
                //aqui el codigo de la pila.
            }
            else if(hit.collider.CompareTag("Locker")){
                if(inputs.playerInput.currentActionMap.Equals("FreeMove")){
                    inputs.playerInput.SwitchCurrentActionMap("Locker");
                }
            }
            else if(hit.collider.CompareTag("Door")){
                //Aqui el codigo para abrir la puerta.
            }
        }
    }
}
