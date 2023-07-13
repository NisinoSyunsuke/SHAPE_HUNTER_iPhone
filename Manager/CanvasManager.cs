using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using SkillDataBase;
using LanguageData;
using UnityEngine.SceneManagement;
using WaveDataBase;
using StateManager;

public class CanvasManager : MonoBehaviour
{
    GameManager gameManager; LanguageManager languageManager;
    Camera _camera;
    [System.NonSerialized] public RectTransform canvasRect;

    public IEnumerator StartProcess()
    {
        gameManager = GameManager.gameManager;
        languageManager = gameManager.languageManager;
        _camera = Camera.main;
        canvasRect = GetComponent<RectTransform>();

        yield return null;

        RegisterSwipeUI();
    }
    #region//GUI
    [System.Serializable]
    public class GUIData
    {
        public List<GameObject> GUIList = new List<GameObject>();
    }
    public GUIData gUIData;
    #endregion

    #region//�X���C�vUI�֘A
    [System.Serializable] public class SwipeUIData
    {
        //public Animator swipeAnimator;
        [System.NonSerialized] public bool trigger; [System.NonSerialized] public float swipeLengh;
        [System.NonSerialized] public Tween swipePointerTransTween;[System.NonSerialized] public Coroutine swipeCoroutine;
        public RectTransform swipePointer;
        [System.NonSerialized] public Tween swipePointerImageTween; public Image swipePointerImage;
        public RectTransform swipeCircle;
        [System.NonSerialized] public Tween swipeCircleImageTween; public Image swipeCircleImage;
        [System.NonSerialized] public bool swipeLineRendererBool; public LineRenderer SwipeLineRenderer;
        [System.NonSerialized] public Vector2 defaultPointerPos;
        

        //public RectTransform swipeTouchUI;
        public Transform x0y0Pointer;
        public Transform x1y0Pointer;
        public Transform x0y1Pointer;
    }
    #region//�X���C�v���_����H
    public int SwipePermissionBoolInt
    {
        get; set;
    }
    #endregion
    public SwipeUIData swipeUIData;
    void RegisterSwipeUI() //�ǂ̉�ʃT�C�Y�ł��^�b�`��ʂ����킹�� ����������Ă���̂�Overlay�̎d�l
    {
        //float width = RectTransformUtility.WorldToScreenPoint(_camera, swipeUIData.x1y0Pointer.transform.position).x- RectTransformUtility.WorldToScreenPoint(_camera, swipeUIData.x0y0Pointer.transform.position).x;
        //float height = RectTransformUtility.WorldToScreenPoint(_camera, swipeUIData.x0y0Pointer.transform.position).y - RectTransformUtility.WorldToScreenPoint(_camera, swipeUIData.x0y1Pointer.transform.position).y;

        Vector2 viewportDeltaX = _camera.WorldToViewportPoint(swipeUIData.x1y0Pointer.transform.position) - _camera.WorldToViewportPoint(swipeUIData.x0y0Pointer.transform.position);
        //Vector2 viewportPositionY = _camera.WorldToViewportPoint(swipeUIData.x0y0Pointer.transform.position - swipeUIData.x0y1Pointer.transform.position);
        Vector2 viewportDeltaY = _camera.WorldToViewportPoint(swipeUIData.x0y0Pointer.transform.position) - _camera.WorldToViewportPoint(swipeUIData.x0y1Pointer.transform.position);
        Vector2 worldObjectScreenPosition = new Vector2(
            ((viewportDeltaX.x * canvasRect.sizeDelta.x)),
            ((viewportDeltaY.y * canvasRect.sizeDelta.y)));

        //swipeUIData.swipeTouchUI.sizeDelta = new Vector2(width, height) / transform.localScale;
        //swipeUIData.swipeTouchUI.sizeDelta = worldObjectScreenPosition;
    }
    public void SwipeStart(bool trigger)
    {
        if (gameManager.playerChip.CurrentWeponState == PlayerChip.WeponState.Spear && gameManager.hpManager.EnergyCheck()) return;
        swipeUIData.trigger = trigger;
        if (trigger)
        {
            if (SwipePermissionBoolInt > 0) { swipeUIData.trigger = false; return; }
            swipeUIData.swipePointerTransTween.Kill();
            swipeUIData.swipePointerImageTween.Kill();
            swipeUIData.swipeCircleImageTween.Kill();
            swipeUIData.swipePointerImage.DOFade(endValue: 0.6f, 0.1f); //swipeUIData.swipePointerImage.color = new Color(255, 255, 255, 100);
            swipeUIData.swipeCircleImage.DOFade(endValue: 0.2f, 0.1f);
            StartCoroutine(SwipeLineRenderer());
            if (swipeUIData.swipeCoroutine != null) StopCoroutine(swipeUIData.swipeCoroutine);
        }
        else
        {
            float speedTime = 0.75f - (0.5f * swipeUIData.swipeLengh);
            swipeUIData.swipePointerTransTween = swipeUIData.swipePointer.DOAnchorPos(swipeUIData.defaultPointerPos, speedTime).SetEase(Ease.OutBack);
            swipeUIData.swipeCoroutine = StartCoroutine(SwipeCoroutine(speedTime));
            if(gameManager.playerChip.CurrentWeponState == PlayerChip.WeponState.Spear)gameManager.hpManager.EnergyFluctuation(-swipeUIData.swipeLengh);
        }
    }
    public void PointerCoordinate(Vector2 coordinate, float lengh)
    {
        swipeUIData.swipeCircle.anchoredPosition = swipeUIData.defaultPointerPos;

        Vector2 worldObjectScreenPosition = new Vector2(
            ((coordinate.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((coordinate.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

        swipeUIData.swipePointer.anchoredPosition = worldObjectScreenPosition;
        swipeUIData.swipeLengh = lengh;
        swipeUIData.SwipeLineRenderer.SetPosition(0, swipeUIData.defaultPointerPos);
        swipeUIData.SwipeLineRenderer.SetPosition(1, worldObjectScreenPosition);
        swipeUIData.SwipeLineRenderer.endWidth = lengh * 1f;

        //swipeUIData.swipeAnimator.SetFloat("state", lengh);
    }
    IEnumerator SwipeCoroutine(float speedTime)
    {
        swipeUIData.SwipeLineRenderer.endColor = new Color32(255, 255, 255, 0);
        swipeUIData.swipeLineRendererBool = true;

        yield return new WaitForSeconds(speedTime);
        //swipeUIData.swipeAnimator.SetFloat("state", 0);
        swipeUIData.swipePointerImageTween = swipeUIData.swipePointerImage.DOFade(endValue: 0f, 0.5f);
        swipeUIData.swipeCircleImageTween = swipeUIData.swipeCircleImage.DOFade(endValue: 0f, 0.5f);
    }

    IEnumerator SwipeLineRenderer()
    {
        swipeUIData.swipeLineRendererBool = false; 
        float fadeValue = 0;
        while (!swipeUIData.swipeLineRendererBool && fadeValue <= 0.5f)
        {
            fadeValue += 0.1f; 
            swipeUIData.SwipeLineRenderer.endColor = new Color(1, 1, 1, fadeValue);
            yield return new WaitForSeconds(0.02f);
        }
    }
    #endregion

    #region//�G�i�W�[�Q�[�W
    [System.Serializable] public class EnergyUIData {
        public Slider energySlider;
        public Image fill;
        public GameObject energyGage;
        public Coroutine energyScaleCoroutine;
        public Tween energyScaleTween;
        public Coroutine energyScaleActiveCoroutine;
    }
    public EnergyUIData energyUIData;
    public void EnergyCounter(float energy, float maxEnergy)
    {
        energyUIData.energySlider.value = energy / maxEnergy;
    }
    public void EnergyBarScale(float scale, bool activeProcess = false)
    {
        if(!activeProcess)energyUIData.energyScaleCoroutine = StartCoroutine(EnergyBarScaleCoroutine(scale));
        //else energyUIData.energyScaleTween = energyUIData.energyGage.transform.DOScale(scale, 0.3f).SetEase(Ease.OutQuint);
    }
    IEnumerator EnergyBarScaleCoroutine(float scale)
    {
        float duration = 0.2f;
        energyUIData.energyScaleTween = energyUIData.energyGage.transform.DOScale(scale, duration).SetEase(Ease.OutQuint);
        yield return new WaitForSeconds(duration + 0.05f);
        energyUIData.energyScaleTween = energyUIData.energyGage.transform.DOScale(1f, duration);
    }
    #endregion

    #region//�q�b�g�|�C���g�֘A
    [System.Serializable] class HPUIData { public Slider hpSlider; public Text hpText; public Text maxHpText; }
    [SerializeField] HPUIData hpUIData;
    public void HPCounter(int hp, int maxhp)
    {
        hpUIData.hpText.text = hp.ToString(); hpUIData.maxHpText.text = maxhp.ToString();
        hpUIData.hpSlider.value = (float)hp / (float)maxhp;
    }
    #endregion

    #region//����ύXUI
    [System.Serializable]
    public class WeponUI
    {
        //[Header("")]
        public GameObject Frame;
        public Sprite spear; public Sprite bow;
        public Image weponImage;
        public Transform backFrameTrans;
        [HideInInspector] public Vector2 recordFramePos;

    }
    public WeponUI weponUI = new WeponUI();
    bool isWeponChangeCoroutineRunning;
    public void WeponChangeButton()
    {
        if (isWeponChangeCoroutineRunning /*&& ���̑��̏���������X���C�v���Ȃ�*/) return;
        StartCoroutine(WeponChangeCoroutine());
    }
    IEnumerator WeponChangeCoroutine()
    {
        isWeponChangeCoroutineRunning = true;
        ++SwipePermissionBoolInt;
        
        float delayTime = 0.5f;
        float changeSpeed = 0.65f * gameManager.skillManager.SpeedChange(); /*s�X�L��_SpeedChange*/

        /*weponUI.recordFramePos = weponUI.Frame.transform.position;
        weponUI.Frame.transform.DOMove(weponUI.recordFramePos, 0.4f * changeSpeed).SetDelay(0.4f * changeSpeed);
        weponUI.Frame.transform.DOMove(weponUI.backFrameTrans.position, 0.4f * changeSpeed);*/
        

        Sprite sprite = default;
        PlayerChip.WeponState weponState = default;
        //gameManager.soundManager.Play("whips_2", 1.25f, 0.2f);
        switch (gameManager.playerChip.CurrentWeponState)
        {
            case PlayerChip.WeponState.Bow:
                weponUI.Frame.transform.DORotate(new Vector3(0, 0, 90), delayTime * changeSpeed).SetEase(Ease.OutBack);
                sprite = weponUI.spear;
                weponState = PlayerChip.WeponState.Spear;
                EnergyBarScale(1f, activeProcess: true);
                break;
            case PlayerChip.WeponState.Spear:
                weponUI.Frame.transform.DORotate(new Vector3(0, 0, 0), delayTime * changeSpeed).SetEase(Ease.OutBack);
                sprite = weponUI.bow;
                weponState = PlayerChip.WeponState.Bow;
                EnergyBarScale(0f, activeProcess: true);
                break;
        }
        weponUI.weponImage.transform.DOScale(Vector3.zero, 0.2f * changeSpeed);

        yield return new WaitForSeconds(0.4f * changeSpeed);

        gameManager.soundManager.Play("wepon_1", 2.5f, 0.3f);
        weponUI.weponImage.transform.DOScale(Vector3.one, 0.2f * changeSpeed);
        weponUI.weponImage.sprite = sprite;
        gameManager.playerChip.ChangeWepon(weponState);

        --SwipePermissionBoolInt;
        isWeponChangeCoroutineRunning = false;
    }
    #endregion

    #region//�X�R�A�֘A
    [System.Serializable] class ScoreUIData { public Text scoreText; }
    [SerializeField] ScoreUIData scoreUIData;
    public void ScoreCounter(int score)
    {
        scoreUIData.scoreText.text = "score�@" + score;
    }
    #endregion

    #region//�E�F�[�u�֘A
    [System.Serializable] class WaveUIData { public Text waveCountText; }
    [SerializeField] WaveUIData waveData;
    public void WaveCounter(int waveCount)
    {
        waveData.waveCountText.text = waveCount.ToString();
    }
    #endregion

    #region//�X�L��UI�֘A
    [System.Serializable] public class SkillUIData { public Transform SkillUIZone; }
    public SkillUIData skillUIData;
    #endregion

    #region//����s�\�֘A
    [System.Serializable] class InoperableUIData { public GameObject inoperablePanel; }
    [SerializeField] InoperableUIData inoperableUIData;
    public void InoperableSwitch(bool _switch)
    {
        if (_switch)
        {
            inoperableUIData.inoperablePanel.SetActive(true);
        }
        else
        {
            inoperableUIData.inoperablePanel.SetActive(false);
        }
    }
    #endregion

    #region//�Q�[���̏��
    public enum GameState { playing, reward, gameover }
    GameState gameState;
    public void GameStateChange(GameState _gameState)
    {
        if (gameState == GameState.gameover) return;
        switch (_gameState)
        {
            case GameState.playing:
                break;
            case GameState.reward:
                RewardStart();
                break;
            case GameState.gameover:
                StartCoroutine(GameOverCorutine());
                break;
        }
        //Debug.Log(_gameState);
        gameState = _gameState;
    }
    #endregion

    #region//��V�v���Z�X
    [System.Serializable] public class RewardUIData
    {
        public GameObject RewardScreen;
        public List<SkilItemSelect> skilItemSelects = new List<SkilItemSelect>();
        [System.NonSerialized] public Image recordSkilItemSelectImage;
        [System.NonSerialized] public SkilItemSelect recordSkilItemSelect;
        [System.NonSerialized] public int skillItemSelectCount;
        [System.NonSerialized] public SkillEnum selectSkill;
        public GridLayoutGroup gridLayoutGroup;
        public Text nameText; public Text explanationText;
        public RewardUIData()
        {
            skillItemSelectCount = 3;
        }
        #region//�X�L������_�}�W�V����
        [System.Serializable] public class MagicianButton
        {
            public GameObject magicianButton;
            public Image skillImage;
            public Image selected;
        }
        public MagicianButton magicianButton;
        #endregion
    }
    public RewardUIData rewardUIData = new RewardUIData();
    //��V��ʃX�^�[�g
    public void RewardStart()
    {
        rewardUIData.RewardScreen.SetActive(true);
        SelectSkill(SkillEnum.none);
        gameManager.skillManager.SkillSelection(rewardUIData.skillItemSelectCount);

        #region//�X�L������
        gameManager.skillManager.MagicianReset();
        #endregion
    }
    //�X�L���I��
    public void SelectSkill(SkillEnum skillEnum, SkilItemSelect skilItemSelect = default)
    {
        rewardUIData.selectSkill = skillEnum;
        //�X�L���̐������͂�ǉ�
        if (skillEnum != SkillEnum.none) rewardUIData.nameText.text = languageManager.RetrunText(skillEnum.ToString(), TextGroup.skillNameText);
        else rewardUIData.nameText.text = "";
        rewardUIData.explanationText.text = languageManager.RetrunText(skillEnum.ToString(), TextGroup.skillExplaiontext);
        //�Z���N�g�G�t�F�N�g
        if(rewardUIData.recordSkilItemSelect != null) rewardUIData.recordSkilItemSelect.Selected = false;
        if (skilItemSelect != default)
        {
            skilItemSelect.Selected = true;
            rewardUIData.recordSkilItemSelect = skilItemSelect;
            gameManager.soundManager.Play("select_1", 1f, 0.1f);
        }
    }
    //��V��ʏI��
    public void RewardEnd()
    {
        if (rewardUIData.selectSkill == SkillEnum.none) { return; }
        else { gameManager.skillManager.SkillItemGet(rewardUIData.selectSkill); }
        rewardUIData.RewardScreen.SetActive(false);
        gameManager.waveManager.WaveStart();
        gameManager.soundManager.Play("select_2", 1f, 0.1f);
        GameStateChange(GameState.playing);
    }
    #region//�X�L������
    //�X�L���A�C�e�����ύX
    //�M�����u���[
    public void IncreaseSkillItemSelectCount(int value = 1)
    {
        rewardUIData.skillItemSelectCount += value;
        switch (rewardUIData.skillItemSelectCount)
        {
            case 3: rewardUIData.gridLayoutGroup.spacing = new Vector2(125f, 0); break;
            case 4: rewardUIData.gridLayoutGroup.spacing = new Vector2(60f, 0); rewardUIData.skilItemSelects[4 - 1].gameObject.SetActive(true); break;
            case 5: rewardUIData.gridLayoutGroup.spacing = new Vector2(37.5f, 0); rewardUIData.skilItemSelects[5 - 1].gameObject.SetActive(true); break;
                //5�����
                //3�̏ꍇ��125 4�̏ꍇ��60�@5�̏ꍇ��37.5
        }
    }
    //�}�W�V����
    public void Magician()
    {
        if (!gameManager.skillManager.Magician()) return;
        gameManager.skillManager.SkillSelection(rewardUIData.skillItemSelectCount);
        gameManager.soundManager.Play("select_3", 1f, 0.1f);
        SelectSkill(SkillEnum.none);
    }
    #endregion
    #endregion

    #region//�Q�[���I�[�o�[�v���Z�X
    [System.Serializable]
    public class GameOverUIData
    {
        [Header("�Q�[���I�[�o�[")]
        public GameObject GameOverScreen;
        public List<Text> MonsterClassValue = new List<Text>();
        public Text WaveValue;
        public Text BonusValue;
        public Text ScoreValue;
        public GameObject ScoreText;

        [Header("�����L���O�o�^")]
        public GameObject rankingRegisterStrikethrough;
        public GameObject RankingRegisterScreen;
        public Text playerNameDisplayText;
        public InputField playerNameDisplayInputField;
        public Text errorTextDisplay;
        public Text rankingRegisterText;
        public Text highScoreText;

        public float resultSkipFloat = 1f;
        public bool resultSkipBool = false;
    }
    public GameOverUIData gameOverUIData = new GameOverUIData();
    public class GameScoreData
    {
        public List<int> monsterClassValues = new List<int>();
        public int waveValue;
        public int bonusValue;
        public int scoreVlue;
    } 
    IEnumerator GameOverCorutine()
    {

        yield return new WaitForSeconds(1f);
        gameOverUIData.resultSkipBool = true;
        gameOverUIData.GameOverScreen.SetActive(true);
        RankingButtonActiveProcess(false);
        //���ySTOP
        //gameManager.soundManager.BGMStop();
        gameManager.soundManager.BgmPlay(0.1f, 0.75f);

        yield return new WaitForSeconds(0.25f);
        GameScoreData gameScoreData = gameManager.scoreManager.FinalScoreRegister(gameManager.waveManager.WaveCount());
        
        //�����X�^�[�X�R�A
        for (int count = 0; count < gameOverUIData.MonsterClassValue.Count; ++count)
        {
            yield return new WaitForSeconds(0.5f * gameOverUIData.resultSkipFloat);
            gameOverUIData.MonsterClassValue[count].text = gameScoreData.monsterClassValues[count].ToString();
        }

        //WAVE�X�R�A
        yield return new WaitForSeconds(0.5f * gameOverUIData.resultSkipFloat);
        gameOverUIData.WaveValue.text = gameScoreData.waveValue.ToString();

        //�{�[�i�X�X�R�A
        yield return new WaitForSeconds(0.5f * gameOverUIData.resultSkipFloat);
        gameOverUIData.BonusValue.text = gameScoreData.bonusValue.ToString();

        //�ŏI�X�R�A
        yield return new WaitForSeconds(0.75f * gameOverUIData.resultSkipFloat);
        gameOverUIData.ScoreValue.text = gameScoreData.scoreVlue.ToString();
        gameOverUIData.ScoreText.SetActive(true);

        //�n�C�X�R�A�X�V
        if(DataBase.GetThisSeasonHighScore() < gameScoreData.scoreVlue)
        {
            //�n�C�X�R�A�����L���O�o�^����{�^����������悤�ɂ��鏈��
            RankingButtonActiveProcess(true);
            //�n�C�X�R�A���o�^
            preScoreValue = gameScoreData.scoreVlue;
        }

        gameManager.canvasManager.InoperableSwitch(false);
    }
    //�^�b�`�Ń��U���g�X�L�b�v
    public void ResultSkip()
    {
        if (!gameOverUIData.resultSkipBool) return;
        TouchManager touchManager = new TouchManager();
        // �^�b�`��ԍX�V
        touchManager.UpdateProcess();
        // �^�b�`�擾
        TouchManager touchState = touchManager.GetTouch();
        if (!touchState._touch_flag) return;
        switch (touchState._touch_phase)
        {
            case TouchPhase.Began:
                gameOverUIData.resultSkipFloat = 0.1f;
                break;
        }
    }

    #region//�^�C�g���ɖ߂�
    public void BackTitleStart()
    {
        StartCoroutine(BackTitleStartCroutine());
    }
    #endregion
    #region//���g���C
    public void RetryStart()
    {
        DataBase.onRetry = true;
        StartCoroutine(BackTitleStartCroutine());
    }
    IEnumerator BackTitleStartCroutine()
    {
        ButtonPushSound();
        InoperableSwitch(true);  
        gameManager.titleManager.TransitionUI.DOFade(1, 1);

        yield return new WaitForSeconds(1f);

        gameManager.googleAdMobManager.InterStitialStart();
        yield return new WaitUntil(() => !gameManager.googleAdMobManager.InterstitialShowing);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion

    #region//�����L���O
    public void RankingRegisterStart()
    {
        ButtonPushSound();
        gameOverUIData.RankingRegisterScreen.SetActive(true);
        gameOverUIData.GameOverScreen.SetActive(false);
    }
    
    public void RankingRegisterEnd()
    {
        ButtonPushSound();
        nowRankingRegiCoroutine = false;
        RegisterTextDisplay("", Color.white);
        gameOverUIData.RankingRegisterScreen.SetActive(false);
        gameOverUIData.GameOverScreen.SetActive(true);
    }
    #region//�����L���O�o�^
    public void RankingRegister()
    {
        if (nowRankingRegiCoroutine) return;
        ButtonPushSound();
        rankingRegisterCoroutine = StartCoroutine(RankingRegisterCoroutine());
    }

    Coroutine rankingRegisterCoroutine;
    Coroutine rankingGetTimeOut;
    bool nowRankingRegiCoroutine;
    int preScoreValue;
    IEnumerator RankingRegisterCoroutine()
    {
        InoperableSwitch(true); nowRankingRegiCoroutine = true;

        //�^�C���A�E�g����
        Coroutine rankingGetTimeOut = StartCoroutine(RankingRegiCancel(languageManager.RetrunText("Timed out", TextGroup.uiText), 15f));

        yield return StartCoroutine(gameManager.rankingManager.RankingRegister(gameOverUIData.playerNameDisplayText.text, preScoreValue));

        //�^�C���A�E�g����
        StopCoroutine(rankingGetTimeOut);

        RegisterTextDisplay(languageManager.RetrunText("Ranking registration completed", TextGroup.uiText), Color.white);

        //�n�C�X�R�A���Z�[�u���鏈��
        DataBase.SetThisSeasonHighScore(preScoreValue);
        gameManager.saveDataManager.saveData.PlayerHighScoreSeson2 = DataBase.GetThisSeasonHighScore();
        gameManager.saveDataManager.Save();

        yield return new WaitForSeconds(3f);

        RankingButtonActiveProcess(false);
        RankingRegisterEnd();

        InoperableSwitch(false);
    }
    bool nowRankingRegiCanceling;
    public IEnumerator RankingRegiCancel(string cancelReasonText, float timeOutTime)
    {
        nowRankingRegiCanceling = true;
        yield return new WaitForSeconds(timeOutTime);

        if (!nowRankingRegiCanceling) yield break;
        nowRankingRegiCanceling = false;

        StopCoroutine(rankingRegisterCoroutine);
        rankingRegisterCoroutine = default;

        //�ʐM�G���[�݂����Ȍx�������o��
        RegisterTextDisplay(cancelReasonText, Color.red);

        yield return new WaitForSeconds(3f);

        InoperableSwitch(false);
        RankingRegisterEnd();
    }

    void RegisterTextDisplay(string text, Color color)
    {
        //languagemanager��ʂ�
        gameOverUIData.errorTextDisplay.color = color;
        gameOverUIData.errorTextDisplay.text = text;
    }
    //�����L���O�{�^���A�N�e�B�u����
    void RankingButtonActiveProcess(bool active)
    {
        if (active)
        {
            gameOverUIData.rankingRegisterStrikethrough.SetActive(false);
            gameOverUIData.highScoreText.gameObject.SetActive(true);
            gameOverUIData.rankingRegisterText.color = Color.white;
        }
        else
        {
            gameOverUIData.rankingRegisterStrikethrough.SetActive(true);
            gameOverUIData.highScoreText.gameObject.SetActive(false);
            gameOverUIData.rankingRegisterText.color = Color.grey;
        }
    }
    #endregion
    
    #region//���[�UID�o�^
    public void StringInput()
    {
        gameOverUIData.playerNameDisplayText.text = gameOverUIData.playerNameDisplayInputField.text;
    }
    #endregion
    #region//���ʉ�
    public void ButtonPushSound()
    {
        gameManager.soundManager.Play("select_1", 0.7f, 0.05f);
    }
    #endregion
    #endregion
    #endregion
}
