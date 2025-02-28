using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Card : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Sprite hiddenIconSprite;
    [SerializeField] private Button actionBtn;

    private Sprite _iconSprite;
    private bool _isSelected;
    private CardController _cardController;
    private float _animationDuration = 0.2f;

    public bool IsSelected => _isSelected;
    public Sprite IconSprite => _iconSprite;

    [Inject]
    public void Construct(CardController cardController)
    {
        _cardController = cardController;
    }

    private void Start()
    {
        actionBtn.onClick.AddListener(() => _cardController.SetSelected(this));
    }

    public void SetIconSprite(Sprite icon)
    {
        _iconSprite = icon;
    }

    public void Show()
    {
        transform.DORotate(new Vector3(0, 90, 0), _animationDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            icon.sprite = _iconSprite;
            transform.DORotate(Vector3.zero, _animationDuration);
        });
        _isSelected = true;
    }

    public void Hide()
    {
        transform.DORotate(new Vector3(0, 90, 0), _animationDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            icon.sprite = hiddenIconSprite;
            transform.DORotate(Vector3.zero, _animationDuration);
        });
        _isSelected = false;
    }
}