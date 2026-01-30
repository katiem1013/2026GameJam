using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float playerLives;
    public float maxPlayerLives;

    public bool takingDamage;

    public PlayeMovement playerMovement;
    public Death death;

    public void Start()
    {
        playerLives = maxPlayerLives;
    }

    public void Update()
    {
        playerMovement.currentAnimals = playerLives;

        // brings up restart screen
        if(playerLives <= 0)
        {
            death.DeathMenu();
        }
    }

    // takes away a life
    public void LoseLives(float livesLost)
    { 
        if(!takingDamage)
        {
            takingDamage = true;
            playerLives -= livesLost;
            print(playerLives);

            // delays player next damage
            StartCoroutine(TakeDamageAgain());
        }
            
       
    }

    // stops the player from instant dying on spikes
    public IEnumerator TakeDamageAgain()
    {
        yield return new WaitForSeconds(1);
        takingDamage = false;
    }


}
