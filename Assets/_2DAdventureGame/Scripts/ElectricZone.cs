using UnityEngine;

public class ElectricZone : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 1;

    [Header("Timing")]
    public float waitTime = 2.5f;
    public float activeTime = 2f;

    [Header("Visuals")]
    public Sprite idleSprite;
    public Sprite activeSprite;

    private SpriteRenderer spriteRenderer;
    private bool isActive;
    private float timer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetActiveState(false);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float currentDuration = isActive ? activeTime : waitTime;
        if (timer >= currentDuration)
        {
            SetActiveState(!isActive);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isActive)
        {
            return;
        }

        if (other.TryGetComponent(out PlayerController controller))
        {
            controller.ChangeHealth(-damage);
        }
    }

    private void SetActiveState(bool active)
    {
        isActive = active;
        timer = 0f;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isActive ? activeSprite : idleSprite;
        }
    }
}
