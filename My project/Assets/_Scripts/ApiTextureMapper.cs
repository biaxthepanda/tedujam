using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiTextureMapper : MonoBehaviour
{
    [Header("API Credentials")]
    public string apiKey = "";
    public string panID = "";

    [Header("Image Settings")]
    [Tooltip("Cubemaps must be perfectly square. Google API max free size is 640.")]
    public int imageSize = 640;

    private Cubemap panoramaCubemap;

    void Start()
    {
        // A temporary trigger just for testing in the Editor
        if (!string.IsNullOrEmpty(panID))
        {
            StartCoroutine(BuildCubemap());
        }
    }

    private IEnumerator BuildCubemap()
    {
        // Initialize an empty Cubemap. RGB24 is the standard format for web images.
        panoramaCubemap = new Cubemap(imageSize, TextureFormat.RGB24, false);

        Debug.Log("Starting 6-face panorama download...");

        // Fetch all 6 faces sequentially to avoid overloading the network
        // Headings: 0=Front, 90=Right, 180=Back, 270=Left
        // Pitch: 90=Up, -90=Down
        yield return StartCoroutine(FetchFace(0, 0, CubemapFace.PositiveZ));   // Front
        yield return StartCoroutine(FetchFace(90, 0, CubemapFace.PositiveX));  // Right
        yield return StartCoroutine(FetchFace(180, 0, CubemapFace.NegativeZ)); // Back
        yield return StartCoroutine(FetchFace(270, 0, CubemapFace.NegativeX)); // Left
        yield return StartCoroutine(FetchFace(0, 90, CubemapFace.PositiveY));  // Up
        yield return StartCoroutine(FetchFace(0, -90, CubemapFace.NegativeY)); // Down

        // Apply all the downloaded pixels to the cubemap asset
        panoramaCubemap.Apply();

        ApplyToSphere(panoramaCubemap);
    }

    private IEnumerator FetchFace(int heading, int pitch, CubemapFace faceTarget)
    {
        string apiUrl = $"https://maps.googleapis.com/maps/api/streetview?size={imageSize}x{imageSize}&pano={panID}&heading={heading}&pitch={pitch}&fov=90&key={apiKey}";

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"API Error on {faceTarget}: {request.error}");
            }
            else
            {
                Texture2D downloadedTex = DownloadHandlerTexture.GetContent(request);

                // Extract the raw pixels from the downloaded texture
                Color[] pixels = downloadedTex.GetPixels();

                // FLIP THE PIXELS VERTICALLY HERE:
                Color[] flippedPixels = FlipPixelsVertically(pixels, downloadedTex.width, downloadedTex.height);

                // Paint the flipped pixels onto the correct face of our Cubemap
                panoramaCubemap.SetPixels(flippedPixels, faceTarget);
            }
        }
    }

    private void ApplyToSphere(Cubemap cubemap)
    {
        Material cubemapMaterial = new Material(Shader.Find("Skybox/Cubemap"));
        cubemapMaterial.SetTexture("_Tex", cubemap);

        // DELETE OR COMMENT OUT THIS LINE:
        // GetComponent<Renderer>().material = cubemapMaterial;

        // ADD THIS LINE INSTEAD:
        RenderSettings.skybox = cubemapMaterial;

        Debug.Log("6-Sided Cubemap successfully built and applied to the World Environment!");
    }

    private Color[] FlipPixelsVertically(Color[] original, int width, int height)
    {
        Color[] flipped = new Color[original.Length];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Swap the row order from bottom-to-top to top-to-bottom
                flipped[y * width + x] = original[(height - 1 - y) * width + x];
            }
        }
        return flipped;
    }
}