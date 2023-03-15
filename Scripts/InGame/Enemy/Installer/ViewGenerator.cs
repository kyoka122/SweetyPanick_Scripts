using InGame.Enemy.View;
using UnityEngine;

namespace InGame.Enemy.Installer
{
    public class ViewGenerator:MonoBehaviour
    {
        
        public DefaultEnemyView GenerateDefaultEnemyView(DefaultEnemyView defaultEnemyPrefab,Vector2 spawnPos)
        {
            return Instantiate(defaultEnemyPrefab,spawnPos,Quaternion.identity);
        }
    }
}