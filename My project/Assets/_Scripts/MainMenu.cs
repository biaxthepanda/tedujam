using UnityEngine;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{

	public AudioMixer audioMixer; 

    public void StartGame() { 
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
	}

	public void OnBeforeTransformParentChanged()
	{
		// This is a workaround for a Unity bug where the first click on the map doesn't register after loading the scene.
		// By forcing a re-render of the UI, we ensure that the map is interactive immediately.
		Application.Quit();
	}

	public void ChangeSoundMixerVolume(float volume)
	{
		audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20); // Convert linear slider value to decibels
	}


}
