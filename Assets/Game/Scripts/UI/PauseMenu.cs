using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using JohnStairs.RCC.Character;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private RectTransform pauseMenuRectTransform;
    private DeathZonePush deathZonePush;
    private RPGCamera rpgCamera;
    private float baseSensitivity;
    private bool showingPauseMenu = false;
    private Bus globalVolumeBus;
    [SerializeField] private EventReference menuOpenSFX;
    [SerializeField] private EventReference menuCloseSFX;

    void Awake()
    {
        pauseMenuRectTransform = GetComponent<RectTransform>();
        deathZonePush = FindObjectOfType<DeathZonePush>();
        rpgCamera = FindObjectOfType<RPGCamera>();
        baseSensitivity = rpgCamera.RotationXSensitivity;
        globalVolumeBus = RuntimeManager.GetBus("bus:/");
    }

    public void DisplayPauseMenu()
    {
        showingPauseMenu = !showingPauseMenu;
        
        float targetPosition = showingPauseMenu ? 250f : 600f;
        Time.timeScale = showingPauseMenu ? 0f : 1f;
        Cursor.lockState = showingPauseMenu ? CursorLockMode.None : CursorLockMode.Locked;
        rpgCamera.enabled = !showingPauseMenu;
        RuntimeManager.PlayOneShot(showingPauseMenu ? menuOpenSFX : menuCloseSFX);
        
        pauseMenuRectTransform.DOAnchorPosY(targetPosition, 1f).SetEase(Ease.InOutSine).SetUpdate(true);
    }

    public void SetVolume(float value)
    {
        globalVolumeBus.setVolume(value);
    }

    public void SetSensitivity(float value)
    {
        rpgCamera.RotationXSensitivity = baseSensitivity * value * 2f;
        rpgCamera.RotationYSensitivity = baseSensitivity * value * 2f;
    }

    public void RespawnAtStartingPoint()
    {
        StartCoroutine(deathZonePush.RespawnAtOriginalPosition());
        DisplayPauseMenu();
    }

    public void QuitGame()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            Application.Quit();
    }
}