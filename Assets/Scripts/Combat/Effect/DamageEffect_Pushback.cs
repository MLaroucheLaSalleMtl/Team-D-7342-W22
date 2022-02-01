using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mover), typeof(Health))]
public class DamageEffect_Pushback : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float distance;

    [SerializeField]
    private float moveDuration;

    private Mover mover;
    private Health health;

    private RaycastHit2D[] raycastHits = new RaycastHit2D[3];

    private void Awake()
    {
        mover = this.GetComponent<Mover>();
        health = this.GetComponent<Health>();

        if (health != null)
        {
            health.AddListener(this);
        }
    }

    public void OnDamaged(DamageInfo _damageInfo)
    {
        // Only do pushback when health is above zero
        if (_damageInfo.MinHP > 0)
        {
            StartCoroutine(MoveCoroutine((this.transform.position - _damageInfo.Causelocation).normalized));
        }
    }

    private IEnumerator MoveCoroutine(Vector2 direction)
    {
        Vector2 startPos = this.transform.position;
        Vector2 targetPosition = startPos + (direction * distance);

        float t = 0;

        while (t != moveDuration)
        {
            t = Mathf.MoveTowards(t, moveDuration, Time.deltaTime);

            mover.SetPosition(Vector3.Lerp(startPos, targetPosition, t / moveDuration));

            yield return null;
        }

    }
}
