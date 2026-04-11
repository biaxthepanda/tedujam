using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiTextureMapper : MonoBehaviour
{
    [Header("API Credentials")]
    public string apiKey = "";
    public string panID = "";

    [Header("UI Reference")]
    public GameObject mapUI; // Drag your MapScrollView here

    [Header("Image Settings")]
    [Tooltip("Cubemaps must be perfectly square. Google API max free size is 640.")]
    public int imageSize = 640;

    private Cubemap panoramaCubemap;

    public void SetPanorama(string newID)
    {
        panID = newID;

        if (!string.IsNullOrEmpty(panID))
        {
            // We start the 6-face process here
            StartCoroutine(BuildCubemap());
        }
    }

    private void Start()
    {

    }

    private IEnumerator BuildCubemap()
    {
        panoramaCubemap = new Cubemap(imageSize, TextureFormat.RGB24, false);

        Debug.Log("Starting 6-face panorama download...");

        // Sequential download of the 6 faces
        yield return StartCoroutine(FetchFace(0, 0, CubemapFace.PositiveZ));   // Front
        yield return StartCoroutine(FetchFace(90, 0, CubemapFace.PositiveX));  // Right
        yield return StartCoroutine(FetchFace(180, 0, CubemapFace.NegativeZ)); // Back
        yield return StartCoroutine(FetchFace(270, 0, CubemapFace.NegativeX)); // Left
        yield return StartCoroutine(FetchFace(0, 90, CubemapFace.PositiveY));  // Up
        yield return StartCoroutine(FetchFace(0, -90, CubemapFace.NegativeY)); // Down

        panoramaCubemap.Apply();

        // Now that the cube is fully built, apply it and hide the UI
        ApplyToSkybox(panoramaCubemap);
    }

    private IEnumerator FetchFace(int heading, int pitch, CubemapFace faceTarget)
    {

        string apiUrl = $"https://maps.googleapis.com/maps/api/streetview?size={imageSize}x{imageSize}&pano={panID}&heading={heading}&pitch={pitch}&fov=90&key={apiKey}";
        Debug.Log(apiUrl);

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"API Error on {faceTarget}: {request.error}");
            }
            else
            {
                Texture2D downloadedTex = DownloadHandlerTexture.GetContent(request);
                Color[] pixels = downloadedTex.GetPixels();
                Color[] flippedPixels = FlipPixelsVertically(pixels, downloadedTex.width, downloadedTex.height);

                panoramaCubemap.SetPixels(flippedPixels, faceTarget);
            }
        }
    }

    private void ApplyToSkybox(Cubemap cubemap)
    {
        // Create the Skybox material
        Material cubemapMaterial = new Material(Shader.Find("Skybox/Cubemap"));
        cubemapMaterial.SetTexture("_Tex", cubemap);

        // Apply it to the world
        RenderSettings.skybox = cubemapMaterial;

        // HIDE THE 2D MAP NOW - The view is ready!
        if (mapUI != null)
        {
            mapUI.SetActive(false);
        }

        Debug.Log("Skybox updated and Map UI hidden!");
    }

    private Color[] FlipPixelsVertically(Color[] original, int width, int height)
    {
        Color[] flipped = new Color[original.Length];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                flipped[y * width + x] = original[(height - 1 - y) * width + x];
            }
        }
        return flipped;
    }
}