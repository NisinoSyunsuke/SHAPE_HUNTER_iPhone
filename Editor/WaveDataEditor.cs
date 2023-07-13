using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterBase;
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(WaveData))]
public class WaveDataEditor : Editor
{

    ReorderableList reorderableList;
    SerializedProperty monsterAppearanceDataList;

    void OnEnable()
    {
        
        monsterAppearanceDataList = serializedObject.FindProperty("monsterAppearanceDataList");

        reorderableList = new ReorderableList(serializedObject, monsterAppearanceDataList);
        reorderableList.drawElementCallback = (rect, index, active, focused) => {
            var Data = monsterAppearanceDataList.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, Data);
        };
        reorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "出現モンスター");
        reorderableList.elementHeightCallback = index => EditorGUI.GetPropertyHeight(monsterAppearanceDataList.GetArrayElementAtIndex(index));
        
    }
    
    public override void OnInspectorGUI()
    {
        // 内部キャッシュから最新のデータを取得する
        serializedObject.Update();

        WaveData mytarget = (WaveData)target;

        EditorGUILayout.BeginHorizontal();
        var waveQuantity = serializedObject.FindProperty("waveQuantity");
        waveQuantity.intValue = EditorGUILayout.IntField("担当するWAVEの数", waveQuantity.intValue, GUILayout.Width(200));
        EditorGUILayout.LabelField("wave", GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        mytarget.waveRarityRate = (WaveDataBase.WaveRarityRate)EditorGUILayout.EnumPopup("WAVE終了後の報酬のRATE", mytarget.waveRarityRate, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        mytarget.waveType = (WaveDataBase.WaveType)EditorGUILayout.EnumPopup("WAVEの種類", mytarget.waveType, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (mytarget.waveType == WaveDataBase.WaveType.bossWave)
        {
            if(mytarget.bossCnt == 0)mytarget.bossCnt = 1;
            mytarget.bossCnt = EditorGUILayout.IntField("ボス数", mytarget.bossCnt, GUILayout.Width(200));
            EditorGUILayout.LabelField("体", GUILayout.Width(50));
        }
        EditorGUILayout.EndHorizontal();


        /*
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("モンスター出現率");
        spawnRateFloat.floatValue = EditorGUILayout.FloatField(spawnRateFloat.floatValue, GUILayout.Width(80));
        EditorGUILayout.LabelField("%", GUILayout.Width(10));
        EditorGUILayout.LabelField("モンスター出現増減率");
        EditorGUILayout.FloatField(rateFluctuationFloat.floatValue, GUILayout.Width(80));
        EditorGUILayout.LabelField("%", GUILayout.Width(10));
        EditorGUILayout.EndHorizontal();*/
        /*
        MonsterAppearanceData Data = (MonsterAppearanceData)target;

        EditorGUILayout.BeginHorizontal();
        //GUILayout.Space(105);
        EditorGUILayout.LabelField("モンスター種類", GUILayout.Width(75));
        Data.monsterName = (MonsterName)EditorGUILayout.EnumPopup(Data.monsterName, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("モンスター出現率");
        Data.spawnRate = EditorGUILayout.FloatField(Data.spawnRate, GUILayout.Width(80));
        EditorGUILayout.LabelField("%", GUILayout.Width(10));
        EditorGUILayout.LabelField("モンスター出現増減率");
        Data.rateFluctuation = EditorGUILayout.FloatField(Data.rateFluctuation, GUILayout.Width(80));
        EditorGUILayout.LabelField("%", GUILayout.Width(10));
        EditorGUILayout.EndHorizontal();
        */
        //ReorderbleListを作る

        // 内部キャッシュに変更点を適用する
        //serializedObject.ApplyModifiedProperties();

        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(MonsterSpawnData))]
public class EventActionDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // List用に1つのプロパティであることを示すためPropertyScopeで囲む
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            // 0指定だとReorderableListのドラッグと被るのでLineHeightを指定
            position.height = EditorGUIUtility.singleLineHeight;

            var actionTypeRect = new Rect(position)
            {
                y = position.y
            };

            var monsterNameProperty = property.FindPropertyRelative("monsterName");
            var spawnRateFloat = property.FindPropertyRelative("spawnRate");
            var rateFluctuationFloat = property.FindPropertyRelative("rateFluctuation");

            monsterNameProperty.enumValueIndex = EditorGUI.Popup(actionTypeRect, "モンスター種類", monsterNameProperty.enumValueIndex, System.Enum.GetNames(typeof(MonsterName)));

            EditorGUI.LabelField(new Rect(actionTypeRect.x,actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2f, 150,20),"モンスター出現率");
            spawnRateFloat.floatValue = EditorGUI.FloatField(new Rect(actionTypeRect.x + EditorGUIUtility.singleLineHeight + 70f, actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2f, 25, 20),spawnRateFloat.floatValue);
            EditorGUI.LabelField(new Rect(actionTypeRect.x + EditorGUIUtility.singleLineHeight + 95f, actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2f, 20, 20), "%");

            float rectX = 175;
            EditorGUI.LabelField(new Rect(actionTypeRect.x + rectX - 25, actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2f, 150, 20), "モンスター出現率増減");
            rateFluctuationFloat.floatValue = EditorGUI.FloatField(new Rect(actionTypeRect.x + EditorGUIUtility.singleLineHeight + 70f + rectX, actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2f, 25, 20), rateFluctuationFloat.floatValue);
            EditorGUI.LabelField(new Rect(actionTypeRect.x + EditorGUIUtility.singleLineHeight + 95f + rectX, actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2f, 20, 20), "%");
            //EditorGUI.LabelField(talkNameRect, "モンスター出現増減率");
            //EditorGUI.FloatField(talkNameRect, rateFluctuationFloat.floatValue);
            //EditorGUI.LabelField(talkNameRect, "%");

            /*
            switch ((EventActionType)actionTypeProperty.enumValueIndex)
            {
                case EventActionType.Talk:
                    var talkNameRect = new Rect(actionTypeRect)
                    {
                        y = actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2f
                    };
                    var talkNameProperty = property.FindPropertyRelative("talkName");
                    talkNameProperty.stringValue = EditorGUI.TextField(talkNameRect, "名前", talkNameProperty.stringValue);

                    var talkTextLabelRect = new Rect(talkNameRect)
                    {
                        y = talkNameRect.y + EditorGUIUtility.singleLineHeight + 2f
                    };
                    EditorGUI.LabelField(talkTextLabelRect, "テキスト");

                    var talkTextRect = new Rect(talkTextLabelRect)
                    {
                        // TextAreaなので3行分確保
                        y = talkTextLabelRect.y + EditorGUIUtility.singleLineHeight + 2f,
                        height = (EditorGUIUtility.singleLineHeight * 3)
                    };
                    var talkTextProperty = property.FindPropertyRelative("talkText");
                    talkTextProperty.stringValue = EditorGUI.TextArea(talkTextRect, talkTextProperty.stringValue);

                    break;
                case EventActionType.CharacterAction:
                    var animationNameRect = new Rect(actionTypeRect)
                    {
                        y = actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2f
                    };
                    var animationNameProperty = property.FindPropertyRelative("talkName");
                    animationNameProperty.stringValue = EditorGUI.TextField(animationNameRect, "アニメーション名", animationNameProperty.stringValue);

                    var localMovePointRect = new Rect(animationNameRect)
                    {
                        y = animationNameRect.y + EditorGUIUtility.singleLineHeight + 2f
                    };
                    var localMovePointProperty = property.FindPropertyRelative("localMovePoint");
                    localMovePointProperty.vector3Value = EditorGUI.Vector3Field(localMovePointRect, "移動先(相対)", localMovePointProperty.vector3Value);

                    break;
            }
            */
        }
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUIUtility.singleLineHeight;

        /*var actionTypeProperty = property.FindPropertyRelative("actionType");
        switch ((EventActionType)actionTypeProperty.enumValueIndex)
        {
            case EventActionType.Talk:
                height = 130f;
                break;
            case EventActionType.CharacterAction:
                height = 70f;
                break;
        }*/

        height = 50f;
        return height;
    }
}
