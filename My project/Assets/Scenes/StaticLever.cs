using UnityEngine;

public class StaticLever : MonoBehaviour, IClickable
{

	public string POIInfoString;
	public Sprite POIInfoSprite;

	bool isClicked  = false;
	public void Clicked()
	{
		if (isClicked) return;
		InfoManager.Instance.SetInfoUIInfo(POIInfoString, POIInfoSprite);
		InfoManager.Instance.AddInfoUIInfo(POIInfoString, POIInfoSprite);
		isClicked = true;
	}

	public void Unclicked()
	{
	}
}