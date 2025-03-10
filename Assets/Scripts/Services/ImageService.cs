using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public class ImageService : MonoBehaviour
{
    [SerializeField] private string JsonUrl =
        "https://drive.usercontent.google.com/download?id=1-V6cv78S8k4HiT0H9TAhBiJsIizViU1j&export=download&authuser=0&confirm=t&uuid=54ccc47f-f18e-41a0-9254-6b81ac18f295&at=AEz70l4br0QeNn_tgFPXMK8oNZk6:1741621781355";

    public Dictionary<string, Sprite> LoadedSprites { get; private set; }

    public async UniTask LoadImagesAsync()
    {
        string json = await LoadJsonAsync();
        if (string.IsNullOrEmpty(json)) return;

        ImageData imageData = JsonConvert.DeserializeObject<ImageData>(json);
        if (imageData?.images == null || imageData.images.Count == 0)
        {
            Debug.LogError("No image data found");
            return;
        }

        LoadedSprites = await LoadAllImagesAsync(imageData.images);
    }

    private async UniTask<string> LoadJsonAsync()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(JsonUrl))
        {
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load JSON: " + request.error);
                return null;
            }

            return request.downloadHandler.text;
        }
    }

    private async UniTask<Dictionary<string, Sprite>> LoadAllImagesAsync(List<ImageInfo> imageInfos)
    {
        Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();
        List<UniTask<(string name, Sprite sprite)>> tasks = new List<UniTask<(string, Sprite)>>();

        foreach (ImageInfo info in imageInfos)
        {
            tasks.Add(LoadImageAsync(info));
        }

        var results = await UniTask.WhenAll(tasks);

        foreach (var (name, sprite) in results)
        {
            if (!string.IsNullOrEmpty(name) && sprite != null && !loadedSprites.ContainsKey(name))
            {
                loadedSprites.Add(name, sprite);
            }
        }

        return loadedSprites;
    }

    private async UniTask<(string, Sprite)> LoadImageAsync(ImageInfo info)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(info.url))
        {
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load image: {info.url}");
                return (null, null);
            }

            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
            return (info.name, sprite);
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