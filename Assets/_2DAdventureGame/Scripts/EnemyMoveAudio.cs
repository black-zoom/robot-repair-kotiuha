using UnityEngine;

public class EnemyMoveAudio : MonoBehaviour
{
    public float minDistance = 1.5f;
    public float maxDistance = 8.0f;

    [Range(0f, 1f)]
    public float maxVolume = 0.8f;
    public AudioClip walkSound;
    public AudioClip fixSound;

    public AudioSource walkAudioSource;
    public AudioSource fixAudioSource;

    private EnemyController enemyController;
    private Transform playerTransform;
    private bool fixSoundPlayed = false;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }

        if (walkSound != null && walkAudioSource != null)
        {
            walkAudioSource.clip = walkSound;
            walkAudioSource.loop = true;
            walkAudioSource.spatialBlend = 0f;
            walkAudioSource.volume = 0f;
            walkAudioSource.Play();
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        float currentVolume = 0f;

        if (distance >= maxDistance)
        {
            currentVolume = 0f;
        }
        else if (distance <= minDistance)
        {
            currentVolume = maxVolume;
        }
        else
        {
            float t = (distance - minDistance) / (maxDistance - minDistance);
            currentVolume = Mathf.Lerp(maxVolume, 0f, t);
        }

        if (enemyController != null && !enemyController.isBroken)
        {
            if (walkAudioSource != null && walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }

            if (!fixSoundPlayed && fixAudioSource != null && fixSound != null)
            {
                fixAudioSource.volume = currentVolume;
                fixAudioSource.PlayOneShot(fixSound);
                fixSoundPlayed = true;
            }
            return;
        }

        if (walkAudioSource != null)
        {
            walkAudioSource.volume = currentVolume;
        }
    }
}
