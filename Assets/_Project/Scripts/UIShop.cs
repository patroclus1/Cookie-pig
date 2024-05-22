using System;
using System.Collections.Generic;
using UnityEngine;

public class UIShop : MonoBehaviour
{
    [SerializeField] private GameObject UIContainer;

    private List<GameObject> _deactivatedButtons;

    private void Awake()
    {
        _deactivatedButtons = new List<GameObject>();
        UIContainer.SetActive(false);
    }

    private void OnEnable()
    {
        TraderPig.OnShopOpen += OpenShop;
        Interactor.OnOutOfInteraction += CloseShop;
        UIVideo.OnSettingsOpen += CloseShop;
    }

    private void OnDisable()
    {
        TraderPig.OnShopOpen -= OpenShop;
        Interactor.OnOutOfInteraction -= CloseShop;
        UIVideo.OnSettingsOpen -= CloseShop;
    }

    public void OpenShop()
    {
        if (UIContainer.activeInHierarchy) return;
        Cursor.lockState = CursorLockMode.None;
        InputManager.Instance.DisableInput();
        UIContainer.SetActive(true);
    }

    private void CloseShop()
    {
        if (!UIContainer.activeInHierarchy) return;
        UIContainer.SetActive(false);
        InputManager.Instance.EnableInput();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void BuyCheckpoint(GameObject buttonToDeactivateAfterPurchase)
    {
        if (GameManager.Instance.ProcessPurchase(Abilities.Checkpoint, 10))
        {
            _deactivatedButtons.Add(buttonToDeactivateAfterPurchase);
            buttonToDeactivateAfterPurchase.SetActive(false);
            AudioManager.Instance.PlaySuccessSound();
        }
        else
        {
            AudioManager.Instance.PlayErrorSound();
        }
    }

    public void BuyDoubleJump(GameObject buttonToDeactivateAfterPurchase)
    {
        if (GameManager.Instance.ProcessPurchase(Abilities.Doublejump, 15))
        {
            _deactivatedButtons.Add(buttonToDeactivateAfterPurchase);
            buttonToDeactivateAfterPurchase.SetActive(false);
            AudioManager.Instance.PlaySuccessSound();
        }
        else
        {
            AudioManager.Instance.PlayErrorSound();
        }
    }

    public void BuySprint(GameObject buttonToDeactivateAfterPurchase)
    {
        if (GameManager.Instance.ProcessPurchase(Abilities.Sprint, 25))
        {
            _deactivatedButtons.Add(buttonToDeactivateAfterPurchase);
            buttonToDeactivateAfterPurchase.SetActive(false);
            AudioManager.Instance.PlaySuccessSound();
        }
        else
        {
            AudioManager.Instance.PlayErrorSound();
        }
    }

    public void BuyPlatform(GameObject buttonToDeactivateAfterPurchase)
    {
        if (GameManager.Instance.ProcessPurchase(Abilities.Platform, 5))
        {
            _deactivatedButtons.Add(buttonToDeactivateAfterPurchase);
            buttonToDeactivateAfterPurchase.SetActive(false);
            AudioManager.Instance.PlaySuccessSound();
        }
        else
        {
            AudioManager.Instance.PlayErrorSound();
        }
    }

    public void SellEverything()
    {
        foreach (var button in _deactivatedButtons)
        {
            button.gameObject.SetActive(true);
        }
        GameManager.Instance.ProcessTotalRefund();
    }
}
