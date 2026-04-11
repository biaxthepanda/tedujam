using UnityEngine;

public class MouseManager : MonoBehaviour
{
	private IClickable currentTarget;

	void Update()
	{
		// 1. Check for Mouse Down
		if (Input.GetMouseButtonDown(0))
		{
			HandleClick();
		}

		// 2. Check for Mouse Up
		if (Input.GetMouseButtonUp(0))
		{
			HandleRelease();
		}



		//

		Ray debugRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(debugRay.origin, debugRay.direction * 20, Color.yellow);

		if (Input.GetMouseButtonDown(0)) { HandleClick(); }
		if (Input.GetMouseButtonUp(0)) { HandleRelease(); }
	}

	private void HandleClick()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			// 1. Did we hit anything at all?
			Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

			// 2. Try to find the interface
			IClickable clickable = hit.collider.GetComponent<IClickable>();

			if (clickable != null)
			{
				Debug.Log("Interface FOUND on " + hit.collider.gameObject.name);
				currentTarget = clickable;
				currentTarget.Clicked();
			}
			else
			{
				// 3. This tells us the ray hit the cube, but the script isn't recognized
				Debug.LogWarning("Hit " + hit.collider.gameObject.name + " but no IClickable found!");
			}
		}
		else
		{
			Debug.Log("Raycast hit NOTHING.");
		}
	}

	private void HandleRelease()
	{
		if (currentTarget != null)
		{
			currentTarget.Unclicked();
			currentTarget = null; // Clear the reference
		}
	}
}
