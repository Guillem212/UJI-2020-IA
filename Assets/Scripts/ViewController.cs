using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ViewController : MonoBehaviour
{
    private InputManager inputs;
    private Camera cam;

    [SerializeField]
    private float focusSpeed = 0;

    public PostProcessVolume volume;
    private DepthOfField depthOf;

    public bool wearingGlasses;

    public static bool isObjectGrabbed;
    public float grabSpeed;
    // Start is called before the first frame update
    void Start()
    {
        inputs = GetComponent<InputManager>();
        cam = Camera.main;

        volume.profile.TryGetSettings(out depthOf);
        depthOf.enabled.value = true;
        isObjectGrabbed = false;
    }

    // Update is called once per frame
    void Update()
    {
        applyFocus();
    }


    private void applyFocus()
    {
        if (wearingGlasses)
        {
            calculateDistanceToObject();
        }
        else
        {
            depthOf.focusDistance.value = Mathf.Lerp(depthOf.focusDistance.value, inputs.focusValue, Time.deltaTime * focusSpeed);
        }

    }

    private void calculateDistanceToObject()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity))
        {
            depthOf.focusDistance.value = Mathf.Lerp(depthOf.focusDistance.value, hit.distance, Time.deltaTime * focusSpeed);
        }
    }
}
