using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
public class ObstacleAgent : Agent
{
    //private Rigidbody2D rb;
    private Vector3 startPos;
    public GameObject skagent;//The object it wants to hit.
    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //public Transform player; //The player agent

    public override void OnEpisodeBegin()
    {
        //base.OnEpisodeBegin(); Was here by default but isn't in the tutorial
        //transform.position = startPos;
        //obstacle.localPosition = new Vector3(Random.value * 4.7f - 2.35f, 5.76f, 0f);


    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(skagent.transform.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        //sensor.AddObservation(this.transform.localPosition);

        //sensor.AddObservation(rb.velocity.x);
        //sensor.AddObservation(rb.velocity.y);
        //base.CollectObservations(sensor); Was here by default but isn't in the tutorial
    }

    //public float speed = 10f;
    //private long dodged = 0;
    private void PlacePosition(float action)
    {
        transform.position = new Vector3(Mathf.Clamp01((action +1f)/2f) * 4.7f - 2.35f, startPos.y, 0f);
    }
    //private int misses = 0;
    //public int missEndNum = 5;
    
    public override void OnActionReceived(float[] vectorAction)
    {
        //vectorAction[0] = Mathf.Clamp(vectorAction[0], -1f, 1f);
        //AddReward(1/Vector3.Distance(skagent.transform.localPosition, this.transform.localPosition)); //Gets reward when close
        float disSqared = Vector3.SqrMagnitude(skagent.transform.position - this.transform.localPosition);
        float fastSqRt = Helpful.FastInverseSquareRoot(disSqared);
        //Debug.Log("Fast sq rt " + fastSqRt + " Inverse " + (1/fastSqRt));
        AddReward(fastSqRt);
        if (transform.position.y < -6.5f)
        {
            //Vector3 controlSignal = Vector3.zero;
            //controlSignal.x = vectorAction[0];

            //transform.position = new Vector3(vectorAction[0] * 4.7f - 2.35f, startPos.y, 0f);

            //controlSignal.y = vectorAction[1];
            //transform.position += controlSignal * speed;
            //rb.AddForce(controlSignal * speed);
            //rb.MovePosition(transform.position + Vector3.Normalize(controlSignal) * speed * Time.deltaTime);
            //rb.velocity = Vector3.Normalize(controlSignal) * speed;
            //Debug.Log("Good enough: " + controlSignal);
            PlacePosition(vectorAction[0]);
            //misses++;
            SetReward(-.5f);
            //if(misses >= missEndNum)
            EndEpisode();
        }
        else
        {
            bool collidedObstacle = skagent.GetComponent<Skagent>().touchingOw > 0;
            if (collidedObstacle)
            {
                //Debug.Log("Hit");

                //Debug.Log("Ended at " + dodged);
                //SetReward(dodged);
                AddReward(1f);
                PlacePosition(vectorAction[0]);
                //dodged = 0;
                //EndEpisode();
            }
        }
        


        //base.OnActionReceived(vectorAction);Was here by default but isn't in the tutorial
    }

    public override void Heuristic(float[] actionsOut)
    {
        //base.Heuristic(actionsOut);Was here by default but isn't in the tutorial

        //actionsOut[0] = Input.GetAxis("Horizontal");
        
        actionsOut[0] = ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x + 2.35f) / (4.7f/2)-1f); //The opposite of PlacePosition
        //Debug.Log("Action " + actionsOut[0]);
        //actionsOut[0] = (Input.mousePosition.x - Screen.width/2.0f)/Screen.width
        //actionsOut[1] = Input.GetAxis("Vertical");
    }
    //private int touchingAgent = 0;
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
    /*private void OnCollisionEnter2D(Collider2D collision)
    {
        //Debug.Log("entered");
        if (collision.gameObject.CompareTag("Agent"))
        {
            touchingAgent++;
            //Debug.Log("Touched");
        }
    }
    private void OnCollisionExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Agent"))
        {
            touchingAgent--;
        }
    }
    */
    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Agent"))
        {
            touchingAgent++;
            //Debug.Log("Touched");
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Agent"))
        {
            touchingAgent--;
        }
    }
    */
}
