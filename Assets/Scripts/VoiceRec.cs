using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows.Speech;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class VoiceRec : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    private Rigidbody2D rb2D;
    public float jumpForce = 10f;

    private bool isFacingRight = true;

    private bool isWallSliding;
    private float wallSlideSpeed;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.1f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.2f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    public Explodable explodable;

    private void Start()
    {
        actions.Add("boing", Jump);
        actions.Add("jump", Jump);

        actions.Add("pow", Smash);
        actions.Add("smash", Smash);

        actions.Add("wall", WallJump);
        actions.Add("bounce", WallJump);

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognisedSpeech;
        keywordRecognizer.Start();

        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        WallSlide();

        if (!isFacingRight && IsGrounded() && !isWallJumping && !isWallSliding)
        {
            isFacingRight = true;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
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

    private void Smash()
    {
        explodable.explode();
        Destroy(explodable.gameObject.GetComponent<BoxCollider2D>());
    }

    private void WallJump()
    {
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

        if(wallJumpingCounter > 0)
        {
            isWallJumping = true;
            rb2D.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0;

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
        return Physics2D.OverlapCircle(wallCheck.position, 1f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && IsGrounded())
        {
            isWallSliding = true;
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocityX, Mathf.Clamp(rb2D.linearVelocityY, -wallSlideSpeed, float.MaxValue));
        }

        else
            isWallSliding= false;
    }
}
