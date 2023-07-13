using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterBase;
using WaveDataBase;

public class WaveManager : MonoBehaviour
{
    GameManager gameManager;

    int monsterCount; //�����Ă��郂���X�^�[�̐����J�E���g
    int waveCount; //�E�F�[�u�����J�E���g 
    int wavePoint; //�E�F�[�u�i�ނ��Ƃɉ��Z
    bool monsterUpperLimitBool; //�����X�^�[�̑��ݏ��
    const int monsterUpperLimit = 50;

    List<WaveNumberData> waveNumberDataList = new List<WaveNumberData>();

    [SerializeField] List<WaveData> waveDatas;
    [System.NonSerialized] public List<Transform> monsterTransfromList = new List<Transform>();

    //�����X�^�[�N���X�ɂ��|�C���g
    Dictionary<MonsterClass, int> monsterClassPoint = new Dictionary<MonsterClass, int>()
    {
        {MonsterClass._leser_,1 },{MonsterClass._soldier_,2 },{MonsterClass._elite_,3 },
        {MonsterClass._general_,10 },{MonsterClass._lord_,25 }
    };
    public void StartProcess()
    {
        gameManager = GameManager.gameManager;
        WaveRegister();
        RangeRegister();
    }

    public void MonsterCountFluctuation(int value = -1, Transform monsTrans = default)
    {
        monsterCount += value;
        if (monsTrans != default) monsterTransfromList.Remove(monsTrans); //�����X�^�[�̈ʒu��������
        if (monsterCount < monsterUpperLimit) monsterUpperLimitBool = false;

        if (monsterCount <= 0)
        {
            if (isMonsSummon) return; //�����X�^�[�������Ȃ烊�^�[��
            monsterCount = 0;
            StartCoroutine(WaveEnd());
        }
    }

    IEnumerator WaveEnd()
    {
        if (gameManager.HorsDeCombat) yield break;
        gameManager.scoreManager.ScoreRegister(waveCount);
        gameManager.effectManager.M_AttackEffectAllClear();
        yield return new WaitForSeconds(1f);
        gameManager.canvasManager.GameStateChange(CanvasManager.GameState.reward);
        //���ySTOP
        gameManager.soundManager.BgmPlay(0.2f, 0.5f);
    }

    public void WaveStart()
    {
        waveCount += 1; ++wavePoint;
        gameManager.canvasManager.WaveCounter(waveCount);
        StartCoroutine(SummonMonster());

        //�X�L��_��h�Z�b�g
        gameManager.skillManager.CampSet();

        //���G����
        gameManager.hpManager.InvincibleTime(2f
            //�X�L��_�v���e�N�V����
            + gameManager.skillManager.Protection()
            );
    }

    #region//WaveData�o�^
    void WaveRegister()
    {
        foreach (var data in waveDatas)
        {
            for (int cnt = 0; cnt < data.waveQuantity; ++cnt)
            {
                float sum = default;
                List<MonsterSpawnData> list = new List<MonsterSpawnData>();�@// MonsterSpawnData�́A�����X�^�[���A�X�|�[���m���A������������N���X

                foreach (MonsterSpawnData monsData in data.monsterAppearanceDataList) {
                    MonsterSpawnData instantData = new MonsterSpawnData();
                    instantData.monsterName = monsData.monsterName; instantData.spawnRate = monsData.spawnRate;
                    float fluctuation = (100 + (monsData.rateFluctuation * cnt)) * 0.01f ; //���ϊ�
                    instantData.spawnRate *= fluctuation;
                    sum += instantData.spawnRate; //sum��WAVE�ɓo�^���ꂽ�����X�^�[�̐��̍��v
                    list.Add(instantData); //fluctuation��o�^����MonsterSpawnData��o�^
                }

                List<MonsterSpawnData> classList = new List<MonsterSpawnData>();
                float odds = default; float deltaRate = 0;

                foreach (var monsData in list) //list�͂���WAVE�ɓo�^���������X�^�[�̐������̌��������Ă���
                {
                    odds = (monsData.spawnRate + deltaRate) / sum; //�m������悹���Ă����A�g���Ƃ��Ɂ����ȏ�Ȃ�Ƃ���������������O��
                    deltaRate += monsData.spawnRate; 

                    MonsterSpawnData instant = new MonsterSpawnData();
                    instant.monsterName = monsData.monsterName; instant.spawnRate = odds;
                    classList.Add(instant);
                }
                classList.Reverse();

                //�E�F�[�u�o�^
                WaveNumberData waveNumberData = new WaveNumberData();
                waveNumberData.monsterSpawnDataList = classList;
                waveNumberData.waveRarityRate = data.waveRarityRate;
                waveNumberData.waveType = data.waveType;
                waveNumberData.bossCnt = data.bossCnt;
                waveNumberDataList.Add(waveNumberData);
            }
        }

        //�f�o�b�O�p
        /*int wave = 0;
        foreach(var item in waveNumberDataList)
        {
            ++wave;
            foreach(var dic in item.monsterSpawnDataList)
            {
                Debug.Log(wave + "wave " + dic.spawnRate + ":" + dic.monsterName);
            }
        }
        */
    }
    #endregion
    #region//�����X�^�[����
    [System.Serializable] class SummonData
    {
        public Transform xy_point; public Transform x_point; public Transform y_point;
        public (float, float) xRange; public (float, float) yRange;
    }
    [SerializeField] SummonData summonData;
    bool isMonsSummon;

    void RangeRegister()
    {

        summonData.xRange = (summonData.xy_point.position.x, summonData.x_point.position.x);
        summonData.yRange = (summonData.y_point.position.y, summonData.xy_point.position.y);
    }

