using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Playstel
{
    public class FpsCounter : MonoBehaviour
    {
        [Header("UI")]
        public TextMeshProUGUI fpsText;

        [Header("Settings")]
        public float updateInterval = 0.05f;
        public float lowFpsThresehold = 10;
        public bool enable;
        public bool receiveLowFPS;

        [Header("Other")]
        public float displayTime;

        public void Start()
        {
            if (!enable)
            {
                if(fpsText) fpsText.gameObject.SetActive(false);
            }
        }

        public void Update()
        {
            if (!enable) return;

            displayTime += Time.deltaTime;

            if(displayTime > updateInterval)
            {
                displayTime = 0;
                FpsDisplay(1.0f / Time.deltaTime);
            }
        }

        public List<int> fpsTable = new List<int>();
        public void FpsDisplay(float value)
        {
            int roundedFps = (int)value;
            fpsText.text = roundedFps.ToString();

            if (!receiveLowFPS) return;

            if (roundedFps < lowFpsThresehold)
            {
                Debug.Log("Low FPS: " + roundedFps);
            }
        }

    }
}
