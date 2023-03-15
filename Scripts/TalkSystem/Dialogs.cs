using System;
using System.Collections;
using MyApplication;
using UnityEngine;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

namespace TalkSystem
{

    public class Dialogs : MonoBehaviour
    {
        [SerializeField, Range(1, 10)] private float normalTextSpeed = 1;
        [SerializeField, Range(1, 10)] private float fastTextSpeed = 1;
        
        [SerializeField]
        DialogData[] dialogDatas;

        [SerializeField]
        TextMeshProUGUI dialogTextsCenter;
        [SerializeField]
        private TextMeshProUGUI dialogTextsLeft;
        [SerializeField]
        private TextMeshProUGUI dialogTextsRight;

        [SerializeField]
        private GameObject[] speechBalloons;

        [SerializeField]
        private GameObject[] nameTextBox;

        [SerializeField]
        private TextMeshProUGUI characterNameTextLeft;
        [SerializeField]
        private TextMeshProUGUI characterNameTextRight;

        [SerializeField]
        private Image characterImageLeft;
        [SerializeField]
        private Image characterImageRight;

        [SerializeField]
        private GameObject nextButtonImageObjLeft;
        [SerializeField]
        private GameObject nextButtonImageObjRight;
        [SerializeField]
        private GameObject nextButtonImageObjCenter;

        private int dialogNum;
        private int count;
        private float toDisplayNextCharacterDurationNormal;
        private float toDisplayNextCharacterDurationFast;
        private float currentToDisplayNextCharacterDuration;
        
        private bool isInputNextKey = false;
        private bool isStartedDialog = false;
        private bool canActiveNextButtonImage=true;
        
        private BaseTalkKeyObserver _talkKeyObserver;
        private DialogFaceSpriteScriptableData _faceData;
        private Coroutine _dialogCoroutine;
        
        private struct SpriteAssetData
        {
            public string SpriteAssetString { get; }
            public int StringIndexCount { get; }

            public SpriteAssetData(string spriteAssetString, int stringIndexCount)
            {
                SpriteAssetString = spriteAssetString;
                StringIndexCount = stringIndexCount;
            }
        }
        
        public void Init(BaseTalkKeyObserver talkKeyObserver,DialogFaceSpriteScriptableData faceData)
        {
            SetTalkKeyObserver(talkKeyObserver);
            Init(faceData);
        }
        
        public void Init(DialogFaceSpriteScriptableData faceData)
        {
            _faceData = faceData;
            
            for (int i = 0; i < speechBalloons.Length; i++){
                speechBalloons[i].SetActive(false);
            }

            for (int i = 0; i < nameTextBox.Length; i++)
            {
                nameTextBox[i].SetActive(false);
            }

            dialogTextsCenter.text = "";
            dialogTextsLeft.text = "";
            dialogTextsRight.text = "";

            characterNameTextLeft.text = "";
            characterNameTextRight.text = "";

            characterImageLeft.enabled = false;
            characterImageRight.enabled = false;

            nextButtonImageObjLeft.SetActive(false);
            nextButtonImageObjRight.SetActive(false);
            nextButtonImageObjCenter.SetActive(false);
            
            dialogNum = dialogDatas.Length;
            count = 0;

            toDisplayNextCharacterDurationNormal = 1 / normalTextSpeed / 8;
            toDisplayNextCharacterDurationFast = 1 / fastTextSpeed /200;
        }

        public void SetTalkKeyObserver(BaseTalkKeyObserver talkKeyObserver)
        {
            _talkKeyObserver = talkKeyObserver;
        }

        public void StartDialogs()
        {
            count = 0;
            canActiveNextButtonImage = true;
            //MEMO: 初回はクリックを待たずに自動呼び出し
            //MEMO: 設定してあるイベントがあれば実行。なければif文内の処理
            if (!TryInvokeNextAction())
            {
                TryDisplayDialog();
            }
            
            //MEMO: ↓は基本Updateメソッドと同じ挙動。ただ、このメソッドを呼び出したタイミングでUpdateを始めることができる。
            this.UpdateAsObservable()
                .Subscribe(_ =>
                    {
                        UpdateDialog();
                    }
                ).AddTo(this);
            
            //MEMO: Input系の入力を受け付け始める
            SetInputObserver();
        }

