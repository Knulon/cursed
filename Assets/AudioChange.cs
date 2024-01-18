using UnityEngine;
using UnityEngine.UI;
public class AudioChange : MonoBehaviour
{
    // Start is called before the first frame update
    Slider slider;
    [SerializeField]
    Settings settings;

    public void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        slider.value = PlayerPrefs.GetFloat("_Volume", 1);
        if (settings != null)
            settings.changeVolume();
    }

    public void OnSliderValueChanged(float value)
    {
        PlayerPrefs.SetFloat("_Volume", value);
        /*
        if(settings != null)
            settings.changeVolume();
        */
    }
}
