using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public enum AniType { 
    LoadingAni=1,// 加载转圈动画
    UpDownFload=2,// 上下浮动动画
}
public class dotweenAni : MonoBehaviour
{
    public float rate=2.5f;
    Sequence tween;
    public AniType _curType = AniType.UpDownFload;
    void Start()
    {
        Sequence tween = DOTween.Sequence();
        if (_curType == AniType.UpDownFload)
        {
            tween.Append(this.transform.DOLocalMoveY(this.transform.localPosition.y + rate, 1).SetEase(Ease.Linear))
            .Append(this.transform.DOLocalMoveY(this.transform.localPosition.y, 1)).SetLoops(-1);
        }
        else if (_curType == AniType.LoadingAni)
        {
            tween.Append(this.transform.DORotate(new Vector3(0, 0, 180), 1).SetEase(Ease.Linear))
             .Append(this.transform.DORotate(new Vector3(0, 0, 360), 1).SetEase(Ease.Linear)).SetLoops(-1);
        }
    }
    public void PauseAni()
    {
        DOTween.PauseAll();
    }
    public void RestarAni()
    {
        DOTween.RestartAll();
    }
}
