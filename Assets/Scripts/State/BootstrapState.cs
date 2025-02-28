using System.Collections;
using UnityEngine;
using Zenject;

namespace State
{
    public class BootstrapState : IGameState
    {
        private readonly DiContainer _diContainer;
        private readonly GameStateMachine _stateMachine;

        public BootstrapState(DiContainer diContainer, GameStateMachine stateMachine)
        {
            _diContainer = diContainer;
            _stateMachine = stateMachine;
        }

        public IEnumerator Enter()
        {
            Debug.Log("Bootstrap: Initializing services....");
            Debug.Log(_diContainer != null ? "DiContainer finished" : "DiContainer = null");

            var imageServiceInstance = _diContainer.InstantiateComponentOnNewGameObject<ImageService>("ImageService");
            _diContainer.Bind<ImageService>().FromInstance(imageServiceInstance).AsSingle();
            
            yield return null;
            _stateMachine.ChangeState(new LoadingState(_diContainer, _stateMachine));
        }

        public void Exit()
        {
            Debug.Log("Bootstrap: Initialization complete");
        }
    }
}