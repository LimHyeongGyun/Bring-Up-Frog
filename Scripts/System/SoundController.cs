using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class SoundController : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Slider audioSlider;
    public GameObject SoundOn;
    public GameObject SoundOff;
    bool activeSound;

    public void AudioControl()
    {
        float sound = audioSlider.value;

        if (sound == -40f)
            masterMixer.SetFloat("BGM", -80);
        else masterMixer.SetFloat("BGM", sound);

        activeSound = AudioListener.volume == 0 ? true : false;
        SoundOn.SetActive(!activeSound);
        SoundOff.SetActive(activeSound);
    }

    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
        Debug.Log("À½¼Ò°Å On");
        activeSound = AudioListener.volume == 0 ? true : false;
        SoundOn.SetActive(!activeSound);
        SoundOff.SetActive(activeSound);
    }
}
