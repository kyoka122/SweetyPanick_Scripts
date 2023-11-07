using Common.View;
using UnityEngine;
using Utility;

namespace InGame.Player.View
{
    public class CallbackAnimatorGeneratorView : MonoBehaviour, IPoolObjectGenerator<CallbackAnimatorView>
    {
        public CallbackAnimatorView Prefab => prefab;
        [SerializeField] private CallbackAnimatorView prefab;

        public CallbackAnimatorView Generate()
        {
            CallbackAnimatorView callbackAnimatorView = Instantiate(prefab);
            callbackAnimatorView.Init();
            return callbackAnimatorView;
        }
    }
}