using UnityEngine;
using System.Collections;

public struct DamageInfo
{
    float minHP;
    public float MinHP
    {
        get { return minHP; }
    }

    float maxHP;
    public float MaxHP
    {
        get { return maxHP; }
    }

    Vector3 hitLocation;
    public UnityEngine.Vector3 Hitlocation
    {
        get { return hitLocation; }
    }

    Vector3 causeLocation;
    public UnityEngine.Vector3 Causelocation
    {
        get { return causeLocation; }
    }

    string attackerName;
    public string AttackerName
    {
        get { return attackerName; }
    }

    float amount;
    public float Amount
    {
        get { return amount; }
    }

    public DamageInfo(float _minHP, float _maxHP, Vector3 _hitLoc, Vector3 _causeLoc, string _attackerName, float _amount)
    {
        minHP = _minHP;
        maxHP = _maxHP;
        hitLocation = _hitLoc;
        causeLocation = _causeLoc;
        attackerName = _attackerName;
        amount = _amount;
    }
}