    public Vector2 RandomPosRetrun()
    {
        Vector2 pos = new Vector2(Random.Range(summonData.xRange.Item1, summonData.xRange.Item2), Random.Range(summonData.yRange.Item1, summonData.yRange.Item2));
        return pos;
    }

    IEnumerator SummonMonster()
    {
        yield return new WaitForSeconds(0.75f);

        FinalWaveCheck();
        //�ŏIWAVE���I��莩���X�V�ɂȂ����ꍇ
        WaveType currentWaveType = waveNumberDataList[waveCount].waveType;
        if (finalWaveFinish && waveCount % 7 == 0) { 
            currentWaveType = WaveType.bossWave;
            waveNumberDataList[waveCount].bossCnt = Random.Range(1, 5);
        }
        //WAVE�̃��A���e�B�I��
        gameManager.skillManager.waveRarityRate = waveNumberDataList[waveCount].waveRarityRate;
        //���yPLAY
        switch (currentWaveType){
            case WaveType.normalWave: gameManager.soundManager.BgmPlay("Normal_BGM2", 0.5f); break;
            case WaveType.bossWave: gameManager.soundManager.BgmPlay("Boss_BGM3", 0.5f); break;
        }

        isMonsSummon = true;
        int point = wavePoint;
        while (point >= 0)
        {
            //�����X�^�[���
            yield return new WaitUntil(() => !monsterUpperLimitBool); //��true�ɂȂ�����i��
            //�����X�^�[�I��
            float random = Random.value; MonsterName summonName = default;
            foreach (var data in waveNumberDataList[waveCount].monsterSpawnDataList)
            {
                if (data.spawnRate >= random) summonName = data.monsterName;
            }
            //��������
            Vector2 pos = RandomPosRetrun();
            gameManager.effectManager.EffectPlay("magicCircle", pos, scale: 0.5f);
            gameManager.soundManager.Play("summon_1", 2f, 0.1f);
            var mons = Instantiate(gameManager.monsterManager.monsterDataDic[summonName].monsterObj, pos, Quaternion.identity);
            monsterTransfromList.Add(mons.transform); //�����X�^�[�̈ʒu��o�^
            //WAVE�̎�ނɂ���ĕ��򏈗�
            switch (currentWaveType)
            {
                case WaveType.normalWave:
                    //�����X�^�[�̊K���ɂ���ă}�C�i�X�����|�C���g���グ��
                    point -= monsterClassPoint[gameManager.monsterManager.monsterDataDic[summonName].monsterClass]; 
                    /*Debug.Log("�����X�^�[point�F"+ monsterClassPoint[gameManager.monsterManager.monsterDataDic[summonName].monsterClass]
                        + " �����X�^�[�N���X�F" + summonName + " �����X�^�[�N���X�F"+ gameManager.monsterManager.monsterDataDic[summonName].monsterClass); */
                    break;
                case WaveType.bossWave: 
                    //�{�X�̏ꍇ�́~3�R�X�g
                    point -= monsterClassPoint[gameManager.monsterManager.monsterDataDic[summonName].monsterClass] * 3;

                    waveNumberDataList[waveCount].bossCnt -= 1;
                    if (waveNumberDataList[waveCount].bossCnt <= 0){ //�{�X�̑��̃����X�^�[�͑OWAVE�Ɉˑ�����
                        waveNumberDataList[waveCount] = waveNumberDataList[waveCount - 1];
                        currentWaveType = WaveType.normalWave;
                    } 

                    if(gameManager.monsterManager.monsterDataDic[summonName].uniqueBoss) mons.GetComponent<MonsterChip>().MonsterType = MonsterType.uniqueBoss;
                    else mons.GetComponent<MonsterChip>().MonsterType = MonsterType.boss;

                    break;
            }

            //�����X�^�[�J�E���g
            ++monsterCount;
            if (monsterCount >= monsterUpperLimit) monsterUpperLimitBool = true;

            //�f�B���C
            yield return new WaitForSeconds(Random.Range(0, 0.5f));
        }
        isMonsSummon = false;
        MonsterCountFluctuation(0);
    }
    #endregion
    #region//�O�����烂���X�^�[����
    public void SummonMonsterxEternal(MonsterName summonName)
    {
        Vector2 pos = RandomPosRetrun();
        gameManager.effectManager.EffectPlay("magicCircle", pos, scale: 0.5f);
        gameManager.soundManager.Play("summon_1", 2f, 0.1f);
        var mons = Instantiate(gameManager.monsterManager.monsterDataDic[summonName].monsterObj, pos, Quaternion.identity);
        monsterTransfromList.Add(mons.transform); //�����X�^�[�̈ʒu��o�^

        //�����X�^�[�J�E���g
        ++monsterCount;
    }
    #endregion
    #region//WAVE���ɂ�郂���X�^�[�̋���
    public int RetrunWaveMultiple(int initialValue)
    {
        int value = (int)(initialValue * (1 + 0.05 * waveCount));
        return value;
    }
    public float RetrunWaveMultiple(float initialValue)
    {
        float value = initialValue * (0.5f + 0.1f * waveCount);
        if (value >= initialValue * 3f) value = initialValue * 3;
        //Debug.Log("�m��:" + value);
        return value;
    }
    #endregion
    #region//WAVE���^�[��
    public int WaveCount()
    {
        return waveCount;
    }
    #endregion
    #region//�ŏIWAVE�������ꍇ
    bool finalWaveFinish = false;
    void FinalWaveCheck()
    {
        if (waveCount <= waveNumberDataList.Count - 1) return;
        WaveNumberData waveNumberData = waveNumberDataList[waveCount - 1];
        waveNumberDataList.Add(waveNumberData);
        finalWaveFinish = true;
    }
    #endregion
}
