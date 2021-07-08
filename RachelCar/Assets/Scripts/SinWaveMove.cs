using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinWaveMove : MonoBehaviour
{
    private Rigidbody2D rb;
    public float amplitude;
    public float frequency;
    private Vector3 startPos;
    private float startTime;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //rb.velocity = Mathf.Cos(frequency * Time.deltaTime * );
        transform.position = new Vector3(startPos.x + amplitude * Mathf.Sin(frequency * (Time.time - startTime)) , transform.position.y, 0f);
    }
}
