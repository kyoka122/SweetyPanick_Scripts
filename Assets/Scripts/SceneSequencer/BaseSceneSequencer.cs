using InGame.Common.Database;
using InGame.Database;
using OutGame.Database;
using UniRx;
using Utility;

namespace SceneSequencer
{
    public abstract class BaseSceneSequencer:SingletonMonoBehaviour<BaseSceneSequencer>
    {
        protected override bool IsDontDestroyOnLoad => false;
        protected Subject<string> toNextSceneFlag;
        
        public void Execute(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            toNextSceneFlag = new Subject<string>();
            Init(inGameDatabase, outGameDatabase, commonDatabase);
            ProcessInOrder();
            toNextSceneFlag
                .Subscribe(Finish)
                .AddTo(this);
        }

        protected abstract void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase);
        protected abstract void ProcessInOrder();
        protected abstract void Finish(string nextSceneName);
    }
}