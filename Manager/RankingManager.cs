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
    //Season1�́@"ScoreRanking"�@
    //Season2�́@"ScoreRankingSeason2"
    public void StartProcess()
    {
        /*TEST
        // �N���X��NCMBObject���쐬
        NCMBObject testClass = new NCMBObject("TestClass");

        // �I�u�W�F�N�g�ɒl��ݒ�

        testClass["message"] = "Hello, NCMB!";
        // �f�[�^�X�g�A�ւ̓o�^
        testClass.SaveAsync();
        */
        gameManager = GameManager.gameManager;
    }
    /*
    #region//�����L���O�\��
    void RankingIndication()
    {
        rankingData.playerRankingData = new PlayerRankingData(DataBase.playerNameID, DataBase.playerHighScore);
        Debug.Log("���g�̃����L���O����:" + rankingData.currentRank + " ���g�̃v���C���[��-�n�C�X�R�A:" + playerName + "-" + score);
        for (int a = 0; a < rankingData.TopRankingList.Count; ++a)
        {
            Debug.Log("���E�����L���O����:" + (a + 1) + " ���g�̃v���C���[��-�n�C�X�R�A:" + rankingData.TopRankingList[a].name + "-" + rankingData.TopRankingList[a].score);
        }
    }
    #endregion
    */
    /*
    #region//�e�X�g
    public void TESTScoreREGI()
    {
        DataBase.playerHighScore = score; //�Ăяo���O�̂ǂ����ɒǉ�

        RankingRegister(playerName, score);
        StartCoroutine(RankingSearchCorutine());        
    }
    #endregion
    */
    public IEnumerator RankingRegister(string name, int score) //���O���d�����Ă���Ƃ��A�o�^������false��Ԃ�
    {
        //�ʐM�G���[
        bool nowRegister = false;

        nowSavingRanking = true;
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(THIS_SEASON_QUERY);
        query.WhereEqualTo("PlayerName", name);
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if(e == null) //�f�[�^��������
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
                    objList[0].SaveAsync((NCMBException e) => { if (e == null) { nowSavingRanking = false; nowRegister = true; } }); // �Z�[�u
                    
                }
                else
                {
                    //���O����player�ɓo�^�ς�
                    nowSavingRanking = false;
                    StartCoroutine(gameManager.canvasManager.RankingRegiCancel(gameManager.languageManager.RetrunText("The name is already in use", LanguageData.TextGroup.uiText), default));
                    nowRegister = true;
                }

                
            }
            else 
            {
                //�ʐM�G���[
                nowSavingRanking = false;
            }
        });

        yield return new WaitUntil(() => nowRegister); //��true�ɂȂ�����i��
        yield return new WaitUntil(() => !nowDeleteRanking);

        //player�����Z�[�u���鏈��
        gameManager.saveDataManager.saveData.playerNameID = name;
        gameManager.saveDataManager.Save();
    }

    void NameDelete(string name)
    {
        nowDeleteRanking = true;
        //���O�̕ύX���������ꍇ�A�O�̖��O������
        NCMBQuery<NCMBObject> nameQuery = new NCMBQuery<NCMBObject>(THIS_SEASON_QUERY);
        nameQuery.WhereEqualTo("PlayerName", DataBase.PlayerNameID);
        nameQuery.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e == null) //�f�[�^��������
            {
                if(objList.Count != 0) objList[0].DeleteAsync((NCMBException e) => { if (e == null) { nowDeleteRanking = false; } });
                else nowDeleteRanking = false;
                Debug.Log("objcount"+objList.Count);
                DataBase.PlayerNameID = name;
            }
            else
            {
                //�ʐM�G���[
                nowDeleteRanking = false;
            }
        });
    }

    public IEnumerator RankingSearchCorutine()
    {

        yield return new WaitUntil(() => !nowSavingRanking);
        yield return new WaitUntil(() => !nowDeleteRanking);

        //�ʐM�G���[
        bool communicationError = false;

        int currentRank = default;
        List<PlayerRankingData> TopRankingList = new List<PlayerRankingData>();
        PlayerRankingData playerRankingData = new PlayerRankingData();

        bool nowSearching1 = true;
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(THIS_SEASON_QUERY);
        query.OrderByDescending("Score");
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e == null) //�f�[�^��������
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
                //�ʐM�G���[
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
                //�����擾����
                currentRank = count + 1; // �������X�R�A����̐l��n�l�����玩����n+1��
                thisSeasonrankingData.currentRank = currentRank;

                nowSearching2 = false;
            }
            else
            {
                //�ʐM�G���[
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
