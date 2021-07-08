using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSet : MonoBehaviour
{
    public float possibleDistanceFromCenter = .8f;
    private GameObject cylinder;
    // Start is called before the first frame update
    void Start()
    {
        cylinder = GameObject.Find("Cup");
        Helpful.SetRandPosInCircle(this.gameObject, Vector3.zero, cylinder.GetComponent<SizeSet>().size);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
