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


}
