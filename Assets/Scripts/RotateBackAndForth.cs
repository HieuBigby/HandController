using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBackAndForth : MonoBehaviour
{
    public float rotationSpeed = 30.0f; // Rotation speed in degrees per second
    public float rotationRange = 30.0f; // Rotation range in degrees

    private float currentRotation = 0.0f;
    private int direction = 1; // 1 for clockwise, -1 for counterclockwise

    private void Update()
    {
        // Calculate the new rotation amount
        float rotationAmount = rotationSpeed * direction * Time.deltaTime;

        // Update the current rotation
        currentRotation += rotationAmount;

        // Check if the current rotation exceeds the range
        if (Mathf.Abs(currentRotation) > rotationRange)
        {
            // Change direction to reverse the rotation
            direction *= -1;

            // Calculate the remaining rotation needed to stay within the range
            float remainingRotation = rotationRange - Mathf.Abs(currentRotation);

            // Update the current rotation
            currentRotation = rotationRange + direction * remainingRotation;
        }

        // Apply the rotation to the GameObject's local rotation
        transform.Rotate(Vector3.right, rotationAmount);
    }
}
