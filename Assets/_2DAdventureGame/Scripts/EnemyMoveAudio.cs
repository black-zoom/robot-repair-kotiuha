using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyMoveAudio : MonoBehaviour
{
    public float minDistance = 1.5f;
    public float maxDistance = 8.0f;

    [Range(0f, 1f)]
    public float maxVolume = 0.8f;
    public AudioClip walkSound;

    private AudioSource audioSource;
    private EnemyController enemyController;
    private Transform playerTransform;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        enemyController = GetComponent<EnemyController>();

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }

        if (walkSound != null)
        {
            audioSource.clip = walkSound;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 0f;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (playerTransform == null || walkSound == null) return;

        if (enemyController != null && !enemyController.isBroken)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            return;
        }

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance >= maxDistance)
        {
            audioSource.volume = 0f;
        }
        else if (distance <= minDistance)
        {
            audioSource.volume = maxVolume;
        }
        else
        {
            float t = (distance - minDistance) / (maxDistance - minDistance);
            audioSource.volume = Mathf.Lerp(maxVolume, 0f, t);
        }
    }
}
