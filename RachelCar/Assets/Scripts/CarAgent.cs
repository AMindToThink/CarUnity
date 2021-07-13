using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.SceneManagement;
public class CarAgent : Agent
{
    private CarController carController;
    private Track_Generator trackGen;
    private Rigidbody rb;
    Scene activeScene;
    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<CarController>();
        activeScene = SceneManager.GetActiveScene();
        //Debug.Log("Scene " + activeScene + " count " + activeScene.rootCount);
        //Debug.Log("Root2 " + activeScene.GetRootGameObjects()[activeScene.rootCount - 1]);
        //start = activeScene.GetRootGameObjects()[activeScene.rootCount - 2].transform.GetChild(0).GetChild(2);
        trackGen = GameObject.Find("TrackGenerator").GetComponent<Track_Generator>();

        startVec = new Vector3(0f, startY, 0f);
        
        rb = GetComponent<Rigidbody>();

        waypointDisSq = waypointDis * waypointDis;

        timePunish = baseTimePunish / trackSize;
    }

    // Update is called once per frame
    void Update()
    {

    }

    
    public float startY = 1f;
    public float startV = 1f;
    [Range(1,10)]
    public int trackSize = 1;

    private Vector3 startVec;
    private GameObject randTrack;
    private Transform waypoints;
    private bool[] reachedWayPoints;
    private Transform mostRecentWayPoint;
    private int reachedCount;
    private float wayAddReward;
    private Transform startGrid;
    private Vector3 end;

    private float stayTime = 0f;
    public override void OnEpisodeBegin()
    {
        stayTime = Time.time;
        //base.OnEpisodeBegin(); Was here by default but isn't in the tutorial

        CreateTrack();

        TrackRB();

        WaypointSetup();

        StartSetup();

        
    }
    private void CreateTrack()
    {
        trackGen.CreateTrack(false, false, trackSize);

        //The track insists upon putting itself last, therefore it is at rootCount-1.
        randTrack = activeScene.GetRootGameObjects()[activeScene.rootCount - 1];
    }
    private void TrackRB()
    {
        Rigidbody trackRB = randTrack.AddComponent<Rigidbody>();
        //By giving the track a rigidbody, OnTriggerEnter in the Crashing.cs script will crash with the track as a whole rather than just a part of it.
        trackRB.isKinematic = true;
    }
    private void WaypointSetup()
    {
        waypoints = randTrack.transform.Find("W-P-C");
        reachedWayPoints = new bool[waypoints.childCount];
        wayAddReward = 1f / waypoints.childCount;
        reachedCount = 0;
    }
    private void StartSetup()
    {
        startGrid = randTrack.transform.GetChild(0).Find("StartGrid");
        end = startGrid.position + startGrid.forward * 100f;
        mostRecentWayPoint = startGrid.transform;
        //Helpful.PrintQuaternion(start.rotation);
        transform.SetPositionAndRotation(startGrid.position + startVec, startGrid.rotation);
        rb.velocity = transform.forward * startV;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        
        //base.CollectObservations(sensor); Was here by default but isn't in the tutorial
    }

    
    
    public override void OnActionReceived(float[] vectorAction)
    {
        
        Act(vectorAction);
        Reward(vectorAction);

        //base.OnActionReceived(vectorAction);Was here by default but isn't in the tutorial
    }
    public float waypointDis;
    private float waypointDisSq;
    private void Act(float[] vectorAction)
    {
        carController.Move(vectorAction[0], vectorAction[1], vectorAction[1], 0f/*vectorAction[2]*/);
        
        
    }

    [Header("Rewards")]
    public float baseTimePunish = -.0001f;
    private float timePunish;

    public float forwardRewardMultiplier = .0002f;
    public float secondsWithoutWaypoint = 100f;
    public float noWaypointPunish = -.2f;
    public float crashPunish = -.2f;
    public float completeReward = .5f;

    private void Reward(float[] vectorAction)
    {
        AddReward(timePunish + vectorAction[1] * forwardRewardMultiplier);
        for (int i = 0; i < waypoints.childCount; i++)
        {
            if (!reachedWayPoints[i] && waypointDisSq >= Vector3.SqrMagnitude(transform.position - waypoints.GetChild(i).position))
            {
                reachedWayPoints[i] = true;
                reachedCount++;
                mostRecentWayPoint = waypoints.GetChild(i);
                stayTime = Time.time;
                AddReward(wayAddReward);
                //Debug.Log("A point");
            }
        }
        //Failing
        if (GetComponent<Crashing>().crashing)
        {
            End(crashPunish);
        }
        else if (Time.time - stayTime >= secondsWithoutWaypoint)
        {
            End(noWaypointPunish);
        }
        //Winning
        else if (reachedCount == reachedWayPoints.Length
            && waypointDisSq >= Vector3.SqrMagnitude(transform.position - end))
        {
            End(completeReward);
            
        }
        
        //SetReward(reachedCount/reachedWayPoints.Length);
        
    }
    private void End(float reward)
    {
        AddReward(reward);
        GetComponent<Crashing>().crashing = false;
        EndEpisode();
    }
    public override void Heuristic(float[] actionsOut)
    {

        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
        //actionsOut[2] = Input.GetAxis("Jump");
    }
    
}
