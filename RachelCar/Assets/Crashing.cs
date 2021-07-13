using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crashing : MonoBehaviour
{
    public bool crashing;
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        if(collision.gameObject.name.Equals("RTG Track"))
        {
            //Debug.Log("Crash");
            crashing = true;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name.Equals("RTG Track"))
        {
            crashing = false;
        }
    }
}
