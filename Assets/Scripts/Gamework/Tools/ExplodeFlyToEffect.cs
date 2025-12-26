using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class ExplodeFlyToEffect
{
    public static async Task PlayEffect(Sprite[] spriteArr, int totalCnt, Vector3 startPos, Vector3 endPos, Transform parent, Action onHit)
    {
        List<Sprite> randomList = new List<Sprite>(spriteArr);
        Sprite GetSprite()
        {
            if (randomList.Count == 0)
            {
                randomList.AddRange(spriteArr);
            }

            randomList.TryRandomPopOne(out var result);
            return result;
        }

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.1f);
        for (int i = 0; i < totalCnt; i++)
        {
            var ima = ImageUtil.SpawnNew("ExplodeFlyToEffect");
            ima.transform.SetParent(parent);
            ima.transform.position = startPos;
            ima.transform.localScale = Vector3.one;
            ima.raycastTarget = false;
            ima.sprite = GetSprite();
            ima.SetNativeSize();
            var expDir = RandomHelp.InsideUnitCircleByRange(new RangeFloat(-1f, 1f), new RangeFloat(-0.1f, 1f)).normalized;
            var expDis = RandomHelp.RandomRange(0.2f, 1.5f);
            var expPos = ima.transform.position + (expDir * expDis).ToVector3_XY();
            var expH = RandomHelp.RandomRange(0.1f, 0.2f);

            var tempSeq = DOTween.Sequence();
            tempSeq.AppendInterval(0.05f + i * RandomHelp.RandomRange(0.02f, 0.05f));
            tempSeq.Append(ima.transform.DoSinLerpMove(expPos, 0.2f, expH).SetEase(Ease.OutCubic));
            tempSeq.AppendInterval(0.5f + i * 0.05f);
            tempSeq.Append(ima.transform.DoSinLerpMove(endPos, 0.4f, RandomHelp.RandomRange(-1f, 1f)).SetEase(Ease.OutCubic));
            tempSeq.AppendCallback(() =>
            {
                GameObject.Destroy(ima.gameObject);
                onHit?.Invoke();
            });
            seq.Join(tempSeq);
        }
        await seq.AsyncWaitForCompletion();
    }

    public static async Task PlayEffectByWorldPos(Sprite sprite, int totalCnt, Vector3 startPos, Vector3 endPos, Transform parent, Action onHit)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.1f);
        for (int i = 0; i < totalCnt; i++)
        {
            var ima = ImageUtil.SpawnNew("ExplodeFlyToEffect");
            ima.transform.SetParent(parent);
            ima.transform.position = startPos;
            ima.transform.localScale = Vector3.one;
            ima.raycastTarget = false;
            ima.sprite = sprite;
            ima.SetNativeSize();
            var expDir = RandomHelp.InsideUnitCircleByRange(new RangeFloat(-1f, 1f), new RangeFloat(-0.1f, 1f)).normalized;
            var expDis = RandomHelp.RandomRange(1f, 3);
            var expPos = ima.transform.position + (expDir * expDis).ToVector3_XY();
            var expH = RandomHelp.RandomRange(1f, 2f);

            var tempSeq = DOTween.Sequence();
            tempSeq.AppendInterval(0.05f + i * RandomHelp.RandomRange(0.02f, 0.05f));
            tempSeq.Append(ima.transform.DoSinLerpMove(expPos, 0.2f, expH).SetEase(Ease.OutCubic));
            tempSeq.AppendInterval(0.5f + i * 0.05f);
            tempSeq.Append(ima.transform.DoSinLerpMove(endPos, 0.4f, RandomHelp.RandomRange(-0.3f, 0.3f)).SetEase(Ease.OutCubic));
            tempSeq.AppendCallback(() =>
            {
                GameObject.Destroy(ima.gameObject);
                onHit?.Invoke();
            });
            seq.Join(tempSeq);
        }
        await seq.AsyncWaitForCompletion();
    }

    public static async Task PlayEffectByLocalPos(Sprite sprite, int totalCnt, Vector3 localPos, Vector3 endPos, Transform parent, Action onHit)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.1f);
        for (int i = 0; i < totalCnt; i++)
        {
            var ima = ImageUtil.SpawnNew("ExplodeFlyToEffect");
            ima.transform.SetParent(parent);
            ima.transform.localPosition = localPos;
            ima.transform.localScale = Vector3.one;
            ima.raycastTarget = false;
            ima.sprite = sprite;
            ima.SetNativeSize();
            var expDir = RandomHelp.InsideUnitCircleByRange(new RangeFloat(-1f, 1f), new RangeFloat(-0.1f, 1f)).normalized;
            var expDis = RandomHelp.RandomRange(0.1f, 0.3f);
            var expPos = ima.transform.position + (expDir * expDis).ToVector3_XY();
            var expH = RandomHelp.RandomRange(0.1f, 0.2f);

            var tempSeq = DOTween.Sequence();
            tempSeq.AppendInterval(0.05f + i * RandomHelp.RandomRange(0.02f, 0.05f));
            tempSeq.Append(ima.transform.DoSinLerpMove(expPos, 0.2f, expH).SetEase(Ease.OutCubic));
            tempSeq.AppendInterval(0.5f + i * 0.05f);
            tempSeq.Append(ima.transform.DoSinLerpMove(endPos, 0.4f, RandomHelp.RandomRange(-0.3f, 0.3f)).SetEase(Ease.OutCubic));
            tempSeq.AppendCallback(() =>
            {
                GameObject.Destroy(ima.gameObject);
                onHit?.Invoke();
            });
            seq.Join(tempSeq);
        }
        await seq.AsyncWaitForCompletion();
    }

}
