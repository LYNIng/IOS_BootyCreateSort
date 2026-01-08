using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FlyItemLogic
{
    public int FlyBoxCnt
    {
        get
        {
            return DataManager.GetDataByInt("FlyBoxCnt", 0);
        }
        set
        {
            DataManager.SetDataByInt("FlyBoxCnt", value);
        }
    }

    private int GetRewardType()
    {
        var c = FlyBoxCnt % 3;
        //Debug.Log($"GetRewardType FlyBoxCnt: {c}");
        if (c == 0)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }

    private float GetTimeLimit()
    {
        if (FlyBoxCnt == 0)
        {
            return 8f;
        }
        else
        {
            var c = FlyBoxCnt % 3;

            if (c == 0)
            {
                return RandomHelp.RandomRange(50f, 60f);
            }
            else if (c == 1)
            {
                return RandomHelp.RandomRange(50f, 60f);
            }
            else
            {
                return RandomHelp.RandomRange(80f, 90f);
            }
        }
    }


    public FlyItemLogic(string prefabPath)
    {
        AssetsManager.LoadAsset<GameObject>(prefabPath, (result) =>
        {
            flyBoxPrefab = result;
        });

        ResetTimer();
        //FlyBoxCnt = 0;

    }

    private GameObject flyBoxPrefab;
    private float flyBoxTimer;
    private int rewardType;
    private FlyItem flyBox;
    private float timerLimit;
    public void Update()
    {
        if (AppExcuteFlagSettings.ToAFlag && AppExcuteFlagSettings.ToBFlag && flyBoxPrefab != null)
        {
            if (flyBox == null)
            {
                flyBoxTimer += Time.deltaTime;
                if (flyBoxTimer > timerLimit)
                {
                    //UM.I.ReportEvent(UM.Module.GAME_FLY_SHOW);
                    var go = Object.Instantiate(flyBoxPrefab, UIManager.GetCanvasLayerTransform(UICanvasLayer.Background_Camera));
                    flyBox = go.GetComponent<FlyItem>();
                    flyBox.SetType(rewardType);
                    flyBox.onDestroy += () =>
                    {

                        flyBox = null;
                    };
                    FlyBoxCnt++;
                    ResetTimer();
                }
            }

        }
    }

    public void Destroy()
    {
        //Debug.Log($"Call destroy");
        if (flyBox != null)
        {
            GameObject.Destroy(flyBox.gameObject);
            flyBox = null;
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        flyBoxTimer = 0f;
        timerLimit = GetTimeLimit();
        rewardType = GetRewardType();
    }

}
