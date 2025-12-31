using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SecuredAllocation : MonoBehaviour
{
    public GameObject tipObj;
    public Image imaLock;
    //public Image imaFront;

    private Button self;

    private void Awake()
    {
        self = GetComponent<Button>();
        self.RegistBtnCallback(OnClickCallback);
    }

    public void ResetThis()
    {
        self.interactable = true;
        tipObj.gameObject.SetActive(true);
        imaLock.gameObject.SetActive(true);
        GetComponent<Image>().color = new Color(1, 1, 1, 0);
    }

    public void OnUnlockAction()
    {
        GetComponent<Image>().color = Color.white;
        self.interactable = false;
        imaLock.gameObject.SetActive(false);
        tipObj.gameObject.SetActive(false);
    }

    private async void OnClickCallback()
    {
        await MiniGame.Instance.OnClickedUnlockTrain();
    }
}
