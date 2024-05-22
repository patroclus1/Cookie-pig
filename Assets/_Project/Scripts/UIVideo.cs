using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIVideo : MonoBehaviour
{
    [SerializeField] private GameObject container;

    [Header("Settings Options")]
    [SerializeField] private Slider qualitySlider;
    [SerializeField] private TMP_InputField fpsInput;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixerGroup generalVolume;

    private bool _defaultVsyncState = true;
    public static event Action OnSettingsOpen;

    private void Awake()
    {
        container.SetActive(false);
    }

    private void OnEnable()
    {
        TraderPig.OnShopOpen += DisableSettingsUI;
    }

    private void OnDisable()
    {
        TraderPig.OnShopOpen -= DisableSettingsUI;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (container.activeInHierarchy)
            {
                case true:
                    Cursor.lockState = CursorLockMode.Locked;
                    InputManager.Instance.EnableInput();
                    DisableSettingsUI();
                    break;

                case false:
                    EnableSettingsUI();
                    break;
            }
        }
    }

    private void EnableSettingsUI()
    {
        if (container.activeInHierarchy) return;
        OnSettingsOpen?.Invoke();
        InputManager.Instance.DisableInput();
        Cursor.lockState = CursorLockMode.None;
        container.SetActive(true);
    }

    private void DisableSettingsUI()
    {
        if (!container.activeInHierarchy) return;
        container.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SwitchVsync()
    {
        switch (_defaultVsyncState)
        {
            case true:
                QualitySettings.vSyncCount = 0;
                break;
            case false:
                QualitySettings.vSyncCount = 1;
                break;
        }
    }

    public void SlideGraphicsQuality()
    {
        QualitySettings.SetQualityLevel((int)qualitySlider.value);
    }

    public void SetTargetFPS()
    {
        Application.targetFrameRate = int.Parse(fpsInput.text);
    }

    public void SetGameVolume()
    {
        generalVolume.audioMixer.SetFloat("MasterVolume", volumeSlider.value);
        if (volumeSlider.value == volumeSlider.minValue) generalVolume.audioMixer.SetFloat("MasterVolume", -80);
    }
}
