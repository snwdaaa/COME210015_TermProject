using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 현재 플레이어 상태
/// </summary>
public class PlayerState : MonoBehaviour
{
    public enum CurrentMoveState
    {
        Idle,
        Walk,
        Sprint,
        OnAir
    };
    public CurrentMoveState currentMoveState = CurrentMoveState.Idle;

    public enum CurrentPostureState
    {
        Stand,
        Crouch
    }
    public CurrentPostureState currentPostureState = CurrentPostureState.Stand;
}
