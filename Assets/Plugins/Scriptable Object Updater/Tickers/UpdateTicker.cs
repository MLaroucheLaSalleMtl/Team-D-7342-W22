using UnityEngine;
using System.Collections;

namespace Lowscope.ScriptableObjectUpdater
{
    [AddComponentMenu("")]
    public class UpdateTicker : Ticker
    {
        private void Update()
        {
            DispatchTick();
        }
    }
}