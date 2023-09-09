using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLimiter : MonoBehaviour
{
    public float maxRotationX = 90.0f; // Maximum rotation angle along the X-axis
    private Quaternion originalRotation;
    private Vector3 originalDirection; // Forward Axis

    private void Awake()
    {
        // Store the original rotation of the object
        originalRotation = transform.rotation;
        originalDirection = transform.forward;
    }

    private void Update()
    {
        // align forward vectors to the XZ plane / Y axis first
        Vector3 sourceForward = transform.forward;
        Vector3 upConstant = Vector3.up;
        Vector3.OrthoNormalize(ref upConstant, ref sourceForward);
        Vector3 targetForward = originalDirection;
        Vector3.OrthoNormalize(ref upConstant, ref targetForward);

        // get the angle between both transforms arount the Y axis
        float dot = Vector3.Dot(sourceForward, targetForward);
        float angleRad = Mathf.Acos(dot);
        float angleDeg = angleRad * Mathf.Rad2Deg;

        // see if the angle is clockwise or counter-clockwise
        Vector3 up = Vector3.Cross(sourceForward, targetForward);
        angleDeg = up.y < 0 ? -angleDeg : +angleDeg;

        Debug.Log("Angle Y: " + angleDeg);

        //// align forward vectors to the XZ plane / Y axis first
        //Vector3 forwardConstant = Vector3.forward;
        //Vector3.OrthoNormalize(ref forwardConstant, ref sourceForward);
        //Vector3.OrthoNormalize(ref forwardConstant, ref targetForward);

        //// get the angle between both transforms arount the Y axis
        //dot = Vector3.Dot(sourceForward, targetForward);
        //angleRad = Mathf.Acos(dot);
        //angleDeg = angleRad * Mathf.Rad2Deg;

        //// see if the angle is clockwise or counter-clockwise
        //Vector3 forward = Vector3.Cross(sourceForward, targetForward);
        //angleDeg = forward.x < 0 ? -angleDeg : +angleDeg;

        //Debug.Log("Angle X: " + angleDeg);


        //// Get the current rotation of the object
        //Quaternion currentRotation = transform.rotation;
        //Vector3 currentDirection = transform.forward;

        //Quaternion changeInRotation = Quaternion.FromToRotation(currentDirection, originalDirection);
        //Vector3 euler = changeInRotation.eulerAngles;
        //Debug.Log("Diffrence: " + euler);


        //// Calculate the rotation difference from the original rotation along the X-axis
        //float xRotationDifference = Quaternion.Angle(originalRotation, currentRotation);

        //Debug.Log("Rotation Diffrence: " + xRotationDifference);

        //// If the X-axis rotation exceeds the maximum allowed angle, clamp it
        //if (xRotationDifference > maxRotationX)
        //{
        //    Debug.Log("Exceed rotation:" + originalRotation);

        //    // Calculate the clamped rotation along the X-axis
        //    Quaternion clampedRotation = originalRotation * Quaternion.Euler(maxRotationX, 0, 0);

        //    // Set the object's rotation to the clamped rotation
        //    transform.rotation = clampedRotation;
        //}
    }
}
