using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ImageLoader : MonoBehaviour
{
   [SerializeField] private CardController _cardController;
   
    private const string JsonUrl =
        "https://drive.usercontent.google.com/download?id=1dO8fLzCbkD_Lbj5TLHBRZGXunsd-HZwB&export=download&authuser=0&confirm=t&uuid=45e9d88f-42af-4f46-914d-067326e6365e&at=AEz70l5fF1kyHy5IxFgjESh06__C:1740764770921";
    
    private void Start()
    {
        StartCoroutine(LoadJson());
    }

    private IEnumerator LoadJson()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(JsonUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load JSON: " + request.error);
                yield break;
            }

            ImageData imageData = JsonConvert.DeserializeObject<ImageData>(request.downloadHandler.text);
            StartCoroutine(LoadImages(imageData.images));
        }
    }

    private IEnumerator LoadImages(List<ImageInfo> imageInfos)
    {
        Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();

        foreach (ImageInfo info in imageInfos)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(info.url))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load image: {info.url}");
                    continue;
                }

                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );

                if (!loadedSprites.ContainsKey(info.name))
                {
                    loadedSprites.Add(info.name, sprite);
                }
                else
                {
                    Debug.LogWarning($"Sprite with name '{info.name}' already exists!");
                }
            }
        }
    }
    

    [System.Serializable]
    private class ImageData
    {
        public List<ImageInfo> images;
    }

    [System.Serializable]
    private class ImageInfo
    {
        public string name;
        public string url;
    }
}