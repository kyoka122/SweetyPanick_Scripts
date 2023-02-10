using System;
using InGame.Common.Database;
using MyApplication;
using OutGame.Database;
using OutGame.PlayerCustom.Installer;
using OutGame.PlayerCustom.View;
using UnityEngine;

namespace OutGame.PlayerCustom.Entity
{
    public class ConstDataEntity
    {
        public float PopUpDuration =>_outGameDatabase.GetPlayerCustomSceneData().PanelPopUpDuration;
        public float PopDownDuration =>_outGameDatabase.GetPlayerCustomSceneData().PanelPopDownDuration;

        public CharacterSelectCursorInstanceManager CharacterSelectCursorInstanceManager =>
            _outGameDatabase.GetPlayerCustomSceneData().CharacterSelectCursorInstanceManager;
        
        public CharacterSelectCursorView CharacterSelectCursorPrefab =>
            _outGameDatabase.GetPlayerCustomSceneData().CharacterSelectCursorView;

        public SquareRange CharacterSelectCursorRange =>
            _outGameDatabase.GetPlayerCustomSceneData().CharacterSelectCursorRange;
        
        public Func<Vector2,Vector2> WorldToScreenPoint=>
        _commonDatabase.GetIReadOnlyCameraController().WorldToScreenPoint;

        private readonly OutGameDatabase _outGameDatabase;
        private readonly CommonDatabase _commonDatabase;

        public ConstDataEntity(OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            _outGameDatabase = outGameDatabase;
            _commonDatabase = commonDatabase;
        }
    }
}