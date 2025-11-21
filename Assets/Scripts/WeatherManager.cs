using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherManager : MonoBehaviour
{

    private const string xmlApi = "https://api.openweathermap.org/data/2.5/weather?lat=28.54&lon=-81.38&mode=xml&appid=dc349e3935101abb942e111db21c2985";
    public Material skyboxMaterial;
    private IEnumerator CallAPI(string url, Action<string> callback)
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
                callback(request.downloadHandler.text);
            }
        }
    }

    public IEnumerator GetWeatherXML(Action<string> callback)
    {
        return CallAPI(xmlApi, callback);
    }

    void Start() 
    {
        StartCoroutine(GetWeatherXML(OnXMLDataLoaded));
        RenderSettings.skybox = skyboxMaterial;
    }

    public void OnXMLDataLoaded(string data)
    {
        Debug.Log(data);
    }
}
