using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterBase;
using WaveDataBase;

public class WaveManager : MonoBehaviour
{
    GameManager gameManager;

    int monsterCount; //生きているモンスターの数をカウント
    int waveCount; //ウェーブ数をカウント 
    int wavePoint; //ウェーブ進むごとに加算
    bool monsterUpperLimitBool; //モンスターの存在上限
    const int monsterUpperLimit = 50;

    List<WaveNumberData> waveNumberDataList = new List<WaveNumberData>();

    [SerializeField] List<WaveData> waveDatas;
    [System.NonSerialized] public List<Transform> monsterTransfromList = new List<Transform>();

    //モンスタークラスによるポイント
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
        if (monsTrans != default) monsterTransfromList.Remove(monsTrans); //モンスターの位置情報を消去
        if (monsterCount < monsterUpperLimit) monsterUpperLimitBool = false;

        if (monsterCount <= 0)
        {
            if (isMonsSummon) return; //モンスター召喚中ならリターン
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
        //音楽STOP
        gameManager.soundManager.BgmPlay(0.2f, 0.5f);
    }

    public void WaveStart()
    {
        waveCount += 1; ++wavePoint;
        gameManager.canvasManager.WaveCounter(waveCount);
        StartCoroutine(SummonMonster());

        //スキル_野宿セット
        gameManager.skillManager.CampSet();

        //無敵時間
        gameManager.hpManager.InvincibleTime(2f
            //スキル_プロテクション
            + gameManager.skillManager.Protection()
            );
    }

