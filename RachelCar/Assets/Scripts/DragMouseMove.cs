using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragMouseMove : MonoBehaviour
{

    private Plane daggingPlane;
    private Vector3 offset;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        daggingPlane = new Plane(mainCamera.transform.forward,
                              transform.position);
        Ray camRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(camRay.origin, camRay.direction * 10, Color.green);

        float planeDistance;
        daggingPlane.Raycast(camRay, out planeDistance);
        offset = transform.position - camRay.GetPoint(planeDistance);
        Debug.Log("Clicked Object");
    }
    void OnMouseDrag()
    {
        Ray camRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        float planeDistance;
        daggingPlane.Raycast(camRay, out planeDistance);
        transform.position = camRay.GetPoint(planeDistance) + offset;
    }
}