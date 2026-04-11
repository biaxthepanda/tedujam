using UnityEngine;
using UnityEngine.UI;

public class MapPOI : MonoBehaviour
{
    [Header("Location")]
    public string poiName;
    public double latitude;
    public double longitude;

    [Header("Visibility Settings")]
    [Tooltip("At what scale should this icon start appearing?")]
    public float minShowScale = 1.5f;

    private RectTransform rectTransform;
    private Image iconImage;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        iconImage = GetComponent<Image>();
    }

    public void UpdateVisibility(float currentScale)
    {
        // Simple toggle: If the map is zoomed in enough, show the pin
        bool shouldShow = currentScale >= minShowScale;

        // You can use setActive or just fade the alpha
        if (iconImage != null) iconImage.enabled = shouldShow;
    }

    public void SetPosition(Vector2 pos)
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = pos;
    }
}