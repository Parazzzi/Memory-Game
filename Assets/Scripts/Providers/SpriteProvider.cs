using System.Collections.Generic;
using UnityEngine;

public class SpriteProvider : MonoBehaviour
{
    public List<Sprite> Sprites { get; private set; } = new List<Sprite>();
    public static SpriteProvider Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetSprites(List<Sprite> sprites)
    {
        Sprites = sprites;
    }
}