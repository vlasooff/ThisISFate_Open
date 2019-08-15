using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenMonitor : MonoBehaviour
{


    // Update is called once per frame
    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.F9))
        {

        }
    }
    [EasyButtons.Button]
    public void ScreenShot()
    {
        ScreenCapture.CaptureScreenshot($"screen_{Time.time}.png");
    }
}
