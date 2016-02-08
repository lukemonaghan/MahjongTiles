using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;

    void OnEnable()
    {
        GameParameters.Instance.LoadAudioLevel();

        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = AudioListener.volume;

        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(UpdateVolume);
    }

    void UpdateVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Vol", value);
        PlayerPrefs.Save();
    }
}
