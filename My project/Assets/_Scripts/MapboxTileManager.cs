using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MapboxTileManager : MonoBehaviour
{
    [Header("Mapbox Settings")]
    public string mapboxToken = "";

    [Header("Location Settings")]
    public double centerLat = 39.9250;
    public double centerLon = 32.8369;
    public int zoomLevel = 15;
    public int tileRadius = 3; // Increased to 3 (7x7 grid) for better panning room

    [Header("UI References")]
    public Transform contentContainer;
    public GameObject tilePrefab;
    public GridLayoutGroup gridLayout;

    public POIManager POIManager;

    void Start()
    {
        // Load token from Resources/map_secret.txt
        TextAsset tokenFile = Resources.Load<TextAsset>("map_secret");
        if (tokenFile != null)
        {
            mapboxToken = tokenFile.text.Trim();
            GenerateMap();
        }
    }

    public void ReloadMap(int newZoom, Vector2 localMousePos)
    {
        // 1. Calculate the Lat/Lon under the mouse BEFORE we change anything
        Vector2 latLonAtMouse = GetLatLonFromLocalPos(localMousePos);
        centerLat = latLonAtMouse.x;
        centerLon = latLonAtMouse.y;

        // 2. Update Zoom
        zoomLevel = newZoom;

        // 3. Clear and Rebuild
        foreach (Transform child in contentContainer) Destroy(child.gameObject);
        contentContainer.localScale = Vector3.one;
        GenerateMap();
    }

    public void GenerateMap()
    {
        int centerX = LonToTileX(centerLon, zoomLevel);
        int centerY = LatToTileY(centerLat, zoomLevel);

        int gridSize = (tileRadius * 2) + 1;
        gridLayout.constraintCount = gridSize;

        for (int y = centerY - tileRadius; y <= centerY + tileRadius; y++)
        {
            for (int x = centerX - tileRadius; x <= centerX + tileRadius; x++)
            {
                SpawnAndLoadTile(x, y, zoomLevel);
            }
        }
    }

    private Vector2 GetLatLonFromLocalPos(Vector2 localPos)
    {
        // Calculate where the mouse is relative to the center of the grid
        float tileSize = 256f;
        float totalSize = ((tileRadius * 2) + 1) * tileSize;

        // Convert local UI coords to tile offset from center
        double offsetX = localPos.x / tileSize;
        double offsetY = -localPos.y / tileSize; // UI Y is inverted compared to Map Y

        double targetTileX = LonToTileX(centerLon, zoomLevel) + offsetX;
        double targetTileY = LatToTileY(centerLat, zoomLevel) + offsetY;

        return new Vector2((float)TileYToLat(targetTileY, zoomLevel), (float)TileXToLon(targetTileX, zoomLevel));
    }

    // --- Math Helpers ---
    private void SpawnAndLoadTile(int x, int y, int z)
    {
        GameObject newTile = Instantiate(tilePrefab, contentContainer);
        StartCoroutine(DownloadTileImage(x, y, z, newTile.GetComponent<RawImage>()));
    }

    private IEnumerator DownloadTileImage(int x, int y, int z, RawImage targetImage)
    {
        string url = $"https://api.mapbox.com/styles/v1/mapbox/streets-v12/tiles/256/{z}/{x}/{y}?access_token={mapboxToken}";
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
                targetImage.texture = DownloadHandlerTexture.GetContent(uwr);
        }
        POIManager.LoadAndPlacePOIs(); 
    }

    private int LonToTileX(double lon, int zoom) => (int)(Mathf.Floor((float)((lon + 180.0) / 360.0 * (1 << zoom))));
    private int LatToTileY(double lat, int zoom) => (int)(Mathf.Floor((float)((1.0 - Mathf.Log(Mathf.Tan((float)lat * Mathf.Deg2Rad) + 1.0f / Mathf.Cos((float)lat * Mathf.Deg2Rad)) / Mathf.PI) / 2.0 * (1 << zoom))));
    private double TileXToLon(double x, int z) => x / System.Math.Pow(2.0, z) * 360.0 - 180.0;
    private double TileYToLat(double y, int z)
    {
        double n = System.Math.PI - 2.0 * System.Math.PI * y / System.Math.Pow(2.0, z);
        return 180.0 / System.Math.PI * System.Math.Atan(0.5 * (System.Math.Exp(n) - System.Math.Exp(-n)));
    }
}