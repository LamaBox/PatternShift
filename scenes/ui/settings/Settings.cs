using Godot;

public partial class Settings : Control
{
	[Export] private OptionButton _screenModeOption;
	[Export] private OptionButton _resolutionOption;
	[Export] private HSlider _uiScaleSlider;
	[Export] private OptionButton _vsyncOption;
	[Export] private OptionButton _fpsLimitOption;
	[Export] private OptionButton _showComboOption;
	[Export] private OptionButton _colorLabelsOption;
	[Export] private OptionButton _languageOption;
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
		_screenModeOption.ItemSelected += OnScreenModeChanged;
		_resolutionOption.ItemSelected += OnResolutionChanged;
		_uiScaleSlider.ValueChanged += OnUIScaleChanged;
		_vsyncOption.ItemSelected += OnVsyncChanged;
		_fpsLimitOption.ItemSelected += OnFpsLimitChanged;
		_showComboOption.ItemSelected += OnShowComboChanged;
		_colorLabelsOption.ItemSelected += OnColorLabelsChanged;
		_languageOption.ItemSelected += OnLanguageChanged;
		_closeButton.Pressed += OnClosePressed;

		LoadSettings();
	}

	private void LoadSettings()
	{
		_screenModeOption.Select(2);
		_resolutionOption.Select(0);
		_uiScaleSlider.Value = 1.0f;
		_vsyncOption.Select(0);
		_fpsLimitOption.Select(1);
		_showComboOption.Select(0);
		_colorLabelsOption.Select(1);
		_languageOption.Select(0);
		_musicVolumeSlider.Value = 80;
		_soundsVolumeSlider.Value = 80;
		UpdateMusicLabel(80);
		UpdateSoundsLabel(80);
	}

	private void OnScreenModeChanged(long index)
	{
		int mode = (int)index;
		switch (mode)
		{
			case 0:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				break;
			case 1:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				break;
			case 2:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				break;
		}
		GD.Print($"Screen mode: {_screenModeOption.GetItemText(mode)}");
	}

	private void OnResolutionChanged(long index)
	{
		int idx = (int)index;
		GD.Print($"Resolution: {_resolutionOption.GetItemText(idx)}");
	}

	private void OnUIScaleChanged(double value)
	{
		GD.Print($"UI Scale: {value:F2}");
	}

	private void OnVsyncChanged(long index)
	{
		bool enabled = index == 0;
		DisplayServer.WindowSetVsyncMode(enabled ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);
		GD.Print($"V-Sync: {(enabled ? "Вкл" : "Выкл")}");
	}

	private void OnFpsLimitChanged(long index)
	{
		int idx = (int)index;
		string fps = _fpsLimitOption.GetItemText(idx);
		
		if (fps == "Отключено" || fps == "Off")
		{
			Engine.MaxFps = 0;
			GD.Print("FPS Limit: Off");
			return;
		}
		
		if (int.TryParse(fps, out int fpsValue))
		{
			Engine.MaxFps = fpsValue;
			GD.Print($"FPS Limit: {fps}");
		}
		else
		{
			GD.PrintErr($"Invalid FPS value: {fps}");
		}
	}

	private void OnShowComboChanged(long index)
	{
		bool enabled = index == 0;
		GD.Print($"Show Combo: {(enabled ? "Вкл" : "Выкл")}");
	}

	private void OnColorLabelsChanged(long index)
	{
		bool enabled = index == 0;
		GD.Print($"Color Labels: {(enabled ? "Вкл" : "Выкл")}");
	}

	private void OnLanguageChanged(long index)
	{
		int idx = (int)index;
		GD.Print($"Language: {_languageOption.GetItemText(idx)}");
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
		QueueFree();
	}
}
