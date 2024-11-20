using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

struct Weather
{
    public string Country { get; set; }
    public string Name { get; set; }
    public double Temp { get; set; }
    public string Description { get; set; }
}

class Program
{
    static async Task Main()
    {
        HttpClient client = new();
        const string ApiKey = "11fa82747ccc6c06003115d1dc541888";

        List<Weather> weathers = new();
        Random random = new();

        while (weathers.Count < 50)
        {
            double latitude = random.NextDouble() * 180 - 90;
            double longitude = random.NextDouble() * 360 - 180;

            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={ApiKey}&units=metric";

            try
            {
                var response = await client.GetStringAsync(url);
                dynamic weatherData = JsonConvert.DeserializeObject(response);

                if (weatherData.name != "" && weatherData.sys.country != "")
                {
                    Weather currentWeather = new()
                    {
                        Country = weatherData.sys.country,
                        Name = weatherData.name,
                        Temp = weatherData.main.temp,
                        Description = weatherData.weather[0].description
                    };
                    weathers.Add(currentWeather);
                }
            }
            catch
            {
                Console.WriteLine("Ошибка при запросе данных.");
            }
        }

        var CountryWithMaxTemp = (from weather in weathers
                                  orderby weather.Temp descending
                                  select weather).First();
        Console.WriteLine($"Страна с наибольшей температурой: {CountryWithMaxTemp.Country}, температура: {CountryWithMaxTemp.Temp}°С");

        var CountryWithMinTemp = (from weather in weathers
                                  orderby weather.Temp
                                  select weather).First();
        Console.WriteLine($"Страна с наименьшей температурой: {CountryWithMinTemp.Country}, температура: {CountryWithMinTemp.Temp}°С");

        double AverageTemp = (from weather in weathers
                              select weather.Temp).Average();
        Console.WriteLine($"Средняя температура в мире: {AverageTemp}°С");

        int CountOfCountries = (from weather in weathers
                                select weather.Country).Distinct().Count();
        Console.WriteLine($"Количество стран: {CountOfCountries}");

        var FirstCountryWithDescription = (from weather in weathers
                                           where weather.Description == "clear sky" ||
                                                 weather.Description == "rain" ||
                                                 weather.Description == "few clouds"
                                           select weather).FirstOrDefault();

        if (FirstCountryWithDescription.Name != null)
        {
            Console.WriteLine($"Первая страна с нужным описанием: {FirstCountryWithDescription.Country}, название местности: {FirstCountryWithDescription.Name}, описание: {FirstCountryWithDescription.Description}");
        }
        else
        {
            Console.WriteLine("Нет местностей с указанным описанием.");
        }
    }
}
