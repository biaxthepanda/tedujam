using System.Collections.Generic;
using UnityEngine;

public class POIManager : MonoBehaviour
{
	public MapboxTileManager tileManager;
	public RectTransform contentPanel;
	public GameObject poiPrefab;

	[Header("Data Loading")]
	public string fileName = "Locations";
	public float defaultAppearanceScale = 1.0f;

	private List<MapPOI> spawnedPOIs = new List<MapPOI>();
	public GameObject LastSpawnedPOIPhysical;

	[Header("Manual Adjustment")]
	public double manualLatOffset = 0.0;
	public double manualLonOffset = 0.0;

	public GameObject StreetViewBackButton;

	[System.Serializable]
	public struct POIData
	{
		public GameObject POIPhysical;
		[TextArea]
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
	public Dictionary<int, POIData> POIDatasMap = new Dictionary<int, POIData>();

	private void Awake()
	{
		foreach (var mapping in visiblePOIList)
		{
			POIDatasMap[mapping.id] = mapping.data;
		}
	}

	public void LoadAndPlacePOIs()
	{
		// SAFETY: If zoom is 0 or center is 0, Mapbox isn't ready yet.
		if (tileManager.centerLat == 0 && tileManager.centerLon == 0)
		{
			Debug.LogWarning("Mapbox not initialized yet. Skipping POI load.");
			return;
		}

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
				// 1. Get PanoID
				string[] panoSplit = line.Split('*');
				string panoID = panoSplit[1].Trim();

				// 2. Split Name/ID and Coords
				string[] nameCoordsSplit = panoSplit[0].Split(':');

				// 3. GET THE ACTUAL ID from the string (e.g., "1-" -> 1)
				string namePart = nameCoordsSplit[0];
				int actualID = int.Parse(namePart.Split('-')[0].Trim());
				string cleanName = namePart.Substring(namePart.IndexOf('-') + 1).Trim();

				// 4. Get Lat and Lon
				string[] coords = nameCoordsSplit[1].Split(',');
				double lat = double.Parse(coords[0].Trim(), System.Globalization.CultureInfo.InvariantCulture);
				double lon = double.Parse(coords[1].Trim(), System.Globalization.CultureInfo.InvariantCulture);

				POIData tempData = new POIData();
				POIDatasMap.TryGetValue(actualID, out tempData);

				SpawnPOI(cleanName, lat, lon, panoID, tempData);
			}
			catch (System.Exception e)
			{
				Debug.LogWarning($"Failed to parse line: {line}. Error: {e.Message}");
			}
		}
	}

	void SpawnPOI(string pName, double pLat, double pLon, string pID, POIData poiData)
	{
		// ADD THESE TWO LINES AT THE VERY START:
		pLat += manualLatOffset;
		pLon += manualLonOffset;

		// The rest of your code remains exactly the same...
		double xOffset = (LonToTileX(pLon, tileManager.zoomLevel) - LonToTileX(tileManager.centerLon, tileManager.zoomLevel)) * 256.0;
		double yOffset = (LatToTileY(pLat, tileManager.zoomLevel) - LatToTileY(tileManager.centerLat, tileManager.zoomLevel)) * 256.0;
		// FINAL SAFETY CHECK: If the offset is still insane, block it.
		if (System.Math.Abs(xOffset) > 1000000 || double.IsNaN(xOffset))
		{
			Debug.LogError($"POI {pName} math failed! Calculated X: {xOffset}. Check Zoom and Center.");
			return;
		}

		GameObject go = Instantiate(poiPrefab, contentPanel);
		go.transform.SetAsLastSibling();

		MapPOI poi = go.GetComponent<MapPOI>();
		poi.poiName = pName;
		poi.latitude = pLat;
		poi.longitude = pLon;
		poi.ID = pID;
		poi.minShowScale = defaultAppearanceScale;

		if (!string.IsNullOrEmpty(poiData.poiInfoString))
		{
			poi.POIPhysical = poiData.POIPhysical;
			poi.POIInfoString = poiData.poiInfoString;
			poi.POIInfoSprite = poiData.poiInfoSprite;
		}
		poi.POIManager = this;
		poi.SetPosition(new Vector2((float)xOffset, -(float)yOffset));
		spawnedPOIs.Add(poi);
	}

	public void SpawnPhysicalPOI(GameObject POIToSpawn)
	{
		if (LastSpawnedPOIPhysical != null) Destroy(LastSpawnedPOIPhysical);
		LastSpawnedPOIPhysical = Instantiate(POIToSpawn, Vector3.zero, Quaternion.identity);
	}

	// High-precision math helpers using System.Math instead of Mathf
	private double LonToTileX(double lon, int zoom) => (lon + 180.0) / 360.0 * (1 << zoom);

	private double LatToTileY(double lat, int zoom)
	{
		double rad = lat * System.Math.PI / 180.0;
		return (1.0 - System.Math.Log(System.Math.Tan(rad) + 1.0 / System.Math.Cos(rad)) / System.Math.PI) / 2.0 * (1 << zoom);
	}

	void Update()
	{
		float currentScale = contentPanel.localScale.x;
		foreach (var poi in spawnedPOIs)
		{
			if (poi != null) poi.UpdateVisibility(currentScale);
		}
	}
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