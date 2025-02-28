using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace State 
{
    public class LoadingState : IGameState
    {
        private readonly DiContainer _diContainer;
        private readonly GameStateMachine _stateMachine;
        private ImageService _imageService;
        
        public LoadingState( DiContainer diContainer, GameStateMachine stateMachine)
        {
            _diContainer = diContainer;
            _stateMachine = stateMachine;
        }

        public IEnumerator Enter()
        {
            Debug.Log("Loading: Loading images...");
            _imageService = _diContainer.Resolve<ImageService>();
            yield return _imageService.LoadImages();
            List<Sprite> spriteList = new List<Sprite>(_imageService.LoadedSprites.Values);
            SpriteProvider.Instance.SetSprites(spriteList);

            _stateMachine.ChangeState(new GameLoopState());
        }

        public void Exit()
        {
            Debug.Log("Loading: Download complete");
        }
    }
}