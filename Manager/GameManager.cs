using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    public EffectManager effectManager;
    public DamageManager damageManager;
    public WaveManager waveManager;
    public MonsterManager monsterManager;
    public CameraManager cameraManager;
    public CanvasManager canvasManager;
    public SkillManager skillManager;
    public LanguageManager languageManager;
    public HPManager hpManager;
    public ScoreManager scoreManager;
    public SoundManager soundManager;
    public TitleManager titleManager;
    public RankingManager rankingManager;
    public SaveDataManager saveDataManager;
    public GoogleAdMobManager googleAdMobManager;

    public Transform worldSpaceCanvas;

    public GameObject player;
    [System.NonSerialized] public PlayerChip playerChip;

    [System.NonSerialized] public bool HorsDeCombat; //戦闘不能
    void Awake()
    {
        gameManager = this;
        //if (gameManager == null) { gameManager = this; DontDestroyOnLoad(gameObject); }
        // else { Destroy(gameObject); }
    }
    void Start()
    {
        playerChip = player.GetComponent<PlayerChip>();
        AllStartProcess();
    }
    [SerializeField] SkillDataBase.SkillEnum test;
    // Update is called once per frame
    void Update()
    {
        /**/
        //if (Input.GetKeyDown("t")) { skillManager.SkillItemGet(test); }
        //if (Input.GetKeyDown("l")) { gameManager.hpManager.HPFluctuation(-99, player.transform.position, HPDataBase.TakeDamageType.normal); }
        //if (Input.GetKeyDown("k")) { for(int cnt = 0; cnt < 2; ++cnt) { gameManager.hpManager.HPFluctuation(-99, player.transform.position, HPDataBase.TakeDamageType.normal); } }
        //if (Input.GetKeyDown("y")) { languageManager.UITextDataApply(); }
        //if (Input.GetKeyDown("g")) { StartCoroutine(gameManager.rankingManager.RankingRegister(playerName, score)); }
        //if (Input.GetKeyDown("i")) { googleAdMobManager.InterStitialStart(); }
        //if (Input.GetKey(KeyCode.Space)) { canvasManager.WeponChangeButton(); }
        //if (Input.GetKeyDown("u")) { saveDataManager.ResetData(); Debug.Log("リセット完了"); }
        playerChip.UpdateProcess();
        canvasManager.ResultSkip();
    }
    private void FixedUpdate()
    {
        playerChip.FixedUpdateProcess();
    }

    void AllStartProcess()
    {
        saveDataManager.StartProcess();
        skillManager.StartProcess();
        effectManager.StartProcess();
        damageManager.StartProcess();
        waveManager.StartProcess();
        monsterManager.StartProcess();
        cameraManager.StartProcess();
        StartCoroutine(canvasManager.StartProcess());
        hpManager.StartProcess();
        scoreManager.StartProcess();
        soundManager.StartProcess();
        StartCoroutine(titleManager.StartProcess());
        rankingManager.StartProcess();
        StartCoroutine(googleAdMobManager.StartProcess());
        languageManager.StarProcess();
        playerChip.StartProcess();

        playerChip.DisableSwitch(true);
        Application.targetFrameRate = 30; //フレームレートを設定
    }
}
