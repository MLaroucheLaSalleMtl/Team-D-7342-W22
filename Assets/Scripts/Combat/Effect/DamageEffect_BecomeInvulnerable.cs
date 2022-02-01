using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DamageEffect_BecomeInvulnerable : MonoBehaviour, IDamageable
{
    [SerializeField, Tooltip("Time before the character becomes vulnerable again after getting hit")]
    protected float invulnerabilityTime = 0.5f;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
        health?.AddListener(this);
    }

    public void OnDamaged(DamageInfo _damageInfo)
    {
        health.SetInvulnerable(invulnerabilityTime);
    }
}
