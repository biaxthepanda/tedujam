using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MapboxTileManager : MonoBehaviour
{
    [Header("Mapbox Settings")]
    public string mapboxToken = ""; // Paste your token from the Mapbox Console here

    [Header("Location Settings")]
    public double centerLat = 39.9250; // Kýzýlay, Ankara
    public double centerLon = 32.8369;
    public int zoomLevel = 15; // Zoom level 15-16 is where streets and buildings appear

    [Header("Grid Settings")]
    [Tooltip("Radius 1 = 3x3 grid. Radius 2 = 5x5 grid.")]
    public int tileRadius = 2;

    [Header("UI References")]
    public Transform contentContainer; // Drag your ScrollView's 'Content' object here
    public GameObject tilePrefab;      // Drag your 256x256 RawImage prefab here
    public GridLayoutGroup gridLayout; // Drag your ScrollView's 'Content' object here again

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        // 1. Calculate the center tile's X and Y index
        int centerX = LonToTileX(centerLon, zoomLevel);
        int centerY = LatToTileY(centerLat, zoomLevel);

        // 2. Set the grid columns based on our radius (e.g., radius 2 = 5 columns)
        int gridSize = (tileRadius * 2) + 1;
        gridLayout.constraintCount = gridSize;

        // 3. Loop through the grid and spawn tiles
        for (int y = centerY - tileRadius; y <= centerY + tileRadius; y++)
        {
            for (int x = centerX - tileRadius; x <= centerX + tileRadius; x++)
            {
                SpawnAndLoadTile(x, y, zoomLevel);
            }
        }
    }

    private void SpawnAndLoadTile(int x, int y, int z)
    {
        // Create a new blank tile in the UI grid
        GameObject newTile = Instantiate(tilePrefab, contentContainer);
        newTile.name = $"Tile_{z}_{x}_{y}";

        RawImage tileImage = newTile.GetComponent<RawImage>();

        // Start downloading the Mapbox image for this specific tile
        StartCoroutine(DownloadTileImage(x, y, z, tileImage));
    }

    private IEnumerator DownloadTileImage(int x, int y, int z, RawImage targetImage)
    {
        // The official Mapbox Raster Tile URL
        string url = $"https://api.mapbox.com/styles/v1/mapbox/streets-v12/tiles/256/{z}/{x}/{y}?access_token={mapboxToken}";

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                targetImage.texture = texture;
            }
            else
            {
                Debug.LogError($"Failed to load tile {x},{y}: {uwr.error}");
            }
        }
    }

    // --- Mathematical Helpers (Standard Web Mercator Projection) ---
    private int LonToTileX(double lon, int zoom)
    {
        return (int)(Mathf.Floor((float)((lon + 180.0) / 360.0 * (1 << zoom))));
    }

    private int LatToTileY(double lat, int zoom)
    {
        float latRad = (float)(lat * Mathf.Deg2Rad);
        return (int)(Mathf.Floor((float)((1.0 - Mathf.Log(Mathf.Tan(latRad) + 1.0f / Mathf.Cos(latRad)) / Mathf.PI) / 2.0 * (1 << zoom))));
    }
}