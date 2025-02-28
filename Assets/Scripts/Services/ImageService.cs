using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace State
{
    public class ImageService : MonoBehaviour
    {
        private const string JsonUrl =
            "https://drive.usercontent.google.com/download?id=1IgsnzGt2GzwFbwHaywdXseffpsmfl1Yp&export=download&authuser=0&confirm=t&uuid=64354712-6a51-4345-93b4-3c8ed91c910f&at=AEz70l4guQtHyoaiiH68B_DiDwys:1740744146984";

        public Dictionary<string, Sprite> LoadedSprites { get; private set; }

        public IEnumerator LoadImages()
        {
            LoadedSprites = new Dictionary<string, Sprite>();

            using (UnityWebRequest request = UnityWebRequest.Get(JsonUrl))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Не вдалося завантажити JSON: " + request.error);
                    yield break;
                }

                ImageData imageData = JsonConvert.DeserializeObject<ImageData>(request.downloadHandler.text);
                yield return StartCoroutine(DownloadImages(imageData.images));
            }
        }

        private IEnumerator DownloadImages(List<ImageInfo> imageInfos)
        {
            foreach (ImageInfo info in imageInfos)
            {
                using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(info.url))
                {
                    yield return request.SendWebRequest();

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"Не вдалося завантажити зображення: {info.url}");
                        continue;
                    }

                    Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    Sprite sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    if (!LoadedSprites.ContainsKey(info.name))
                    {
                        LoadedSprites.Add(info.name, sprite);
                    }
                    else
                    {
                        Debug.LogWarning($"Спрайт з ім'ям '{info.name}' вже існує!");
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
}