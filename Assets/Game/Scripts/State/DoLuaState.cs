using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public class DoLuaState : FSMState<GameStateContext>
	{
        public override void OnEnter(FSM<GameStateContext> fsm)
        {
            base.OnEnter(fsm);
            GameMode.XLua.DoMainLua();
        }
    }
}