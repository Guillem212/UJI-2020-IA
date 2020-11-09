using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Media;
using UnityEngine;

/* HE TENIDO QUE COMENTAR TODO PORQUE ME DABA ERRORES DE COMPILACIÓN
public class WalluigiCleverBrain : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 playerPosition;
    StateControlWaluigi scriptWaluigi;
    public bool inside_closet; //Indica si el jugador está dentro o fuera del armario
    float m_closetmultiplierDown; //Un número menor a la siguiente variable
    float m_closetmultiplierUp; //Un número mayor a la anterior variable
    float m_closetRatio; //El medidor de cabreo de Waluigi por estar dentro de un armario
    float half_ratio; //En caso de estar mucho tiempo en taquilla irá a por ti
*/
    /* Esto lo dejo en caso de que nos diera tiempo a un estado avanzado
    float low_ratio; 30%
    float medium_ratio; 50%
    float high_ratio; 70%
    float top_ratio; 100%
    */
/* HE TENIDO QUE COMENTAR TODO PORQUE ME DABA ERRORES DE COMPILACIÓN
    void Start()
    {
        playerPosition = GameObject.Find("Player").transform.position;
        scriptWaluigi = GameObject.Find("Waluigi").GetComponent<StateControlWaluigi>();
        inside_closet = false;
    }

    // Update is called once per frame
    void Update()
    {
    //Esto va subiendo o bajando el nivel de alerta de Waluigi dependiendo si el jugador está metido en un armario o no
        if (inside_closet){ //Si el jugador está dentro del armario
            if(m_closetRatio >= 1){ //En caso de exceder el límite se queda a 1
                m_closetRatio = 1;
            }
            else{ //En caso de que el medidor no sea el máximo va subiendo
                m_closetRatio += m_closetmultiplierUp * Time.deltaTime;
            }
        }
        else{ //Si el jugador no está dentro de un armario el medidor baja hasta 0.
            if (m_closetRatio<=0){
                m_closetRatio = 0
            }
            else{
                m_closetRatio -= m_closetmultiplierDown * Time.deltaTime;
            }
        }
        if (m_closetRatio >= half_ratio){
            //Aquí llamaría a una función del script del waluigi tonto
        }
    }   
}
*/