using System.Collections;

namespace State
{
    public interface IGameState
    {
        IEnumerator Enter();
        void Exit();
    }
}