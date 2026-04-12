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

				// 1. Flip them
				Color[] processedPixels = FlipPixelsVertically(pixels, downloadedTex.width, downloadedTex.height);

				// 2. DITHER THEM (0.05f is a good starting intensity)
				processedPixels = ApplyDither(processedPixels, 0.1f);

				panoramaCubemap.SetPixels(processedPixels, faceTarget);
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
        UIMAnager.Instance.ToggleMapUI(false);

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


	private Color[] ApplyDither(Color[] pixels, float intensity)
	{
		for (int i = 0; i < pixels.Length; i++)
		{
			// Generate a small random offset for each color channel
			float rNoise = Random.Range(-intensity, intensity);
			float gNoise = Random.Range(-intensity, intensity);
			float bNoise = Random.Range(-intensity, intensity);

			// Apply noise and clamp between 0 and 1
			pixels[i].r = Mathf.Clamp01(pixels[i].r + rNoise);
			pixels[i].g = Mathf.Clamp01(pixels[i].g + gNoise);
			pixels[i].b = Mathf.Clamp01(pixels[i].b + bNoise);

			// OPTIONAL: Quantize (crunch) the colors for a 16-bit look
			pixels[i].r = Mathf.Round(pixels[i].r * 8f) / 8f;
			pixels[i].g = Mathf.Round(pixels[i].g * 8f) / 8f;
			pixels[i].b = Mathf.Round(pixels[i].b * 8f) / 8f;
		}
		return pixels;
	}
}