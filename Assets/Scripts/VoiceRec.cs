using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class VoiceRec : MonoBehaviour
{
    public PlayeMovement playeMovement;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    private Rigidbody2D rb2D;
    public float jumpForce = 10f;

    private bool isFacingRight = true;

    private bool isWallSliding;
    private float wallSlideSpeed;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(4f, 8f);

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    public Explodable explodable;
    public Death death;

    public AudioSource audioSource;
    public AudioClip jump, smash;

    public GameObject levelCompelete;

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

    private void Awake()
    {
        ClearAudio();
    }

    private void Start()
    {
        // voice controls
        actions.Add("boing", Jump);
        actions.Add("jump", Jump);

        actions.Add("pow", Smash);
        actions.Add("smash", Smash);

        actions.Add("wall", WallJump);
        actions.Add("bounce", WallJump);
        actions.Add("hiyah", WallJump);

        actions.Add("restart", death.Restart);
        actions.Add("menu", death.MainMenu);

        actions.Add("finished", ReturnToMenu);

        // creates the words to listen
        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognisedSpeech;
        keywordRecognizer.Start();

        rb2D = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // looks for the closest smashable obj if there is one
        if(FindClosestSmashableObj()!= null)
            explodable = FindClosestSmashableObj().GetComponent<Explodable>();

        // jumping and wall slide
        WallSlide();

        // makes the player face left when on the ground
        if (!isFacingRight && IsGrounded() && !isWallJumping && !isWallSliding)
        {
            isFacingRight = true;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

        // allows the jump off the wall if wall jumping
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }

        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
    }

    public void ReturnToMenu()
    {
        if (levelCompelete.activeSelf)
        {
            SceneManager.LoadScene(0);
        }
    }

    // smash commands 
    private void Smash()
    {
        explodable.explode();
        audioSource.clip = smash;
        audioSource.Play();
        Destroy(explodable.gameObject.GetComponent<BoxCollider2D>());
    }

    // smashing stuff
    public GameObject FindClosestSmashableObj()
    {
        // finds the cloest smashable object
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("BreakableObj");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    // jump commands

    private void Jump()
    {
        // adds to the velocity to make the character jump
        if (IsGrounded())
        {
            rb2D.linearVelocity = new Vector2(0, jumpForce);
            WallJump();

            audioSource.clip = jump;
            audioSource.Play();
        }
        
    }

    private void WallJump()
    {
        // character bounces off the wall
        if (wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb2D.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            // changes the direction to the opposite
            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    // more jumping stuff

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        // causes the player to wall slide when against the wall 
        if (IsWalled() && !IsGrounded())
        {
            playeMovement.moving = false;
            isWallSliding = true;
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, Mathf.Clamp(rb2D.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }

        else{
            isWallSliding = false;
            playeMovement.moving = true;
        }
    }
}
