using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;
using RankingData;

public class RankingManager : MonoBehaviour
{
    GameManager gameManager;

    public RankingDataClass thisSeasonrankingData = new RankingDataClass();

    //[SerializeField] string playerName { get; set; }
    //[SerializeField] int score { get; set; }

    bool nowSavingRanking;
    bool nowDeleteRanking;

    const string THIS_SEASON_QUERY = "ScoreRankingSeason2";
    //Season1は　"ScoreRanking"　
    //Season2は　"ScoreRankingSeason2"
    public void StartProcess()
    {
        /*TEST
        // クラスのNCMBObjectを作成
        NCMBObject testClass = new NCMBObject("TestClass");

        // オブジェクトに値を設定

        testClass["message"] = "Hello, NCMB!";
        // データストアへの登録
        testClass.SaveAsync();
        */
        gameManager = GameManager.gameManager;
    }
    /*
    #region//ランキング表示
    void RankingIndication()
    {
        rankingData.playerRankingData = new PlayerRankingData(DataBase.playerNameID, DataBase.playerHighScore);
        Debug.Log("自身のランキング順位:" + rankingData.currentRank + " 自身のプレイヤー名-ハイスコア:" + playerName + "-" + score);
        for (int a = 0; a < rankingData.TopRankingList.Count; ++a)
        {
            Debug.Log("世界ランキング順位:" + (a + 1) + " 自身のプレイヤー名-ハイスコア:" + rankingData.TopRankingList[a].name + "-" + rankingData.TopRankingList[a].score);
        }
    }
    #endregion
    */
    /*
    #region//テスト
    public void TESTScoreREGI()
    {
        DataBase.playerHighScore = score; //呼び出し前のどこかに追加

        RankingRegister(playerName, score);
        StartCoroutine(RankingSearchCorutine());        
    }
    #endregion
    */
    public IEnumerator RankingRegister(string name, int score) //名前が重複しているとき、登録をせずfalseを返す
    {
        //通信エラー
        bool nowRegister = false;

        nowSavingRanking = true;
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(THIS_SEASON_QUERY);
        query.WhereEqualTo("PlayerName", name);
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if(e == null) //データ検索成功
            {
                if (objList.Count == 0)
                {
                    NCMBObject cloudObj = new NCMBObject(THIS_SEASON_QUERY);
                    cloudObj["PlayerName"] = name;
                    cloudObj["Score"] = score;
                    cloudObj.SaveAsync((NCMBException e) => { if (e == null) { nowSavingRanking = false; nowRegister = true; } });

                    NameDelete(name);
                }
                else if(name == DataBase.PlayerNameID)
                {
                    objList[0]["Score"] = score;
                    objList[0].SaveAsync((NCMBException e) => { if (e == null) { nowSavingRanking = false; nowRegister = true; } }); // セーブ
                    
                }
                else
                {
                    //名前が他playerに登録済み
                    nowSavingRanking = false;
                    StartCoroutine(gameManager.canvasManager.RankingRegiCancel(gameManager.languageManager.RetrunText("The name is already in use", LanguageData.TextGroup.uiText), default));
                    nowRegister = true;
                }

                
            }
            else 
            {
                //通信エラー
                nowSavingRanking = false;
            }
        });

        yield return new WaitUntil(() => nowRegister); //←trueになったら進む
        yield return new WaitUntil(() => !nowDeleteRanking);

        //player名をセーブする処理
        gameManager.saveDataManager.saveData.playerNameID = name;
        gameManager.saveDataManager.Save();
    }

    void NameDelete(string name)
    {
        nowDeleteRanking = true;
        //名前の変更があった場合、前の名前を消去
        NCMBQuery<NCMBObject> nameQuery = new NCMBQuery<NCMBObject>(THIS_SEASON_QUERY);
        nameQuery.WhereEqualTo("PlayerName", DataBase.PlayerNameID);
        nameQuery.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e == null) //データ検索成功
            {
                if(objList.Count != 0) objList[0].DeleteAsync((NCMBException e) => { if (e == null) { nowDeleteRanking = false; } });
                else nowDeleteRanking = false;
                Debug.Log("objcount"+objList.Count);
                DataBase.PlayerNameID = name;
            }
            else
            {
                //通信エラー
                nowDeleteRanking = false;
            }
        });
    }

    public IEnumerator RankingSearchCorutine()
    {

        yield return new WaitUntil(() => !nowSavingRanking);
        yield return new WaitUntil(() => !nowDeleteRanking);

        //通信エラー
        bool communicationError = false;

        int currentRank = default;
        List<PlayerRankingData> TopRankingList = new List<PlayerRankingData>();
        PlayerRankingData playerRankingData = new PlayerRankingData();

        bool nowSearching1 = true;
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(THIS_SEASON_QUERY);
        query.OrderByDescending("Score");
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e == null) //データ検索成功
            {
                for (int i = 0; i < System.Math.Min(objList.Count, 5); i++) 
                {
                    TopRankingList.Add(new PlayerRankingData(System.Convert.ToString(objList[i]["PlayerName"]), System.Convert.ToInt32(objList[i]["Score"])));
                    thisSeasonrankingData.TopRankingList = TopRankingList;
                }
                nowSearching1 = false;

            }//System.Convert.ToInt32(obj["Score"])
            else
            {
                //通信エラー
                nowSearching1 = false;
                communicationError = true;
            }
        });

        yield return new WaitUntil(() => !nowSearching1);
        bool nowSearching2 = true;

        query.WhereGreaterThan("Score", DataBase.GetThisSeasonHighScore());
        query.CountAsync((int count, NCMBException e) => {

            if (e == null)
            {
                //件数取得成功
                currentRank = count + 1; // 自分よりスコアが上の人がn人いたら自分はn+1位
                thisSeasonrankingData.currentRank = currentRank;

                nowSearching2 = false;
            }
            else
            {
                //通信エラー
                nowSearching2 = false;
                communicationError = true;
            }
        });

        yield return new WaitUntil(() => !nowSearching2);
        yield return new WaitUntil(() => !communicationError);

        thisSeasonrankingData.playerRankingData = new PlayerRankingData(DataBase.PlayerNameID, DataBase.GetThisSeasonHighScore());
        //RankingIndication();
    }
}
