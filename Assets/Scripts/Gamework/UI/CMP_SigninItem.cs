using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class CMP_SigninItem : MonoComposite
{
    private Action<int> onClickCallback;
    private int index = 0;
    public void Set(int idx, Action<int> onClick, PNL_Signin sigin)
    {
        onClickCallback = onClick;
        index = idx;
        var state = SignInRecordManager.Instance.GetSignInState(index);
        var rewards = GlobalSingleton.GetSigninRewards();
        var item = rewards[index];
        if (state == 2)
        {
            imaGreen.gameObject.SetActive(false);
            imaGrey.gameObject.SetActive(true);
            imaRed.gameObject.SetActive(false);
            btnClick.ClearAllBtnCallback();

            imaGrey.transform.Find("txtTitle").GetComponent<TextMeshProUGUI>().text = $"Day{index + 1}";
            if (index < 6)
            {
                if (sigin.TryGetSprite(item.assetType, out Sprite result))
                {
                    imaGrey.transform.Find("imaIcon").GetComponent<Image>().SetSprite(result).SetNativeSize();
                }
                imaGrey.transform.Find("txtVal").GetComponent<TextMeshProUGUI>().text = item.rewardCnt.ToNumText(item.assetType);
            }
            else
            {
                var item1 = rewards[6];
                var item2 = rewards[7];
                var item3 = rewards[8];
                var item4 = rewards[9];

                if (sigin.TryGetSprite(item1.assetType, out Sprite result))
                {
                    imaGrey.transform.Find("Contents/imaIcon_1").GetComponent<Image>().SetSprite(result).SetNativeSize();
                    imaGrey.transform.Find("Contents/imaIcon_1/txtVal").GetComponent<TextMeshProUGUI>().text = item1.rewardCnt.ToNumText(item1.assetType);
                }

                if (sigin.TryGetSprite(item2.assetType, out result))
                {
                    imaGrey.transform.Find("Contents/imaIcon_2").GetComponent<Image>().SetSprite(result).SetNativeSize();
                    imaGrey.transform.Find("Contents/imaIcon_2/txtVal").GetComponent<TextMeshProUGUI>().text = item2.rewardCnt.ToNumText(item2.assetType);
                }

                if (sigin.TryGetSprite(item3.assetType, out result))
                {
                    imaGrey.transform.Find("Contents/imaIcon_3").GetComponent<Image>().SetSprite(result).SetNativeSize();
                    imaGrey.transform.Find("Contents/imaIcon_3/txtVal").GetComponent<TextMeshProUGUI>().text = item3.rewardCnt.ToNumText(item3.assetType);
                }

                if (sigin.TryGetSprite(item4.assetType, out result))
                {
                    imaGrey.transform.Find("Contents/imaIcon_4").GetComponent<Image>().SetSprite(result).SetNativeSize();
                    imaGrey.transform.Find("Contents/imaIcon_4/txtVal").GetComponent<TextMeshProUGUI>().text = item4.rewardCnt.ToNumText(item4.assetType);
                }
            }
        }
        else if (state == 1)
        {
            imaGreen.gameObject.SetActive(true);
            imaGrey.gameObject.SetActive(false);
            imaRed.gameObject.SetActive(false);
            btnClick.ClearAllBtnCallback();
            btnClick.RegistBtnCallback(() =>
            {
                onClickCallback?.Invoke(index);
            });

            imaGreen.transform.Find("txtTitle").GetComponent<TextMeshProUGUI>().text = $"Day{index + 1}";
            if (index < 6)
            {
                if (sigin.TryGetSprite(item.assetType, out Sprite result))
                {
                    imaGreen.transform.Find("imaIcon").GetComponent<Image>().SetSprite(result).SetNativeSize();
                }
                imaGreen.transform.Find("txtVal").GetComponent<TextMeshProUGUI>().text = item.rewardCnt.ToNumText(item.assetType);

            }
            else
            {
                var item1 = rewards[6];
                var item2 = rewards[7];
                var item3 = rewards[8];
                var item4 = rewards[9];

                if (sigin.TryGetSprite(item1.assetType, out Sprite result))
                {
                    imaGreen.transform.Find("Contents/imaIcon_1").GetComponent<Image>().SetSprite(result).SetNativeSize();
                    imaGreen.transform.Find("Contents/imaIcon_1/txtVal").GetComponent<TextMeshProUGUI>().text = item1.rewardCnt.ToNumText(item1.assetType);
                }

                if (sigin.TryGetSprite(item2.assetType, out result))
                {
                    imaGreen.transform.Find("Contents/imaIcon_2").GetComponent<Image>().SetSprite(result).SetNativeSize();
                    imaGreen.transform.Find("Contents/imaIcon_2/txtVal").GetComponent<TextMeshProUGUI>().text = item2.rewardCnt.ToNumText(item2.assetType);
                }

                if (sigin.TryGetSprite(item3.assetType, out result))
                {
                    imaGreen.transform.Find("Contents/imaIcon_3").GetComponent<Image>().SetSprite(result).SetNativeSize();
                    imaGreen.transform.Find("Contents/imaIcon_3/txtVal").GetComponent<TextMeshProUGUI>().text = item3.rewardCnt.ToNumText(item3.assetType);
                }

                if (sigin.TryGetSprite(item4.assetType, out result))
                {
                    imaGreen.transform.Find("Contents/imaIcon_4").GetComponent<Image>().SetSprite(result).SetNativeSize();
                    imaGreen.transform.Find("Contents/imaIcon_4/txtVal").GetComponent<TextMeshProUGUI>().text = item4.rewardCnt.ToNumText(item4.assetType);
                }
            }
        }
        else
        {
            imaGreen.gameObject.SetActive(false);
            imaGrey.gameObject.SetActive(false);
            imaRed.gameObject.SetActive(true);
            btnClick.ClearAllBtnCallback();

            imaRed.transform.Find("txtTitle").GetComponent<TextMeshProUGUI>().text = $"Day{index + 1}";
            if (index < 6)
            {
                if (sigin.TryGetSprite(item.assetType, out Sprite result))
                {
                    imaRed.transform.Find("imaIcon").GetComponent<Image>().SetSprite(result).SetNativeSize();
                }
                imaRed.transform.Find("txtVal").GetComponent<TextMeshProUGUI>().text = item.rewardCnt.ToNumText(item.assetType);
            }
            else
            {
                var item1 = rewards[6];
                var item2 = rewards[7];
                var item3 = rewards[8];
                var item4 = rewards[9];

                if (sigin.TryGetSprite(item1.assetType, out Sprite result))
                {
                    imaRed.transform.Find("Contents/imaIcon_1").GetComponent<Image>().SetSprite(result).SetNativeSize();
                    imaRed.transform.Find("Contents/imaIcon_1/txtVal").GetComponent<TextMeshProUGUI>().text = item1.rewardCnt.ToNumText(item1.assetType);
                }

                if (sigin.TryGetSprite(item2.assetType, out result))
                {
                    imaRed.transform.Find("Contents/imaIcon_2").GetComponent<Image>().SetSprite(result).SetNativeSize();
                    imaRed.transform.Find("Contents/imaIcon_2/txtVal").GetComponent<TextMeshProUGUI>().text = item2.rewardCnt.ToNumText(item2.assetType);
                }

                if (sigin.TryGetSprite(item3.assetType, out result))
                {
                    imaRed.transform.Find("Contents/imaIcon_3").GetComponent<Image>().SetSprite(result).SetNativeSize();
                    imaRed.transform.Find("Contents/imaIcon_3/txtVal").GetComponent<TextMeshProUGUI>().text = item3.rewardCnt.ToNumText(item3.assetType);
                }

                if (sigin.TryGetSprite(item4.assetType, out result))
                {
                    imaRed.transform.Find("Contents/imaIcon_4").GetComponent<Image>().SetSprite(result).SetNativeSize();
                    imaRed.transform.Find("Contents/imaIcon_4/txtVal").GetComponent<TextMeshProUGUI>().text = item4.rewardCnt.ToNumText(item4.assetType);
                }
            }
        }
    }


}
