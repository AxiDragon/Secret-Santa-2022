using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeathZonePush : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float touchVelocity;
    [SerializeField] private Transform spawnPoint;
    [HideInInspector] public Vector3 spawnPosition;
    private CharacterController playerCharacterController;

    private void Awake()
    {
        playerCharacterController = GameObject.FindWithTag("Player").GetComponent<CharacterController>();
        spawnPosition = spawnPoint.position;
        canvasGroup.alpha = 1f;
        canvasGroup.DOFade(0f, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterController characterController))
        {
            // Respawn
            StartCoroutine(Respawn(characterController));
        }

        if (other.TryGetComponent(out Rigidbody rb))
        {
            rb.velocity = new Vector3(rb.velocity.x, touchVelocity, rb.velocity.z);
        }
    }

    private IEnumerator Respawn(CharacterController characterController)
    {
        canvasGroup.DOFade(1f, .5f).SetEase(Ease.InSine);
        yield return new WaitForSeconds(1.5f);
        canvasGroup.DOFade(0f, .5f).SetEase(Ease.OutSine);
        characterController.enabled = false;
        characterController.transform.position = spawnPosition;
        characterController.enabled = true;
    }

    public IEnumerator RespawnAtOriginalPosition()
    {
        canvasGroup.DOFade(1f, .5f).SetEase(Ease.InSine);
        yield return new WaitForSeconds(1.5f);
        canvasGroup.DOFade(0f, .5f).SetEase(Ease.OutSine);
        playerCharacterController.enabled = false;
        playerCharacterController.transform.position = spawnPoint.position;
        playerCharacterController.enabled = true;
    }
}