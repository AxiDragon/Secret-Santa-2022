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

    private void Awake()
    {
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
}