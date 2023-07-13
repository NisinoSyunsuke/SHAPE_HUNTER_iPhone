using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    public static bool onRetry = false;
    public static bool firstLaunchBool= false;

    public static string PlayerNameID { get; set; }
    public static int PlayerHighScoreSeason1 { get; set; }
    public static int PlayerHighScoreSeason2 { get; set; }
    public static LanguageData.Language Language { get; set; }
    public static float MasterVolume { get; set; }

    public enum Quality { low, standard }
    public static Quality quality { get; } = Quality.low; //IOS版では常にlow

    #region//共有プロセス
    #region//角度から座標を取得
    public static Vector2 CoordinateUpperLimit(float degree, float upperLimit)
    {
        Vector2 coordinate = new Vector2(Mathf.Cos(degree * (Mathf.PI / 180)), Mathf.Sin(degree * (Mathf.PI / 180))) * upperLimit;
        return coordinate;
    }
    #endregion
    #region//座標から角度を取得
    public static float GetAngle(Vector2 coordinate)
    {
        float radian = Mathf.Atan2(coordinate.y, coordinate.x);
        float degree = radian * Mathf.Rad2Deg;
        return degree;
    }
    #endregion
    #region//現Seasonのハイスコアを取得・保存
    public static int GetThisSeasonHighScore()
    {
        return PlayerHighScoreSeason2;  //←ここに現Seasonのハイスコア入れる
    }
    public static void SetThisSeasonHighScore(int highScore)
    {
        PlayerHighScoreSeason2 = highScore;  //←ここに現Seasonのハイスコア入れる
    }
    #endregion
    #endregion
}

namespace LanguageData
{
    public enum Language { english, japanese }
    public enum TextGroup { uiText, skillNameText, skillExplaiontext }
    //public enum TextGroup { monster_name, wepon_name, accessory_name, interaction_text, uiText, explanation_equipment_text, explanation_accessory_text, explanation_text }
}

namespace MonsterBase
{
    public enum MonsterName
    {
        Cuber, Cuberin, Trialime, Trianger, Triar, Traigon, Pyramian, Spher, Sphefangor
    }
    public enum MonsterClass
    {
        _leser_, _soldier_, _elite_, _general_, _lord_
    }
    [System.Serializable]
    public class MonsterSpawnData
    {
        public MonsterName monsterName; 
        public float spawnRate;
        public float rateFluctuation;
    }
    public enum MonsterType
    {
        normal, boss, uniqueBoss, red, blue, gold
    }
}

namespace HPDataBase
{
    public enum TakeDamageType
    {
        normal, recovery
    }

    public enum DealDamageType
    {
        nonContact,
        spear,
        nonContactParticle,
        arrow
    }
}

namespace WaveDataBase
{
    public class WaveNumberData
    {
        public List<MonsterBase.MonsterSpawnData> monsterSpawnDataList = new List<MonsterBase.MonsterSpawnData>();
        public WaveRarityRate waveRarityRate;
        public WaveType waveType;
        public int bossCnt;
    }
    public enum WaveRarityRate { commonRate, rareRate, epicRate }
    public enum WaveType { normalWave, bossWave }
}

namespace EffectData
{
    public enum EffectType { animation, particle }

    public enum EffectTextType { physics, floatUp, skillCritical, critical, block, invincible }

    public enum ParticleType { particle, shot }  public enum ShotType { none, explosion, chain, greatChain, gungnirLight, gungnir }
    
    public enum M_ShotType { none, beem }

    public enum ArrowType { simpleArrow, miniArrow, HollyArrow }

    public enum EffectLayer { effect, backEffect }
}

namespace SkillDataBase
{
    public enum SkillEnum
    {
        none, ZeroGravity, LowGravity, SmallKnifeLv1, AthleteLv1,
        Thunder, Bomber, FineSpark, IronArmorLv1, LifeFragmentLv1,
        InotiShizukuLv1, EnergyTankLv1, Resurrection, EnchantmentFire,HighSpeedLv1,
        GreenApple, Potion, Gambler, Magician, SummonCircle, DoubleEdgedSword,
        Blacksmith, Deadly, Chain, Infinity, GiantLv1, Climber, 
        Vampire, Predator, BoundArrow, SpeedChange, RapidFire, DragonShot,
        GreatChain, Explosion, CampSetLv1, Shieid, AegisShieid, MiniShot,

