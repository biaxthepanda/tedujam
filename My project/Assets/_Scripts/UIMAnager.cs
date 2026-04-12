using TMPro;
using UnityEngine;

public class UIMAnager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static UIMAnager Instance { get; private set; }

	public GameObject mapUI;
	public TextMeshProUGUI mapNameText;
	public GameObject BackButton;

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

	public void ToggleMapUI(bool isOn) 
	{
		if(isOn)
		{
			mapUI.SetActive(true);
			mapNameText.gameObject.SetActive(false);
		}
		else
		{
			mapUI.SetActive(false);
			mapNameText.gameObject.SetActive(true);
		}
	}

	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
