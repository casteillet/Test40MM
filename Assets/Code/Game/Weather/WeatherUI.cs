using KBCore.Refs;
using TMPro;
using UnityEngine;

public class WeatherUI : MonoBehaviour
{
    [SerializeField] private InterfaceRef<IWeatherView> view;
    [SerializeField] private TextMeshProUGUI weatherText;
    [SerializeField] private GameObject overrideIcon;

    private void OnEnable()
    {
        view.Value.OnWeatherChanged += Refresh;
        Refresh(view.Value.Weather);
    }

    private void OnDisable()
    {
        if (view.Value == null) return;
        
        view.Value.OnWeatherChanged -= Refresh;
    }

    private void Refresh(WeatherType weather)
    {
        if (weatherText)
        {
            weatherText.text = weather.ToString();
        }

        if (overrideIcon)
        {
            overrideIcon.SetActive(view.Value.IsWeatherOverridden);
        }
    }
}
