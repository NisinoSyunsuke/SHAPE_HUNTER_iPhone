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
    public static Quality quality { get; } = Quality.low; //IOS�łł͏��low

    #region//���L�v���Z�X
    #region//�p�x������W���擾
    public static Vector2 CoordinateUpperLimit(float degree, float upperLimit)
    {
        Vector2 coordinate = new Vector2(Mathf.Cos(degree * (Mathf.PI / 180)), Mathf.Sin(degree * (Mathf.PI / 180))) * upperLimit;
        return coordinate;
    }
    #endregion
    #region//���W����p�x���擾
    public static float GetAngle(Vector2 coordinate)
    {
        float radian = Mathf.Atan2(coordinate.y, coordinate.x);
        float degree = radian * Mathf.Rad2Deg;
        return degree;
    }
    #endregion
    #region//��Season�̃n�C�X�R�A���擾�E�ۑ�
    public static int GetThisSeasonHighScore()
    {
        return PlayerHighScoreSeason2;  //�������Ɍ�Season�̃n�C�X�R�A�����
    }
    public static void SetThisSeasonHighScore(int highScore)
    {
        PlayerHighScoreSeason2 = highScore;  //�������Ɍ�Season�̃n�C�X�R�A�����
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

        //�����n
        SmallKnifeLv2, SmallKnifeLv3, AthleteLv2, AthleteLv3,
        IronArmorLv2, IronArmorLv3, LifeFragmentLv2, InotiShizukuLv2,
        EnergyTankLv2, EnergyTankLv3, HighSpeedLv2, HighSpeedLv3,
        Apple, HighPotion, GiantLv2, GiantLv3, CampSetLv2, CampSetLv3,

        //�ǉ� ���@���ɒǉ����Ȃ����Ԃ��o�O��
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
        [Header("�X�L���̂�")] public int upperLimit;
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
        public int PlayerHighScoreSeson2; //{get;set;}�ɂ����json�ɃZ�[�u����Ȃ�
        public float masterVolume = 0.5f;
        public LanguageData.Language Language = LanguageData.Language.japanese;
        public bool firstLaunchBool;
    }
}

namespace StateManager/* ��ԊǗ� */
{
    /**
     * �^�b�`�Ǘ��N���X
     */
    public class TouchManager
    {
        public bool _touch_flag;      // �^�b�`�L��
        public Vector2 _touch_position;   // �^�b�`���W
        public TouchPhase _touch_phase;   // �^�b�`���

        /**
         * �R���X�g���N�^
         *
         * @param bool flag �^�b�`�L��
         * @param Vector2 position �^�b�`���W(�����̏ȗ����s����悤��Null���e�^��)
         * @param Touchphase phase �^�b�`���
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
         * �X�V
         *
         * @access public
         */
        public void UpdateProcess()
        {
            this._touch_flag = false;

            // �G�f�B�^
            if (Application.isEditor)
            {
                // �������u��
                if (Input.GetMouseButtonDown(0))
                {
                    this._touch_flag = true;
                    this._touch_phase = TouchPhase.Began;
                    //Debug.Log("�������u��");
                }

                // �������u��
                else if (Input.GetMouseButtonUp(0))
                {
                    this._touch_flag = true;
                    this._touch_phase = TouchPhase.Ended;
                    //Debug.Log("�������u��");
                }

                // �������ςȂ�
                else if (Input.GetMouseButton(0))
                {
                    this._touch_flag = true;
                    this._touch_phase = TouchPhase.Moved;
                    //Debug.Log("�������ςȂ�");
                }

                // ���W�擾
                if (this._touch_flag) this._touch_position = Input.mousePosition;

                // �[��
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
         * �^�b�`��Ԏ擾
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
                // �ڑ�����������
                if (e == null)
                {
                    currentPlayerName = id;
                }
            });
        }

        // mobile backend�ɐڑ����ĐV�K����o�^ ------------------------

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

        // ���݂̃v���C���[����Ԃ� --------------------
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

        // ���v���C���[�̃n�C�X�R�A���󂯂Ƃ��ă����N���擾 ---------------
        public void fetchRank(int currentScore)
        {
            // �f�[�^�X�R�A�́uHighScore�v���猟��
            NCMBQuery<NCMBObject> rankQuery = new NCMBQuery<NCMBObject>("HighScore");
            rankQuery.WhereGreaterThan("Score", currentScore);
            rankQuery.CountAsync((int count, NCMBException e) => {

                if (e != null)
                {
                    //�����擾���s
                }
                else
                {
                    //�����擾����
                    currentRank = count + 1; // �������X�R�A����̐l��n�l�����玩����n+1��
                }
            });
        }

        // �T�[�o�[����g�b�v5���擾 ---------------    
        public void fetchTopRankers()
        {
            // �f�[�^�X�g�A�́uHighScore�v�N���X���猟��
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("HighScore");
            query.OrderByDescending("Score");
            query.Limit = 5;
            query.FindAsync((List<NCMBObject> objList, NCMBException e) => {

                if (e != null)
                {
                    //�������s���̏���
                }
                else
                {
                    //�����������̏���
                    List<NCMB.HighScore> list = new List<NCMB.HighScore>();
                    // �擾�������R�[�h��HighScore�N���X�Ƃ��ĕۑ�
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

        // �T�[�o�[����rank�̑O��2�����擾 ---------------
        public void fetchNeighbors()
        {
            neighbors = new List<NCMB.HighScore>();

            // �X�L�b�v���鐔�����߂�i������������1�ʂ�2�ʂ̂Ƃ��͒�������j
            int numSkip = currentRank - 3;
            if (numSkip < 0) numSkip = 0;

            // �f�[�^�X�g�A�́uHighScore�v�N���X���猟��
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("HighScore");
            query.OrderByDescending("Score");
            query.Skip = numSkip;
            query.Limit = 5;
            query.FindAsync((List<NCMBObject> objList, NCMBException e) => {

                if (e != null)
                {
                    //�������s���̏���
                }
                else
                {
                    //�����������̏���
                    List<NCMB.HighScore> list = new List<NCMB.HighScore>();
                    // �擾�������R�[�h��HighScore�N���X�Ƃ��ĕۑ�
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

