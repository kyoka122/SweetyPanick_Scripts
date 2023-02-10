using System;
using UniRx;
using Unity.VisualScripting;
using Utility.TransitionFade;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    //MEMO: 参考：https://github.com/TripleAt/ShaderFade
    public class FadeTest : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite;
        
        private IUtilTransition _fadeObj;
        private bool _isDoneFade;

        private void Start()
        {
            Button btn = GetComponent<Button>();
            _fadeObj = new Transition(sprite.material,this,1);

            //ボタンが押されなおかつ、フェード中ではない?
            btn.OnClickAsObservable().Where(_ => !_fadeObj.IsActiveFadeIn()).Subscribe(_ =>
            {
                if (_isDoneFade)
                {
                    //フェードが完了してたら、フェードインする
                    _fadeObj.FadeOut(1f).Complete(() =>
                    {
                        _isDoneFade = false;
                        Debug.Log("フェードイン終了");
                    });
                    return;
                }

                //フェードが行われてなかったら、フェードアウトする
                _fadeObj.FadeIn(1f).Complete(() =>
                {
                    Debug.Log("フェードアウト終了");
                    _isDoneFade = true;
                });
            });
        }
    }
}