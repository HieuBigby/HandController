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
    public GameObject[] handPoints;
    public float positionChangeThreshold = 0.001f; // Adjust as needed

    // Initialize an array to store information for each keypoint
    private KeypointInfo[] keypointInfos = new KeypointInfo[21];
    // Smoothing factor (0 = no smoothing, 1 = full smoothing)
    public float smoothingFactor = 1f; // Adjust as needed


    private void Update() 
    {
        string data = udpReceiver.data;

        if(!string.IsNullOrEmpty(data))
        {
            data = data.Remove(0, 1);
            data = data.Remove(data.Length - 1, 1);
            print(data);

            string[] points = data.Split(',');

            for (int i = 0; i < 21; i++)
            {
                float x = 10 - float.Parse(points[i * 3]) / 100f;
                float y = float.Parse(points[i * 3 + 1]) / 100f;
                float z = float.Parse(points[i * 3 + 2]) / 30f;

                Vector3 newPosition = new Vector3(x, y, z);

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

                ////handPoints[i].transform.localPosition = new Vector3(x, y, z);
                //handPoints[i].transform.position = new Vector3(x, y, z);    
            }
        }
    }
}
