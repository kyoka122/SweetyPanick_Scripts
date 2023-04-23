using System;
using MyApplication;
using UnityEditor;
using UnityEngine;
//MEMO: 参考：https://gomafrontier.com/unity/4553

/// <summary>
/// イベントデータの各要素のinspector表示をカスタマイズするためのDrawerクラス
/// </summary>
[CustomPropertyDrawer(typeof(DialogFaceSpriteData))]
public class DialogFaceSpriteDataDrawer : PropertyDrawer
{
    private const string SpriteLabelName = "顔画像";
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            position.height = EditorGUIUtility.singleLineHeight;

            var characterNameRect = new Rect(position)
            {
                y = position.y
            };

            var nameProperty = property.FindPropertyRelative("name");
            nameProperty.enumValueIndex = EditorGUI.Popup(characterNameRect, "キャラ名",
                nameProperty.enumValueIndex, Enum.GetNames(typeof(CharacterName)));

            var faceRect = new Rect(characterNameRect)
            {
                y = characterNameRect.y + EditorGUIUtility.singleLineHeight + 2f
            };
            SerializedProperty faceProperty;
            switch ((CharacterName) nameProperty.enumValueIndex)
            {
                case CharacterName.Candy:
                    faceProperty = property.FindPropertyRelative("candyFace");
                    faceProperty.enumValueIndex = EditorGUI.Popup(faceRect, SpriteLabelName,
                        faceProperty.enumValueIndex, Enum.GetNames(typeof(CandyFaceSpriteType)));
                    break;
                case CharacterName.Fu:
                    faceProperty = property.FindPropertyRelative("fuFace");
                    faceProperty.enumValueIndex = EditorGUI.Popup(faceRect, SpriteLabelName,
                        faceProperty.enumValueIndex, Enum.GetNames(typeof(FuFaceSpriteType)));
                    break;
                case CharacterName.Mash:
                    faceProperty = property.FindPropertyRelative("mashFace");
                    faceProperty.enumValueIndex = EditorGUI.Popup(faceRect, SpriteLabelName,
                        faceProperty.enumValueIndex, Enum.GetNames(typeof(MashFaceSpriteType)));
                    break;
                case CharacterName.Kure:
                    faceProperty = property.FindPropertyRelative("kureFace");
                    faceProperty.enumValueIndex = EditorGUI.Popup(faceRect, SpriteLabelName,
                        faceProperty.enumValueIndex, Enum.GetNames(typeof(KureFaceSpriteType)));
                    break;
                case CharacterName.Queen:
                    faceProperty = property.FindPropertyRelative("queenFace");
                    faceProperty.enumValueIndex = EditorGUI.Popup(faceRect, SpriteLabelName,
                        faceProperty.enumValueIndex, Enum.GetNames(typeof(QueenFaceSpriteType)));
                    break;
                case CharacterName.Mob:
                    faceProperty = property.FindPropertyRelative("mobFace");
                    faceProperty.enumValueIndex = EditorGUI.Popup(faceRect, SpriteLabelName,
                        faceProperty.enumValueIndex, Enum.GetNames(typeof(MobFaceSpriteType)));
                    break;
                case CharacterName.Colete:
                    faceProperty = property.FindPropertyRelative("colateFace");
                    faceProperty.enumValueIndex = EditorGUI.Popup(faceRect, SpriteLabelName,
                        faceProperty.enumValueIndex, Enum.GetNames(typeof(ColateFaceSpriteType)));
                    break;
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var singleLineHeight = EditorGUIUtility.singleLineHeight;
        //var nameProperty = property.FindPropertyRelative("name");//MEMO: 今回は各項目行数が同じのため、switch文で場合分けする必要がない
        var height = singleLineHeight * 2; //行数
        height += singleLineHeight/2; //余白
        return height;
    }
}