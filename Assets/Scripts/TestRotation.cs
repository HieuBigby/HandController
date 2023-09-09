using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if (target != null)
        {
            // Calculate the direction vector from the object to the target
            Vector3 directionToTarget = target.position - transform.position;

            // Calculate the rotation quaternion to face the target only along the X and Y axes
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, directionToTarget.y, 0));

            // Apply the rotation to the object, preserving the Z-axis rotation
            transform.rotation = targetRotation;
        }
    }

    //public Transform target;

    //private Vector3 originalForward;

    //private void Start()
    //{
    //    originalForward = transform.forward;
    //}

    //private void Update()
    //{
    //    transform.LookAt(target);

    //    Quaternion quaternion = Quaternion.FromToRotation(originalForward, transform.forward);
    //    Debug.Log(quaternion.eulerAngles);
    //}
}
