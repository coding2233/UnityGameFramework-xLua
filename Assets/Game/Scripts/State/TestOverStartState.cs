﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wanderer.GameFramework;

[FSM(FSMStateType.OverStart)]
public class TestOverStartState : FSMState<GameStateContext>
{
    // Debug.Log($"TestStart OnEnter");

    public override void OnEnter(FSM<GameStateContext> fsm)
    {
        base.OnEnter(fsm);
        Debug.Log($"TestStart OnEnter");
    }

    public override void OnExit(FSM<GameStateContext> fsm)
    {
        base.OnExit(fsm);
    }

    public override void OnInit(FSM<GameStateContext> fsm)
    {
        base.OnInit(fsm);
    }

    public override void OnUpdate(FSM<GameStateContext> fsm)
    {
        base.OnUpdate(fsm);

        ChangeState<LaunchGameState>(fsm);
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