    #region//WaveData登録
    void WaveRegister()
    {
        foreach (var data in waveDatas)
        {
            for (int cnt = 0; cnt < data.waveQuantity; ++cnt)
            {
                float sum = default;
                List<MonsterSpawnData> list = new List<MonsterSpawnData>();　// MonsterSpawnDataは、モンスター名、スポーン確率、増減率を入れるクラス

                foreach (MonsterSpawnData monsData in data.monsterAppearanceDataList) {
                    MonsterSpawnData instantData = new MonsterSpawnData();
                    instantData.monsterName = monsData.monsterName; instantData.spawnRate = monsData.spawnRate;
                    float fluctuation = (100 + (monsData.rateFluctuation * cnt)) * 0.01f ; //％変換
                    instantData.spawnRate *= fluctuation;
                    sum += instantData.spawnRate; //sumはWAVEに登録されたモンスターの数の合計
                    list.Add(instantData); //fluctuationを登録したMonsterSpawnDataを登録
                }

                List<MonsterSpawnData> classList = new List<MonsterSpawnData>();
                float odds = default; float deltaRate = 0;

                foreach (var monsData in list) //listはそのWAVEに登録したモンスターの数だけの個数が入っている
                {
                    odds = (monsData.spawnRate + deltaRate) / sum; //確率を上乗せしていく、使うときに○○以上ならといった条件をつける前提
                    deltaRate += monsData.spawnRate; 

                    MonsterSpawnData instant = new MonsterSpawnData();
                    instant.monsterName = monsData.monsterName; instant.spawnRate = odds;
                    classList.Add(instant);
                }
                classList.Reverse();

                //ウェーブ登録
                WaveNumberData waveNumberData = new WaveNumberData();
                waveNumberData.monsterSpawnDataList = classList;
                waveNumberData.waveRarityRate = data.waveRarityRate;
                waveNumberData.waveType = data.waveType;
                waveNumberData.bossCnt = data.bossCnt;
                waveNumberDataList.Add(waveNumberData);
            }
        }

        //デバッグ用
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
    #region//モンスター召喚
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
        //最終WAVEが終わり自動更新になった場合
        WaveType currentWaveType = waveNumberDataList[waveCount].waveType;
        if (finalWaveFinish && waveCount % 7 == 0) { 
            currentWaveType = WaveType.bossWave;
            waveNumberDataList[waveCount].bossCnt = Random.Range(1, 5);
        }
        //WAVEのレアリティ選択
        gameManager.skillManager.waveRarityRate = waveNumberDataList[waveCount].waveRarityRate;
        //音楽PLAY
        switch (currentWaveType){
            case WaveType.normalWave: gameManager.soundManager.BgmPlay("Normal_BGM2", 0.5f); break;
            case WaveType.bossWave: gameManager.soundManager.BgmPlay("Boss_BGM3", 0.5f); break;
        }

        isMonsSummon = true;
        int point = wavePoint;
        while (point >= 0)
        {
            //モンスター上限
            yield return new WaitUntil(() => !monsterUpperLimitBool); //←trueになったら進む
            //モンスター選択
            float random = Random.value; MonsterName summonName = default;
            foreach (var data in waveNumberDataList[waveCount].monsterSpawnDataList)
            {
                if (data.spawnRate >= random) summonName = data.monsterName;
            }
            //召喚処理
            Vector2 pos = RandomPosRetrun();
            gameManager.effectManager.EffectPlay("magicCircle", pos, scale: 0.5f);
            gameManager.soundManager.Play("summon_1", 2f, 0.1f);
            var mons = Instantiate(gameManager.monsterManager.monsterDataDic[summonName].monsterObj, pos, Quaternion.identity);
            monsterTransfromList.Add(mons.transform); //モンスターの位置を登録
            //WAVEの種類によって分岐処理
            switch (currentWaveType)
            {
                case WaveType.normalWave:
                    //モンスターの階級によってマイナスされるポイントを上げる
                    point -= monsterClassPoint[gameManager.monsterManager.monsterDataDic[summonName].monsterClass]; 
                    /*Debug.Log("モンスターpoint："+ monsterClassPoint[gameManager.monsterManager.monsterDataDic[summonName].monsterClass]
                        + " モンスタークラス：" + summonName + " モンスタークラス："+ gameManager.monsterManager.monsterDataDic[summonName].monsterClass); */
                    break;
                case WaveType.bossWave: 
                    //ボスの場合は×3コスト
                    point -= monsterClassPoint[gameManager.monsterManager.monsterDataDic[summonName].monsterClass] * 3;

                    waveNumberDataList[waveCount].bossCnt -= 1;
                    if (waveNumberDataList[waveCount].bossCnt <= 0){ //ボスの他のモンスターは前WAVEに依存する
                        waveNumberDataList[waveCount] = waveNumberDataList[waveCount - 1];
                        currentWaveType = WaveType.normalWave;
                    } 

                    if(gameManager.monsterManager.monsterDataDic[summonName].uniqueBoss) mons.GetComponent<MonsterChip>().MonsterType = MonsterType.uniqueBoss;
                    else mons.GetComponent<MonsterChip>().MonsterType = MonsterType.boss;

                    break;
            }

            //モンスターカウント
            ++monsterCount;
            if (monsterCount >= monsterUpperLimit) monsterUpperLimitBool = true;

            //ディレイ
            yield return new WaitForSeconds(Random.Range(0, 0.5f));
        }
        isMonsSummon = false;
        MonsterCountFluctuation(0);
    }
    #endregion
    #region//外部からモンスター召喚
    public void SummonMonsterxEternal(MonsterName summonName)
    {
        Vector2 pos = RandomPosRetrun();
        gameManager.effectManager.EffectPlay("magicCircle", pos, scale: 0.5f);
        gameManager.soundManager.Play("summon_1", 2f, 0.1f);
        var mons = Instantiate(gameManager.monsterManager.monsterDataDic[summonName].monsterObj, pos, Quaternion.identity);
        monsterTransfromList.Add(mons.transform); //モンスターの位置を登録

        //モンスターカウント
        ++monsterCount;
    }
    #endregion
    #region//WAVE数によるモンスターの強化
    public int RetrunWaveMultiple(int initialValue)
    {
        int value = (int)(initialValue * (1 + 0.05 * waveCount));
        return value;
    }
    public float RetrunWaveMultiple(float initialValue)
    {
        float value = initialValue * (0.5f + 0.1f * waveCount);
        if (value >= initialValue * 3f) value = initialValue * 3;
        //Debug.Log("確率:" + value);
        return value;
    }
    #endregion
    #region//WAVEリターン
    public int WaveCount()
    {
        return waveCount;
    }
    #endregion
    #region//最終WAVEだった場合
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
