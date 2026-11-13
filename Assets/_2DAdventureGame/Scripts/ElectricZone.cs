using UnityEngine;

public class ElectricZone : MonoBehaviour
{
    public int damage = 1;

    public float waitTime = 2.5f;
    public float activeTime = 2f;

    public Sprite idleSprite;
    public Sprite activeSprite;

    private SpriteRenderer spriteRenderer;
    private bool isActive = false;
    private float timer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = idleSprite;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!isActive && timer >= waitTime)
        {
            isActive = true;
            timer = 0f;
            spriteRenderer.sprite = activeSprite;
        }
        else if (isActive && timer >= activeTime)
        {
            isActive = false;
            timer = 0f;
            spriteRenderer.sprite = idleSprite;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isActive) return;

        PlayerController controller =
            other.GetComponent<PlayerController>();

        if (controller != null)
        {
            controller.ChangeHealth(-damage);
        }
    }
}
