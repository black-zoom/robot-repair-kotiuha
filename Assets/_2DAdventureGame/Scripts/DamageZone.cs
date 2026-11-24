using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerController controller))
        {
            controller.ChangeHealth(-damage);
        }
    }
}
