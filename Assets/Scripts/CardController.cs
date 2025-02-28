using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

public class CardController : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private RectTransform placeToSpawn;

    [SerializeField] private Button restartButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Text pairCounterText;

    [SerializeField] private CanvasGroup mainContentGrp;
    [SerializeField] private CanvasGroup playBtnGrp;
    [SerializeField] private CanvasGroup restartBtnGrp;

    private readonly List<Card> _cards = new List<Card>();
    private List<Sprite> _sprites = new List<Sprite>();
    private List<Sprite> _spritePairs = new List<Sprite>();

    private Card _firstSelected;
    private Card _secondSelected;
    private DiContainer _diContainer;

    private float _animationDuration = 0.5f;
    private int _matchedPairs;
    private const int MaxPairs = 3;
    private bool _isChecking;


    [Inject]
    public void Construct(DiContainer diContainer)
    {
        _diContainer = diContainer;
    }

    private void Start()
    {
        _sprites = SpriteProvider.Instance.Sprites;
        Debug.Log($"Отримано {_sprites.Count} спрайтів з SpriteProvider.");

        restartButton.onClick.AddListener(RestartGame);
        playButton.onClick.AddListener(() =>
        {
            AnimateCanvasGrp(playBtnGrp, 0, () =>
            {
                playButton.gameObject.SetActive(false);
                AnimateCanvasGrp(mainContentGrp, 1);
            });
        });


        StartGame();
    }

    private void StartGame()
    {
        _matchedPairs = 0;
        UpdatePairCounter();
        PrepareSprites();
        CreateCards();
    }

    private void RestartGame()
    {
        foreach (var card in _cards)
        {
            Destroy(card.gameObject);
        }

        _cards.Clear();
        AnimateCanvasGrp(mainContentGrp, 1);
        AnimateCanvasGrp(restartBtnGrp, 0, () => restartButton.gameObject.SetActive(false));
        StartGame();
    }

    public void SetSelected(Card card)
    {
        if (card.IsSelected || _isChecking) return;
        card.Show();
        if (_firstSelected == null)
        {
            _firstSelected = card;
            return;
        }

        if (_secondSelected == null)
        {
            _secondSelected = card;
            StartCoroutine(CheckMatching());
        }
    }

    private IEnumerator CheckMatching()
    {
        _isChecking = true;
        yield return new WaitForSeconds(0.3f);
        if (_firstSelected.IconSprite == _secondSelected.IconSprite)
        {
            _matchedPairs++;
            UpdatePairCounter();
            if (_matchedPairs == MaxPairs)
            {
                restartButton.gameObject.SetActive(true);
                AnimateCanvasGrp(restartBtnGrp, 1);
                AnimateCanvasGrp(mainContentGrp, 0);
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
            _firstSelected.Hide();
            _secondSelected.Hide();
        }

        _firstSelected = null;
        _secondSelected = null;
        yield return new WaitForSeconds(0.3f);
        _isChecking = false;
    }

    private void PrepareSprites()
    {
        _spritePairs = new List<Sprite>();
        List<Sprite> availableSprites = new List<Sprite>(_sprites);

        for (int i = 0; i < MaxPairs; i++)
        {
            if (availableSprites.Count == 0)
            {
                Debug.LogError("Не вистачає спрайтів для створення всіх пар.");
                break;
            }
        
            int randomIndex = Random.Range(0, availableSprites.Count);
            Sprite selectedSprite = availableSprites[randomIndex];
        
            _spritePairs.Add(selectedSprite);
            _spritePairs.Add(selectedSprite);
        
            availableSprites.RemoveAt(randomIndex);
        }
    
        ShuffleSprites(_spritePairs);
    }


    private void CreateCards()
    {
        foreach (var sprite in _spritePairs)
        {
            GameObject cardGameObject = _diContainer.InstantiatePrefab(cardPrefab);
            cardGameObject.transform.SetParent(placeToSpawn, false);
            Card card = cardGameObject.GetComponent<Card>();
            card.SetIconSprite(sprite);
            _cards.Add(card);
        }
    }

    private void ShuffleSprites(List<Sprite> spriteList)
    {
        for (int i = spriteList.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (spriteList[i], spriteList[randomIndex]) = (spriteList[randomIndex], spriteList[i]);
        }
    }

    private void UpdatePairCounter() => pairCounterText.text = $"Pairs: {_matchedPairs}/{MaxPairs}";

    private void AnimateCanvasGrp(CanvasGroup canvasGroup, float value, Action onComplete = null) =>
        canvasGroup.DOFade(value, _animationDuration).SetEase(Ease.Linear).OnComplete(() => onComplete?.Invoke());
}