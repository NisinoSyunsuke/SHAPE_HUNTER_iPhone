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
        //�S������wave�̐�
        public int waveQuantity;
        //�o�������X�^�[
        //  �������X�^�[�ɊK�������ăE�F�C�g��ς���
        //�����X�^�[�̏o�����@���~���㏸��
        //�@���P�x���ׂĂ̊m���������č��v�l�Ŋ���B�p�[�Z���e�[�W�͔䗦������
        public List<MonsterAppearanceData> monsterAppearanceDataList;
    }
    public List<WaveDataInfo> waveDataInfoList;
    */
    
}

