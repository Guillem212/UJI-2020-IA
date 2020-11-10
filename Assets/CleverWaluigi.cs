using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleverWaluigi : MonoBehaviour
{
    /// <summary>
    /// Sends a clue to waluigi depending on character's abuse of wardroves
    /// </summary>


    StateControlWaluigi stateControl;

    public bool m_playerInWardrobe = false;
    public float m_wardrobeRatio = 0f;
    const float m_wardroveMultiplierUp = 0.1f;//detection up multiplier ratio
    const float m_wardroveMultiplierDown = 0.05f;//detection down multiplier ratio

    bool checkingWardrobes = false;
    bool sendingAlert = false;

    // Start is called before the first frame update
    void Start()
    {
        m_wardrobeRatio = 0f;
        stateControl = GameObject.Find("Waluigi").GetComponent<StateControlWaluigi>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerInWardrobe)
        {
            if (m_wardrobeRatio >= 1f) m_wardrobeRatio = 1f;
            else
            {
                m_wardrobeRatio += m_wardroveMultiplierUp * Time.deltaTime;
            }
        }
        else
        {
            if (m_wardrobeRatio > 0f)
            {
                m_wardrobeRatio -= m_wardroveMultiplierDown * Time.deltaTime;                
            }
            else m_wardrobeRatio = 0f;
        }

        if (m_wardrobeRatio > 0.5f && !checkingWardrobes)
        {
            //stateControl.CheckWardrobes();
            checkingWardrobes = true;
        }        
        else if (m_wardrobeRatio >= 1f && !sendingAlert)
        {
            //stateControl.WardrobeAlert();
            ///estado de alerta por armario: implica que al llegar a un armario tiene en cuenta si m_playerInWardrobe = true
            sendingAlert = true;
        }
    }
}
