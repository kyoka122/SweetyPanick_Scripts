using InGame.Stage.Entity;
using InGame.Stage.View;
using UniRx;

namespace InGame.Stage.Logic
{
    public class CandyLightsLogic
    {
        private readonly StageGimmickEntity _stageGimmickEntity;
        private readonly CandyLightsGimmickView[] _candyLightsGimmickViews;

        public CandyLightsLogic(StageGimmickEntity stageGimmickEntity, CandyLightsGimmickView[] candyLightsGimmickViews)
        {
            _stageGimmickEntity = stageGimmickEntity;
            _candyLightsGimmickViews = candyLightsGimmickViews;
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            foreach (var candyLightsGimmickView in _candyLightsGimmickViews)
            {
                candyLightsGimmickView.FixedCandy
                    .Subscribe(_ =>
                    {
                        candyLightsGimmickView.OnLightAnimation(_stageGimmickEntity.CandyLightsParticleDuration);
                    }).AddTo(candyLightsGimmickView);
            }
        }
    }
}