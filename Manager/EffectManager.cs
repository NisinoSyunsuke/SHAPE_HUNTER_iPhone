using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EffectData;
using HPDataBase;
using DG.Tweening;

public class EffectManager : MonoBehaviour
{
    GameManager gameManager;

    [System.Serializable] class EffectData
    {
        public string effectName;
        public ParticleChip effectObj;
    }
    [SerializeField] List<EffectData> effectData = new List<EffectData>();
    Dictionary<string, ParticleChip> effectDataDic = new Dictionary<string, ParticleChip>();
    [System.Serializable]
    class M_MonsterEffectData
    {
        public string effectName;
        public M_AttackEffectParticle effectObj;
    }
    [SerializeField] List<M_MonsterEffectData> m_MonsterEffectDatas = new List<M_MonsterEffectData>();
    Dictionary<string, M_AttackEffectParticle> m_MonsterEffectDic = new Dictionary<string, M_AttackEffectParticle>();

    [SerializeField] EffectChip effectChip;
    [SerializeField] AttackEffectChip attackEffectChip;
    [SerializeField] M_AttackEffect m_AttackEffect;
    [SerializeField] TextEffect damageText;
    [SerializeField] ArrowEffectChip arrowEffectChip;
    public void StartProcess()
    {
        gameManager = GameManager.gameManager;
        foreach(EffectData data in effectData)
        {
            effectDataDic.Add(data.effectName, data.effectObj);
        }
        foreach(M_MonsterEffectData data in m_MonsterEffectDatas)
        {
            m_MonsterEffectDic.Add(data.effectName, data.effectObj);
        }
    }

    public void ArrowEffect(ArrowType arrowType, Vector2 pos, float scale, Vector2 forceSpeed)
    {
        var arrow = Instantiate(arrowEffectChip, pos, Quaternion.identity);
        arrow.StartProcess(arrowType, forceSpeed);
        arrow.transform.localScale = new Vector2(arrow.transform.localScale.x * scale, arrow.transform.localScale.y * scale);
    }

    public void EffectPlay(
        string effectName, Vector2 pos, float scale = 1f, float angle = default,
        EffectType effectType = EffectType.animation, float destroyTime = 3,
        Transform followTrans = default, Transform followRotate = default,
        /*パーティクルだけの引数*/ ParticleType particleType = ParticleType.particle, ShotType shotType = ShotType.none,
        bool scaleEffect = false, EffectLayer effectLayer = EffectLayer.effect
        )
    {
        switch (effectType)
        {
            case EffectType.animation:
                var anim = Instantiate(effectChip, pos, Quaternion.identity);
                anim.InfoRegister(effectName, followTrans, effectLayer);
                anim.transform.localScale = new Vector2(anim.transform.localScale.x * scale, anim.transform.localScale.y * scale);
                if (angle != default) anim.transform.eulerAngles = new Vector3(0, 0, angle);
                if (scaleEffect) ScaleEffect(anim.gameObject);
                Destroy(anim.gameObject, destroyTime);
                break;
            case EffectType.particle:
                var obj = Instantiate(effectDataDic[effectName], pos, effectDataDic[effectName].transform.localRotation);
                obj.InfoRegister(particleType, shotType, followTrans, followRotate);
                if (angle != default) obj.transform.eulerAngles = new Vector3(0, 0, angle);
                Destroy(obj.gameObject, destroyTime);
                break;
        }
    }

    public void AttackEffectPlay(
        string effectName, Vector2 pos, int damageValue,
        float rotate = 0, float scale = 1f,
        EffectType effectType = EffectType.animation, DealDamageType dealDamageType = DealDamageType.nonContact)
    {
        switch (effectType)
        {
            case EffectType.animation:
                var attackAnim = Instantiate(attackEffectChip, pos, Quaternion.identity);
                attackAnim.InfoRegister(effectName, damageValue, effectType, dealDamageType);
                attackAnim.transform.rotation = Quaternion.Euler(0, 0, rotate);
                attackAnim.transform.localScale = new Vector2(attackAnim.transform.localScale.x * scale, attackAnim.transform.localScale.y * scale);
                break;
            case EffectType.particle: break;
        }
    }