        //強化系
        SmallKnifeLv2, SmallKnifeLv3, AthleteLv2, AthleteLv3,
        IronArmorLv2, IronArmorLv3, LifeFragmentLv2, InotiShizukuLv2,
        EnergyTankLv2, EnergyTankLv3, HighSpeedLv2, HighSpeedLv3,
        Apple, HighPotion, GiantLv2, GiantLv3, CampSetLv2, CampSetLv3,

        //追加 →　下に追加しない順番がバグる
        DoubleShot, CriticalLv1, CriticalLv2, CriticalLv3, ShieldForge, Protection, 
        Lucky, CricalWave, HollyArrow, Trident, Gungnir, FriendOfLight
    }
    public enum SkillRarity
    {
        common, rare, epic, legendary
    }
    public enum SkillItemType
    {
        skill, item
    }
    [System.Serializable]
    public class SkillData
    {
        public SkillEnum skillEnum;
        public Sprite skillSprite;
        public SkillRarity skillRarity;
        public SkillItemType type;
        [Header("スキルのみ")] public int upperLimit;
        [System.NonSerialized] public int count;
        [System.NonSerialized] public SkillItemUI skillItemUI;
    }
}

namespace RankingData
{
    public class PlayerRankingData
    {
        public string name;
        public int score;
        public PlayerRankingData(string _name = default, int _score = default)
        {
            name = _name;
            score = _score;
        }
    }

    public class RankingDataClass
    {
        public int currentRank = default;
        public List<PlayerRankingData> TopRankingList = new List<PlayerRankingData>();
        public PlayerRankingData playerRankingData = new PlayerRankingData();
    }

    public enum RankingSeason
    {
        Season1 = 1, Season2 = 2
    }
}

namespace SaveDataSpace
{
    [System.Serializable]
    public class SaveData
    {
        public string playerNameID;
        public int PlayerHighScoreSeson1;
        public int PlayerHighScoreSeson2; //{get;set;}にするとjsonにセーブされない
        public float masterVolume = 0.5f;
        public LanguageData.Language Language = LanguageData.Language.japanese;
        public bool firstLaunchBool;
    }
}

namespace StateManager/* 状態管理 */
{
    /**
     * タッチ管理クラス
     */
    public class TouchManager
    {
        public bool _touch_flag;      // タッチ有無
        public Vector2 _touch_position;   // タッチ座標
        public TouchPhase _touch_phase;   // タッチ状態

        /**
         * コンストラクタ
         *
         * @param bool flag タッチ有無
         * @param Vector2 position タッチ座標(引数の省略が行えるようにNull許容型に)
         * @param Touchphase phase タッチ状態
         * @access public
         */
        public TouchManager(bool flag = false, Vector2? position = null, TouchPhase phase = TouchPhase.Began)
        {
            this._touch_flag = flag;
            if (position == null)
            {
                this._touch_position = new Vector2(0, 0);
            }
            else
            {
                this._touch_position = (Vector2)position;
            }
            this._touch_phase = phase;
        }

        /**
         * 更新
         *
         * @access public
         */
        public void UpdateProcess()
        {
            this._touch_flag = false;

            // エディタ
            if (Application.isEditor)
            {
                // 押した瞬間
                if (Input.GetMouseButtonDown(0))
                {
                    this._touch_flag = true;
                    this._touch_phase = TouchPhase.Began;
                    //Debug.Log("押した瞬間");
                }

                // 離した瞬間
                else if (Input.GetMouseButtonUp(0))
                {
                    this._touch_flag = true;
                    this._touch_phase = TouchPhase.Ended;
                    //Debug.Log("離した瞬間");
                }

                // 押しっぱなし
                else if (Input.GetMouseButton(0))
                {
                    this._touch_flag = true;
                    this._touch_phase = TouchPhase.Moved;
                    //Debug.Log("押しっぱなし");
                }

                // 座標取得
                if (this._touch_flag) this._touch_position = Input.mousePosition;

                // 端末
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    this._touch_position = touch.position;
                    this._touch_phase = touch.phase; if (touch.phase == TouchPhase.Stationary) this._touch_phase = TouchPhase.Moved;
                    this._touch_flag = true;
                }
            }
        }

