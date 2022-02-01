using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class OnTriggerListener : MonoBehaviour
{
    [SerializeField]
    private string[] allowedTags;

    [SerializeField]
    private UnityEvent OnTriggerEnter;

    [SerializeField]
    private UnityEvent OnTriggerExit;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (HasAllowedTag(collision.gameObject))
        {
            OnTriggerEnter.Invoke();

        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (HasAllowedTag(collision.gameObject))
        {
            OnTriggerExit.Invoke();
        }
    }

    private bool HasAllowedTag(GameObject gameObject)
    {
        for (int i = 0; i < allowedTags.Length; i++)
        {
            if (gameObject.CompareTag(allowedTags[i]))
            {
                return true;
            }
        }

        return false;
    }
}
