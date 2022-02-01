using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private float WaitTime;

    [SerializeField]
    private UnityEvent eventToHappen;

    public void StartTimer()
    {
        StopAllCoroutines();
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(WaitTime);
        eventToHappen.Invoke();
    }

}
