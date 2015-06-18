using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsPanel : MonoBehaviour {
	public GameObject Panel;
	public InputField InitialLivesField;
	public Button SaveButton;
	public Button CloseButton;
	public GameObject ConfigurationPrefab;
	private ConfigurableSettings settings;

	// Use this for initialization
	private void Start () {
		settings = ConfigurationPrefab.GetComponent<ConfigurableSettings>();
		SaveButton.onClick.AddListener(() => SaveButtonClick());
		CloseButton.onClick.AddListener(() => CloseSettingsPanel());
		InitialLivesField.text = settings.InitialLives.ToString();
	}

	private void SaveButtonClick ()
	{
		string value = InitialLivesField.text;
		int numLives;
		if(int.TryParse(value, out numLives))
		{
			settings.InitialLives = numLives;
		}
		else
		{
			InitialLivesField.text = settings.InitialLives.ToString();
		}
		CloseSettingsPanel();
	}

	public void CloseSettingsPanel(){
		Panel.SetActive(false);
	}

	public void OpenSettingsPanel(){
		Panel.SetActive(true);
	}
}
