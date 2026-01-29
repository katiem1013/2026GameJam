using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceRec : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    private Rigidbody2D rb2D;

    public float jumpForce = 10f;

    private void Start()
    {
        actions.Add("boing", Jump);
        actions.Add("jump", Jump);

        actions.Add("back", Back);
        actions.Add("down", Back);

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognisedSpeech;
        keywordRecognizer.Start();

        rb2D = GetComponent<Rigidbody2D>();
    }

    private void RecognisedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void Jump()
    {
        rb2D.linearVelocity = new Vector2(0, jumpForce);
    }

    private void Back()
    {
        rb2D.linearVelocity = new Vector2(0.0f, -2.0f);
    }

}
