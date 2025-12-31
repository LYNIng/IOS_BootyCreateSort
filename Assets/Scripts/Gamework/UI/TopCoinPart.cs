using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public partial class TopCoinPart : MonoCompositeSingleton<TopCoinPart>, IMsgObj
{
    public override bool DontDestory => false;

    Tween coinTween = null;
    int tmpCoin;

    protected override void OnAwake()
    {
        base.OnAwake();

        tmpCoin = GlobalSingleton.Coin;
        txtValue.text = tmpCoin.ToString();

        OnRefresh();
    }

    [CmdCallback((ushort)GameEvent.RefreshCoin)]
    private void OnRefresh()
    {
        int dst = GlobalSingleton.Coin - tmpCoin;
        tmpCoin = GlobalSingleton.Coin;
        int tmp = GlobalSingleton.Coin;

        if (dst != 0)
        {
            if (coinTween != null)
            {
                coinTween.Kill();
                coinTween = null;
            }

            //SetEffect(dst);
            coinTween = DOTween.To(
                () => tmpCoin,
                (curV) =>
                {
                    txtValue.text = curV.ToString();
                },
                tmp,
                0.4f).OnComplete(() =>
                {
                    coinTween = null;
                });
            SetEffect(dst);
        }

        Sequence seq = DOTween.Sequence();
        seq.Append(txtValue.transform.DOScale(1.2f, 0.05f).SetEase(Ease.InCubic));
        seq.Append(txtValue.transform.DOScale(1f, 0.1f).SetEase(Ease.InCubic));


    }

    private void SetEffect(int dst)
    {
        if (Effect == null) return;

        var v = Mathf.Abs(dst);
        var effect = Effect.SpawnNewOne(transform);
        effect.gameObject.SetActive(true);
        effect.GetComponentInChildren<TextMeshProUGUI>().text = dst > 0 ? $"+{v.ToString()}" : $"-{v.ToString()}";

        var rt = effect.GetComponent<RectTransform>();
        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOScale(1.1f, 0.2f).SetEase(Ease.OutCubic));
        seq.Append(rt.DOScale(1f, 0.2f).SetEase(Ease.OutCubic));
        seq.AppendInterval(0.6f);
        seq.Append(rt.DoFadeAndAnchorsMoveTo(new Vector2(20, 20), 0.4f).SetEase(Ease.Linear));

        seq.OnComplete(() =>
        {
            Destroy(effect);
        });

    }
}
