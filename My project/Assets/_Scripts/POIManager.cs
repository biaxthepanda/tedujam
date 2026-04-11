using System.Collections.Generic;
using UnityEngine;

public class POIManager : MonoBehaviour
{
    public MapboxTileManager tileManager;
    public RectTransform contentPanel;
    public GameObject poiPrefab;

    [Header("Data Loading")]
    public string fileName = "Locations"; // Do not include .txt
    public float defaultAppearanceScale = 1.0f;

    private List<MapPOI> spawnedPOIs = new List<MapPOI>();

    void Start()
    {
        // Wait for Mapbox to initialize, then load the file
    }

    void Update()
    {
        float currentScale = contentPanel.localScale.x;
        foreach (var poi in spawnedPOIs)
        {
            if (poi != null) poi.UpdateVisibility(currentScale);
        }
    }

    public void LoadAndPlacePOIs()
    {
        TextAsset textFile = Resources.Load<TextAsset>(fileName);

        if (textFile == null)
        {
            Debug.LogError("Could not find " + fileName + " in Resources folder!");
            return;
        }

        string[] lines = textFile.text.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            try
            {
                // Format: 1- name: lat, lon * panoID

                // 1. Split by '*' to get the PanoID
                string[] panoSplit = line.Split('*');
                string panoID = panoSplit[1].Trim();

                // 2. Split the left side by ':' to get Name and Coordinates
                string[] nameCoordsSplit = panoSplit[0].Split(':');

                // 3. Get the name (removing the "1- " part)
                string namePart = nameCoordsSplit[0];
                string cleanName = namePart.Substring(namePart.IndexOf('-') + 1).Trim();

                // 4. Get Lat and Lon
                string[] coords = nameCoordsSplit[1].Split(',');
                double lat = double.Parse(coords[0].Trim());
                double lon = double.Parse(coords[1].Trim());

                SpawnPOI(cleanName, lat, lon, panoID);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to parse line: {line}. Error: {e.Message}");
            }
        }
    }

    void SpawnPOI(string pName, double pLat, double pLon, string pID)
    {
        GameObject go = Instantiate(poiPrefab, contentPanel);
        go.transform.SetAsLastSibling();

        MapPOI poi = go.GetComponent<MapPOI>();
        poi.poiName = pName;
        poi.latitude = pLat;
        poi.longitude = pLon;
        poi.ID = pID;
        poi.minShowScale = defaultAppearanceScale;

        // Position math
        float xOffset = (float)((LonToTileX(pLon, tileManager.zoomLevel) - LonToTileX(tileManager.centerLon, tileManager.zoomLevel)) * 256);
        float yOffset = (float)((LatToTileY(pLat, tileManager.zoomLevel) - LatToTileY(tileManager.centerLat, tileManager.zoomLevel)) * 256);

        poi.SetPosition(new Vector2(xOffset, -yOffset));
        spawnedPOIs.Add(poi);
    }

    private double LonToTileX(double lon, int zoom) => (lon + 180.0) / 360.0 * (1 << zoom);
    private double LatToTileY(double lat, int zoom) => (1.0 - System.Math.Log(System.Math.Tan(lat * Mathf.Deg2Rad) + 1.0 / System.Math.Cos(lat * Mathf.Deg2Rad)) / System.Math.PI) / 2.0 * (1 << zoom);
}