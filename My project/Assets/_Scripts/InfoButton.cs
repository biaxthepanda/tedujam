using UnityEngine;

public class InfoButton : MonoBehaviour
{
    public string ButtonInfoText; // Text to display when the button is clicked
	public Sprite ButonInfoSprite;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public void ShowButtonInfo()
    {
		InfoManager.Instance.SetInfoUIInfo(ButtonInfoText, ButonInfoSprite);
	}
}
