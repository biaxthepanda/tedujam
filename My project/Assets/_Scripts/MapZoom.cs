using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapZoom : MonoBehaviour, IScrollHandler
{
    [Header("References")]
    public RectTransform contentPanel; // Drag your 'Content' object here
    public ScrollRect scrollRect;      // Drag your 'MapScrollView' here

    [Header("Zoom Settings")]
    public float zoomSpeed = 0.1f;
    public float minZoom = 0.5f;
    public float maxZoom = 5.0f;

    public void OnScroll(PointerEventData eventData)
    {
        float scrollDelta = eventData.scrollDelta.y;
        if (Mathf.Abs(scrollDelta) < 0.01f) return;

        // 1. Store the mouse position relative to the Content panel BEFORE zooming
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(contentPanel, eventData.position, eventData.pressEventCamera, out localMousePos);

        // 2. Calculate the new scale
        Vector3 oldScale = contentPanel.localScale;
        Vector3 newScale = oldScale + (Vector3.one * scrollDelta * zoomSpeed);

        // Clamp the scale
        newScale.x = Mathf.Clamp(newScale.x, minZoom, maxZoom);
        newScale.y = Mathf.Clamp(newScale.y, minZoom, maxZoom);
        newScale.z = 1f;

        // 3. Apply the scale
        contentPanel.localScale = newScale;

        // 4. THE MAGIC: Adjust the position so the mouse stays over the same spot
        // We calculate how much the point under the mouse "moved" due to scaling
        Vector3 scaleRatio = new Vector3(newScale.x / oldScale.x, newScale.y / oldScale.y, 1);
        Vector2 displacement = new Vector2(localMousePos.x * (scaleRatio.x - 1), localMousePos.y * (scaleRatio.y - 1));

        // Shift the content position by that displacement
        contentPanel.localPosition -= (Vector3)displacement;
    }
}