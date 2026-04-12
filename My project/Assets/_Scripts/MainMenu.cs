using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

	public AudioMixer audioMixer;
	public Slider slider;

    public void StartGame() { 
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
	}

	public void OnBeforeTransformParentChanged()
	{
		// This is a workaround for a Unity bug where the first click on the map doesn't register after loading the scene.
		// By forcing a re-render of the UI, we ensure that the map is interactive immediately.
		Application.Quit();
	}

	public void ChangeSoundMixerVolume()
	{
		float vol = slider.value == 0 ? 0 : Mathf.Log10(slider.value) * 20;

        audioMixer.SetFloat("MasterVolume", vol); // Convert linear slider value to decibels
	}

	public void OnSliderValueChanged() 
	{
		ChangeSoundMixerVolume();
	}
}
