using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float playerLives;
    public float maxPlayerLives;

    public bool takingDamage;

    public PlayeMovement playerMovement;

    public void Start()
    {
        playerLives = maxPlayerLives;
    }

    public void Update()
    {
        playerMovement.currentAnimals = playerLives;
    }

    public void LoseLives(float livesLost)
    { 
        if(!takingDamage)
        {
            takingDamage = true;
            playerLives -= livesLost;
            print(playerLives);

            StartCoroutine(TakeDamageAgain());
        }
            
       
    }

    public IEnumerator TakeDamageAgain()
    {
        yield return new WaitForSeconds(2);
        takingDamage = false;
    }
}
