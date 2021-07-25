using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
/*Simulation of ultrasonic distance sensor*/

public class Dalnomer : MonoBehaviour
{
    public Text Dalnost;
    float rasstoyanie = 0; // переменная для расстояния до цели

    // Use this for initialization 
    void Start()
    {

    }

    // Update is called once per frame 
    void FixedUpdate()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, 200))
        {
            rasstoyanie = hitInfo.distance;
            Debug.DrawRay(transform.position, transform.forward * rasstoyanie, Color.yellow);
            Dalnost.text = rasstoyanie.ToString();
        }
    }
}

