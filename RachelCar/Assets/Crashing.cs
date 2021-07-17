using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crashing : MonoBehaviour
{
    public bool crashing;
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        string name = collision.gameObject.name;
        if (name.Equals("RTG Track") || name.Equals("Plane"))
        {
            //Debug.Log("Crash");
            crashing = true;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        string name = collision.gameObject.name;
        if (name.Equals("RTG Track") || name.Equals("Plane"))
        {
            crashing = false;
        }
    }
}
