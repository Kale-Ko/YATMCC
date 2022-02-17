using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static Settings Instance;

    public Slider sensitivitySlider;
    public Text sensitivityText;

    public Slider distanceSlider;
    public Text distanceText;

    void Start()
    {
        Settings.Instance = this;

        sensitivitySlider.value = Config.sensitivity;
        sensitivityText.text = Config.sensitivity.ToString();
        distanceSlider.value = Config.distance;
        distanceText.text = Config.distance.ToString();
    }

    public void UpdateSensitivity()
    {
        Config.sensitivity = sensitivitySlider.value;
        sensitivityText.text = Config.sensitivity.ToString();
    }

    public void UpdateDistance()
    {
        Config.distance = Mathf.RoundToInt(distanceSlider.value);
        distanceText.text = Config.distance.ToString();
    }
}