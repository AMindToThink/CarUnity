using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class MouseDrownCamera : Agent
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
        

        //goalCam = goalCamGo.GetComponent<Camera>();
        //goalCam.enabled = false;
        //blindPlane = goalCamGo.transform.GetChild(0).gameObject;
        //camTexture = new WebCamTexture();

        //positionAgent = GameObject.Find("Position Agent");
        //pAgent = positionAgent.GetComponent<PositionAgent>();

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
        //successList = new List<float[]>();
        //numLevel++;
        /*numLevel++;
        levelHash = Hash128.Compute(numLevel);
        levelHashFloat = Helpful.Hash128ToFloat(levelHash);
        Debug.Log(levelHashFloat);
        */
        //levelRandom = Random.value;
        //startTime = Time.time;
        /*It is unfortunate that NaN could not be passed in for the observation float value. 
         * I fear that putting Vector3.zero here will make the agent want to go to that position
         * rather than realizing that it has to explore. At least it will go towards the center, which seems
         like a good idea.*/
        //successPos = Vector3.zero;/*nanVect*/
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
        //It gets two cameras
        //sensor.AddObservation(numLevel);


        //sensor.AddObservation(levelHashFloat);
        //sensor.AddObservation(levelRandom); I feel like more observations is a bad idea.
        //sensor.AddObservation(pAgent.positionGuess);
        //sensor.AddObservation(successPos);
        //sensor.AddObservation(pAgent.rotationGuess);
        //base.CollectObservations(sensor); Was here by default but isn't in the tutorial
    }

    public float speed = 10f;
    public float swimTime = 10f;//the amount of time the rat can search before episode ends.
    public float rotationSpeed = 2f;
    public float foundReward = 1f;
    public float winBonus = 1f;
    public float drownPunish = -.05f;
    public int numBeforeChanging = 5;
    private int numFound = 0;

    public Vector3 successPos;
    
    //private long dodged = 0;
    public override void OnActionReceived(float[] vectorAction)
    {
        //float[] copy = { vectorAction[0], vectorAction[1] };
        //successList.Add(copy);
        //Vector3 controlSignal = Vector3.zero;
        
        this.transform.localRotation = Quaternion.Euler(0f, transform.localRotation.eulerAngles.y + rotationSpeed * vectorAction[0], 0f);
        //controlSignal.x = vectorAction[0];
        //controlSignal.z = vectorAction[1];
        //transform.localPosition += controlSignal * speed;
        //rb.AddForce(controlSignal * speed);
        //rb.MovePosition(transform.localPosition + Vector3.Normalize(controlSignal) * speed * Time.deltaTime);
        rb.velocity = vectorAction[1] * transform.forward * speed;
        //Debug.Log("Good enough: " + controlSignal);

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
                //startTime = Time.time;
                
                //Take this out of the comment if using the two cameras
                /*if (!goalCam.enabled)
                {
                    goalCam.enabled = true;
                    goalCamGo.transform.parent = transform.parent;
                    goalCam.cullingMask++;
                }
                else
                {
                    goalCamGo.transform.position = camera.transform.position;
                    goalCamGo.transform.rotation = camera.transform.rotation;
                }
                */





                ResetPos();


            }

        }
        else
        {
            AddReward(drownPunish);//Try 1 instead
            
            //dodged++;
            //SetReward(1f);
            //obstacle.localPosition = new Vector3(Random.value * 4.7f - 2.35f, 5.76f, 0f);
            //EndEpisode();

            //Debug.Log("Glug");

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
    private void ResetPos()
    {
        transform.localPosition = dropPos;
        this.transform.localRotation = Quaternion.Euler(0f, dropRot, 0f);
    }
    private bool TouchingPlatform()
    {
        return thisCollider.bounds.Intersects(platformCollider.bounds);
    }
    /*private byte[] ImageBytes()
    {
        RenderTexture rt = camera.activeTexture;
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(cSensor.Width, cSensor.Height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0,0, cSensor.Width, cSensor.Height), 0, 0);
        RenderTexture.active = null;

        byte[] bytes = new byte[];
        Color[] colorAr= tex.GetPixels(0, 0, cSensor.Width, cSensor.Height);
        foreach (Color c in colorAr)
        {

        }
        bytes = tex.EncodeToPNG();
        tex.encod
        return bytes;
        //string path = AssetDatabase.GetAssetPath(rt) + ".png";
        //System.IO.File.WriteAllBytes(path, bytes);
        //AssetDatabase.ImportAsset(path);
        //Debug.Log("Saved to " + path);
    }*/
    /*IEnumerator TakePhoto()
    {
        yield return new WaitForEndOfFrame();

        Texture2D photo = new Texture2D(camTexture.width, camTexture.height);
        photo.SetPixels(camTexture.GetPixels());
        photo.Apply();

        //Encode to a PNG
        byte[] bytes = photo.EncodeToPNG();

        File.WriteAllBytes("/Users/matthew.khoriaty/Desktop/GitHub/MatthewML/RatCup/Assets photo.png", bytes);
        Debug.Log("After image");
    }*/
    //public bool foundIt = false;
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
    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("entered");
        if (collision.gameObject.CompareTag("Finish"))
        {
            foundIt = true;
            //Debug.Log("Touched");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            foundIt = false;
        }
    }
    */
    /*
    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("entered");
        if (collision.gameObject.CompareTag("Finish"))
        {
            foundIt = true;
            //Debug.Log("Touched");
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            foundIt = false;
        }
    }
    */
}