        public void PlayFirstDialog()
        {
            count = 0;
            canActiveNextButtonImage = false;
            if (!TryInvokeNextAction())
            {
                TryDisplayDialog();
            }
        }
        
        public void ExitDialog()
        {
            StopCoroutine(_dialogCoroutine);
            InitNextDialogSettings();
            HideDialog();
        }

        public void SetFinishAction()
        {
            TryDisplayDialog();
        }

        private void SetInputObserver()
        {
            if (_talkKeyObserver==null)
            {
                Debug.LogWarning($"Off InputKey");
            }
            else
            {
                _talkKeyObserver.OnNext.Subscribe(_ =>
                {
                    isInputNextKey = true;
                });
            }

            //MEMO: ↓はデバッグにマウスクリックを使えるようにするため。最終的には削除する
            /*this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ =>
                {
                    isInputNextKey = true;
                }).AddTo(this);*/
        }

        // Update is called once per frame
        void UpdateDialog()
        {
            //MEMO: コントローラークリックによる入力通知を受け取ったら
            if (isInputNextKey)
            {
                //MEMO:テキスト表示中、またはイベント実行中にクリックを押したら高速表示
                if (isStartedDialog)
                {
                    currentToDisplayNextCharacterDuration = toDisplayNextCharacterDurationFast;
                }
                
                //MEMO: 設定してあるイベントがあれば実行。なければif文内の処理
                else if (!TryInvokeNextAction())
                {
                    TryDisplayDialog();
                }
            }
            isInputNextKey = false;
        }

        private void TryDisplayDialog()
        {
            if(count < dialogNum){
                HideDialog();
                if (dialogDatas[count].dialog.Length==0){//MEMO: イベント処理のみの実行を簡易に実装するため
                    return;
                }
                isStartedDialog = true;
                switch (dialogDatas[count].characterFace.Name)
                {
                    case CharacterName.Candy:
                        NextDialogCandy(dialogDatas[count]);
                        break;
                    case CharacterName.Fu:
                        NextDialogFu(dialogDatas[count]);
                        break;
                    case CharacterName.Mash:
                        NextDialogMash(dialogDatas[count]);
                        break;
                    case CharacterName.Kure:
                        NextDialogKure(dialogDatas[count]);
                        break;
                    case CharacterName.Queen:
                        NextDialogQueen(dialogDatas[count]);
                        break;
                    case CharacterName.Narration:
                        NextDialogNarration(dialogDatas[count]);
                        break;
                    case CharacterName.Mob:
                        NextDialogMob(dialogDatas[count]);
                        break;
                    case CharacterName.Colete:
                        NextDialogCorete(dialogDatas[count]);
                        break;
                    default:
                        Debug.Log("default");
                        break;
                }
            }
        }

        /// <summary>
        /// ダイアログ再生前に実行するメソッドがあれば実行する
        /// </summary>
        private bool TryInvokeNextAction()
        {
            if (count>=dialogNum)
            {
                return false;
            }
            if (dialogDatas[count].prevEvent!=null&&dialogDatas[count].prevEvent.GetPersistentEventCount()>0)//MEMO:インスペクター上でイベントが登録してあったら
            {
                HideDialog();
                isStartedDialog = true;
                dialogDatas[count].prevEvent.Invoke();//MEMO: イベントを実行
                return true;
            }
            
            return false;
        }
        
        void NextDialogCandy(DialogData _dialogData){
            characterImageLeft.sprite = GetEachCharacterSprite(_dialogData);
            characterImageLeft.enabled = true;
            nameTextBox[0].SetActive(true);
            characterNameTextLeft.text = "キャンディ";
            speechBalloons[0].SetActive(true);
            //dialogTextsLeft.text = _dialogData.dialog;
            _dialogCoroutine=StartCoroutine(DisplayDialogLeft(_dialogData));
        }

