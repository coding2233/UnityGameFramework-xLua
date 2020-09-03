using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public partial class PreloadState
    {
        public override void OnEnter(FSM<GameStateContext> fsm)
        {
            base.OnEnter(fsm);
             Debug.Log("PreloadState");
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
        }

      
    }
}

