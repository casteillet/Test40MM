using System;

public interface IWeatherView
{
    WeatherType Weather { get; }
    bool IsWeatherOverridden { get; }
    event Action<WeatherType> OnWeatherChanged;
}
