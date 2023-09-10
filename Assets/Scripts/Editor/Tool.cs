using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tool : ScriptableWizard
{
    public GameObject newHand;
    public GameObject handTracking;


    [MenuItem("Tools/Hand Tool")]
    public static void Init()
    {
        EditorWindow.GetWindow(typeof(Tool), false, "Tool");
    }

    private void OnGUI()
    {
        base.DrawWizardGUI();

        if(GUILayout.Button("Change new Hand"))
        {
            OverrideRotation[] overrideRotations = handTracking.GetComponentsInChildren<OverrideRotation>(true);
            OverridePosition[] overridePositions = handTracking.GetComponentsInChildren<OverridePosition>(true);
            Transform[] newHandChildren = newHand.GetComponentsInChildren<Transform>(true);

            // Thay OverrideRotation
            foreach (OverrideRotation overrideRotation in overrideRotations)
            {
                if (!overrideRotation.overrideTransform)
                {
                    Debug.LogError(overrideRotation.name + " không có override transform", overrideRotation);
                    continue;
                }
                Transform overrideTransform = overrideRotation.overrideTransform;

                bool found = false;
                foreach (Transform child in newHandChildren)
                {
                    if (overrideTransform.name == child.name)
                    {
                        Undo.RecordObject(overrideRotation, "Change Rotation Override");
                        overrideRotation.overrideTransform = child;
                        found = true;
                        break;
                    }
                }

                if (!found) Debug.LogError("Không tìm thấy " + overrideTransform.name + " để thay Override");
            }

            // Thay OverridePosition
            foreach (OverridePosition overridePosition in overridePositions)
            {
                if (!overridePosition.overrideTransform)
                {
                    Debug.LogError(overridePosition.name + " không có override transform", overridePosition);
                    continue;
                }
                Transform overrideTransform = overridePosition.overrideTransform;

                bool found = false;
                foreach (Transform child in newHandChildren)
                {
                    if (overrideTransform.name == child.name)
                    {
                        Undo.RecordObject(overridePosition, "Change Position Override");
                        overridePosition.overrideTransform = child;
                        found = true;
                        break;
                    }
                }

                if (!found) Debug.LogError("Không tìm thấy " + overrideTransform.name + " để thay Override");
            }
        }
    }
}
