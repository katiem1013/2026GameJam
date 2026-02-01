using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    public GameObject deathMenu;
    public VoiceRec voiceRec;


    public void DeathMenu()
    {
        // brings up death menu
        deathMenu.SetActive(true);
    }

    public void Restart()
    {
        if(deathMenu.activeSelf){
        
            // clears old audio
            voiceRec.ClearAudio();

            // loads new scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);       } 
    }

    public void MainMenu()
    {
        if (deathMenu.activeSelf)
        {

            // clears old audio
            voiceRec.ClearAudio();

            // loads new scene
            SceneManager.LoadScene(0);
        }
    }
}
