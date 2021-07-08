using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmScript : MonoBehaviour
{
    public GameObject joint;
    public GameObject capsule;
    [Header("Arm numbers")]
    public uint numJoints = 8;
    [SerializeField] float jointDis = 5;
    public float rotationSpeed = 1f;

    public GameObject[] joints;
    private float[] distances;
    public Vector2 extremeLengths;
    public List<Material> colors = new List<Material>();
    public Material capsuleColor;
    
    // Start is called before the first frame update
    void Awake()
    {
        joints = new GameObject[numJoints];
        distances = new float[numJoints];

        GameObject temp = this.gameObject;
        for (int i = 0; i < numJoints-1; i++)
        {
            temp.GetComponent<Renderer>().material = colors[i%colors.Count];
            joints[i] = temp;
            GameObject child = Instantiate(joint, new Vector3(0f, -i * jointDis, 0f), Quaternion.identity, temp.transform);
            GameObject connection = Instantiate(capsule, temp.transform);
            connection.transform.position = new Vector3(0f, -jointDis *  .5f, 0f);
            connection.GetComponent<Renderer>().material = capsuleColor;
            connection.transform.localScale = new Vector3(joint.transform.localScale.x, jointDis *.5f, joint.transform.localScale.x);
            distances[i] = jointDis;
            temp = child;
        }
        temp.GetComponent<Renderer>().material = colors[((int)numJoints - 1) % colors.Count];
        joints[numJoints - 1] = temp;
    }
    public void Reset()
    {
        //GameObject temp = this.gameObject;
        for (int i = 0; i < numJoints; i++)
        {
            joints[i].transform.rotation = Quaternion.identity;
            //GameObject child = Instantiate(joint, new Vector3(0f, -i * jointDis, 0f), Quaternion.identity, temp.transform);
            joints[i].transform.position = new Vector3(0f, -i * jointDis, 0f);
            distances[i] = jointDis;
            //Rigidbody jrb = joints[i].GetComponent<Rigidbody>();
            //rb.velocity = Vector3.zero;
            
            
            //temp = child;
        }
    }
    public void Stretch(int index, float amount)
    {

        distances[index] += amount;
        /*if(distances[index] > extremeLengths[0] && distances[index] < extremeLengths[1])
        {
            joints[index].transform.GetChild(0).transform.position -= amount * joints[index].transform.up;
        }
        else
        {
            distances[index] -= amount;//I could do something with clamping here, but speed is important. Later I decided that I wanted it to work more than I wanted to squeeze unnecessary speed out of this, hence below.
        }*/
        if (distances[index] > extremeLengths[0])
        {
            if (distances[index] < extremeLengths[1])
            {
                joints[index].transform.GetChild(0).transform.position -= amount * joints[index].transform.up;
            }
            else
            {
                joints[index].transform.GetChild(0).transform.position -= (amount - (distances[index] - extremeLengths[1])) *  joints[index].transform.up;
                distances[index] = extremeLengths[1];
            }
        }
        else
        {
            joints[index].transform.GetChild(0).transform.position -= (amount - (-extremeLengths[0] + distances[index])) * joints[index].transform.up;
            distances[index] = extremeLengths[0];
        }
        Transform capsuleTransform = joints[index].transform.GetChild(1).transform;
        Vector3 capsuleScale = capsuleTransform.localScale;
        capsuleTransform.localScale = new Vector3(capsuleScale.x, .5f*distances[index]/*- capsuleScale.y*/, capsuleScale.z);
        capsuleTransform.position = .5f * (joints[index].transform.position + joints[index + 1].transform.position);
        
    }
    // Update is called once per frame
    void Update()
    {
        
        
        
    }
}
