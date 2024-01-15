using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsSetter : MonoBehaviour
{

    Slider slider;

    public void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void OnSliderValueChanged(float value)
    {
        Debug.Log("Set TurningSensitivity to " + value);
        PlayerPrefs.SetFloat("_TurningSensitivity", value);
    }
}
