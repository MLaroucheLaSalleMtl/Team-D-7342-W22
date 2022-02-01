using Lowscope.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Set To Temporary Save Slot")]
public class ActionSetToTemporarySaveSlot : ScriptableObject
{
    public void SetToTemporarySlot()
    {
        SaveMaster.SetSlotToTemporarySlot(false);
    }
}
