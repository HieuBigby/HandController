using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public UdpReceiver udpReceiver;
    public HandTracking leftHandTracking, rightHandTracking;

    private void Update()
    {
        string data = udpReceiver.data;

        if (!string.IsNullOrEmpty(data))
        {
            // Xóa ký tự [] 
            data = data.Remove(0, 1);
            data = data.Remove(data.Length - 1, 1);
            print(data);

            string[] segments = data.Split("'|',");

            print("Segment length: " + segments.Length);
            foreach (string segment in segments)
            {
                print(segment);
                string[] dataRaw = segment.Split(',');
                string handTypeData = dataRaw[0];
                float zoomFactor = float.Parse(dataRaw[1]);
                
                print("HandType: " + handTypeData);
                if (string.IsNullOrEmpty(handTypeData))
                {
                    continue;
                }

                string[] handPoints = dataRaw.Skip(2).ToArray();

                if(handTypeData.Contains("Left"))
                {
                    print("Control left hand!");
                    leftHandTracking.TrackHand(zoomFactor, handPoints);
                }
                else if(handTypeData.Contains("Right"))
                {
                    print("Control right hand!");
                    rightHandTracking.TrackHand(zoomFactor, handPoints);
                }
            }
        }
    }
}
