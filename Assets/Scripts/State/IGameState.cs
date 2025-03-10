using System.Collections;
using Cysharp.Threading.Tasks;

namespace State
{
    public interface IGameState
    {
        IEnumerator Enter();
        void Exit();
    } 

}