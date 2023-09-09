using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverrideRotation : MonoBehaviour
{
    public Transform overrideTransform;

    private void Awake()
    {
        transform.rotation = overrideTransform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //Quaternion parentRotation = Quaternion.LookRotation(overrideTransform.forward);
        //transform.rotation = parentRotation;    

        //Quaternion rotation = Quaternion.LookRotation(transform.forward, transform.up);
        //overrideTransform.rotation = rotation;

        overrideTransform.rotation = transform.rotation;
    }
}
