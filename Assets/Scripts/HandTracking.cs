using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define a class to store information for each keypoint
public class KeypointInfo
{
    public Vector3 currentPosition;
    public Vector3 targetPosition;
}

public class HandTracking : MonoBehaviour
{
    public UdpReceiver udpReceiver;
    public Vector3 ratio;
    public float moveSpeed = 1f;
    public float xZoomFactor = 2f;
    public float xOffset = 10f;
    public GameObject[] handPoints;
    public GameObject handModel;
    public float zoomMultipler = 1;
    public float positionChangeThreshold = 0.001f; // Adjust as needed
    // Smoothing factor (0 = no smoothing, 1 = full smoothing)
    public float smoothingFactor = 1f; // Adjust as needed

    // Initialize an array to store information for each keypoint
    private KeypointInfo[] keypointInfos = new KeypointInfo[21];


    public void TrackHand(float zoomFactor, string[] points)
    {
        if (points == null) return;
        //handModel.transform.localScale = Vector3.one * zoomFactor * zoomMultipler;

        for (int i = 0; i < 21; i++)
        {
            float x = xOffset - float.Parse(points[i * 3]) / ratio.x;
            float y = float.Parse(points[i * 3 + 1]) / ratio.y;
            float z = float.Parse(points[i * 3 + 2]) / ratio.z;

            float centerX = xOffset - 320 / ratio.x;
            float xPoint0 = xOffset - float.Parse(points[0]) / ratio.x;
            Vector3 newPosition = new Vector3(x, y, z - zoomFactor * moveSpeed);
            if (xPoint0 > centerX)
            {
                //Debug.Log("xPoint0 ở bên phải, giảm x về phía bên trái: " + xPoint0 + "/" + centerX);
                newPosition.x -= zoomFactor * xZoomFactor;
            }
            else
            {
                //Debug.Log("xPoint0 ở bên trái, đưa x về phía bên phải: " + xPoint0 + "/" + centerX);
                newPosition.x += zoomFactor * xZoomFactor;
            }

            //Vector3 newPosition = new Vector3(x, y, z);

            if (keypointInfos[i] == null)
            {
                // If no information exists for this keypoint, create a new entry
                keypointInfos[i] = new KeypointInfo
                {
                    currentPosition = newPosition,
                    targetPosition = newPosition
                };
            }
            else
            {
                //// Update the target position for smoothing
                //keypointInfos[i].targetPosition = newPosition;
                if (Vector3.Distance(keypointInfos[i].targetPosition, newPosition) > positionChangeThreshold)
                {
                    keypointInfos[i].targetPosition = newPosition;
                }

                // Smoothly interpolate between the current and target positions
                keypointInfos[i].currentPosition = Vector3.Lerp(keypointInfos[i].currentPosition, keypointInfos[i].targetPosition, smoothingFactor);


            }

            // Set the position of the handPoint
            handPoints[i].transform.position = keypointInfos[i].currentPosition;
        }

        // Xoay mô hình bàn tay theo hướng của lòng bàn tay 
        transform.position = handPoints[0].transform.position;
        Vector3 vector_0_5 = handPoints[5].transform.position - handPoints[0].transform.position;
        Vector3 vector_0_17 = handPoints[17].transform.position - handPoints[0].transform.position;
        if(handModel.name.Contains("left"))
        {
            transform.forward = Vector3.Cross(vector_0_5, vector_0_17);
        }
        else if (handModel.name.Contains("right"))
        {
            transform.forward = -Vector3.Cross(vector_0_5, vector_0_17);
        }
    }
}
