using DG.Tweening;

namespace Utility.TransitionFade
{
    //MEMO: 参考：https://github.com/TripleAt/ShaderFade
    public interface IUtilTransition
    {
        bool IsActiveFadeIn();
        IUtilTransition Fade(float startVal, float endVal, float duration);
        IUtilTransition Fade(float startVal, float endVal, float duration, Ease easeMode);
        IUtilTransition Complete(Transition.Callback action);
        IUtilTransition FadeIn(float duration);
        IUtilTransition FadeIn(float duration, Ease easeMode);
        IUtilTransition FadeOut(float duration);
        IUtilTransition FadeOut(float duration, Ease easeMode);

    }
}