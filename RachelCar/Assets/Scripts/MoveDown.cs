using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDown : MonoBehaviour
{
    private LevelSettings LevSet;
    private Rigidbody2D rb;
    void Start()
    {
        LevSet = GameObject.Find("LevelSettings").GetComponent<LevelSettings>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, -speed());

    }
        // Update is called once per frame
    void Update()
    {
        //transform.position = transform.position + new Vector3(0.0f, -speed() * Time.deltaTime, 0.0f);
    }
    private float speed()
    {
        return LevSet.roadSpeed;
    }
}
