using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderScript : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("VolumeValue");
    }

    public void changeVolume(float newVolume)
    {
        AudioListener.volume = newVolume;
        PlayerPrefs.SetFloat("VolumeValue", newVolume);
    }

    public void savePlayerPrefs()
    {
        PlayerPrefs.Save();
    }
}
