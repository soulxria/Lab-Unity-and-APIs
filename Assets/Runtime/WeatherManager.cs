using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherManager : MonoBehaviour
{
    public string location = "Orlando,us";
    public Material clearDawnSkyboxMaterial;
    public Material clearDaySkyboxMaterial;
    public Material clearDuskSkyboxMaterial;
    public Material clearNightSkyboxMaterial;
    public Material rainSkyboxMaterial;
    public Material snowSkyboxMaterial;
    public Light directionalLight;
    public float dawnDuskMargin;

    private enum TimeOfDay
    {
        Dawn,
        Day,
        Dusk,
        Night
    }
    private TimeOfDay currentTimeOfDay;
    private string xmlApi => "http://api.openweathermap.org/data/2.5/weather?q=" + location + "&mode=xml&appid=dc349e3935101abb942e111db21c2985";

    private IEnumerator CallAPI(string url, Action<XDocument> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError($"network problem: {request.error}");
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"response error: {request.responseCode}");
            }
            else
            {
                callback(XDocument.Parse(request.downloadHandler.text));
            }
        }
    }

    public IEnumerator GetWeatherXML(Action<XDocument> callback)
    {
        return CallAPI(xmlApi, callback);
    }

    void Start() 
    {
        directionalLight.color = Color.white;
        directionalLight.intensity = 1.0f;
        RenderSettings.skybox = clearDaySkyboxMaterial;
        StartCoroutine(GetWeatherXML(OnXMLDataLoaded));

        DateTime now = DateTime.Now;
    }

    public void OnXMLDataLoaded(XDocument data)
    {
        Debug.Log(data);

        if (data.Root.Element("city") != null)
        {
            Debug.Log("Successfully retrieved weather data for " + data.Root.Element("city").Attribute("name").Value);
        }
        
        DateTime now = DateTime.Now;

        XElement sun = data.Root.Element("city").Element("sun");
        DateTime sunrise = DateTime.Parse(sun.Attribute("rise").Value).ToLocalTime();
        DateTime sunset = DateTime.Parse(sun.Attribute("set").Value).ToLocalTime();


        if (now < sunrise.AddMinutes(-dawnDuskMargin) || now > sunset.AddMinutes(dawnDuskMargin))
        {
            Debug.Log("It's night time.");
            directionalLight.intensity = 1.3f;
            directionalLight.color = new Color32(55, 95, 135, 255);
            currentTimeOfDay = TimeOfDay.Night;
        }
        else if (now >= sunrise.AddMinutes(-dawnDuskMargin) && now <= sunrise.AddMinutes(dawnDuskMargin))
        {
            Debug.Log("It's dawn.");
            directionalLight.intensity = 1.2f;
            directionalLight.color = new Color32(255, 222, 50, 255);
            currentTimeOfDay = TimeOfDay.Dawn;
        }
        else if (now >= sunset.AddMinutes(-dawnDuskMargin) && now <= sunset.AddMinutes(dawnDuskMargin))
        {
            Debug.Log("It's dusk.");
            directionalLight.intensity = 1.6f;
            directionalLight.color = new Color32(254, 180, 32, 255);
            currentTimeOfDay = TimeOfDay.Dusk;
        }
        else
        {
            Debug.Log("It's daytime.");
            directionalLight.intensity = 1.25f;
            directionalLight.color = new Color32(255, 255, 224, 255);
            currentTimeOfDay = TimeOfDay.Day;
        }

        switch (currentTimeOfDay)
        {
            case TimeOfDay.Dawn:
                RenderSettings.skybox = clearDawnSkyboxMaterial;
                break;
            case TimeOfDay.Day:
                RenderSettings.skybox = clearDaySkyboxMaterial;
                break;
            case TimeOfDay.Dusk:
                RenderSettings.skybox = clearDuskSkyboxMaterial;
                break;
            case TimeOfDay.Night:
                RenderSettings.skybox = clearNightSkyboxMaterial;
                break;
        }

        XElement weather = data.Root.Element("precipitation");
        if (weather != null)
        {
            switch (weather.Attribute("mode").Value)
            {
                case "rain":
                    Debug.Log("It's raining!");
                    RenderSettings.skybox = rainSkyboxMaterial;
                    break;
                case "snow":
                    Debug.Log("It's snowing!");
                    RenderSettings.skybox = snowSkyboxMaterial;
                    break;
                default:
                    Debug.Log("No precipitation.");
                    break;
            }
        }
    }
}