        /**
         * タッチ状態取得
         *
         * @access public
         */
        public TouchManager GetTouch()
        {
            return new TouchManager(this._touch_flag, this._touch_position, this._touch_phase);
        }
    }
}

namespace NCMBData
{
    /*
    public class UserAuth
    {
        private string currentPlayerName;

        public void logIn(string id, string pw = "notSet")
        {
            NCMBUser.LogInAsync(id, pw, (NCMBException e) => {
                // 接続成功したら
                if (e == null)
                {
                    currentPlayerName = id;
                }
            });
        }

        // mobile backendに接続して新規会員登録 ------------------------

        public void signUp(string id, string mail, string pw)
        {

            NCMBUser user = new NCMBUser();
            user.UserName = id;
            user.Email = mail;
            user.Password = pw;
            user.SignUpAsync((NCMBException e) => {

                if (e == null)
                {
                    currentPlayerName = id;
                }
            });
        }

        // 現在のプレイヤー名を返す --------------------
        public string currentPlayer()
        {
            return currentPlayerName;
        }

    }
    */
    /*
    public class LeaderBoard
    {
        public int currentRank = 0;
        public List<NCMB.HighScore> topRankers = null;
        public List<NCMB.HighScore> neighbors = null;

        // 現プレイヤーのハイスコアを受けとってランクを取得 ---------------
        public void fetchRank(int currentScore)
        {
            // データスコアの「HighScore」から検索
            NCMBQuery<NCMBObject> rankQuery = new NCMBQuery<NCMBObject>("HighScore");
            rankQuery.WhereGreaterThan("Score", currentScore);
            rankQuery.CountAsync((int count, NCMBException e) => {

                if (e != null)
                {
                    //件数取得失敗
                }
                else
                {
                    //件数取得成功
                    currentRank = count + 1; // 自分よりスコアが上の人がn人いたら自分はn+1位
                }
            });
        }

        // サーバーからトップ5を取得 ---------------    
        public void fetchTopRankers()
        {
            // データストアの「HighScore」クラスから検索
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("HighScore");
            query.OrderByDescending("Score");
            query.Limit = 5;
            query.FindAsync((List<NCMBObject> objList, NCMBException e) => {

                if (e != null)
                {
                    //検索失敗時の処理
                }
                else
                {
                    //検索成功時の処理
                    List<NCMB.HighScore> list = new List<NCMB.HighScore>();
                    // 取得したレコードをHighScoreクラスとして保存
                    foreach (NCMBObject obj in objList)
                    {
                        int s = System.Convert.ToInt32(obj["Score"]);
                        string n = System.Convert.ToString(obj["Name"]);
                        list.Add(new HighScore(s, n));
                    }
                    topRankers = list;
                }
            });
        }

        // サーバーからrankの前後2件を取得 ---------------
        public void fetchNeighbors()
        {
            neighbors = new List<NCMB.HighScore>();

            // スキップする数を決める（ただし自分が1位か2位のときは調整する）
            int numSkip = currentRank - 3;
            if (numSkip < 0) numSkip = 0;

            // データストアの「HighScore」クラスから検索
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("HighScore");
            query.OrderByDescending("Score");
            query.Skip = numSkip;
            query.Limit = 5;
            query.FindAsync((List<NCMBObject> objList, NCMBException e) => {

                if (e != null)
                {
                    //検索失敗時の処理
                }
                else
                {
                    //検索成功時の処理
                    List<NCMB.HighScore> list = new List<NCMB.HighScore>();
                    // 取得したレコードをHighScoreクラスとして保存
                    foreach (NCMBObject obj in objList)
                    {
                        int s = System.Convert.ToInt32(obj["Score"]);
                        string n = System.Convert.ToString(obj["Name"]);
                        list.Add(new HighScore(s, n));
                    }
                    neighbors = list;
                }
            });
        }
    }*/
}

