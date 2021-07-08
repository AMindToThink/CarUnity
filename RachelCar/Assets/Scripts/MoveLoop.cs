using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLoop : MonoBehaviour
{
    private LevelSettings LevSet;
    // Start is called before the first frame update
    private float startY;
    void Start()
    {
        LevSet = GameObject.Find("LevelSettings").GetComponent<LevelSettings>();
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position.Set(transform.position.x, startY + (Time.time * speed) /*% transform.localScale.y*/, transform.position.z);
        transform.position = transform.position + new Vector3(0.0f, -speed() * Time.deltaTime, 0.0f);
        //transform.position = new Vector3(0.0f, transform.position.y % )
        //float greaterThanZero = (transform.position.y + transform.localScale.y / 2) - (startY + transform.localScale.y);
        //Debug.Log(greaterThanZero);
        if (transform.position.y <= -9.76f)//That represents off the screen. Unfortunately, I don't know how to find that from transform numbers
        {
            transform.position = new Vector3(0.0f, /*startY - transform.localScale.y*/10.4676f, 0.0f);//The position of the top one. Unfortunately, I don't know how to find that from transform numbers
        }
    }
    private float speed()
    {
        return LevSet.roadSpeed;
    }
}