    public void M_AttackEffectPlay(string effectName, int damageValue, Vector2 pos, TakeDamageType takeDamageType = TakeDamageType.normal, Transform followTrans = default, float scale = 1f, bool reverse = false, float rotate = default)
    {
        Quaternion quaternion = Quaternion.identity;
        if (rotate != default) quaternion= Quaternion.Euler(new Vector3(0, 0, rotate));
        var effect = Instantiate(m_AttackEffect, pos, quaternion);
        float reverseFloat = reverse ? -1 : 1;
        effect.transform.localScale = new Vector2(effect.transform.localScale.x * scale * reverseFloat, effect.transform.localScale.y * scale);
        effect.InfoRegister(effectName, damageValue, monsterDamageType: takeDamageType, _followTrans: followTrans);
        if (rotate != default) effect.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotate)); 

        m_AttackEffectList.Add(effect.gameObject); //リストに登録
    }
    public void M_AttackEffectParticlePlay(
        string effectName, int damageValue, Vector2 pos, float onDamageTime = default,
        float destroyTime = 3f,
        TakeDamageType takeDamageType = TakeDamageType.normal, M_ShotType m_ShotType = M_ShotType.none,
        Transform followTrans = default, bool onDireRotation = false, Vector2 force = default,
        bool _triggerToDestroy = false, bool _onDestroyEffect = false, bool reverse = false)
    {
        var effect = Instantiate(m_MonsterEffectDic[effectName], pos, m_MonsterEffectDic[effectName].transform.localRotation);
        //effect.transform.localScale = new Vector2(effect.transform.localScale.x * scale, effect.transform.localScale.y * scale);
        //パーティクルのスケールを変更する処理はNGのようだ
        effect.InfoRegister(damageValue, onDamageTime: onDamageTime, monsterDamageType: takeDamageType, m_ShotType, _followTrans: followTrans, onDireRotation, force, _triggerToDestroy, _onDestroyEffect, reverse);
        Destroy(effect.gameObject, destroyTime);

        m_AttackEffectList.Add(effect.gameObject); //リストに登録
    }
    public void TextEffectPlay(
        string value, Vector2 pos, Color color = default,
        EffectTextType effectTextType = EffectTextType.physics, bool activeOutline = false,
        float multiple = 1f, float scale = 1f
        )
    {
        TextEffect obj = Instantiate(damageText, pos, Quaternion.identity, gameManager.worldSpaceCanvas);
        obj.text.text = value;
        obj.transform.localScale *= scale;
        if (color == default) color = Color.white;
        obj.StartProcess(color, effectTextType, activeOutline, multiple);
    }

    #region//攻撃弾全消去
    List<GameObject> m_AttackEffectList = new List<GameObject>();
    public void M_AttackEffectAllClear()
    {
        foreach (var obj in m_AttackEffectList)
        {
            if (obj == null) continue;
            Destroy(obj);
        }
        m_AttackEffectList.Clear();
    }
    #endregion

    #region//エフェクト周期float
    [System.NonSerialized] public float animCycleFloat;
    float animCycleFloatLimit;
    private void FixedUpdate()
    {
        animCycleFloat += Time.deltaTime * 0.3f;
        if (animCycleFloat > 1f) {
            animCycleFloatLimit += Time.deltaTime;
            animCycleFloat = 1;
        }
        if(animCycleFloatLimit > 2.5f)
        {
            animCycleFloat = 0;
            animCycleFloatLimit = 0;
        } 
    }
    #endregion
    #region//スケールエフェクト
    public void ScaleEffect(GameObject image)
    {
        StartCoroutine(ScaleEffectCoroutine(image));
    }
    IEnumerator ScaleEffectCoroutine(GameObject image)
    {
        Vector3 defaultVector3 = image.transform.localScale;
        while (true)
        {
            if (image == null) break;
            image.transform.DOScale(defaultVector3 * 1.25f, 0.5f);
            yield return new WaitForSeconds(0.5f);
            if (image == null) break;
            image.transform.DOScale(defaultVector3, 0.5f);
            yield return new WaitForSeconds(0.5f);
        }
    }
    #endregion
}
