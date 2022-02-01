using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Farming Kit/Entity Components/Combat/Damage Volume")]
public class DamageVolume : MonoBehaviour
{
    [SerializeField]
    private new BoxCollider2D collider;

    [SerializeField]
    private string[] damageableTags;

    [SerializeField]
    private float damage;

    private List<int> attackedInstances = new List<int>();

    private IDamageCallback callbackInterface;

    protected bool canDealDamage = true;

    [SerializeField]
    private bool canDoDuplicateDamage = false;

    [SerializeField]
    private bool canDamageMultiple = true;

    public GameObject owner;

    public void Configure(DamageVolumeConfiguration config)
    {
        owner = config.Owner;
        damage = config.Damage;
        damageableTags = config.TargetTags;
        collider.size = new Vector2(config.Size.x, config.Size.y);
        canDoDuplicateDamage = config.AllowDuplicateDamage;
        canDamageMultiple = config.CanDamageMultiple;

        attackedInstances.Clear();
        canDealDamage = true;

        this.gameObject.SetActive(true);

        if (config.ActiveTime != 0)
        {
            StartCoroutine(DisableDamageAfterTime(config.ActiveTime));
        }
    }

    public void OnEnable()
    {
        attackedInstances.Clear();
    }

    public void SetCallBack(IDamageCallback _callbackInterface)
    {
        callbackInterface = _callbackInterface;
    }

    protected IEnumerator DisableDamageAfterTime(float _disableTime)
    {
        yield return new WaitForSeconds(_disableTime);
        this.gameObject.SetActive(false);
    }

    private bool HasDamageableTag(string _targetTag)
    {
        for (int i = 0; i < damageableTags.Length; i++)
        {
            if (_targetTag == damageableTags[i])
                return true;
        }

        return false;
    }

    public float RequestDamage(Health health)
    {
        if (!canDealDamage || health.IsDead)
            return 0;

        if (!canDamageMultiple)
        {
            canDealDamage = false;
        }

        int getInstanceID = health.gameObject.GetInstanceID();

        if (!CheckDuplicateDamage(getInstanceID) && HasDamageableTag(health.tag))
        {
            attackedInstances.Add(getInstanceID);

            if (callbackInterface != null)
            {
                DamageInfo damageInfo = new DamageInfo
                    (
                    health.MinHP,
                    health.MaxHP,
                    transform.position,
                    health.transform.position,
                    health.gameObject.name,
                    damage
                    );

                callbackInterface.OnDamageDone(health, damageInfo);
            }

            OnDamageRequested(health);

            return damage;
        }

        return 0;
    }

    private bool CheckDuplicateDamage(int _id)
    {
        if (canDoDuplicateDamage)
        {
            return false;
        }

        for (int i = 0; i < attackedInstances.Count; i++)
        {
            if (attackedInstances[i] == _id)
                return true;
        }
        return false;
    }

    public virtual void OnDamageRequested(Health _Health) { }

    private void ApplyDamage(Collider2D other)
    {
        Health getHealth = other.GetComponent<Health>();
        if (getHealth != null)
        {
            float getDamage = RequestDamage(getHealth);
            if (getDamage != 0)
            {
                Vector3 hitLocation = collider.bounds.ClosestPoint(((other.transform.position - this.transform.position) * 0.5f) + this.transform.position);
                getHealth.Damage(getDamage, hitLocation, owner);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (HasDamageableTag(other.tag))
        {
            ApplyDamage(other);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (HasDamageableTag(collision.gameObject.tag))
        {
            ApplyDamage(collision.collider);
        }
    }
}
