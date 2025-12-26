using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GamePlay_LockSlot : MonoBehaviour
{
    public GameObject tipObj;
    public Image imaLock;
    //public Image imaFront;

    private Button self;

    private void Awake()
    {
        self = GetComponent<Button>();
        self.RegistBtnCallback(OnClicked);
    }

    public void ResetThis()
    {
        self.interactable = true;
        tipObj.gameObject.SetActive(true);
        imaLock.gameObject.SetActive(true);
        GetComponent<Image>().color = new Color(1, 1, 1, 0);
    }

    public void CallUnlock()
    {
        GetComponent<Image>().color = Color.white;
        self.interactable = false;
        imaLock.gameObject.SetActive(false);
        tipObj.gameObject.SetActive(false);
    }

    private async void OnClicked()
    {
        await GamePlay_ShelfGame.Instance.OnClickedUnlockTrain();
    }
}
