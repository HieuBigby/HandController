using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tool : ScriptableWizard
{
    public GameObject oldHand, newHand;
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
            LineConnect[] lineConnects = handTracking.GetComponentsInChildren<LineConnect>(true);
            Transform[] newHandChildren = newHand.GetComponentsInChildren<Transform>(true);

            foreach(LineConnect lineConnect in lineConnects)
            {
                if (!lineConnect.overrideTransform) continue;
                Transform overrideTransform = lineConnect.overrideTransform;

                bool found = false;
                foreach(Transform child in newHandChildren)
                {
                    if(overrideTransform.name == child.name)
                    {
                        Undo.RecordObject(lineConnect, "Change Override");
                        lineConnect.overrideTransform = child;
                        found = true;
                        break;
                    }
                }

                if(!found) Debug.LogError("Không tìm thấy " + overrideTransform.name + " để thay Override"); 
            }
        }
    }
}
