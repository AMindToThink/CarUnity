using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeSet : MonoBehaviour
{
    public float size = .8f;//Change this, not scale directly
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale *= size/.8f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
