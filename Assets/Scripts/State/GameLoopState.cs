using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace State
{
    public class GameLoopState : IGameState
    {
        public IEnumerator Enter()
        {
            Debug.Log("GameLoop: Launching the game...");
            SceneManager.LoadScene("Main");
            yield return null;
        }

        public void Exit()
        {
            Debug.Log("GameLoop: End of the game cycle");
        }
    }
}