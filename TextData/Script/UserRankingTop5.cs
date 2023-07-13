using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RankingData")]
public class UserRankingTop5 : ScriptableObject
{ 
    [System.Serializable]public class UserData
    {
        [Header("���O")] public string userName;
        [Header("�X�R�A")] public int userScore;
    }

    public List<UserData> userDatas = new List<UserData>();
}