        void NextDialogFu(DialogData _dialogData){
            characterImageLeft.sprite = GetEachCharacterSprite(_dialogData);
            characterImageLeft.enabled = true;
            nameTextBox[1].SetActive(true);
            characterNameTextLeft.text = "フー";
            speechBalloons[1].SetActive(true);
            //dialogTextsLeft.text = _dialogData.dialog;
            _dialogCoroutine=StartCoroutine(DisplayDialogLeft(_dialogData));
        }

        void NextDialogMash(DialogData _dialogData){
            characterImageLeft.sprite = GetEachCharacterSprite(_dialogData);
            characterImageLeft.enabled = true;
            nameTextBox[2].SetActive(true);
            characterNameTextLeft.text = "マシュ";
            speechBalloons[2].SetActive(true);
            //dialogTextsLeft.text = _dialogData.dialog;
            _dialogCoroutine=StartCoroutine(DisplayDialogLeft(_dialogData));
        }

        void NextDialogKure(DialogData _dialogData){
            characterImageLeft.sprite = GetEachCharacterSprite(_dialogData);
            characterImageLeft.enabled = true;
            nameTextBox[3].SetActive(true);
            characterNameTextLeft.text = "クレー";
            speechBalloons[3].SetActive(true);
            //dialogTextsLeft.text = _dialogData.dialog;
            _dialogCoroutine=StartCoroutine(DisplayDialogLeft(_dialogData));
        }

        void NextDialogQueen(DialogData _dialogData){
            characterImageRight.sprite = GetEachCharacterSprite(_dialogData);
            characterImageRight.enabled = true;
            nameTextBox[4].SetActive(true);
            characterNameTextRight.text = "女王様";
            speechBalloons[4].SetActive(true);
            //dialogTextsRight.text = _dialogData.dialog;
            _dialogCoroutine=StartCoroutine(DisplayDialogRight(_dialogData));
        }

        void NextDialogNarration(DialogData _dialogData){
            speechBalloons[5].SetActive(true);
            //dialogTextsCenter.text = _dialogData.dialog;
            _dialogCoroutine=StartCoroutine(DisplayDialogCenter(_dialogData));
        }

        void NextDialogMob(DialogData _dialogData){
            characterImageLeft.sprite = GetEachCharacterSprite(_dialogData);
            characterImageLeft.enabled = true;
            nameTextBox[5].SetActive(true);
            characterNameTextLeft.text = "とある住民";
            speechBalloons[6].SetActive(true);
            //dialogTextsCenter.text = _dialogData.dialog;
            _dialogCoroutine=StartCoroutine(DisplayDialogLeft(_dialogData));
        }

        void NextDialogCorete(DialogData _dialogData){
            characterImageRight.sprite = GetEachCharacterSprite(_dialogData);
            characterImageRight.enabled = true;
            nameTextBox[6].SetActive(true);
            characterNameTextRight.text = "コレート";
            speechBalloons[7].SetActive(true);
            //dialogTextsCenter.text = _dialogData.dialog;
            _dialogCoroutine=StartCoroutine(DisplayDialogRight(_dialogData));
        }

        void HideDialog(){
            for (int i = 0; i < speechBalloons.Length; i++){
                speechBalloons[i].SetActive(false);
            }

            for (int i = 0; i < nameTextBox.Length; i++)
            {
                nameTextBox[i].SetActive(false);
            }

            dialogTextsCenter.text = "";
            dialogTextsLeft.text = "";
            dialogTextsRight.text = "";

            characterNameTextLeft.text = "";
            characterNameTextRight.text = "";

            characterImageLeft.enabled = false;
            characterImageRight.enabled = false;
            
            nextButtonImageObjLeft.SetActive(false);
            nextButtonImageObjRight.SetActive(false);
            nextButtonImageObjCenter.SetActive(false);
        }

