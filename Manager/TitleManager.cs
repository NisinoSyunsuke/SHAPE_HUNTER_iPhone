using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using RankingData;

public class TitleManager : MonoBehaviour
{
    GameManager gameManager; CanvasManager canvasManager;
    Camera _camera;

    public Image TransitionUI;
    bool nowTransiton;

    [System.Serializable]
    public class TitleUIData
    {
        public RectTransform TitleUI; public Transform TitlePointer;
        public RectTransform OptionUI; public Transform OptionPointer;

        public GameObject TitleObjUI;
        public GameObject RankingObjUI;
        public GameObject OptionObjUI;
        public GameObject CreditObjUI;
    }
    [SerializeField] TitleUIData titleUIData;
    [System.Serializable]
    public class TitleAnimData
    {
        public Animator TitleBackgroundAnim;
        public Animator TitleLogoAnim;
    }
    [SerializeField] TitleAnimData titleAnimData;
    public IEnumerator StartProcess()
    {
        gameManager = GameManager.gameManager;
        canvasManager = gameManager.canvasManager;
        _camera = Camera.main;

        StartCoroutine(TitleStartProcess());

        yield return null;

        GameManager.gameManager.titleManager.OpsionSetUp();
        UIPosFixation();
    }
    private void FixedUpdate()
    {
        if (nowTransiton) UIPosFixation();
    }
    public enum MenuState
    {
        title, start, ranking, option, credit
    }
    #region//遷移プロセス
    public void TransitionButton(string menuStateStr)
    {
        MenuState menuStateEnum = (MenuState)System.Enum.Parse(typeof(MenuState), menuStateStr);
        gameManager.canvasManager.ButtonPushSound();
        StartCoroutine(TransitionCoroutine(menuStateEnum));
    }
    IEnumerator TransitionCoroutine(MenuState menuState)
    {
        gameManager.canvasManager.InoperableSwitch(true); nowTransiton = true;
        float transitionTime = 0.75f;
        switch (menuState)
        {
            case MenuState.title:
                titleUIData.TitleObjUI.SetActive(true);
                _camera.transform.DOMove(new Vector3(-5, 0, _camera.transform.position.z), transitionTime);
                break;
            case MenuState.start:
                _camera.transform.DOMove(new Vector3(0, 0, _camera.transform.position.z), transitionTime);
                titleUIData.TitlePointer.DOMoveX(-3, 1f).SetRelative(true);
                titleUIData.OptionUI.DOMoveX(3, 1f).SetRelative(true);
                break;
            case MenuState.ranking:
                _camera.transform.DOMove(new Vector3(5, 0, _camera.transform.position.z), transitionTime);
                titleUIData.RankingObjUI.SetActive(true);
                RankingStart();
                break;
            case MenuState.option:
                titleUIData.OptionObjUI.SetActive(true);
                _camera.transform.DOMove(new Vector3(5, 0, _camera.transform.position.z), transitionTime);
                break;
            case MenuState.credit:
                titleUIData.CreditObjUI.SetActive(true);
                _camera.transform.DOMove(new Vector3(5, 0, _camera.transform.position.z), transitionTime);
                break;
        }

        yield return new WaitForSeconds(transitionTime);

        titleUIData.TitleObjUI.SetActive(false);
        titleUIData.RankingObjUI.SetActive(false);
        titleUIData.OptionObjUI.SetActive(false);
        titleUIData.CreditObjUI.SetActive(false);


        switch (menuState)
        {
            case MenuState.title:
                titleUIData.TitleObjUI.SetActive(true);
                gameManager.canvasManager.InoperableSwitch(false);
                break;
            case MenuState.start:
                StartCoroutine(GameStartCroutine());
                break;
            case MenuState.ranking:
                titleUIData.RankingObjUI.SetActive(true);
                break;
            case MenuState.option:
                titleUIData.OptionObjUI.SetActive(true);
                gameManager.canvasManager.InoperableSwitch(false);
                break;
            case MenuState.credit:
                titleUIData.CreditObjUI.SetActive(true);
                gameManager.canvasManager.InoperableSwitch(false);
                break;
        }

        if(menuState != MenuState.start) nowTransiton = false;
    }
    #endregion
    #region//ゲームスタート
    IEnumerator GameStartCroutine()
    {
        titleAnimData.TitleBackgroundAnim.CrossFadeInFixedTime("start", 0.1f, layer: 0);
        titleAnimData.TitleLogoAnim.CrossFadeInFixedTime("start", 0.1f, layer: 0);

        yield return new WaitForSeconds(1f);

        gameManager.effectManager.EffectPlay("chargeEffect_3", new Vector2(0, 0), effectType: EffectData.EffectType.particle);

        yield return new WaitForSeconds(0.75f);

        gameManager.player.transform.position = new Vector2(0, 0);
        gameManager.playerChip.DisableSwitch(false);
        StartCoroutine(gameManager.playerChip.EntryArmAnim());

        yield return new WaitForSeconds(1f);

        foreach (var obj in gameManager.canvasManager.gUIData.GUIList)
        {
            obj.SetActive(true);
        }
        gameManager.canvasManager.InoperableSwitch(false);

        yield return new WaitForSeconds(2f);

        //gameManager.waveManager.WaveStart();
        gameManager.waveManager.SummonMonsterxEternal(MonsterBase.MonsterName.Cuber);
        //音楽PLAY
        gameManager.soundManager.BgmPlay("Normal_BGM2", 0.5f);
    }
    #endregion
    #region//ランキング
    [System.Serializable]
    class RankingText
    {
        public Text scoreText;
        public Text userNameText;
    }
    [System.Serializable]
    class RankingUIData
    {
        public List<RankingText> rankingTexts = new List<RankingText>();

