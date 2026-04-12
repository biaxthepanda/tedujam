using TMPro;           // Required for TextMeshPro
using UnityEngine;
using UnityEngine.UI; // Required for Image component

public class InfoUI : MonoBehaviour
{

	[Header("UI References")]
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private Image displayImage;

	public InfoButton InfoButton;

	public Transform InfoContainer;

	public static InfoUI Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this.gameObject);
	}


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

		this.gameObject.SetActive(true); // Ensure the UI is visible when updated
	}

	public void AddInfoButton(string newText, Sprite newSprite)
	{
		InfoButton InfoBut = Instantiate(InfoButton, InfoContainer);
		InfoBut.ButonInfoSprite = newSprite;
		InfoBut.ButtonInfoText = newText;
		InfoBut.transform.SetAsFirstSibling();
	}

	public void HideInfoUI() 
	{
		 this.gameObject.SetActive(false);
	}
}