using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class AutoSetCanvasScale : MonoBehaviour
{

    private void Awake()
    {
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        var p = (float)Screen.width / Screen.height;

        if (p > 0.6f)
        {
            scaler.matchWidthOrHeight = 1f;
        }
        else
        {
            scaler.matchWidthOrHeight = 1f;
        }

    }


}
