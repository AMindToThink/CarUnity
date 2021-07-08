using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class DrownMouseMisha : Agent
{
    private Rigidbody rb;
    private Vector3 startPos;
    private GameObject cylinder;
    private GameObject platform;
    private Camera childCamera;
    private GameObject goalCamGo;
    private GameObject blindPlane;
    private Camera goalCam;
    private CameraSensorComponent cSensor;
    //private GameObject positionAgent;
    //private PositionAgent pAgent;

    private Collider thisCollider;
    //private Collider cylinderCollider;
    private Collider platformCollider;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.localPosition;
        cylinder = GameObject.Find("Cup");
        platform = transform.parent.transform.Find("Goal").gameObject;

        thisCollider = this.gameObject.GetComponent<Collider>();
        //cylinderCollider = cylinder.GetComponent<Collider>();
        platformCollider = platform.GetComponent<Collider>();

        childCamera = gameObject.transform.GetChild(0).GetComponent<Camera>();

        goalCamGo = gameObject.transform.GetChild(1).gameObject;

        mishaMemory = new float[2];
    }

    // Update is called once per frame
    void Update()
    {

    }


    private Vector3 dropPos;
    private float dropRot;
    private float startTime;
    public override void OnEpisodeBegin()
    {
        //Debug.Log("End Episode");
        numFound = 0;

        //base.OnEpisodeBegin(); Was here by default but isn't in the tutorial
        do//We don't want the rat to start on the platform. I don't like this solution. 
        {
            PlaceObjects();
        } while (Vector3.Distance(transform.localPosition - new Vector3(0f, -transform.localScale.y / 2, 0f), platform.transform.localPosition)
        < ((SphereCollider)platformCollider).radius * platform.transform.localScale.x + transform.localScale.x * 1.41421356f);

        dropPos = transform.localPosition;
        dropRot = transform.localRotation.eulerAngles.y;
    }
    private void PlaceObjects()
    {
        Helpful.SetRandPosInCircle(this.gameObject, Vector3.zero, cylinder.GetComponent<SizeSet>().size);
        this.transform.localRotation = Quaternion.Euler(0f, Random.value * 360f, 0f);

        Helpful.SetRandPosInCircle(platform, Vector3.zero, cylinder.GetComponent<SizeSet>().size);
    }
    //private long numLevel;//The number of times the platform and the mouse have been reset
    //private Hash128 levelHash;
    /*We don't want the agent to try using the value in calculations. 
                                This should discourage that.*/
    //private float levelHashFloat;
    //private float levelRandom;//Instead of the hash, a random number will probably do the same job but better.

    //private List<float[]> successList;
    //private WebCamTexture camTexture;
    public override void CollectObservations(VectorSensor sensor)
    {
        //It gets the camera
        sensor.AddObservation(mishaMemory);

    }

    public float speed = 1f;
    public float swimTime = 10f;//the amount of time the rat can search before episode ends.
    public float rotationSpeed = 2f;
    public float foundReward = 1f;
    public float winBonus = 1f;
    public float drownPunish = -.05f;
    public float wallPunish = -.01f;
    public int numBeforeChanging = 5;
    private int numFound = 0;

    //public Vector3 successPos;

    //private long dodged = 0;
    private float[] mishaMemory;
    public override void OnActionReceived(float[] vectorAction)
    {

        
        mishaMemory[0] = vectorAction[2];
        mishaMemory[1] = vectorAction[3];
        this.transform.localRotation = Quaternion.Euler(0f, transform.localRotation.eulerAngles.y + rotationSpeed * vectorAction[0], 0f);
        //controlSignal.x = vectorAction[0];
        //controlSignal.z = vectorAction[1];
        //transform.localPosition += controlSignal * speed;
        //rb.AddForce(controlSignal * speed);
        //rb.MovePosition(transform.localPosition + Vector3.Normalize(controlSignal) * speed * Time.deltaTime);
        rb.velocity = vectorAction[1] * transform.forward * speed;
        //Debug.Log("Good enough: " + controlSignal);
        if (collideWall)
        {
            AddReward(wallPunish);

        }
        if (TouchingPlatform())
        {
            //Debug.Log("Found It");
            //foundIt = false;
            //Debug.Log("Ended at " + dodged);
            //SetReward(dodged);
            AddReward(foundReward);//Try -1 instead
            //dodged = 0;
            numFound++;
            if (numFound >= numBeforeChanging)
            {
                AddReward(winBonus);
                //Debug.Log("Won"

                //SetReward(1f);
                EndEpisode();
            }
            else
            {
                ResetPos();
            }

        }
        else
        {
            AddReward(drownPunish);//Try 1 instead

        }

        //base.OnActionReceived(vectorAction);Was here by default but isn't in the tutorial
    }

    public override void Heuristic(float[] actionsOut)
    {
        //base.Heuristic(actionsOut);Was here by default but isn't in the tutorial

        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
        actionsOut[2] = mishaMemory[0];
        actionsOut[3] = mishaMemory[1];
    }
    private void ResetPos()
    {
        transform.localPosition = dropPos;
        this.transform.localRotation = Quaternion.Euler(0f, dropRot, 0f);
    }
    private bool TouchingPlatform()
    {
        return thisCollider.bounds.Intersects(platformCollider.bounds);
    }
    private bool collideWall = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("default")) //Unfortunate name for the tube
        {
            collideWall = true;
            Debug.Log("hit Side");
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name.Equals("default"))
        {//Unfortunate name for the tube
            collideWall = false;
            Debug.Log("left Side");
        }
    }

}
