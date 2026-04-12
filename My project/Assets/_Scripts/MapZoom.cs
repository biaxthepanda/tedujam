using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapZoom : MonoBehaviour, IScrollHandler
{
    [Header("References")]
    public RectTransform contentPanel;
    public ScrollRect scrollRect;

    [Header("Zoom Settings")]
    public float zoomSpeed = 0.1f;
    public float minZoom = 0.5f;
    public float maxZoom = 5.0f;

    public void OnScroll(PointerEventData eventData)
    {
        float scrollDelta = eventData.scrollDelta.y;
        if (Mathf.Abs(scrollDelta) < 0.01f) return;

        // 1. Mouse'un Content üzerindeki LOCAL (kendi iç) pozisyonunu alýyoruz
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(contentPanel, eventData.position, eventData.pressEventCamera, out localMousePos);

        // 2. Yeni boyutu (Scale) hesapla ve sýnýrla
        Vector3 oldScale = contentPanel.localScale;
        Vector3 newScale = oldScale + (Vector3.one * scrollDelta * zoomSpeed);

        newScale.x = Mathf.Clamp(newScale.x, minZoom, maxZoom);
        newScale.y = Mathf.Clamp(newScale.y, minZoom, maxZoom);
        newScale.z = 1f;

        // Eðer min/max limitlerine ulaþtýysa ve scale deðiþmeyecekse hesaplamayý durdur
        if (newScale == oldScale) return;

        // 3. Scale'i uygula
        contentPanel.localScale = newScale;

        // 4. THE MAGIC (Düzeltilmiþ Matematik)
        // Yeni scale ile eski scale arasýndaki net büyüme miktarýný buluyoruz
        float scaleDiff = newScale.x - oldScale.x;

        // Bu farký, mouse'un içerideki pozisyonuyla çarparak ne kadar kaydýðýný buluyoruz
        Vector2 displacement = localMousePos * scaleDiff;

        // Haritayý o kayma miktarý kadar ters yöne itiyoruz ki mouse'un altýndaki nokta sabit kalsýn
        contentPanel.anchoredPosition -= displacement;
    }
}