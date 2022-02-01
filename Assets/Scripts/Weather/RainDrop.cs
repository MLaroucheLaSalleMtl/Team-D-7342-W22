using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Single purpose script.
/// Normally I take a modular approach. But this is too small to split up.
/// </summary>
public class RainDrop : MonoBehaviour
{
    [SerializeField]
    private Vector2 moveDirection;

    [SerializeField]
    private Vector2 minMaxDropTime;

    [SerializeField]
    private Vector2 minMaxMoveSpeed;

    [SerializeField]
    private SpriteRenderer rainLine = null;

    [SerializeField]
    private SpriteRenderer rainDrop = null;

    private float currentDropTime;
    private float currentMoveSpeed;

    private WaitForSeconds rainUpdateTime = new WaitForSeconds(0.05f);
    private WaitForSeconds rainStopTime = new WaitForSeconds(0.25f);

    private void OnEnable()
    {
        currentDropTime = Random.Range(minMaxDropTime.x, minMaxDropTime.y);
        currentMoveSpeed = Random.Range(minMaxMoveSpeed.x, minMaxMoveSpeed.y);

        rainLine.enabled = true;
        rainDrop.enabled = false;

        StartCoroutine(FallCoroutine());
    }

    private void OnValidate()
    {
        moveDirection.Normalize();
    }

    // Display the direction of movement with the use of gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(this.transform.position, this.transform.position + ((Vector3)moveDirection) * 0.1f);
    }

    IEnumerator FallCoroutine()
    {
        while (currentDropTime > 0)
        {
            this.transform.Translate((moveDirection * currentMoveSpeed));
            currentDropTime -= Time.deltaTime;
            yield return rainUpdateTime;
        }

        rainLine.enabled = false;
        rainDrop.enabled = true;

        yield return rainStopTime;

        this.gameObject.SetActive(false);
    }

}