        public Text seasonText;

        public Text currentRankText;
        public Text playerScoreText;
        public Text playerUserNameText;

        public GameObject rankingScreen;
        public GameObject backButton;
        public GameObject playerRanking;
        [System.NonSerialized] public RankingSeason rankingSeason = RankingSeason.Season2; //現Season

        public UserRankingTop5 Season1Top5;
        [System.Serializable] public class ButtonAnimator
        {
            public RankingSeason rankingSeason;
            public Animator animator;
        }
        public List<ButtonAnimator> buttonAnimators = new List<ButtonAnimator>();
    }

    [SerializeField] RankingUIData rankingUIData = new RankingUIData();
    Coroutine rankingStartCoroutine;

    void RankingStart()
    {
        titleAnimData.TitleBackgroundAnim.CrossFadeInFixedTime("ranking", 0.1f, layer: 0);
        titleAnimData.TitleLogoAnim.CrossFadeInFixedTime("ranking", 0.1f, layer: 0);

        rankingStartCoroutine = StartCoroutine(RankingStartCorutine(true));
    }

    public void RankingSelect(int select)
    {
        rankingUIData.rankingSeason = (RankingSeason)select;

        gameManager.canvasManager.ButtonPushSound();

        rankingStartCoroutine = StartCoroutine(RankingStartCorutine());
    }

    IEnumerator RankingStartCorutine(bool buttonDelay = false)
    {
        gameManager.canvasManager.InoperableSwitch(true);
        #region//UIボタンアニメーション
        if (!buttonDelay) {
            foreach (var item in rankingUIData.buttonAnimators)
            {
                if (rankingUIData.rankingSeason == item.rankingSeason) item.animator.CrossFadeInFixedTime("Pressed", 0.1f, layer: 0);
                else item.animator.CrossFadeInFixedTime("Normal", 0.1f, layer: 0);
            }
        }
        #endregion

        yield return new WaitForSeconds(1f);

        //初期化
        for (int rankNum = 0; rankNum < 5; ++rankNum)
        {
            rankingUIData.rankingTexts[rankNum].scoreText.text = default;
            rankingUIData.rankingTexts[rankNum].userNameText.text = default;

        }

        switch (rankingUIData.rankingSeason)
        {
            case RankingSeason.Season2: //現Season

                //タイムアウト処理
                Coroutine rankingGetTimeOut = StartCoroutine(RankingGetTimeOut());

                //ランキング取得
                yield return StartCoroutine(gameManager.rankingManager.RankingSearchCorutine());

                //タイムアウト処理
                StopCoroutine(rankingGetTimeOut);

                RankingDataClass gotRankingData = gameManager.rankingManager.thisSeasonrankingData;


                for (int rankNum = 0; rankNum < gotRankingData.TopRankingList.Count; ++rankNum)
                {
                    rankingUIData.rankingTexts[rankNum].scoreText.text = gotRankingData.TopRankingList[rankNum].score.ToString();
                    rankingUIData.rankingTexts[rankNum].userNameText.text = gotRankingData.TopRankingList[rankNum].name;

                    //Debug.Log("score:" + gotRankingData.TopRankingList[rankNum].score.ToString() + ",name:" + gotRankingData.TopRankingList[rankNum].name);
                }

                if (gotRankingData.playerRankingData.name != null)
                {
                    rankingUIData.currentRankText.text = gotRankingData.currentRank.ToString();
                    rankingUIData.playerScoreText.text = gotRankingData.playerRankingData.score.ToString();
                    rankingUIData.playerUserNameText.text = gotRankingData.playerRankingData.name.ToString();
                }

                rankingUIData.playerRanking.SetActive(true);

                break;

            case RankingSeason.Season1: //過去Season

                //RankingDataClass gotRankingData1 = RankingManager.Season1rankingData;
                //↑スクリプタブルオブジェクトデータを入れる

                for (int rankNum = 0; rankNum < 5; ++rankNum)
                {
                    rankingUIData.rankingTexts[rankNum].scoreText.text = rankingUIData.Season1Top5.userDatas[rankNum].userScore.ToString();
                    rankingUIData.rankingTexts[rankNum].userNameText.text = rankingUIData.Season1Top5.userDatas[rankNum].userName;

                    //Debug.Log(gotRankingData1.TopRankingList[rankNum].score.ToString());
                }

                rankingUIData.playerRanking.SetActive(false);

                break;
        }

        rankingUIData.seasonText.text = rankingUIData.rankingSeason.ToString();
        #region//UIボタンアニメーション
        if (buttonDelay)
        {
            foreach (var item in rankingUIData.buttonAnimators)
            {
                if (rankingUIData.rankingSeason == item.rankingSeason) item.animator.CrossFadeInFixedTime("Pressed", 0.1f, layer: 0);
                else item.animator.CrossFadeInFixedTime("Normal", 0.1f, layer: 0);
            }
        }
        #endregion

        //ランキング表示
        rankingUIData.rankingScreen.SetActive(true);

        gameManager.canvasManager.InoperableSwitch(false);
    }
    IEnumerator RankingGetTimeOut()
    {
        yield return new WaitForSeconds(15f);

        StopCoroutine(rankingStartCoroutine);
        rankingStartCoroutine = default;
        RankingEnd();

        //通信エラーみたいな警告文を出す
    }

