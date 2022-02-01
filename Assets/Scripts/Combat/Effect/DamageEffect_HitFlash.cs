using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DamageEffect_HitFlash : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Material baseMaterial;

    [SerializeField]
    private Material flashMaterial;

    [SerializeField]
    private float totalFlashTime = 0.05f;

    private SpriteRenderer[] spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        GetComponent<Health>().AddListener(this);
    }

    public void OnDamaged(DamageInfo _damageInfo)
    {
        StartCoroutine(HitFlashCoroutine());
    }

    private IEnumerator HitFlashCoroutine()
    {
        for (int i = 0; i < spriteRenderer.Length; i++)
        {
            spriteRenderer[i].sharedMaterial = flashMaterial;
        }

        yield return new WaitForSeconds(totalFlashTime);

        for (int i = 0; i < spriteRenderer.Length; i++)
        {
            spriteRenderer[i].sharedMaterial = baseMaterial;
        }
    }

}
