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
        reorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "�o�������X�^�[");
        reorderableList.elementHeightCallback = index => EditorGUI.GetPropertyHeight(monsterAppearanceDataList.GetArrayElementAtIndex(index));
        
    }
    
    public override void OnInspectorGUI()
    {
        // �����L���b�V������ŐV�̃f�[�^���擾����
        serializedObject.Update();

        WaveData mytarget = (WaveData)target;

        EditorGUILayout.BeginHorizontal();
        var waveQuantity = serializedObject.FindProperty("waveQuantity");
        waveQuantity.intValue = EditorGUILayout.IntField("�S������WAVE�̐�", waveQuantity.intValue, GUILayout.Width(200));
        EditorGUILayout.LabelField("wave", GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        mytarget.waveRarityRate = (WaveDataBase.WaveRarityRate)EditorGUILayout.EnumPopup("WAVE�I����̕�V��RATE", mytarget.waveRarityRate, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        mytarget.waveType = (WaveDataBase.WaveType)EditorGUILayout.EnumPopup("WAVE�̎��", mytarget.waveType, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (mytarget.waveType == WaveDataBase.WaveType.bossWave)
        {
            if(mytarget.bossCnt == 0)mytarget.bossCnt = 1;
            mytarget.bossCnt = EditorGUILayout.IntField("�{�X��", mytarget.bossCnt, GUILayout.Width(200));
            EditorGUILayout.LabelField("��", GUILayout.Width(50));
        }
        EditorGUILayout.EndHorizontal();


        /*
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("�����X�^�[�o����");
        spawnRateFloat.floatValue = EditorGUILayout.FloatField(spawnRateFloat.floatValue, GUILayout.Width(80));
        EditorGUILayout.LabelField("%", GUILayout.Width(10));
        EditorGUILayout.LabelField("�����X�^�[�o��������");
        EditorGUILayout.FloatField(rateFluctuationFloat.floatValue, GUILayout.Width(80));
        EditorGUILayout.LabelField("%", GUILayout.Width(10));
        EditorGUILayout.EndHorizontal();*/
        /*
        MonsterAppearanceData Data = (MonsterAppearanceData)target;

        EditorGUILayout.BeginHorizontal();
        //GUILayout.Space(105);
        EditorGUILayout.LabelField("�����X�^�[���", GUILayout.Width(75));
        Data.monsterName = (MonsterName)EditorGUILayout.EnumPopup(Data.monsterName, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("�����X�^�[�o����");
        Data.spawnRate = EditorGUILayout.FloatField(Data.spawnRate, GUILayout.Width(80));
        EditorGUILayout.LabelField("%", GUILayout.Width(10));
        EditorGUILayout.LabelField("�����X�^�[�o��������");
        Data.rateFluctuation = EditorGUILayout.FloatField(Data.rateFluctuation, GUILayout.Width(80));
        EditorGUILayout.LabelField("%", GUILayout.Width(10));
        EditorGUILayout.EndHorizontal();
        */
        //ReorderbleList�����

        // �����L���b�V���ɕύX�_��K�p����
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
        // List�p��1�̃v���p�e�B�ł��邱�Ƃ���������PropertyScope�ň͂�
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            // 0�w�肾��ReorderableList�̃h���b�O�Ɣ��̂�LineHeight���w��
            position.height = EditorGUIUtility.singleLineHeight;

            var actionTypeRect = new Rect(position)
            {
                y = position.y
            };

            var monsterNameProperty = property.FindPropertyRelative("monsterName");
            var spawnRateFloat = property.FindPropertyRelative("spawnRate");
            var rateFluctuationFloat = property.FindPropertyRelative("rateFluctuation");

            monsterNameProperty.enumValueIndex = EditorGUI.Popup(actionTypeRect, "�����X�^�[���", monsterNameProperty.enumValueIndex, System.Enum.GetNames(typeof(MonsterName)));

            EditorGUI.LabelField(new Rect(actionTypeRect.x,actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2f, 150,20),"�����X�^�[�o����");
            spawnRateFloat.floatValue = EditorGUI.FloatField(new Rect(actionTypeRect.x + EditorGUIUtility.singleLineHeight + 70f, actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2f, 25, 20),spawnRateFloat.floatValue);
            EditorGUI.LabelField(new Rect(actionTypeRect.x + EditorGUIUtility.singleLineHeight + 95f, actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2f, 20, 20), "%");

            float rectX = 175;
            EditorGUI.LabelField(new Rect(actionTypeRect.x + rectX - 25, actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2f, 150, 20), "�����X�^�[�o��������");
            rateFluctuationFloat.floatValue = EditorGUI.FloatField(new Rect(actionTypeRect.x + EditorGUIUtility.singleLineHeight + 70f + rectX, actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2f, 25, 20), rateFluctuationFloat.floatValue);
            EditorGUI.LabelField(new Rect(actionTypeRect.x + EditorGUIUtility.singleLineHeight + 95f + rectX, actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2f, 20, 20), "%");
            //EditorGUI.LabelField(talkNameRect, "�����X�^�[�o��������");
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
                    talkNameProperty.stringValue = EditorGUI.TextField(talkNameRect, "���O", talkNameProperty.stringValue);

                    var talkTextLabelRect = new Rect(talkNameRect)
                    {
                        y = talkNameRect.y + EditorGUIUtility.singleLineHeight + 2f
                    };
                    EditorGUI.LabelField(talkTextLabelRect, "�e�L�X�g");

                    var talkTextRect = new Rect(talkTextLabelRect)
                    {
                        // TextArea�Ȃ̂�3�s���m��
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
                    animationNameProperty.stringValue = EditorGUI.TextField(animationNameRect, "�A�j���[�V������", animationNameProperty.stringValue);

                    var localMovePointRect = new Rect(animationNameRect)
                    {
                        y = animationNameRect.y + EditorGUIUtility.singleLineHeight + 2f
                    };
                    var localMovePointProperty = property.FindPropertyRelative("localMovePoint");
                    localMovePointProperty.vector3Value = EditorGUI.Vector3Field(localMovePointRect, "�ړ���(����)", localMovePointProperty.vector3Value);

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
