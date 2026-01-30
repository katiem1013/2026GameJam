using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    private void RecognisedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void Start()
    {
        actions.Add("boing", Jump);
        actions.Add("jump", Jump);

        actions.Add("pow", Smash);
        actions.Add("smash", Smash);

        actions.Add("wall", WallJump);
        actions.Add("bounce", WallJump);
        actions.Add("hiyah", WallJump);

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognisedSpeech;
        keywordRecognizer.Start();

        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        explodable = FindClosestSmashableObj().GetComponent<Explodable>();
        print(explodable);

        // jumping and wall slide

        WallSlide();

        if (!isFacingRight && IsGrounded() && !isWallJumping && !isWallSliding)
        {
            isFacingRight = true;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

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


    // smash commands 
    private void Smash()
    {
        explodable.explode();
        Destroy(explodable.gameObject.GetComponent<BoxCollider2D>());
    }

    // smashing stuff
    public GameObject FindClosestSmashableObj()
    {
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
        rb2D.linearVelocity = new Vector2(0, jumpForce);
        WallJump();
    }

   
    private void WallJump()
    {

        if (wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb2D.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

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
