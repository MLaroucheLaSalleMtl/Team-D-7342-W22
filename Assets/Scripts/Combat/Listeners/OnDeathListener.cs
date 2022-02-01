using UnityEngine;
using System.Collections;

public class OnDeathListener : MonoBehaviour, IKillable
{
    [SerializeField]
    private Health[] targets;

    [SerializeField]
    private float delay;

    public enum NotifyLocation { thisLocation, targetLocation };
    [SerializeField]
    private NotifyLocation location;

    [SerializeField]
    private UnityEventVector2 onDamaged;

    private void Awake()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].AddListener(this);
        }
    }

    public void OnDeath(Health health)
    {
        StartCoroutine(DispatchAction(health));
    }

    private IEnumerator DispatchAction(Health health)
    {
        yield return (delay == 0) ? null : new WaitForSeconds(delay);
        onDamaged.Invoke((location == NotifyLocation.thisLocation) ?
            this.transform.position : health.transform.position);
    }
}
