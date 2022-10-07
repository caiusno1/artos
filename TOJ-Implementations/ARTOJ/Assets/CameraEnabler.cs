using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEnabler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera.main.clearFlags = CameraClearFlags.Color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
