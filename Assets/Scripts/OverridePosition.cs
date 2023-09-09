using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverridePosition : MonoBehaviour
{
    public Transform overrideTransform;

    private void Update()
    {
        overrideTransform.position = transform.position;
    }
}
