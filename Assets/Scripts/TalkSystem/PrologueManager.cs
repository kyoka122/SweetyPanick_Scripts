using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace TalkSystem{

    [Serializable]
    public class PrologueManager
    {
        private enum characterName
        {
            Candy,
            Fu,
            Mash,
            Kure
        }

        [SerializeField]
        private characterName _characterName;

        [SerializeField]
        private Sprite[] characterFaces;
    }
}
