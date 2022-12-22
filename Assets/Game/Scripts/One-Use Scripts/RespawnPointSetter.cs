using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPointSetter : MonoBehaviour
{
    private DeathZonePush[] deathZonePushes;

    void Awake()
    {
        deathZonePushes = FindObjectsOfType<DeathZonePush>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var deathZonePush in deathZonePushes)
            {
                deathZonePush.spawnPosition = transform.position + Vector3.up;
            }
        }
    }
}