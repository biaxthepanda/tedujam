using UnityEngine;
using UnityEngine.UI; // Required for Image component
using TMPro;           // Required for TextMeshPro

public class InfoUI : MonoBehaviour
{
	[Header("UI References")]
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private Image displayImage;

	/// <summary>
	/// Call this method to update the UI content dynamically.
	/// </summary>
	/// <param name="newText">The text to display</param>
	/// <param name="newSprite">The image to display</param>
	public void SetInfo(string newText, Sprite newSprite)
	{
		if (titleText != null)
		{
			titleText.text = newText;
		}

		if (displayImage != null)
		{
			displayImage.sprite = newSprite;

			// Optional: If you want to hide the image if the sprite is null
			displayImage.enabled = (newSprite != null);
		}

		Debug.Log($"UI Updated: {newText}");

		this.enabled = true;
	}

	public void HideInfoUI() 
	{
		 this.enabled = false;
	}
}