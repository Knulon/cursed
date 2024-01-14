using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] Slider volume;
    [SerializeField] AudioSource musicSource;

    public void changeVolume(){
        musicSource.volume = volume.value;
    }
}
