using UnityEngine;

namespace NukeFactory
{
    public class RectRotate : MonoBehaviour
    {
        public bool autoRotation;
        public int speed;

        RectTransform rect;
        Vector3 v;
        Quaternion q;

        public void Start()
        {
            if (GetComponent<RectTransform>())
                rect = GetComponent<RectTransform>();
        }

        public void Update()
        {
            if (autoRotation)
            {
                v.x = 0;
                v.y = 0;
                v.z += 1 * Time.deltaTime * speed;

                q = Quaternion.Euler(v);
                rect.transform.localRotation = q;
            }
        }
    }
}
