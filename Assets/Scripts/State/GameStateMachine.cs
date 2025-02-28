using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace State
{
    public class GameStateMachine : MonoBehaviour
    {
        [SerializeField] private Image progressIcon;

        private float _rotationDuration = 2f;
        private IGameState _currentState;

        [Inject] private DiContainer _diContainer;

        public void ChangeState(IGameState newState)
        {
            if (_currentState != null)
                _currentState.Exit();

            _currentState = newState;
            StartCoroutine(_currentState.Enter());

            progressIcon.rectTransform.DORotate(new Vector3(0, 0, -360f), _rotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }

        private void Start()
        {
            var bootstrapState = new BootstrapState(_diContainer, this);
            ChangeState(bootstrapState);
        }
    }
}