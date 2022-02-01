using UnityEngine;
using System.Collections;

namespace Lowscope.ScriptableObjectUpdater
{
    [AddComponentMenu("")]
    public class FixedUpdateTicker : Ticker
    {
        private void FixedUpdate()
        {
            DispatchTick();
        }
    }
}
