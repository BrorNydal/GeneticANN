using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasToMainCamera : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
