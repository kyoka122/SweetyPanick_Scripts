using InGame.Colate.View;
using InGame.Enemy.View;
using UnityEngine;

namespace InGame.Enemy.Installer
{
    public class ViewGenerator:MonoBehaviour
    {
        
        public DefaultEnemyView GenerateDefaultEnemyView(DefaultEnemyView defaultEnemyPrefab)
        {
            return Instantiate(defaultEnemyPrefab);
        }
    }
}