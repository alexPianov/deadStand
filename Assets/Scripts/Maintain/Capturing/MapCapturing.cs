using System.IO;
using UnityEngine;

namespace Playstel
{
    public class MapCapturing : MonoBehaviour
    {
        public int superSize;
        public string directoryName;
        public string fileName;
        public ImageFormat imageFormat;
        
        public enum ImageFormat 
        {
            png, jpg
        }
        
        [ContextMenu("TakeScreenshot")]
        public void TakeScreenshot()
        {
            DirectoryInfo screenshotDirectory = Directory.CreateDirectory(directoryName);
            string fullPath = Path.Combine(screenshotDirectory.FullName, fileName + "." + imageFormat);

            ScreenCapture.CaptureScreenshot(fullPath, superSize);
        }
    }
}