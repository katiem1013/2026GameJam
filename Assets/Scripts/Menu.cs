using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;

public class Menu : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    private void RecognisedSpeech(PhraseRecognizedEventArgs speech)
    {
        // listens for the speech and prints it in the console
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    public void ClearAudio()
    {
        // clears the old audio so it can be remade on a restart
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.OnPhraseRecognized -= RecognisedSpeech;
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
            // Crucial step to force a hard reset
            PhraseRecognitionSystem.Shutdown();
        }
    }

    private void Start()
    {
        // voice controls
        actions.Add("level", LevelSelect);
        actions.Add("quit", QuitGame);
        actions.Add("exit", QuitGame);

        actions.Add("level one", Level1);
        actions.Add("one", Level1);

        actions.Add("level two", Level2);
        actions.Add("two", Level2);

        actions.Add("level three", Level3);
        actions.Add("three", Level3);

        actions.Add("back", StartMenu);

        // creates the words to listen
        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognisedSpeech;
        keywordRecognizer.Start();
    }

    public GameObject startMenu;
    public GameObject levelSelectMenu;

    public void LevelSelect()
    {
        if(startMenu.activeSelf){
            startMenu.SetActive(false);
            levelSelectMenu.SetActive(true);
        }
    }

    public void StartMenu()
    {
        startMenu.SetActive(true);
        levelSelectMenu.SetActive(false);
    }
    
    public void Level1()
    {
        if (levelSelectMenu.activeSelf)
        {
            SceneManager.LoadScene(1);
        }
    }

    public void Level2()
    {
        if (levelSelectMenu.activeSelf)
        {
            SceneManager.LoadScene(2);
        }
    }

    public void Level3()
    {
        if (levelSelectMenu.activeSelf)
        {
            SceneManager.LoadScene(3);
        }
    }

    public void QuitGame()
    {
        if (startMenu.activeSelf)
        {
            Application.Quit();
        }
    }
}
