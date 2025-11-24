using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class BillboardImagePicker : MonoBehaviour
{
    public string billboardID;
    public string[] imageUrls;

    private string localPath;
    private Renderer billboardRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        billboardRenderer = GetComponent<Renderer>();
        int randomIndex = UnityEngine.Random.Range(0, imageUrls.Length);
        Debug.Log("Selected image URL: " + imageUrls[randomIndex]);
        localPath = Application.persistentDataPath + "/" + billboardID + "_" + randomIndex + ".png";
        StartCoroutine(LoadImageFromURL(imageUrls[randomIndex], ApplyTexture));
    }

    private IEnumerator LoadImageFromURL(string url, Action<Texture2D> callback)
    {
        byte[] imageData;
        if(!File.Exists(localPath))
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            Debug.Log("Downloading image from URL: " + url);
            imageData = DownloadHandlerTexture.GetContent(request).EncodeToPNG();
            File.WriteAllBytes(localPath, imageData);
        }
        else
        {
            imageData = File.ReadAllBytes(localPath);
        }
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);
        callback(texture);
    }

    private void ApplyTexture(Texture2D texture)
    {
        billboardRenderer.material.mainTexture = texture;
    }

    public void ClearCachedImage()
    {
        for (int i = 0; i < imageUrls.Length; i++)
        {
            string path = Application.persistentDataPath + "/" + billboardID + "_" + i + ".png";
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("Deleted cached image at: " + path);
            }
        }
    }
}
