using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
public class OctopusAgent : Agent
{
    [Header("Also determines the range of the light and distance of the camera:")]
    public float goalRadius = 5f;


    private GameObject goal;
    private ArmScript armScript;
    private uint numJoints;
    private GameObject[] joints;
    // Start is called before the first frame update
    void Start()
    {
        goal = GameObject.Find("Goal");
        armScript = GetComponent<ArmScript>();
        numJoints = armScript.numJoints;
        joints = armScript.joints;
        //GameObject.Find("Point Light").GetComponent<Light>().range = goalRadius;
        Camera.main.transform.position = new Vector3(0f, 0f, -goalRadius / Mathf.Tan(25f * Mathf.Deg2Rad));
        //GameObject.Find("Directional Light").transform.position = Camera.main.transform.position;
        collisionDis = joints[joints.Length - 1].transform.localScale.x / 2 + goal.transform.localScale.x / 2;
        inverseCollisionDis = 1 / collisionDis;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public Transform obstacle; //Opposite of target

    public override void OnEpisodeBegin()
    {
        //Debug.Log("New Episode");
        //base.OnEpisodeBegin(); Was here by default but isn't in the tutorial
        armScript.Reset();
        RandGoalPos();
    }

    private void RandGoalPos()
    {
        do
        {
            goal.transform.position = Random.insideUnitSphere * goalRadius;
        } while (IsTouching(InverseGoalDis()));

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(obstacle.localPosition);
        
        //base.CollectObservations(sensor); Was here by default but isn't in the tutorial
    }
    [Header("Stats")]
    public float rotSpeed = 10f;
    public float squishSpeed = 1f;
    
    
    [Header("Rewards: ")]
    public float timePunish = -.1f;
    public float goalReward = 1f;
    public float invDisMultiplier = .01f;

    //private long dodged = 0;
    public override void OnActionReceived(float[] vectorAction)//Please make actions match the number of (joints-1)*3. I can't do that in script, sadly. I think? 
    {

        MoveArm(vectorAction);
        
        AwardIt();
        //base.OnActionReceived(vectorAction);Was here by default but isn't in the tutorial
    }
    private void MoveArm(float[] vectorAction)
    {
        for (int i = 0; i < numJoints-1; i++)
        {
            Quaternion tempRot = joints[i].transform.localRotation;
            //Vector3 tempPos = joints[i].transform.position;
            joints[i].transform.Rotate(vectorAction[i*3] * rotSpeed, 0f, vectorAction[i*3 + 1] * rotSpeed);
            float stretchAmount = squishSpeed * vectorAction[i * 3 + 2];
            armScript.Stretch(i, stretchAmount);

            /*This bit reverses the movement if it results in a 
             * collision (with exceptions of with itself, 
             * with its parent or child (the ones connecting
             * to it (with workarounds for the first and last))).
             * Because of the capsules, this is obsolete.
             */
            /*GameObject child = joints[i];
            bool breakIt = false;
            do
            {
                foreach(GameObject g in joints)
                {
                    if(g != child
                        && (child == joints[0] || g != child.transform.parent.gameObject)
                        && (child.transform.childCount == 0 || g != child.transform.GetChild(0).gameObject)
                        && IsTouching(g, child)
                        )
                    {
                        joints[i].transform.localRotation = tempRot;
                        //joints[i].transform.position = tempPos;
                        armScript.Stretch(i, -squishSpeed * vectorAction[i*3 + 2]);
                        breakIt = true;
                        break;
                    }
                }
                if (breakIt || child.transform.childCount <= 0)
                {
                    break;
                }
                child = child.transform.GetChild(0).gameObject;
                //if(IsTouching(child, ))
            } while (true);
            */
            Physics.SyncTransforms();//Without this, it gets stuck.
            if (CapsuleTouching())
            {
                joints[i].transform.localRotation = tempRot;
                
                armScript.Stretch(i, -stretchAmount);
                //Physics.SyncTransforms();
                //Physics.
                //break;
            }
        }
    }
    private void AwardIt()
    {
        float goalDistance = InverseGoalDis();
        AddReward(timePunish + invDisMultiplier * goalDistance);
        if (IsTouching(goalDistance))
        {
            Debug.Log("GOAL!!!");
            SetReward(goalReward);
            EndEpisode();
        }
    }
    
    public override void Heuristic(float[] actionsOut)
    {
        
        //base.Heuristic(actionsOut);Was here by default but isn't in the tutorial
        
        /*
         * I had to do some whackiness to allow for any number of joints (inputs = (joints-1)*3).
         * There should be at least two joints for 3 inputs.
         * This code pairs positive and negative inputs into a single actionsOut value.
         * For 3 nodes, the input keys are the arrows + numbers[0:8] (zero through seven inclusive).
         * After that, we run out of numbers, so we move onto letter keys, alphabetically. This gets us more inputs. I would calculate it, but I am certain that I would have an off-by-one error.
         * */
        
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
        for (int i = 2; i < System.Math.Min(actionsOut.Length*2-2, 10); i+=2)
        {
            //Debug.Log(1 + i/2);
            int actionIndex = 1 + i / 2;
            actionsOut[actionIndex] = (Input.GetKey("" + (i - 2))) ? 1f : 0f;//We can set the first one with a simple = rather than += because we know that it starts at 0.
            actionsOut[actionIndex] += (Input.GetKey("" + (i - 1))) ? -1f : 0f;
            //actionsOut[i] += (Input.GetKeyDown("" + (i-1))) ? -1f : 0f;
        }
        /*int startLetters = 12;//Something weird is happening here. Letters shouldn't be used, but a,b, and d are having effects, and the debug.log isn't doing anything.
        for(int i = startLetters; i < System.Math.Min(22, actionsOut.Length*2 - 2); i+=2)
        {
            int actionIndex = 1 + i / 2;
            //Debug.Log("ActionIndex " + actionIndex);
            actionsOut[actionIndex] = (Input.GetKey("" + (char)(i + 97 - startLetters + i))) ? 1f : 0f;//Plus 97 and minus startLetters because we want to start with 'a', and we are starting at startLetters. ASCII 'a' is 97.
            actionsOut[actionIndex] += (Input.GetKey("" + (char)(i + 97 - startLetters + i + 1))) ? -1f : 0f;//Add one to get the next one for the reverse.
        }*/
        for (int i = 6; i < actionsOut.Length; i++)
        {
            actionsOut[i] = Random.value * 2 - 1;
        }
        
    }
    private float collisionDis;
    private float inverseCollisionDis;
    /*private bool IsTouching(float[] invGD)//Will only work if everything is a sphere
    {
        foreach (float f in invGD)
            if (f >= inverseCollisionDis)
                return true;

        return false;
    }*/
    private bool CapsuleTouching()
    {
        Bounds[] bar = new Bounds[joints.Length-1];
        for (int i = 0; i < joints.Length-1; i++)
        {
            bar[i] = joints[i].transform.GetChild(1).GetComponent<Collider>().bounds;
        }
        Bounds[] jbar = new Bounds[joints.Length];
        for (int i = 0; i < joints.Length; i++)
        {
            jbar[i] = joints[i].GetComponent<Collider>().bounds;
        }
        for (int i = 0; i < joints.Length; i++)
        {
            
            for (int i2 = i + 1; i2 < bar.Length; i2++)/*In retrospect, I'm not sure about setting i2 to i+1 instead of 0.
                                                        * fI'd have to reconsider neighbors if I changed it. 
                                                        * If there is a bug involving collision, look here first.
                                                        * */
            {
                if ((i2-i > 1  && i < bar.Length && bar[i].Intersects(bar[i2]))
                    || (jbar[i].Intersects(bar[i2])))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool IsTouching(float f)
    {
        return f >= inverseCollisionDis;
    }
    private float InverseGoalDis()
    {
        return Helpful.FastInverseSquareRoot(Vector3.SqrMagnitude(joints[joints.Length-1].transform.position - goal.transform.position));//Is this silly? Maybe, but it is fast and it works well enough.
    }
    private bool IsTouching(GameObject g, GameObject o)
    {
        float rads = g.transform.lossyScale.x / 2 + o.transform.lossyScale.x / 2;
        return (rads * rads) >= Vector3.SqrMagnitude(g.transform.position - o.transform.position);
    }
    /*private float[] InverseGoalDis()
    {
        float[] ar = new float[joints.Length];
        for (int i = 0; i < joints.Length; i++)
            ar[i] = Helpful.FastInverseSquareRoot(Vector3.SqrMagnitude(joints[i].transform.position - goal.transform.position));//Is this silly? Maybe, but it is fast and it works well enough.


        return ar;
    }*/

}
