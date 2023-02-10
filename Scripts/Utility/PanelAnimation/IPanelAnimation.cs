using System.Threading;
using Cysharp.Threading.Tasks;

namespace Utility.PanelAnimation
{
    public interface IPanelAnimation
    {
        public CancellationToken Token { get; }
        public UniTask Enter(CancellationToken token);
        public UniTask Exit(CancellationToken token);
    }
}