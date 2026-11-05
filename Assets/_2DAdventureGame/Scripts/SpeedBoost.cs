using System.Collections;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float boostFactor = 1.5f;
    public float boostTime = 5.0f;

    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null || isCollected)
        {
            return;
        }

        isCollected = true;

        StartCoroutine(BoostPlayer(player));
    }

    IEnumerator BoostPlayer(PlayerController player)
    {
        float baseSpeed = player.speed;

        player.speed = baseSpeed * boostFactor;

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(boostTime);

        player.speed = baseSpeed;

        Destroy(gameObject);
    }
}

