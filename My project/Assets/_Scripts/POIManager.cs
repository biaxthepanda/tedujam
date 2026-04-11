using System.Collections.Generic;
using System.Xml.Serialization;
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

	public GameObject LastSpawnedPOIPhysical;

    public struct POIData
    {
        int id;
        public GameObject POIPhysical;
        public string poiInfoString;
        public Sprite poiInfoSprite;
	}

	[System.Serializable]
	public class POIMapping
	{
		public int id;
		public POIData data;
	}

	public List<POIMapping> visiblePOIList = new List<POIMapping>();

	public Dictionary<int,POIData> POIDatasMap = new Dictionary<int, POIData>();

	private void Awake()
	{
		// Transfer the list items into the dictionary for fast lookups
		foreach (var mapping in visiblePOIList)
		{
			POIDatasMap[mapping.id] = mapping.data;
		}
	}

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
        int index = 0;
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

                POIData tempData;
				
                POIDatasMap.TryGetValue(index,out tempData);
				SpawnPOI(cleanName, lat, lon, panoID, tempData);

				index++;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to parse line: {line}. Error: {e.Message}");
            }
        }
    }

    void SpawnPOI(string pName, double pLat, double pLon, string pID, POIData poiData)
    {
        GameObject go = Instantiate(poiPrefab, contentPanel);
        go.transform.SetAsLastSibling();

        MapPOI poi = go.GetComponent<MapPOI>();
        poi.poiName = pName;
        poi.latitude = pLat;
        poi.longitude = pLon;
        poi.ID = pID;
        poi.minShowScale = defaultAppearanceScale;
        if(poiData.poiInfoString != null)
        {
            poi.POIPhysical = poiData.POIPhysical;
            poi.POIInfoString = poiData.poiInfoString;
            poi.POIInfoSprite = poiData.poiInfoSprite;
		}
		// Position math
		float xOffset = (float)((LonToTileX(pLon, tileManager.zoomLevel) - LonToTileX(tileManager.centerLon, tileManager.zoomLevel)) * 256);
        float yOffset = (float)((LatToTileY(pLat, tileManager.zoomLevel) - LatToTileY(tileManager.centerLat, tileManager.zoomLevel)) * 256);

        poi.SetPosition(new Vector2(xOffset, -yOffset));
        spawnedPOIs.Add(poi);
    }

	public void SpawnPhysicalPOI(GameObject POIToSpawn)
	{
		if (LastSpawnedPOIPhysical != null)
		{
			Destroy(LastSpawnedPOIPhysical);
		}
		LastSpawnedPOIPhysical = Instantiate(POIToSpawn, Vector3.zero, Quaternion.identity);
	}

	private double LonToTileX(double lon, int zoom) => (lon + 180.0) / 360.0 * (1 << zoom);
    private double LatToTileY(double lat, int zoom) => (1.0 - System.Math.Log(System.Math.Tan(lat * Mathf.Deg2Rad) + 1.0 / System.Math.Cos(lat * Mathf.Deg2Rad)) / System.Math.PI) / 2.0 * (1 << zoom);
}



//public class POIManager : MonoBehaviour
//{
//	public MapboxTileManager tileManager;
//	public RectTransform contentPanel;
//	public GameObject poiPrefab; // A simple UI Image prefab with the MapPOI script

//	

//	[Header("Points of Interest")]
//	public List<POIData> locations;

//	private List<MapPOI> spawnedPOIs = new List<MapPOI>();

//	public InfoUI InfoUI;

//	[System.Serializable]
//	public class POIData
//	{
//		public string name;
//		public double lat;
//		public double lon;
//		public float appearanceScale = 1.0f;
//		public string PanaramicID = "";
//		public GameObject POIPhysical;
//		public string poiInfoString;
//		public Sprite poiInfoSprite;
//	}

//	void Start()
//	{
//		// Wait a tiny bit for the tile manager to set its center coordinates
//		// Invoke("PlacePOIs", 5);
//	}

//	void Update()
//	{
//		// Check visibility every frame based on the current scale of the map
//		float currentScale = contentPanel.localScale.x;
//		foreach (var poi in spawnedPOIs)
//		{
//			poi.UpdateVisibility(currentScale);
//		}
//	}

//	public void PlacePOIs()
//	{
//		foreach (var data in locations)
//		{
//			GameObject go = Instantiate(poiPrefab, contentPanel);
//			MapPOI poi = go.GetComponent<MapPOI>();

//			poi.poiName = data.name;
//			poi.minShowScale = data.appearanceScale;

//			// Math: Calculate pixel offset from the map center
//			float xOffset = (float)((LonToTileX(data.lon, tileManager.zoomLevel) - LonToTileX(tileManager.centerLon, tileManager.zoomLevel)) * 256);
//			float yOffset = (float)((LatToTileY(data.lat, tileManager.zoomLevel) - LatToTileY(tileManager.centerLat, tileManager.zoomLevel)) * 256);

//			// UI Y is inverted
//			poi.SetPosition(new Vector2(xOffset, -yOffset));
//			poi.POISetPanoramicID(data.PanaramicID);
//			poi.POIPhysical = data.POIPhysical;
//			poi.POIManager = this;
//			poi.POIInfoSprite = data.poiInfoSprite;
//			poi.POIInfoString = data.poiInfoString;

//			spawnedPOIs.Add(poi);
//		}
//	}




//	// Standard Web Mercator math to match the tiles
//	private double LonToTileX(double lon, int zoom) => (lon + 180.0) / 360.0 * (1 << zoom);
//	private double LatToTileY(double lat, int zoom) => (1.0 - System.Math.Log(System.Math.Tan(lat * Mathf.Deg2Rad) + 1.0 / System.Math.Cos(lat * Mathf.Deg2Rad)) / System.Math.PI) / 2.0 * (1 << zoom);
//}