    public void RankingEnd()
    {
        titleAnimData.TitleBackgroundAnim.CrossFadeInFixedTime("scenery", 1f, layer: 0);
        titleAnimData.TitleLogoAnim.CrossFadeInFixedTime("idle", 1f, layer: 0);

        rankingUIData.rankingScreen.SetActive(false);

        TransitionButton(MenuState.title.ToString());
    }
    #endregion
    #region//オプション
    [System.Serializable]
    public class OpsionUIData
    {
        public Slider masterVolume;
        public Text languageText;
    }
    public OpsionUIData opsionUIData = new OpsionUIData();
    public void OpsionSetUp()
    {
        opsionUIData.masterVolume.value = DataBase.MasterVolume;
        switch (DataBase.Language)
        {
            case LanguageData.Language.japanese:
                opsionUIData.languageText.text = "English"; break;
            case LanguageData.Language.english:
                opsionUIData.languageText.text = "日本語"; break;
        }
    }
    public void MasterVolume()
    {
        float masterVolume = opsionUIData.masterVolume.value;
        DataBase.MasterVolume = masterVolume;
        gameManager.soundManager.BgmPlay(0.5f, 0.5f);
    }
    public void LanguageChangeButton()
    {
        gameManager.canvasManager.ButtonPushSound();
        switch (DataBase.Language)
        {
            case LanguageData.Language.japanese:
                DataBase.Language = LanguageData.Language.english;
                opsionUIData.languageText.text = "日本語";
                break;
            case LanguageData.Language.english:
                DataBase.Language = LanguageData.Language.japanese;
                opsionUIData.languageText.text = "English";
                break;
        }
        gameManager.languageManager.UITextDataApply();
    }
    public void DataSave()
    {
        gameManager.saveDataManager.saveData.masterVolume = DataBase.MasterVolume;
        gameManager.saveDataManager.saveData.Language = DataBase.Language;
        gameManager.saveDataManager.Save();
    }
    #endregion
    #region//クレジット
    #endregion

    #region//立ち上げり処理
    IEnumerator TitleStartProcess()
    {
        bool onRetry = FromRetry();
        TransitionUI.color = new Color(0, 0, 0, 1);

        yield return new WaitForSeconds(0.75f);

        if(!onRetry) titleAnimData.TitleLogoAnim.CrossFadeInFixedTime("awake", 0.1f, layer: 0);
        TransitionUI.DOFade(0, 1);

        yield return new WaitForSeconds(2.5f);
        //音楽PLAY
        gameManager.soundManager.BgmPlay("Home_BGM1", 0.5f, 0.5f);
    }
    #endregion

    #region//リトライ後
    bool FromRetry()
    {
        if (!DataBase.onRetry) return false;
        DataBase.onRetry = false;

        TransitionButton(MenuState.start.ToString());

        return true;
    }
    #endregion

    #region//UI位置固定
    [SerializeField] RectTransform canvasRect;
    void UIPosFixation()
    {
        Vector2 viewportPosition = _camera.WorldToViewportPoint(titleUIData.TitlePointer.position); 
        Vector2 worldObjectScreenPosition = new Vector2(
            ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
        //titleUIData.TitleUI.position = RectTransformUtility.WorldToScreenPoint(_camera, titleUIData.TitlePointer.position);
        titleUIData.TitleUI.anchoredPosition = worldObjectScreenPosition; 
        //Debug.Log("viewport:" + viewportPosition + " worldObj:" + worldObjectScreenPosition +" SizeDelta:"+ CanvasRect.sizeDelta);

        viewportPosition = _camera.WorldToViewportPoint(titleUIData.OptionPointer.position);
        worldObjectScreenPosition = new Vector2(
            ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
        //titleUIData.OptionUI.position = RectTransformUtility.WorldToScreenPoint(_camera, titleUIData.OptionPointer.position);
        titleUIData.OptionUI.anchoredPosition = worldObjectScreenPosition;

    }
    #endregion
}
