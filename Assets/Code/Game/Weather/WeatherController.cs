using UnityEngine;

public class WeatherController : MonoBehaviour
{
    [SerializeField] private PlayerView view;
    
    private void OnEnable()
    {
        view.OnWeatherChanged += HandleWeatherChanged;
        SetWeather(view.Weather);
    }

    private void OnDisable()
    {
        view.OnWeatherChanged -= HandleWeatherChanged;
    }

    private void HandleWeatherChanged(WeatherType weather) => SetWeather(weather);

    private void SetWeather(WeatherType weather)
    {
        Debug.Log($"Weather set to {weather}");
    }
    
    public void OverrideWeather(WeatherType weather)
    {
        view.RequestWeatherOverrideRpc(weather);
    }
}
