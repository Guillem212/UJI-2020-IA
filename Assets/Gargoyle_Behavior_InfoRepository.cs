using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gargoyle_Behavior_InfoRepository : MonoBehaviour
{
    //Este script se usa para guardar la informacion que los estados necesitan para el comportamiento.
    //Hecho aqui en lugar de en los estados porque no se puede asignar en ellos.

    [Header("Alert")]
    public Material lightAlert;

    [Header("Patrol")]
    public GameObject gargoyle;
    public Material lightPatrol;
}
