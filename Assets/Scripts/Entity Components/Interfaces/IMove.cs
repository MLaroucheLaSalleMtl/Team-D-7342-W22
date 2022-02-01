using UnityEngine;
using System.Collections;

public interface IMove
{
    void OnMove(Vector2 direction, float velocity);
}
