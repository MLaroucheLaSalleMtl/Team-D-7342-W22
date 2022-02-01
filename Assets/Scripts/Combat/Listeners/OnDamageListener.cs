using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDamageListener : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Health[] targets;

    [SerializeField]
    private float delay;

    [SerializeField]
    private UnityEventVector2 onDamaged;

    [SerializeField, Tooltip("The current health in percentage (1.0 = 100%)")]
    private UnityEventFloat onHealthChanged;

    private void Awake()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].AddListener(this);
        }
    }

    public void OnDamaged(DamageInfo _damageInfo)
    {
        StartCoroutine(DispatchAction(_damageInfo.Causelocation, _damageInfo));
    }

    private IEnumerator DispatchAction (Vector2 location, DamageInfo info)
    {
        yield return (delay == 0) ? null : new WaitForSeconds(delay);
        onDamaged.Invoke(location);
        onHealthChanged.Invoke((info.MinHP >0 )? (info.MinHP / info.MaxHP) : 0);
    }
}
