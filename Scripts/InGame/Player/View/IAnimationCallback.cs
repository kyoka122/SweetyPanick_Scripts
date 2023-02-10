namespace InGame.Player.View
{
    public interface IAnimationCallback
    {
        /// <summary>
        /// Animatorからのコールバック
        /// </summary>
        public void CallbackAnimation(string animationClipName);
    }
}