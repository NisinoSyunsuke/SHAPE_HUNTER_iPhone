using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RankingData")]
public class UserRankingTop5 : ScriptableObject
{ 
    [System.Serializable]public class UserData
    {
        [Header("名前")] public string userName;
        [Header("スコア")] public int userScore;
    }

    public List<UserData> userDatas = new List<UserData>();
}
