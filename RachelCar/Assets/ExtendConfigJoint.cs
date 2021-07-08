using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendConfigJoint : MonoBehaviour
{
    ConfigurableJoint cj;
    // Start is called before the first frame update
    void Start()
    {
        //cj = GetComponent<ConfigurableJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.Translate(Vector3.down);
    }
}
