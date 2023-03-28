using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class UiBackgroundScroll: MonoBehaviour
    {
        public RawImage Image;
        public float _x = 0.01f, _y = 0.01f;

        private void Update()
        {
            Image.uvRect = new Rect(Image.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, 
                Image.uvRect.size);
        }
    }
}
