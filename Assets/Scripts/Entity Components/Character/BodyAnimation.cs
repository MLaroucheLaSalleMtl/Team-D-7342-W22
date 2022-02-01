using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Farming Kit/Entity Components/Body/Animation")]
[RequireComponent(typeof(Animator))]
public class BodyAnimation : MonoBehaviour, IMove, IAim
{
    protected Animator animator;

    protected int directionXID;
    protected int directionYID;
    protected int velocityID;
    protected int useSpeedID;
    protected int triggerFinishedAction;

    protected Vector2 lastDirection;
    protected float lastVelocity;

    private bool initialized = false;

    [SerializeField]
    private SoundCollection walkingSteps;
    private AudioSource audioSource;

    [SerializeField]
    private SpriteRenderer toolSprite;

    public void OnStep()
    {
        if (walkingSteps != null)
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            walkingSteps.Play(audioSource);
        }
    }

    private void Initialize()
    {
        animator = GetComponent<Animator>();

        directionXID = Animator.StringToHash("Direction X");
        directionYID = Animator.StringToHash("Direction Y");
        velocityID = Animator.StringToHash("Velocity");
        useSpeedID = Animator.StringToHash("Use Speed");
        triggerFinishedAction = Animator.StringToHash("Finished Action");

        initialized = true;
    }

    public float ApplySmashAnimation(float speed, Sprite sprite)
    {
        PlayAnimation("Smash", speed, sprite);

        // For simplicity, all action animations are set to be 1 seconds within the animation tab.
        return 1 / speed;
    }

    public float ApplySlashAnimation(float speed, Sprite sprite)
    {
        PlayAnimation("Slash", speed, sprite);

        // For simplicity, all action animations are set to be 1 seconds within the animation tab.
        return 1 / speed;
    }

    public float ApplyDropAnimation(float speed, Sprite sprite)
    {
        PlayAnimation("Drop", speed, sprite);

        // For simplicity, all action animations are set to be 1 seconds within the animation tab.
        return 1 / speed;
    }

    private void PlayAnimation (string animationName, float speed, Sprite sprite)
    {
        animator.SetFloat(useSpeedID, speed);
        animator.PlayInFixedTime(animationName);
        animator.SetFloat(velocityID, 0);

        if (toolSprite != null)
        {
            toolSprite.sprite = sprite;
        }
    }

    public void OnMove(Vector2 direction, float velocity)
    {
        UpdateAnimationParameters(direction, velocity);
    }

    public void OnAim(Vector2 direction)
    {
        UpdateAnimationParameters(direction, 0);
    }

    private void UpdateAnimationParameters(Vector2 direction, float velocity)
    {
        if (!initialized)
        {
            Initialize();
        }

        if (direction.sqrMagnitude > 0)
        {
            if (Mathf.Abs(direction.x) == Mathf.Abs(direction.y))
            {
                direction.y = 0;
            }

            lastDirection = direction.normalized;
        }

        animator.SetFloat(directionXID, lastDirection.x);
        animator.SetFloat(directionYID, lastDirection.y);
        animator.SetFloat(velocityID, velocity);

        lastVelocity = velocity;
    }
}
