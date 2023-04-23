using System.Collections.Generic;
using System.Linq;
using MyApplication;
using UnityEngine;

namespace InGame.Player.View
{
    public class StageObjectsView:MonoBehaviour
    {
        [SerializeField] private List<DoorData> doorData;

        public List<DoorData> GetDoorData()
        {
            return doorData.Where(param => param.DoorView != null).ToList();
        }
    }
}