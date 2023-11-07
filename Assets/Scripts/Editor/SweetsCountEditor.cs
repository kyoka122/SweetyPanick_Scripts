using MyApplication;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(SceneSweetsCounter))] //拡張するクラスを指定
public class SweetsCountEditor : Editor
{

    /// <summary>
    /// InspectorのGUIを更新
    /// </summary>
    public override void OnInspectorGUI()
    {
        //元のInspector部分を表示
        base.OnInspectorGUI();

        //ボタンを表示
        if (GUILayout.Button("CountAndRegisterMaxSweetsScore"))
        {
            SceneSweetsCounter sceneSweetsCounter = target as SceneSweetsCounter;
            if (sceneSweetsCounter == null)
            {
                Debug.LogError($"SceneSweetsCounter Is Not Found");
                return;
            }

            sceneSweetsCounter.CountAndRegisterMaxSweetsScore();
        }
    }
}
#endif