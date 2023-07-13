using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PushButton : MonoBehaviour
{
    [SerializeField] Transform targetTrans;
    Vector2 recordScale;
    void Start()
    {
        recordScale = targetTrans.localScale;
    }

    public void Push(float scaleParameter)
    {
        //StartCoroutine(PushCorutine(scaleParameter));
    }

    IEnumerator PushCorutine(float scaleParameter)
    {
        targetTrans.DOScale(recordScale * scaleParameter, 0.25f).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(0.25f);
        targetTrans.DOScale(recordScale, 0.25f).SetEase(Ease.OutCubic);
    }
}
