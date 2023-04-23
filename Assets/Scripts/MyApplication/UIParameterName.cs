using System;

namespace MyApplication
{
    public static class UIAnimatorParameter
    {
        /// <summary>
        /// コントローラー設定画面のコントローラーアニメーション
        /// </summary>
        public const string Change = "Change";
        
        /// <summary>
        /// キャラクター設定画面のKeyConfig遷移アニメーションを行うかどうか
        /// </summary>
        public const string OnChangeAnimation = "OnChangeAnimation";
        
        /// <summary>
        /// キャラクター設定画面のJoycon用KeyConfigアニメーション
        /// </summary>
        public const string UseJoycon = "UseJoycon";
        
        /// <summary>
        /// キャラクター設定画面のJoycon用KeyConfigアニメーション
        /// </summary>
        public const string UseProcon = "UseProcon";
        
        /// <summary>
        /// キャラクター設定画面のJoycon用KeyConfigアニメーション
        /// </summary>
        public const string UseXBox = "UseXBox";
        
        /// <summary>
        /// キャラクター設定画面のJoycon用KeyConfigアニメーション
        /// </summary>
        public const string UseKeyboard = "UseKeyboard";

        public static string GetKeyConfigAnimatorParameter(MyInputDeviceType type)
        {
            return type switch
            {
                MyInputDeviceType.Keyboard => UseKeyboard,
                MyInputDeviceType.Procon => UseProcon,
                MyInputDeviceType.GamePad => UseXBox,
                MyInputDeviceType.JoyconLeft => UseJoycon,
                MyInputDeviceType.JoyconRight => UseJoycon,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}