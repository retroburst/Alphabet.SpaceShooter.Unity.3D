using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Settings controller.
/// Controls the display and input of
/// configurable settings at runtime.
/// </summary>
public class SettingsController : MonoBehaviour
{
	public GameObject Panel;
	public InputField InitialLivesField;
	public Button SaveButton;
	public Button CloseButton;
	public GameObject ConfigurationPrefab;
	private ConfigurableSettings settings;

	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start ()
	{
		settings = ConfigurationPrefab.GetComponent<ConfigurableSettings> ();
		SaveButton.onClick.AddListener (() => SaveButtonClick ());
		CloseButton.onClick.AddListener (() => CloseSettingsPanel ());
		InitialLivesField.text = settings.InitialLives.ToString ();
	}
	
	/// <summary>
	/// Save button click handler.
	/// </summary>
	private void SaveButtonClick ()
	{
		string value = InitialLivesField.text;
		int numLives;
		if (int.TryParse (value, out numLives)) {
			settings.InitialLives = numLives;
		} else {
			InitialLivesField.text = settings.InitialLives.ToString ();
		}
		CloseSettingsPanel ();
	}
	
	/// <summary>
	/// Closes the settings panel.
	/// </summary>
	public void CloseSettingsPanel ()
	{
		Panel.SetActive (false);
	}
	
	/// <summary>
	/// Opens the settings panel.
	/// </summary>
	public void OpenSettingsPanel ()
	{
		Panel.SetActive (true);
	}
}
