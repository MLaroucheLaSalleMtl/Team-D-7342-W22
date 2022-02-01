using UnityEngine;
using System.Collections;

namespace Lowscope.ScriptableObjectUpdater
{
    [AddComponentMenu("")]
    public class StartTick : Ticker
    {
        public void Start()
        {
            DispatchTick();
        }
    }
}