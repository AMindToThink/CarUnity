using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicObstacle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided");
        if (collision.gameObject.CompareTag("Agent"))
        {
            Destroy(this.gameObject);
        }
    }
}
