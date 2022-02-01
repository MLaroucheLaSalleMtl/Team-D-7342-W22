using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mover), typeof(Aimer), typeof(BoxCollider2D))]
public class Enemy_WanderAttacker : MonoBehaviour, IDamageable, IKillable
{
    [SerializeField]
    private ScriptableReference playerReference;

    [SerializeField]
    private float updateCooldown;

    [SerializeField]
    private float destinationCooldown = 1.5f;

    private float currentUpdateCooldown;

    [SerializeField, Tooltip("At what distance should we start going after the player?")]
    private float aggroDistance = 1;

    [SerializeField, Tooltip("What is the total radius range the monster should wander?")]
    private float wanderRadius = 1;

    private Mover mover;
    private SpriteRenderer spriteRenderer;

    private Vector2 targetLocation;
    private Vector2 startPosition;
    private Animator animator;

    private bool followingPlayer;

    [SerializeField]
    private float hitPauzeTime = 1;

    private float currentHitPauzeTime;

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, aggroDistance);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(this.transform.position, wanderRadius);
    }

#endif

    private void Awake()
    {
        startPosition = this.transform.position;
        mover = GetComponent<Mover>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        Health healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.AddListener((IDamageable)this);
            healthComponent.AddListener((IKillable)this);
        }

    }

    private void Update()
    {
        if (currentHitPauzeTime > 0)
        {
            currentHitPauzeTime -= Time.deltaTime;
            return;
        }

        if (targetLocation != Vector2.zero && Vector2.Distance((Vector2)this.transform.position, targetLocation) > 0.03)
        {
            Vector2 moveDirection = targetLocation - (Vector2)this.transform.position;

            mover.Move(moveDirection);

            spriteRenderer.flipX = moveDirection.x > 0;

            animator.Play("Move");
        }
        else
        {
            animator.Play("Idle");
        }

        if (currentUpdateCooldown > 0)
        {
            currentUpdateCooldown -= Time.deltaTime;
            return;
        }
        else
        {
            currentUpdateCooldown = updateCooldown;
        }

        if (IsPlayerInAggroRange())
        {
            targetLocation = playerReference.Reference.transform.position;
        }
        else
        {
            if ((Vector2)this.transform.position == startPosition || Vector2.Distance((Vector2)this.transform.position, targetLocation) < 0.3)
            {
                Vector3 randPos = UnityEngine.Random.insideUnitSphere * wanderRadius;

                targetLocation = startPosition + (Vector2)randPos;

                currentUpdateCooldown = destinationCooldown;
            }
        }
    }

    private bool IsPlayerInAggroRange()
    {
        if (playerReference?.Reference?.transform == null)
        {
            return false;
        }

        float distanceToPlayer = Vector2.Distance(playerReference.Reference.transform.position, this.transform.position);

        return distanceToPlayer < aggroDistance;
    }

    public void OnDamaged(DamageInfo _damageInfo)
    {
        currentHitPauzeTime = hitPauzeTime;
    }

    public void OnDeath(Health health)
    {
        this.enabled = false;
    }
}
