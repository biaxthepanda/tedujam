using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class UIPanoramaStitcher : MonoBehaviour
{
    [Header("API Credentials")]
    public string apiKey = "YOUR_API_KEY_HERE";
    public string panoId = "YOUR_ANKARA_PANO_ID";

    [Header("UI Display")]
    public RawImage displayImage;

    // We will download 4 images at 640x640 resolution
    private const int imageSize = 640;
    private Texture2D panoramaTexture;

    void Start()
    {
        // Create a blank texture that is 4 times wider than a single image (2560 x 640)
        panoramaTexture = new Texture2D(imageSize * 4, imageSize, TextureFormat.RGB24, false);
        StartCoroutine(BuildPanorama());
    }

    IEnumerator BuildPanorama()
    {
        // Fetch North, East, South, West
        int[] headings = { 0, 90, 180, 270 };

        for (int i = 0; i < headings.Length; i++)
        {
            string url = $"https://maps.googleapis.com/maps/api/streetview?size={imageSize}x{imageSize}&pano={panoId}&heading={headings[i]}&pitch=0&fov=90&key={apiKey}";

            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    Texture2D downloadedTex = DownloadHandlerTexture.GetContent(uwr);

                    // Extract the pixels from the downloaded image
                    Color[] pixels = downloadedTex.GetPixels();

                    // "Paint" those pixels into the correct section of our master panorama texture
                    int targetXOffset = i * imageSize;
                    panoramaTexture.SetPixels(targetXOffset, 0, imageSize, imageSize, pixels);

                    // Destroy the temporary texture to save memory
                    Destroy(downloadedTex);
                }
                else
                {
                    Debug.LogError($"Failed to download heading {headings[i]}: {uwr.error}");
                }
            }
        }

        // Apply all the painted pixels to the master texture
        panoramaTexture.Apply();

        // Display the final stretched panorama on the UI
        displayImage.texture = panoramaTexture;
        Debug.Log("Panorama built successfully!");
    }
}