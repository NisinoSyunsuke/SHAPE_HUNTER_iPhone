using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterBase;
using WaveDataBase;

//[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WaveData")]
public class WaveData : ScriptableObject
{
    //
    public int waveQuantity;
    public WaveRarityRate waveRarityRate;
    public WaveType waveType;
    public int bossCnt;
    public List<MonsterSpawnData> monsterAppearanceDataList;
    //
    /*
    [System.Serializable] 
    public class WaveDataInfo
    {
        //担当するwaveの数
        public int waveQuantity;
        //出現モンスター
        //  →モンスターに階級をつけてウェイトを変える
        //モンスターの出現率　下降か上昇か
        //　→１度すべての確率をたして合計値で割る。パーセンテージは比率をする
        public List<MonsterAppearanceData> monsterAppearanceDataList;
    }
    public List<WaveDataInfo> waveDataInfoList;
    */
    
}

