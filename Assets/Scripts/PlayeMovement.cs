using UnityEngine;
using UnityEngine.Windows.Speech;

public class PlayeMovement : MonoBehaviour
{
    private Rigidbody2D rb2D;
    public float movementSpeed;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb2D.position += Vector2.right * movementSpeed;
    }
}
