using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterBase;

public class ScoreManager : MonoBehaviour
{
    public int Score { get; set; }

    GameManager gameManager;
    Dictionary<MonsterClass, int> defeatedMonsCount = new Dictionary<MonsterClass, int>();
    Dictionary<MonsterClass, int> monsterClassScore = new Dictionary<MonsterClass, int>()
    {
        {MonsterClass._leser_,5 },{MonsterClass._soldier_,15 },{MonsterClass._elite_,30 },
        {MonsterClass._general_,50 },{MonsterClass._lord_,100 }
    };
    public void StartProcess()
    {
        gameManager = GameManager.gameManager;
        for(int number = 0; number < System.Enum.GetNames(typeof(MonsterClass)).Length; ++number)
        {
            defeatedMonsCount.Add((MonsterClass)number, 0);
        }
    }

    public void ScoreRegister(MonsterClass monsterClass, Vector2 pos)
    {
        defeatedMonsCount[monsterClass] += 1;
        int value = monsterClassScore[monsterClass];
        float multiple = ReturnComboValue(pos);
        Score += (int)(value * multiple);
        gameManager.canvasManager.ScoreCounter(Score);
    }
    public void ScoreRegister(int wave)
    {
        Score += (int)(10 * (1 + wave * 0.1f));
        gameManager.canvasManager.ScoreCounter(Score);
    }
    public void BonusScoreRegister(int score)
    {
        Score += score;
    }
    #region//最終スコア
    public CanvasManager.GameScoreData FinalScoreRegister(int finalWave)
    {
        CanvasManager.GameScoreData gameScoreData = new CanvasManager.GameScoreData();
        //モンスター
        int defeatedMonsAllScore = default;
        for (int _class = 0; _class < monsterClassScore.Count; ++_class)
        {
            int defeatedMonsScore = defeatedMonsCount[(MonsterClass)_class] * monsterClassScore[(MonsterClass)_class];
            gameScoreData.monsterClassValues.Add(defeatedMonsScore);
            defeatedMonsAllScore += defeatedMonsScore;
            Debug.Log((MonsterClass)_class + ":" + defeatedMonsScore);
        }
        //WAVE
        int waveScore = default;
        for(int wave = 0; wave < finalWave; ++wave)
        {
            waveScore += (int)(10 * (1 + wave * 0.1f));
        }
        gameScoreData.waveValue = waveScore;
        Debug.Log("WaveScore:" + waveScore);
        //ボーナス
        int bonusScore = Score - (defeatedMonsAllScore + waveScore);
        gameScoreData.bonusValue = bonusScore;
        Debug.Log("BonusSore:" + bonusScore);
        Debug.Log("RegisterFinalScore:" + (defeatedMonsAllScore + waveScore + bonusScore));
        
        Debug.Log("FactFinalScore:" + Score);
        gameScoreData.scoreVlue = Score;

        return gameScoreData;
    }
    #endregion
    #region//コンボ
    int combo;
    float ReturnComboValue(Vector2 pos)
    {
        float multiple = 1f;
        if(combo > 0)
        {
            multiple = 1 + (combo * 0.25f);
            gameManager.effectManager.TextEffectPlay("COMBO " + (combo + 1), pos, effectTextType: EffectData.EffectTextType.floatUp, activeOutline: true, multiple: 2.5f);
        }
        ++combo;
        //comboResetInvalid = StartCoroutine(ComboResetInvalid());
        if(comboTimeCorutine != null) StopCoroutine(comboTimeCorutine);
        comboTimeCorutine = StartCoroutine(ComboTimeCorutine(2.0f));
        return multiple;
    }
    //コンボをリセット
    public void ResetCombo()
    {
        //if (comboResetInvalidBool){ /*Debug.Log("無効");*/ return; }
        combo = 0;
    }
    //リセットを一定時間無効
    /*
    Coroutine comboResetInvalid;
    bool comboResetInvalidBool = false;
    IEnumerator ComboResetInvalid()
    {
        comboResetInvalidBool = true;
        yield return new WaitForSeconds(0.1f);
        comboResetInvalidBool = false;
    }
    */
    Coroutine comboTimeCorutine;
    IEnumerator ComboTimeCorutine(float time)
    {
        yield return new WaitForSeconds(time);
        ResetCombo();
    }
    #endregion
}
