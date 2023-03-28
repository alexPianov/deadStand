using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;

public class IconCapture : MonoBehaviour
{
    [Header("Camera")]
    public Camera mainCamera;

    public List<Transform> itemsContainers;

    public string folderIsAssets = "Icons";

    private int frameRate = 60;

    private float cameraSize = 0.14f; 

    private int millisecondsToWaitBeforeStart = 0;
    private float cameraX = -0.14f;
    private float cameraY = 0.0f;
    private float cameraZ = 0.07f;

    private Camera whiteCam;
    private Camera blackCam;

    // Pixels to World Unit size
    private float pixelsToWorldUnit = 74.48275862068966f;

    // If you have Unity Pro you can use a RenderTexture which will render the full camera width, otherwise it will only render half
    private bool useRenderTexture = true;

    private int videoframe = 0; // how many frames we've rendered

    private float originaltimescaleTime; // track the original time scale so we can freeze the animation between frames

    private string realFolder = ""; // real folder where the output files will be

    private bool done = false; // is the capturing finished?

    private bool readyToCapture = false;  // Make sure all the camera setup is complete before capturing

    private Texture2D texb; // black camera texture

    private Texture2D texw; // white camera texture

    private Texture2D outputtex; // final output texture

    private RenderTexture blackCamRenderTexture; // black camera render texure

    private RenderTexture whiteCamRenderTexture; // white camera render texure

    private bool ortographic = true;

    private void Start()
    {
        cameraSize = mainCamera.orthographicSize;

        useRenderTexture = Application.HasProLicense();

        // Set the playback framerate!
        // (real time doesn't influence time anymore)
        Time.captureFramerate = frameRate;

        originaltimescaleTime = Time.timeScale;

        GameObject bc = new GameObject("Black Camera");
        bc.transform.localPosition = new Vector3(cameraX, cameraY, cameraZ);
        bc.transform.Rotate(new Vector3(90, 0, 0));
        blackCam = bc.AddComponent<Camera>();
        blackCam.backgroundColor = Color.black;
        blackCam.clearFlags = CameraClearFlags.SolidColor;
        blackCam.orthographic = ortographic;
        blackCam.orthographicSize = cameraSize;
        blackCam.tag = "MainCamera";

        GameObject wc = new GameObject("White Camera");
        wc.transform.localPosition = new Vector3(cameraX, cameraY, cameraZ);
        wc.transform.Rotate(new Vector3(90, 0, 0));
        whiteCam = wc.AddComponent<Camera>();
        whiteCam.backgroundColor = Color.white;
        whiteCam.clearFlags = CameraClearFlags.SolidColor;
        whiteCam.orthographic = true;
        whiteCam.orthographicSize = cameraSize;

        // If not using a Render Texture then set the cameras to split the screen to ensure we have an accurate image with alpha
        if (!useRenderTexture)
        {
            // Change the camera rects to have split on screen to capture the animation properly
            blackCam.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);

            whiteCam.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
        }
        // Cameras are set ready to capture!

