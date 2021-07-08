using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
public class Skagent : Agent
{
    private Rigidbody2D rb;
    private Vector3 startPos;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform obstacle; //Opposite of target

    public override void OnEpisodeBegin()
    {
        //base.OnEpisodeBegin(); Was here by default but isn't in the tutorial
        transform.position = startPos;
        RandObPos();
    }
    private void RandObPos()
    {
        if (obstacle.GetComponent<ObstacleAgent>() == null)//Check if this is handled by ObstacleAgent
            obstacle.localPosition = new Vector3(Random.value * 4.7f - 2.35f, 5.76f, 0f);

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(obstacle.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        //Debug.Log("Observed: " + obstacle.localPosition);
        //sensor.AddObservation(rb.velocity.x);
        //sensor.AddObservation(rb.velocity.y);
        //base.CollectObservations(sensor); Was here by default but isn't in the tutorial
    }

    public float speed = 10f;
    //private long dodged = 0;
    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.y = vectorAction[1];
        //transform.position += controlSignal * speed;
        //rb.AddForce(controlSignal * speed);
        //rb.MovePosition(transform.position + Vector3.Normalize(controlSignal) * speed * Time.deltaTime);
        rb.velocity = Vector3.Normalize(controlSignal) * speed;
        //Debug.Log("Good enough: " + controlSignal);
        bool collidedObstacle = touchingOw > 0;
        if (collidedObstacle)
        {
            Debug.Log("Hit");
            
            //Debug.Log("Ended at " + dodged);
            //SetReward(dodged);
            AddReward(-10f);//Try -1 instead
            //dodged = 0;
            EndEpisode();
        }else if (obstacle.transform.position.y < -6)
        {
            AddReward(.05f);//Try 1 instead
            //dodged++;
            //SetReward(1f);
            //obstacle.localPosition = new Vector3(Random.value * 4.7f - 2.35f, 5.76f, 0f);
            //EndEpisode();
            Debug.Log("Rewarded");
            RandObPos();
            //SetReward(1f);

            //EndEpisode();
            //AddReward(.01f);
        }
        
        //base.OnActionReceived(vectorAction);Was here by default but isn't in the tutorial
    }

    public override void Heuristic(float[] actionsOut)
    {
        //base.Heuristic(actionsOut);Was here by default but isn't in the tutorial
        
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }
    public int touchingOw = 0;
    /*void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("entered");
        if (collision.gameObject.CompareTag("Hurt"))
        {
            touchingOw++;
            Debug.Log("Touched");
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hurt"))
        {
            touchingOw--;
        }
    }
    */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("entered");
        if (collision.gameObject.CompareTag("Hurt"))
        {
            touchingOw++;
            //Debug.Log("Touched");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hurt"))
        {
            touchingOw--;
        }
    }

}
