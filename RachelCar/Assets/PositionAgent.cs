using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.IO;

public class PositionAgent : Agent
{
    private GameObject mouse;
    private Transform mouseTransform;
    private GameObject cylinder;
    // Start is called before the first frame update
    void Start()
    {
        mouse = transform.parent.gameObject;
        mouseTransform = mouse.transform;
        cylinder = GameObject.Find("Cup");

    }

    // Update is called once per frame
    void Update()
    {

    }


    private Vector3 dropPos;
    public override void OnEpisodeBegin()
    {
        Helpful.SetRandPosInCircle(this.gameObject, Vector3.zero, cylinder.GetComponent<SizeSet>().size);
        this.transform.rotation = Quaternion.Euler(0f, Random.value * 360f, 0f);
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        //Just gets camera view


       
        
        //base.CollectObservations(sensor); Was here by default but isn't in the tutorial
    }

    public float positionPunishmentMultiplier = 1.0f;
    public float rotationPunishmentMultiplier = 1.0f;
    public Vector3 positionGuess;
    public float rotationGuess;
    public float positionDis;
    public float rotationDis;
    //private long dodged = 0;
    public override void OnActionReceived(float[] vectorAction)
    {
        positionGuess = new Vector3(vectorAction[0], 0f, vectorAction[1]);//The training area is conveniently in a -1 to 1 square 
        rotationGuess = (vectorAction[2] + 1f) * 180f;//Convert from -1 to 1 to 0 to 360
        //I could use fast inverse square root here... this is called very often.
        positionDis = Vector3.Distance(positionGuess, mouseTransform.position);
        rotationDis = Mathf.Abs(rotationGuess - mouseTransform.rotation.eulerAngles.y);
        SetReward(-positionDis * positionPunishmentMultiplier
                  -rotationDis * rotationPunishmentMultiplier);
        EndEpisode();
        //base.OnActionReceived(vectorAction);Was here by default but isn't in the tutorial
    }

    public override void Heuristic(float[] actionsOut)
    {
        //base.Heuristic(actionsOut);Was here by default but isn't in the tutorial

        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    
    
}