        System.Threading.Timer timer = null;
        timer = new System.Threading.Timer((obj) =>
        {
            readyToCapture = true;
            timer.Dispose();
        },
                null, millisecondsToWaitBeforeStart, System.Threading.Timeout.Infinite);
    }

    [ContextMenu("StartCapturingProcess")]
    public async void StartCapturingProcess()
    {
        if (!readyToCapture)
        {
            Debug.Log("Script components not ready yet");
            return;
        }

        foreach (var itemsContainer in itemsContainers)
        {
            await CaptureItemsInContainer(itemsContainer);
        }
    }

    private async UniTask CaptureItemsInContainer(Transform itemsContainer)
    {
        for (int i = 0; i < itemsContainer.childCount; i++)
        {
            itemsContainer.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < itemsContainer.childCount; i++)
        {
            var item = itemsContainer.GetChild(i).gameObject;
            
            item.SetActive(true);
            await MakeCapture(item.name, itemsContainer.name);
            item.SetActive(false);
        }
    }

    private string GetFoldierName(string containerName)
    {
        var foldierName = folderIsAssets + "/" + containerName;

        if (!Directory.Exists(foldierName))
            Directory.CreateDirectory(foldierName);

        return foldierName;
    }

    public async UniTask MakeCapture(string itemName, string containerName)
    {
        done = false;
        StartCoroutine(CaptureItem(itemName, containerName));
        await UniTask.WaitUntil(() => done);
        RemoveScreenSetup();
        done = false;
    }

    private void RemoveScreenSetup()
    {
        DestroyImmediate(texb);
        DestroyImmediate(texw);
        DestroyImmediate(outputtex);

        if (useRenderTexture)
        {
            //Clean Up
            whiteCam.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(whiteCamRenderTexture);

            blackCam.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(blackCamRenderTexture);
        }
    }

    private IEnumerator CaptureItem(string itemName, string containerName)
    {
            // name is "realFolder/animationName0000.png"
            // string name = realFolder + "/" + animationName + Time.frameCount.ToString("0000") + ".png";
            string filename = String.Format("{0}/" + itemName + ".png", GetFoldierName(containerName));
            
            // Stop time
            Time.timeScale = 0;
            // Yield to next frame and then start the rendering
            yield return new WaitForEndOfFrame();

            // If we are using a render texture to make the animation frames then set up the camera render textures
            if (useRenderTexture)
            {
                //Initialize and render textures
                blackCamRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
                whiteCamRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);

                blackCam.targetTexture = blackCamRenderTexture;
                blackCam.Render();
                RenderTexture.active = blackCamRenderTexture;
                texb = GetTex2D(true);

                //Now do it for Alpha Camera
                whiteCam.targetTexture = whiteCamRenderTexture;
                whiteCam.Render();
                RenderTexture.active = whiteCamRenderTexture;
                texw = GetTex2D(true);
            }
            // If not using render textures then simply get the images from both cameras
            else
            {
                // store 'black background' image
                texb = GetTex2D(true);

                // store 'white background' image
                texw = GetTex2D(false);
            }

            // If we have both textures then create final output texture
            if (texw && texb)
            {

                int width = Screen.width;
                int height = Screen.height;

                // If we are not using a render texture then the width will only be half the screen
                if (!useRenderTexture)
                {
                    width = width / 2;
                }

                outputtex = new Texture2D(width, height, TextureFormat.ARGB32, false);

                // Create Alpha from the difference between black and white camera renders
                for (int y = 0; y < outputtex.height; ++y)
                {
                    // each row
                    for (int x = 0; x < outputtex.width; ++x)
                    {
                        // each column
                        float alpha;
                        if (useRenderTexture)
                        {
                            alpha = texw.GetPixel(x, y).r - texb.GetPixel(x, y).r;
                        }
                        else
                        {
                            alpha = texb.GetPixel(x + width, y).r - texb.GetPixel(x, y).r;
                        }

                        alpha = 1.0f - alpha;
                        Color color;
                        //Debug.Log(alpha);
                        if (alpha < 0.01f)
                        {
                            color = Color.clear;
                        }
                        else
                        {
                            color = texb.GetPixel(x, y) / alpha;
                        }
                        //if (Math.Abs(color.r - 103.0f) < 100.0f &&
                        //    Math.Abs(color.g - 97.0f) < 100.0f &&
                        //    Math.Abs(color.b - 92.0f) < 100.0f) {
                        //    color = Color.clear;
                        //    alpha = 0.0f;
                        //}

                        color.a = alpha;
                        outputtex.SetPixel(x, y, color);
                    }
                }

                // Encode the resulting output texture to a byte array then write to the file
                byte[] pngShot = outputtex.EncodeToPNG();
                File.WriteAllBytes(filename, pngShot);

                // Reset the time scale, then move on to the next frame.
                Time.timeScale = originaltimescaleTime;
                done = true;
            }
    }

    // Get the texture from the screen, render all or only half of the camera
    private Texture2D GetTex2D(bool renderAll)
    {
        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        if (!renderAll)
        {
            width = width / 2;
        }

        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        return tex;
    }
}