using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // Required for clicking UI

public class MapPOI : MonoBehaviour, IPointerClickHandler
{
    [Header("Location")]
    public string poiName;
    public double latitude;
    public double longitude;
    public string ID = ""; // This is your Panoramic/PanID

    [Header("Visibility Settings")]
    public float minShowScale = 1.5f;

    private RectTransform rectTransform;
    private Image iconImage;
    public POIManager POIManager;

    public GameObject POIPhysical;

    public string POIInfoString;
    public Sprite POIInfoSprite;

    public TextMeshProUGUI TextMeshProUGUI;



    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        iconImage = GetComponent<Image>();
    }

    // This function triggers when you click the Pin on the map
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(ID))
        {
            var textureManager = Object.FindFirstObjectByType<ApiTextureMapper>();


            if (textureManager != null)
            {

                Debug.Log("Name"+ poiName + " ID: " + ID);
                // Just tell the manager to load. 
                // The manager will handle hiding the map once it's finished!
                textureManager.SetPanorama(ID);
                if(POIPhysical != null) 
                {
                    POIManager.SpawnPhysicalPOI(POIPhysical);

				}
                UIMAnager.Instance.mapNameText.text = poiName;
				POIManager.StreetViewBackButton.SetActive(true);
			}
        }
        else { 
            Debug.LogError("PanID is EMPTY!");
        
        
        
        }
    }

    public void UpdateVisibility(float currentScale)
    {
        bool shouldShow = currentScale >= minShowScale;
        if (iconImage != null) iconImage.enabled = shouldShow;

        // Counter-scale so pins don't get massive when zooming in
        if (shouldShow)
        {
            transform.localScale = new Vector3(1f / currentScale, 1f / currentScale, 1f);
        }
    }

    public void SetPosition(Vector2 pos)
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        {

        rectTransform.anchoredPosition = pos;

        }

        TextMeshProUGUI.text = poiName;
	}

    public void POISetPanoramicID(string Id)
    {
        this.ID = Id;
    }
}