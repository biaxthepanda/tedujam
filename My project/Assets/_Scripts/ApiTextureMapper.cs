using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Renderer))]
public class ApiTextureMapper : MonoBehaviour
{
    [Header("API Credentials")]
    public string apiKey = "YOUR_API_KEY_HERE";
    public string panID = "YOUR_PAN_ID_HERE";

    [Header("Image Settings")]
    public int imageWidth = 1024;
    public int imageHeight = 512;

    void Start()
    {
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(panID))
        {
            Debug.LogError("API Key or PanID is missing! Please set them in the Inspector.");
            return;
        }

        StartCoroutine(FetchPanorama());
    }

    private IEnumerator FetchPanorama()
    {
        // Construct the URL. (Adjust this base URL if you are using a different service than Google)
        string apiUrl = $"https://maps.googleapis.com/maps/api/streetview?size={imageWidth}x{imageHeight}&pano={panID}&key={apiKey}";

        Debug.Log("Requesting panorama from: " + apiUrl);

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"API Error: {request.error}");
            }
            else
            {
                Texture2D panoramaTexture = DownloadHandlerTexture.GetContent(request);

                // Clamp and Bilinear filtering usually look best for 360 spheres
                panoramaTexture.wrapMode = TextureWrapMode.Clamp;
                panoramaTexture.filterMode = FilterMode.Bilinear;

                ApplyToSphere(panoramaTexture);
            }
        }
    }

    private void ApplyToSphere(Texture2D texture)
    {
        // Using an Unlit texture shader so the image displays exactly as downloaded, unaffected by Unity scene lighting
        Material unlitMaterial = new Material(Shader.Find("Unlit/Texture"));
        unlitMaterial.mainTexture = texture;

        GetComponent<Renderer>().material = unlitMaterial;
        Debug.Log("Panorama successfully applied to the sphere!");
    }
}