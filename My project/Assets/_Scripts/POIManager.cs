using System.Collections.Generic;
using UnityEngine;

public class POIManager : MonoBehaviour
{
    public MapboxTileManager tileManager;
    public RectTransform contentPanel;
    public GameObject poiPrefab; // A simple UI Image prefab with the MapPOI script

    [Header("Points of Interest")]
    public List<POIData> locations;

    private List<MapPOI> spawnedPOIs = new List<MapPOI>();

    [System.Serializable]
    public class POIData
    {
        public string name;
        public double lat;
        public double lon;
        public float appearanceScale = 1.0f;
        public string PanaramicID = "";
    }

    void Start()
    {
        // Wait a tiny bit for the tile manager to set its center coordinates
       // Invoke("PlacePOIs", 5);
    }

    void Update()
    {
        // Check visibility every frame based on the current scale of the map
        float currentScale = contentPanel.localScale.x;
        foreach (var poi in spawnedPOIs)
        {
            poi.UpdateVisibility(currentScale);
        }
    }

    public void PlacePOIs()
    {
        foreach (var data in locations)
        {
            GameObject go = Instantiate(poiPrefab, contentPanel);
            MapPOI poi = go.GetComponent<MapPOI>();

            poi.poiName = data.name;
            poi.minShowScale = data.appearanceScale;

            // Math: Calculate pixel offset from the map center
            float xOffset = (float)((LonToTileX(data.lon, tileManager.zoomLevel) - LonToTileX(tileManager.centerLon, tileManager.zoomLevel)) * 256);
            float yOffset = (float)((LatToTileY(data.lat, tileManager.zoomLevel) - LatToTileY(tileManager.centerLat, tileManager.zoomLevel)) * 256);

            // UI Y is inverted
            poi.SetPosition(new Vector2(xOffset, -yOffset));
            poi.POISetPanoramicID(data.PanaramicID);
            spawnedPOIs.Add(poi);
        }
    }

    // Standard Web Mercator math to match the tiles
    private double LonToTileX(double lon, int zoom) => (lon + 180.0) / 360.0 * (1 << zoom);
    private double LatToTileY(double lat, int zoom) => (1.0 - System.Math.Log(System.Math.Tan(lat * Mathf.Deg2Rad) + 1.0 / System.Math.Cos(lat * Mathf.Deg2Rad)) / System.Math.PI) / 2.0 * (1 << zoom);
}