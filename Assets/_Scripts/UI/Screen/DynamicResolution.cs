using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DynamicResolution : MonoBehaviour
{
    public TextMeshProUGUI screenText;

    private Vector2 MainRes;
    public bool enableWrapping;
    public float CurScale = 1;
    public float MinScale = 0.5f;
    public float ScaleStep = 0.05f;

    public int MinFPS = 40;
    public int MaxFPS = 55;
    private float MinFPSS;
    private float MaxFPSS;
    public float Delay;
    private float DelayTime;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        MainRes = new Vector2(Screen.width, Screen.height);
        DelayTime = Delay;
        MinFPSS = 1f / (float)MinFPS;
        MaxFPSS = 1f / (float)MaxFPS;
    }

    float displayTime;
    float updateInterval = 0.3f;
    void Update()
    {
        if (!enableWrapping) return;

        if (Time.time > DelayTime)
        {
            if (Time.deltaTime > MinFPSS)
            {
                if (CurScale > MinScale)
                {
                    CurScale -= ScaleStep;
                    Screen.SetResolution((int)(MainRes.x * CurScale), (int)(MainRes.y * CurScale), true);
                    DelayTime = Time.time + Delay;
                }
            }
            else
            if (CurScale < 1 && Time.deltaTime < MaxFPSS)
            {
                CurScale += ScaleStep;
                Screen.SetResolution((int)(MainRes.x * CurScale), (int)(MainRes.y * CurScale), true);
                DelayTime = Time.time + Delay;
            }
            DelayTime = Time.time + 0.5f;
        }

        if(screenText)
        {
            displayTime += Time.deltaTime;

            if (displayTime > updateInterval)
            {
                displayTime = 0;

                screenText.text = "X " + Screen.width +
                    " / Y " + Screen.height + " / scale " + CurScale + " / " + Time.deltaTime;
            }
        }
    }
}