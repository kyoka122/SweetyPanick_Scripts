using InGame.Database;
using InGame.Colate.Manager;
using InGame.Stage.Manager;
using UnityEngine;

namespace InGame.Stage.Installer
{
    public class ColateStageGimmickInstaller:MonoBehaviour
    {
        public ColateStageGimmickManager Install(InGameDatabase inGameDatabase)
        {
            return new ColateStageGimmickManager();
        }
    }
}