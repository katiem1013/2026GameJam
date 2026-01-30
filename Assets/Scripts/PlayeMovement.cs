using UnityEngine;
using UnityEngine.Windows.Speech;

public class PlayeMovement : MonoBehaviour
{
    private Rigidbody2D rb2D;
    public float movementSpeed;

    public bool moving;

    public float currentAnimals;
    public Animator animator;

    void Start()
    {

        currentAnimals = 4;
        moving = true;
        rb2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {

        if(moving)
            rb2D.position += Vector2.right * movementSpeed;
    }
    
    void Update()
    {
        GetComponent<Animator>().SetTrigger(GetCurrentAnimal());
    }

    public string GetCurrentAnimal()
    {
        string currentAnim = currentAnimals.ToString() + "Anim";
        return currentAnim;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike") || collision.gameObject.CompareTag("BreakableObj"))
        {
            // Calculate the direction away from the wall
            Vector2 pushDirection = (transform.position - collision.transform.position).normalized;

            // Apply force to the Rigidbody2D
            float pushForce = 5f;
            GetComponent<Rigidbody2D>().AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
        }
    }

}
