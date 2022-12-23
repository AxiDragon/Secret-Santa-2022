using System;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using TMPro;
using FMODUnity;
using UnityEngine;
using UnityTimer;

public class BuildingCollectionTracker : MonoBehaviour
{
    private List<Building> collectedBuildings = new();
    private TextMeshProUGUI collectedBuildingsText;
    [SerializeField] private RectTransform title;
    [SerializeField] private TextMeshProUGUI collectedBuildingsTextPauseMenu;
    private bool titleDisplayed = false;

    [SerializeField] private EventReference soundTrackEventReference;
    [SerializeField] private EventReference introJingle;
    private EventInstance soundTrackInstance;

    void Awake()
    {
        collectedBuildingsText = GetComponentInChildren<TextMeshProUGUI>();
        soundTrackInstance = RuntimeManager.CreateInstance(soundTrackEventReference);
        soundTrackInstance.start();
    }

    public void AttemptAddBuildingToCollection(Building addedBuilding)
    {
        if (!collectedBuildings.Contains(addedBuilding))
        {
            collectedBuildings.Add(addedBuilding);
            UpdateBuildingCollection();
        }
    }

    private void UpdateBuildingCollection()
    {
        collectedBuildingsText.text = $"buildings discovered: {collectedBuildings.Count} | 10";
        collectedBuildingsTextPauseMenu.text = $"{collectedBuildings.Count} | 10";
        
        DisplayCollectionText(true);
        Timer.Register(2f, () => { DisplayCollectionText(false); });
        soundTrackInstance.setParameterByName("BuildingsCollected", collectedBuildings.Count);

        if (!titleDisplayed)
            ShowTitle();
    }

    public void DisplayCollectionText(bool show)
    {
        float targetPosition = show ? 50f : -200f;
        transform.DOMoveY(targetPosition, 1f).SetEase(Ease.InOutSine);
    }

    private void ShowTitle()
    {
        titleDisplayed = true;


        title.DOAnchorPosY(-600f, 2f).SetEase(Ease.InOutSine).OnComplete(() => RuntimeManager.PlayOneShot(introJingle));
        Timer.Register(5f, () => { title.DOAnchorPosY(150f, 2f).SetEase(Ease.InOutSine); });
    }
}