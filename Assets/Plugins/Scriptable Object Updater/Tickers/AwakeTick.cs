using UnityEngine;
using System.Collections;

namespace Lowscope.ScriptableObjectUpdater
{
    [AddComponentMenu("")]
    public class AwakeTick : Ticker
    {
        public void Awake()
        {
            DispatchTick();
        }
    }
}