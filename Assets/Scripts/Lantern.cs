using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Lantern : MonoBehaviour
{
    private float  m_maxEnergy = 5f;
    [Range(0f,5f)]
    [SerializeField] private float m_currentEnergy = 0f;
    private float initialEnergy = 5f;
    [SerializeField] private float m_energyDecrementFactor = 0.05f;

    GameObject m_lantern;

    public Image m_lanternImage;
    private InputManager inputs;


    bool m_usingLantern = false;

    // Start is called before the first frame update
    void Start()
    {
        m_currentEnergy = initialEnergy;
        m_lantern = GameObject.Find("Lantern");
        m_lantern.SetActive(false);
        m_lanternImage.fillAmount = m_currentEnergy / m_maxEnergy;
        inputs = GetComponent<InputManager>();
        //initialize UI
        m_lanternImage.fillAmount = m_currentEnergy / m_maxEnergy;
        m_lanternImage.color = Color.Lerp(new Color(1f, 0.17f, 0f, 0.4f), new Color(0f, 1f, 0.144f, 0.4f), m_lanternImage.fillAmount);
    }

    // Update is called once per frame
    void Update()
    {
        //usar
        if (inputs.laternValue && m_currentEnergy > 0f)
        {
            if(!m_lantern.activeInHierarchy){
                m_usingLantern = true;
                m_lantern.SetActive(m_usingLantern);
            }        
        }
        else{
            if(m_lantern.activeInHierarchy){
                m_usingLantern = false;
                m_lantern.SetActive(m_usingLantern);
            }
        }

        if (m_usingLantern)
        {            
            m_currentEnergy -= m_energyDecrementFactor * Time.deltaTime;
            //color
            m_lanternImage.fillAmount = m_currentEnergy / m_maxEnergy;
            m_lanternImage.color = Color.Lerp(new Color(1f, 0.17f, 0f, 0.4f), new Color(0f, 1f, 0.144f, 0.4f) , m_lanternImage.fillAmount);

            if (m_currentEnergy <= 0f)
            {
                m_currentEnergy = 0f;                
            }
        }
    }

}
