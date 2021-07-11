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
    Scene activeScene;
    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<CarController>();
        activeScene = SceneManager.GetActiveScene();
        //Debug.Log("Scene " + activeScene + " count " + activeScene.rootCount);
        //Debug.Log("Root2 " + activeScene.GetRootGameObjects()[activeScene.rootCount - 1]);
        //start = activeScene.GetRootGameObjects()[activeScene.rootCount - 2].transform.GetChild(0).GetChild(2);
        trackGen = GameObject.Find("TrackGen").GetComponent<Track_Generator>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public Transform start;
    public override void OnEpisodeBegin()
    {
        //base.OnEpisodeBegin(); Was here by default but isn't in the tutorial

        //trackGen.CreateTrack(false, false, 1);
        //The track insists upon putting itself last, therefore it is at rootCount-1.
        start = activeScene.GetRootGameObjects()[activeScene.rootCount-2].transform.GetChild(0).Find("StartGrid");
        transform.position = start.position;
        //Helpful.PrintQuaternion(start.rotation);
        transform.rotation = start.rotation;
        //transform.rotation.eulerAngles.Set(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y+Mathf.PI, transform.rotation.eulerAngles.z);
        //Helpful.PrintQuaternion(transform.rotation);
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
    private void Act(float[] vectorAction)
    {
        carController.Move(vectorAction[0], vectorAction[1], vectorAction[1], vectorAction[2]);

    }
    private void Reward(float[] vectorAction)
    {
        AddReward(vectorAction[1]);
    }

    public override void Heuristic(float[] actionsOut)
    {

        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
        actionsOut[2] = Input.GetAxis("Jump");
    }
    
}
