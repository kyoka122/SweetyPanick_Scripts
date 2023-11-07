namespace InGame.Stage.View
{
    public interface IBoundAble
    {
        public float BoundPower { get; }
        public bool BoundAble { get; }
        public void PlayPressAnimation();
    }
}