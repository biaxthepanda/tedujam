using UnityEngine;

public class StaticLever : MonoBehaviour, IClickable
{
	[TextArea]
	public string POIInfoString;
	public Sprite POIInfoSprite;

	bool isClicked  = false;
	public bool isFirst = false;

    private void Start()
    {
		if (isFirst) 
		{
			UIMAnager.Instance.BackButton.SetActive(false);
		}
    }
    public void Clicked()
	{
		if (isClicked) return;
		InfoManager.Instance.SetInfoUIInfo(POIInfoString, POIInfoSprite);
		InfoManager.Instance.AddInfoUIInfo(POIInfoString, POIInfoSprite);
		isClicked = true;
        if (isFirst)
        {
            UIMAnager.Instance.BackButton.SetActive(true);
        }

    }

	public void Unclicked()
	{
	}
}