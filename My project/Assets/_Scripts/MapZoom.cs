using UnityEngine;
using UnityEngine.EventSystems; // Required for UI scrolling

// IScrollHandler allows this script to intercept the mouse wheel natively
public class MapZoom : MonoBehaviour, IScrollHandler
{
    [Header("Zoom Settings")]
    public RectTransform contentPanel; // The box holding your 25 tiles
    public float zoomSpeed = 0.1f;
    public float minZoom = 0.5f; // How far out you can zoom
    public float maxZoom = 3.0f; // How close you can zoom in

    public void OnScroll(PointerEventData eventData)
    {
        // Get the scroll wheel movement (usually 1 or -1)
        float scrollAmount = eventData.scrollDelta.y;
        Debug.Log(scrollAmount);

        // Calculate what the new scale should be
        Vector3 currentScale = contentPanel.localScale;
        Vector3 newScale = currentScale + (Vector3.one * scrollAmount * zoomSpeed);

        // Clamp the scale so the player can't zoom infinitely
        newScale.x = Mathf.Clamp(newScale.x, minZoom, maxZoom);
        newScale.y = Mathf.Clamp(newScale.y, minZoom, maxZoom);
        newScale.z = 1f;

        // Apply the new scale to the map
        contentPanel.localScale = newScale;
    }
}