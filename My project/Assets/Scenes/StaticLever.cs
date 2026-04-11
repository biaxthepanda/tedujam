using UnityEngine;

public class StaticLever : MonoBehaviour, IClickable
{
	public void Clicked()
	{
		Debug.Log("Raycast hit the Cube!");
		GetComponent<Renderer>().material.color = Color.red;
	}

	public void Unclicked()
	{
		Debug.Log("Released the Cube!");
		GetComponent<Renderer>().material.color = Color.white;
	}
}