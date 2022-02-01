using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceCameraPositionOnWarp : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private FloatEvent onWarpDoneEvent;

    private void Awake()
    {
        onWarpDoneEvent.AddListener(OnWarped);
    }

    private void OnWarped(float obj)
    {
        virtualCamera.ForceCameraPosition(virtualCamera.Follow.transform.position, new Quaternion());
    }

    //private IEnumerator Start()
    //{
    //    Debug.Log("Called");

    //    var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

    //    float oldSmoothing = transposer.m_LookaheadSmoothing;
    //    float oldXDamping = transposer.m_XDamping;
    //    float oldYDamping = transposer.m_YDamping;
    //    float onyZDamping = transposer.m_ZDamping;

    //    transposer.m_LookaheadSmoothing = 0;
    //    transposer.m_XDamping = 0;
    //    transposer.m_YDamping = 0;
    //    transposer.m_ZDamping = 0;

    //    yield return new WaitForSeconds(0.1f);

    //    transposer.m_LookaheadSmoothing = oldSmoothing;
    //    transposer.m_XDamping = oldXDamping;
    //    transposer.m_YDamping = oldYDamping;
    //    transposer.m_ZDamping = onyZDamping;
    //}
}
