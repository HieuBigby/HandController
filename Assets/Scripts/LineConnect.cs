using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnect : MonoBehaviour
{
    public Transform origin, destination;
    public Transform overrideTransform;

    private LineRenderer lineRenderer;


    private void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;

        origin.rotation = overrideTransform.rotation;
    }

    private void Update() {
        lineRenderer.SetPosition(0, origin.position);

        //origin.LookAt(destination);
        //////origin.Rotate(0, 90f, 0); // face -x
        //origin.Rotate(90f, 0f, 0f); // face y
        
        //if(overrideTransform)
        //{
        //    //overrideTransform.rotation = origin.rotation;
        //    //var direction = destination.position - origin.position;
        //    //overrideTransform.up = direction;

        //    // Assuming you have defined your origin and destination positions
        //    Vector3 direction = destination.position - origin.position;

        //    // Calculate the desired rotation quaternion
        //    Quaternion desiredRotation = Quaternion.LookRotation(origin.forward, direction);

        //    // Apply the clamped rotation
        //    overrideTransform.rotation = desiredRotation;
        //}

        //transform.position = origin.position;
        //transform.LookAt(destination);

        lineRenderer.SetPosition(1, destination.position);
    }
}
