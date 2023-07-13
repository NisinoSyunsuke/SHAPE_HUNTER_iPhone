using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SaveDataSpace;

public class SaveDataManager : MonoBehaviour
{
    public SaveData saveData = new SaveData();

    public void Save(SaveData data = default) //セーブするときに呼び出す
    {
        if(data == default) //デフォルトの場合はsaveDataに保存されているデータをセーブする
        {
            data = saveData;
        }      
       
        string dataStr = JsonUtility.ToJson(data);
        StreamWriter writer;
        writer = new StreamWriter(Application.persistentDataPath + "/savedata.json", false); Debug.Log(dataStr);
                                             
        writer.Write(dataStr);
        writer.Flush();
        writer.Close();
    }

    void LoadData()
    {

        string dataStr = "";
        StreamReader reader;
        reader = new StreamReader(Application.persistentDataPath + "/savedata.json");
        dataStr = reader.ReadToEnd();
        reader.Close();

        saveData = JsonUtility.FromJson<SaveData>(dataStr);

        //

        DataBase.PlayerNameID = saveData.playerNameID;
        DataBase.PlayerHighScoreSeason1 = saveData.PlayerHighScoreSeson1;
        DataBase.PlayerHighScoreSeason2 = saveData.PlayerHighScoreSeson2; //Debug.Log(DataBase.PlayerHighScoreSeason2);
        DataBase.Language = saveData.Language;
        DataBase.MasterVolume = saveData.masterVolume;
        DataBase.firstLaunchBool = saveData.firstLaunchBool;

        GameManager.gameManager.languageManager.UITextDataApply();
    }

    public void ResetData()
    {
        SaveData data = new SaveData();
        saveData = data;
        Save(data);
    }

    public void StartProcess()
    {
        if(System.IO.File.Exists(Application.persistentDataPath + "/savedata.json") == false)
        {
            ResetData(); 
        }

        LoadData();
    }

}
