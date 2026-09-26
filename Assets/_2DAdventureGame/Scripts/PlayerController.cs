using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;

    private Rigidbody2D rigidbody2d;
    private Vector2 move;

    void Start()
    {
        MoveAction.Enable();

        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();
        Debug.Log(move);
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position + move * 3.0f * Time.fixedDeltaTime;

        rigidbody2d.MovePosition(position);
    }
}

