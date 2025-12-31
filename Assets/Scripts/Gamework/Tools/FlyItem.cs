using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlyItem : MonoBehaviour
{
    public event Action onDestroy;

    public Image imaAD;
    private Button btnClick;

    private float moveSpeed = 200f;
    private float timer = 0;
    private Vector2 moveDir;

    public TextMeshProUGUI txtM;
    private int rewardType;
    private RectTransform _rectTransform;

    private RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    private Canvas parentCanvas;

    private float PantographRatioX;
    private float PantographRatioY;

    private void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        var rt = parentCanvas.GetComponent<RectTransform>();
        PantographRatioX = rt.sizeDelta.x / Screen.width;
        PantographRatioY = rt.sizeDelta.y / Screen.height;

        if (txtM != null)
            txtM.gameObject.SetActive(false);
        btnClick = GetComponent<Button>();
        var tmpLs = new List<int> { 1, -1 };
        var x = RandomHelp.RandomSelectCollectionsElement(tmpLs);
        var pos = Vector2.zero;
        if (x == 1)
        {
            pos = new Vector2(Screen.width / 2 * PantographRatioX, Screen.height * PantographRatioY);
            moveDir = new Vector2(RandomHelp.RandomSelectCollectionsElement(tmpLs), -1).normalized;
        }
        else if (x == -1)
        {
            pos = new Vector2(Screen.width / 2 * PantographRatioX, 0);
            moveDir = new Vector2(RandomHelp.RandomSelectCollectionsElement(tmpLs), 1).normalized;
        }

        rectTransform.anchoredPosition = pos;

        btnClick.RegistBtnCallback(() =>
        {
            //UM.I.ReportEvent(UM.Module.GAME_FLY_CLICK);
            AudioManager.AudioPlayer.PlayOneShot(SoundName.Click);
            btnClick.ClickScaleAni(async () =>
            {
                if (isDestroy) return;
                isDestroy = true;
                if (rewardType == 0)
                {
                    await GetReward();
                    Destroy(gameObject);
                }
                else
                {
                    ShowADManager.PlayVideoAD("FlyItem", async (code, msg) =>
                    {
                        if (code > 0)
                        {
                            UIManager.ShowToast(msg);
                            onDestroy?.Invoke();
                            Destroy(gameObject);
                            return;
                        }
                        await GetReward();
                        Destroy(gameObject);
                    });
                }
            });
        });
    }

    private async Task GetReward()
    {

        var value = RandomHelp.RandomRange(1000, 1501);
        var ui = await UIManager.OpenUIAsync<PNL_ClaimPop>(new PNL_ClaimPopParam
        {
            reward = value
        });

        var result = await ui.WaitClose();

        _ = GlobalSingleton.AsyncGetReward(GameAssetType.SuperCoin, value, startPos: transform.position, 3);
    }

    bool isDestroy = false;

    private void Update()
    {
        if (isDestroy) return;
        float outTime = 60f;

        timer += Time.deltaTime;
        if (timer > 2f)
        {
            if (timer < outTime)
            {
                var pos = rectTransform.anchoredPosition;
                if (pos.x < 0)
                {
                    moveDir = new Vector2(1, moveDir.y).normalized;
                }
                else if (pos.x > Screen.width * PantographRatioX)
                {
                    moveDir = new Vector2(-1, moveDir.y).normalized;
                }

                if (pos.y < 0)
                {
                    moveDir = new Vector2(moveDir.x, 1).normalized;
                }
                else if (pos.y > Screen.height * PantographRatioY)
                {
                    moveDir = new Vector2(moveDir.x, -1).normalized;
                }

            }
            else if (timer > outTime + 5f)
            {
                onDestroy?.Invoke();
                Destroy(gameObject);
                return;
            }
        }
        rectTransform.anchoredPosition += moveSpeed * moveDir * Time.deltaTime;
    }

    public void SetType(int type)
    {
        rewardType = type;
        if (rewardType == 0)
        {
            imaAD.gameObject.SetActive(false);
        }
        else
        {
            imaAD.gameObject.SetActive(true);
        }
    }
}
