using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Item Action Attack", menuName = "Items/Item Actions/Attack")]
public class ItemAction_Attack : ItemAction
{
    [SerializeField]
    private ScriptablePoolContainer damageVolumePool;

    [SerializeField]
    private string[] targetTags;

    [SerializeField]
    private int damage;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float attackDistance;

    [SerializeField, Tooltip("Is one attack able to hit multiple targets")]
    private bool hitMultipleTargets;

    private enum AttackType { Smash, Slash };

    [SerializeField]
    private AttackType attackType = AttackType.Smash;

    public override IEnumerator ItemUseAction(Inventory userInventory, int itemIndex)
    {
        Aimer getAimer = userInventory.GetComponent<Aimer>();
        Mover getMover = userInventory.GetComponent<Mover>();
        //GridSelector getGridSelector = userInventory.GetComponent<GridSelector>();

        if (getMover.IsMovementFrozen)
            yield break;

        Vector2 attackLocation = (Vector2)userInventory.transform.position + (getAimer.GetAimDirection() * attackDistance);
        //Vector2 attackLocation = getGridSelector.GetGridWorldSelectionPosition();

        getAimer.LookAt(attackLocation);

        BodyAnimation[] getEntityAnimator = userInventory.GetComponentsInChildren<BodyAnimation>();

        float animationTime = 0;

        for (int i = 0; i < getEntityAnimator.Length; i++)
        {
            switch (attackType)
            {
                case AttackType.Smash:
                    animationTime = getEntityAnimator[i].ApplySmashAnimation(speed, userInventory.GetItem(itemIndex).Data.Icon);
                    break;
                case AttackType.Slash:
                    animationTime = getEntityAnimator[i].ApplySlashAnimation(speed, userInventory.GetItem(itemIndex).Data.Icon);
                    break;
                default:
                    break;
            }
        }

        getMover.FreezeMovement(true);

        yield return new WaitForSeconds(animationTime * 0.5f);

        GameObject damageVolume = damageVolumePool.Retrieve(attackLocation, new Quaternion());

        damageVolume.GetComponent<DamageVolume>().Configure(new DamageVolumeConfiguration()
        {
            Damage = damage,
            Owner = userInventory.gameObject,
            ActiveTime = 0.5f,
            TargetTags = targetTags,
            AllowDuplicateDamage = true,
            CanDamageMultiple = hitMultipleTargets,
            Size = new Vector2(0.10f, 0.10f)
        });

        yield return new WaitForSeconds(animationTime * 0.5f);

        getMover.FreezeMovement(false);

        yield return null;
    }

    public override void ItemAcquisitionAction(Inventory userInventory, int itemIndex)
    {
        
    }

    public override bool ItemUseCondition(Inventory userInventory, int itemIndex)
    {
        return true;
    }
}
