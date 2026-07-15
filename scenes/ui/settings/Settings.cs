using Godot;

public partial class Settings : Control
{
	[Export] private HSlider _musicVolumeSlider;
	[Export] private HSlider _soundsVolumeSlider;
	[Export] private LineEdit _musicValueEdit;
	[Export] private LineEdit _soundsValueEdit;
	[Export] private Button _closeButton;

	public override void _Ready()
	{
		_musicVolumeSlider.ValueChanged += OnMusicSliderChanged;
		_soundsVolumeSlider.ValueChanged += OnSoundsSliderChanged;
		_musicValueEdit.TextChanged += OnMusicTextChanged;
		_soundsValueEdit.TextChanged += OnSoundsTextChanged;
		_musicValueEdit.TextSubmitted += OnMusicValueSubmitted;
		_soundsValueEdit.TextSubmitted += OnSoundsValueSubmitted;
		_closeButton.Pressed += OnClosePressed;

		UpdateMusicLabel((float)_musicVolumeSlider.Value);
		UpdateSoundsLabel((float)_soundsVolumeSlider.Value);
	}

	private void OnMusicSliderChanged(double value) => UpdateMusicLabel((float)value);
	private void OnSoundsSliderChanged(double value) => UpdateSoundsLabel((float)value);

	private void UpdateMusicLabel(float value) =>
		_musicValueEdit.Text = Mathf.Round(value).ToString() + "%";

	private void UpdateSoundsLabel(float value) =>
		_soundsValueEdit.Text = Mathf.Round(value).ToString() + "%";

	private void OnMusicTextChanged(string newText)
	{
		string filtered = FilterDigitsPercent(newText);
		if (filtered != newText)
			_musicValueEdit.Text = filtered;
	}

	private void OnSoundsTextChanged(string newText)
	{
		string filtered = FilterDigitsPercent(newText);
		if (filtered != newText)
			_soundsValueEdit.Text = filtered;
	}

	private string FilterDigitsPercent(string text)
	{
		string result = "";
		foreach (char c in text)
			if (char.IsDigit(c) || c == '%')
				result += c;
		return result;
	}

	private void OnMusicValueSubmitted(string newText) =>
		ApplyValueFromText(newText, _musicVolumeSlider, UpdateMusicLabel);

	private void OnSoundsValueSubmitted(string newText) =>
		ApplyValueFromText(newText, _soundsVolumeSlider, UpdateSoundsLabel);

	private void ApplyValueFromText(string newText, HSlider slider, System.Action<float> updateLabel)
	{
		string cleanText = newText.Replace("%", "").Trim();
		if (float.TryParse(cleanText, out float newValue))
		{
			newValue = Mathf.Clamp(newValue, 0, 100);
			slider.Value = newValue;
			updateLabel(newValue);
		}
		else
		{
			updateLabel((float)slider.Value);
		}
	}

	private void OnClosePressed()
	{
		GD.Print("Return to the main menu");
		GetNode<SceneManager>("/root/SceneManager").GoToMainMenu();
	}
}
