using UniRx;

namespace OutGame.MyInput
{
    //MEMO: PCキー、Joyconの両方を使えるようにするための基底クラス
    public abstract class BasePlayerInput
    {
        public IReadOnlyReactiveProperty<float> HorizontalValue=>horizontalValue;
        public IReadOnlyReactiveProperty<float> VerticalValue => verticalValue;
        public IReadOnlyReactiveProperty<bool> Next => next;
        public IReadOnlyReactiveProperty<bool> Back => back;
        public IReadOnlyReactiveProperty<bool> BackPrevMenu => backPrevMenu;
        
        protected readonly ReactiveProperty<float> horizontalValue;
        protected readonly ReactiveProperty<float> verticalValue;
        protected readonly ReactiveProperty<bool> next;
        protected readonly ReactiveProperty<bool> back;
        protected readonly ReactiveProperty<bool> backPrevMenu;
        
        
        protected BasePlayerInput()
        {
            horizontalValue = new ReactiveProperty<float>();
            verticalValue = new ReactiveProperty<float>();
            next = new ReactiveProperty<bool>();
            back = new ReactiveProperty<bool>();
            backPrevMenu = new ReactiveProperty<bool>();
        }

        public virtual BasePlayerInput Clone()
        {
            return MemberwiseClone() as BasePlayerInput;
        }
    }
}