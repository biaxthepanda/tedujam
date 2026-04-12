using UnityEngine;

public class InfoManager : MonoBehaviour
{
	public static InfoManager Instance { get; private set; }

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

	public InfoUI InfoUI;


    public void SetInfoUIInfo(string infoText, Sprite infoImage)
	{
		InfoUI.SetInfo(infoText,infoImage); // Example usage, you can replace with actual text and sprite
	}

	public void HideInfoUIInfo()
	{
		InfoUI.HideInfoUI();
	}

	public void AddInfoUIInfo(string infoText, Sprite infoImage) 
	{
		InfoUI.AddInfoButton(infoText, infoImage);
	}
}
