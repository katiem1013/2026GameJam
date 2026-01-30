using UnityEngine;
using UnityEngine.Windows.Speech;

public class PlayeMovement : MonoBehaviour
{
    private Rigidbody2D rb2D;
    public float movementSpeed;

    public bool moving;

    private void Start()
    {
        moving = true;
        rb2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(moving)
            rb2D.position += Vector2.right * movementSpeed;
    }

    
}
