using System.Collections.Generic;
using System.Linq;

namespace InGame.Enemy.View
{
    public class AllEnemyView
    {
        public IReadOnlyList<DefaultEnemyView> DefaultEnemyViews => _defaultEnemyViews;
        public IReadOnlyList<EternitySleepEnemyView> EternitySleepEnemyViews => _eternitySleepEnemyViews;
        public IReadOnlyList<WakeUpEnemyView> WakeUpEnemyViews => _wakeUpEnemyViews;
        
        private readonly List<DefaultEnemyView> _defaultEnemyViews;
        private readonly List<EternitySleepEnemyView> _eternitySleepEnemyViews;
        private readonly List<WakeUpEnemyView> _wakeUpEnemyViews;
        
        public AllEnemyView(List<DefaultEnemyView> defaultEnemyViews,
            List<EternitySleepEnemyView> eternitySleepEnemyViews, List<WakeUpEnemyView> wakeUpEnemyViews)
        {
            _defaultEnemyViews = defaultEnemyViews;
            _eternitySleepEnemyViews = eternitySleepEnemyViews;
            _wakeUpEnemyViews = wakeUpEnemyViews;
        }

        public IReadOnlyList<BaseEnemyView> GetALlEnemyView()
        {
            IEnumerable<BaseEnemyView> enemyViews = _defaultEnemyViews;
            return enemyViews.Concat(_eternitySleepEnemyViews).Concat(_wakeUpEnemyViews).ToList();
        }
        
        public void RemoveDefaultEnemyFromList(DefaultEnemyView defaultEnemyView)
        {
            _defaultEnemyViews.Remove(defaultEnemyView);
        }
        
        public void RemoveEternitySleepEnemyView(EternitySleepEnemyView eternitySleepEnemyView)
        {
            _eternitySleepEnemyViews.Remove(eternitySleepEnemyView);
        }
        
        public void RemoveWakeUpEnemyView(WakeUpEnemyView wakeUpEnemyView)
        {
            _wakeUpEnemyViews.Remove(wakeUpEnemyView);
        }
    }
}