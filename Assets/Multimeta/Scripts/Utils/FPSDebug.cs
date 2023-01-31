using TMPro;
using UnityEngine;

public class FPSDebug : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsDebugLabel;

    // Update is called once per frame
    void Update()
    {
        float fps = 1.0f / Time.deltaTime;
        fpsDebugLabel.text = "FPS: " + fps.ToString("00");
    }
}
