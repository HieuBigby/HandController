using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnect : MonoBehaviour
{
    public Transform origin, destination;

    private LineRenderer lineRenderer;


    private void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
    }

    private void Update() {
        lineRenderer.SetPosition(0, origin.position);

        lineRenderer.SetPosition(1, destination.position);
    }
}
