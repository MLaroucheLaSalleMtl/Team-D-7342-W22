using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Farming Kit/Entity Components/Combat/Health")]
public class Health : MonoBehaviour
{
    protected List<IDamageable> iDamageableInterfaces = new List<IDamageable>();
    protected List<IHealable> iHealableInterfaces = new List<IHealable>();
    protected List<IInvulnerable> iInvulnerableInterfaces = new List<IInvulnerable>();
    protected List<IKillable> iKillableInterfaces = new List<IKillable>();

    [SerializeField]
    protected float minHP;
    public float MinHP
    {
        get { return minHP; }
    }

    [SerializeField]
    protected float maxHP;
    public float MaxHP
    {
        get { return maxHP; }
    }


    [SerializeField]
    protected bool canBecomeInvulnerable = false;

    [SerializeField]
    private bool startInvulnerable;

    private new Collider collider;
    public UnityEngine.Collider Collider
    {
        get { return collider; }
    }

    private bool isDead;
    public bool IsDead
    {
        get { return isDead; }
    }

    protected bool invulnerable;

    public void SetInvulnerable(bool state)
    {
        invulnerable = state;
    }

    private void Awake()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        minHP = maxHP;

        collider = this.gameObject.GetComponentInChildren<Collider>();

        if (startInvulnerable)
        {
            invulnerable = true;
        }
    }

    public void Revive()
    {
        minHP = maxHP;
        isDead = false;
    }

    public void Configure (int _minHP, int _maxHP)
    {
        minHP = _minHP;
        maxHP = _maxHP;
        isDead = false;
    }

    public void AddListener(IDamageable _iDamageable)
    {
        if (_iDamageable != null)
        {
            iDamageableInterfaces.Add(_iDamageable);
        }
    }

    public void AddListener(IHealable _iHealable)
    {
        if (_iHealable != null)
        {
            iHealableInterfaces.Add(_iHealable);
        }
    }

    public void AddListener(IKillable _iKillable)
    {
        if (_iKillable != null)
        {
            iKillableInterfaces.Add(_iKillable);
        }
    }

    public void AddListener(IInvulnerable _iInvulnerable)
    {
        if (_iInvulnerable != null)
        {
            iInvulnerableInterfaces.Add(_iInvulnerable);
        }
    }

    public void SetInvulnerable(float _time, bool _state = true)
    {
        // This is set because of cache reasons, using GetComponentsInChildren can be expensive and might cause lag spikes.
        if (canBecomeInvulnerable == false)
        {
            Debug.LogFormat("The stats component of {0} does not have canBecomeInvulnerable set as true.", this.transform.root);
            iInvulnerableInterfaces = new List<IInvulnerable>(GetComponentsInChildren<IInvulnerable>());
            canBecomeInvulnerable = true;
        }

        invulnerable = _state;

        for (int i = 0; i < iInvulnerableInterfaces.Count; i++)
        {
            iInvulnerableInterfaces[i].BecameInvulnerable(_state);
        }

        if (_time == 0)
        {
            return;
        }

        StartCoroutine(RecoverVolnerabilityCoroutine(_time));
    }

    private IEnumerator RecoverVolnerabilityCoroutine(float _time)
    {
        yield return new WaitForSeconds(_time);

        invulnerable = false;

        for (int i = 0; i < iInvulnerableInterfaces.Count; i++)
        {
            iInvulnerableInterfaces[i].BecameInvulnerable(false);
        }
    }

    public void Heal(float _amount)
    {
        minHP = Mathf.Clamp(minHP + _amount, 0, maxHP);

        for (int i = 0; i < iHealableInterfaces.Count; i++)
        {
            iHealableInterfaces[i].OnHealed(minHP, maxHP);
        }
    }

    public void Kill()
    {
        Damage(minHP, this.transform.position, null);
    }

    public void Damage(float _amount, Vector3 _hitlocation, GameObject _cause)
    {
        if (invulnerable)
            return;

        minHP -= _amount;

        if (!isDead)
        {
            for (int i = 0; i < iDamageableInterfaces.Count; i++)
            {
                iDamageableInterfaces[i].OnDamaged(new DamageInfo(minHP, maxHP, _hitlocation, (_cause != null) ? _cause.transform.position : Vector3.zero, (_cause != null) ? _cause.name : "", _amount));
            }
        }

        if (minHP <= 0 && !isDead)
        {
            for (int i = 0; i < iKillableInterfaces.Count; i++)
            {
                iKillableInterfaces[i].OnDeath(this);
            }

            isDead = true;
        }

    }

    public void IncreaseHealth(int _amount)
    {
        maxHP += _amount;

        for (int i = 0; i < iHealableInterfaces.Count; i++)
        {
            iHealableInterfaces[i].OnHealed(minHP, maxHP);
        }
    }

    public void DecreaseHealth(int _amount)
    {
        maxHP -= _amount;

        for (int i = 0; i < iHealableInterfaces.Count; i++)
        {
            iHealableInterfaces[i].OnHealed(minHP, maxHP);
        }
    }

    public bool IsDamaged { get { return minHP < maxHP; } }
}
