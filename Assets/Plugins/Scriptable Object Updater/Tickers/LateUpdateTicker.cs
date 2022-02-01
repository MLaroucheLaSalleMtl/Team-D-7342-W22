using UnityEngine;
using System.Collections;

namespace Lowscope.ScriptableObjectUpdater
{
    [AddComponentMenu("")]
    public class LateUpdateTicker : Ticker
    {
        private void LateUpdate()
        {
            DispatchTick();
        }
    }
}