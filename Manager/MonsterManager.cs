using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterBase;
using DG.Tweening;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] List<MonsterData> monsterDataList = new List<MonsterData>();
    public Dictionary<MonsterName, MonsterData> monsterDataDic = new Dictionary<MonsterName, MonsterData>();
    public Material monsterEdgeShader;
    public Material monsterEdgeWaveShader;
    public void StartProcess()
    {
        foreach(var data in monsterDataList)
        {
            monsterDataDic.Add(data.monsterName, data);
        }
    }

    #region//モンスター関数ショートカット
    #region//パルススピードを加える
    public void ForceSpeed(Rigidbody2D rb2D, Vector2 speed) 
    {
        rb2D.AddForce(speed, ForceMode2D.Impulse);
    }
    #endregion
    #region//ローテーションZからその方向から見た真下のベクトルを求める
    public Vector2 ReturnDireY(int rotateY)
    {
        Vector2 dire = default;
        if (rotateY == 0) dire = new Vector2(0, -1);
        if (rotateY == 90) dire = new Vector2(1, 0);
        if (rotateY == 180) dire = new Vector2(0, 1);
        if (rotateY == 270) dire = new Vector2(-1, 0);
        return dire;
    }
    #endregion
   
    #endregion
}
