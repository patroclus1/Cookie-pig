using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private TMP_Text _uiText;
    private int _currentCookies = 0;
    private int _totalCookiesSpent = 0;

    [HideInInspector] public bool isCheckpointPurchased, isDoublejumpPurchased, isSprintPurchased, isPlatformPurchased;
    [SerializeField] private GameObject checkpointPrompt, doublejumpPrompt, sprintPrompt, platformPrompt;
    [SerializeField] private int startingCookies = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _uiText = GetComponentInChildren<TMP_Text>();
        AddCookiesToCount(startingCookies);
    }

    private void OnEnable()
    {
        Cookie.OnCookieCollect += AddCookiesToCount;
    }

    private void OnDisable()
    {
        Cookie.OnCookieCollect -= AddCookiesToCount;

    }

    private void AddCookiesToCount(int amount)
    {
        _currentCookies += amount;
        _uiText.text = $"Cookies: {_currentCookies}";
    }

    private void RemoveCookiesFromCount(int amount)
    {
        _totalCookiesSpent += amount;
        _currentCookies -= amount;
        _uiText.text = $"Cookies: {_currentCookies}";
    }

    public bool ProcessPurchase(Abilities purhasedItem, int price)
    {
        if (_currentCookies >= price) 
        {
            RemoveCookiesFromCount(price);

            switch (purhasedItem)
            {
                case Abilities.Checkpoint: isCheckpointPurchased = true;
                    checkpointPrompt.SetActive(true);
                    break;
                case Abilities.Doublejump: isDoublejumpPurchased = true;
                    doublejumpPrompt.SetActive(true);
                    break;
                case Abilities.Sprint: isSprintPurchased = true;
                    sprintPrompt.SetActive(true);
                    break;
                case Abilities.Platform: isPlatformPurchased = true;
                    platformPrompt.SetActive(true);
                    break;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void ProcessTotalRefund()
    {
        isCheckpointPurchased = false;
        isDoublejumpPurchased = false;
        isSprintPurchased = false;
        isPlatformPurchased = false;

        AddCookiesToCount(_totalCookiesSpent);
        _totalCookiesSpent = 0;
    }
}