        IEnumerator DisplayDialogLeft(DialogData _dialogData)
        {
            currentToDisplayNextCharacterDuration = toDisplayNextCharacterDurationNormal;
            char[] message = _dialogData.dialog.ToCharArray();
            for (int i = 0; i < message.Length; i++)
            {
                if (message[i]=='<')
                {
                    SpriteAssetData data = GetSpriteAssetData(message,i);
                    dialogTextsLeft.text += data.SpriteAssetString;
                    i += data.StringIndexCount;
                }
                else
                {
                    dialogTextsLeft.text += message[i];
                }
                yield return new WaitForSeconds(currentToDisplayNextCharacterDuration);
            }

            if (canActiveNextButtonImage)
            {
                nextButtonImageObjLeft.SetActive(true);
            }
            InitNextDialogSettings();
        }

        IEnumerator DisplayDialogRight(DialogData _dialogData)
        {
            currentToDisplayNextCharacterDuration = toDisplayNextCharacterDurationNormal;
            char[] message = _dialogData.dialog.ToCharArray();
            for (int i = 0; i < message.Length; i++)
            {
                if (message[i]=='<')
                {
                    SpriteAssetData data = GetSpriteAssetData(message,i);
                    dialogTextsRight.text += data.SpriteAssetString;
                    i += data.StringIndexCount;
                }
                else
                {
                    dialogTextsRight.text += message[i];
                }

                yield return new WaitForSeconds(currentToDisplayNextCharacterDuration);
            }
            if (canActiveNextButtonImage)
            {
                nextButtonImageObjRight.SetActive(true);
                
            }
            InitNextDialogSettings();
        }

        IEnumerator DisplayDialogCenter(DialogData _dialogData)
        {
            currentToDisplayNextCharacterDuration = toDisplayNextCharacterDurationNormal;
            char[] message = _dialogData.dialog.ToCharArray();
            for (int i = 0; i < message.Length; i++)
            {
                if (message[i]=='<')
                {
                    SpriteAssetData data = GetSpriteAssetData(message,i);
                    dialogTextsCenter.text += data.SpriteAssetString;
                    i += data.StringIndexCount;
                }
                else
                {
                    dialogTextsCenter.text += message[i];
                }
                yield return new WaitForSeconds(currentToDisplayNextCharacterDuration);
            }
            if (canActiveNextButtonImage)
            {
                nextButtonImageObjCenter.SetActive(true);
            }
            InitNextDialogSettings();
        }

        private SpriteAssetData GetSpriteAssetData(char[] message,int headNum)
        {
            string spriteAssetString="";
            for (int i = headNum; i < message.Length; i++)
            {
                spriteAssetString += message[i];
                if (message[i]=='>')
                {
                    return new SpriteAssetData(spriteAssetString,i-headNum);
                }
            }
            Debug.LogError($"Not Found SpriteStringEndMark (>).");
            return new SpriteAssetData("",1);
        }

        private void InitNextDialogSettings()
        {
            isStartedDialog = false;
            count++;
        }

        private Sprite GetEachCharacterSprite(DialogData data)
        {
            switch (data.characterFace.Name)
            {
                case CharacterName.Candy:
                    return _faceData.GetCandyFace(data.characterFace.CandyFace);
                case CharacterName.Fu:
                    return _faceData.GetFuFace(data.characterFace.FuFace);
                case CharacterName.Mash:
                    return _faceData.GetMashFace(data.characterFace.MashFace);
                case CharacterName.Kure:
                    return _faceData.GetKureFace(data.characterFace.KureFace);
                case CharacterName.Queen:
                    return _faceData.GetQueenFace(data.characterFace.QueenFace);
                case CharacterName.Narration:
                    return null;
                case CharacterName.Mob:
                    return _faceData.GetMobFace(data.characterFace.MobFace);
                case CharacterName.Colete:
                    return _faceData.GetCoreteFace(data.characterFace.ColateFace);
                default:
                    Debug.LogError($"Not Found CharacterName. type:{data.characterFace.Name}");
                    return null;
            }
        }
    }

}