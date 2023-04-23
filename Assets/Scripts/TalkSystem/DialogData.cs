using UnityEngine;
using System;
using MyApplication;
using UnityEngine.Events;

namespace TalkSystem{

    [Serializable]
    public class DialogData
    {
        //[SerializeField]
        [SerializeField][TextArea]
        public string dialog;
        
        //[SerializeField]
        public DialogFaceSpriteData characterFace;

        //public UnityAction action;

        [SerializeField,Tooltip("Dialog出力前のイベント")]
        public UnityEvent prevEvent;
    }

}
