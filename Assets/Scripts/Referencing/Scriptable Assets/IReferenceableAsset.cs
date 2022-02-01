using UnityEngine;
using System.Collections;

public interface IReferenceableAsset
{
    string GetGuid();

    // Required when checking for duplicate guids.
    void GenerateNewGuid();
}
