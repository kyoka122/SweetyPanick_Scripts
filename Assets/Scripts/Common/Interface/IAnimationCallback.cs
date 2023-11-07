namespace Common.Interface
{
    public interface IAnimationCallback
    {
        /// <summary>
        /// Animatorからのコールバック
        /// </summary>
        public void CallbackAnimation(string animationClipName);
    }
}