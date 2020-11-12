using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class UGUIViewCallXLua: ICSharpCallXLua
    {
        //ui初始化
        public delegate void OnInit();
        public OnInit LuaOnInit;

        //更新
        public delegate void OnUpdate(float deltaTime);
        public OnUpdate LuaOnUpdate;

        //进入
        public delegate void OnEnter(IUIContext uiContext, Action<string> callback, object userData);
        public OnEnter LuaOnEnter;

        //退出
        public delegate void OnExit(IUIContext uiContext);
        public OnExit LuaOnExit;

        //暂停
        public delegate void Pause(IUIContext uiContext);
        public OnExit LuaPause;

        //恢复
        public delegate void OnResume(IUIContext uiContext);
        public OnExit LuaOnResume;

        //动画开始
        public delegate void OnAnimationStart(IUIAnimation uiAnimation);
        public OnAnimationStart LuaOnAnimationStart;

        //动画结束
        public delegate void OnAnimationComplete(IUIAnimation uiAnimation);
        public OnAnimationComplete LuaOnAnimationComplete;

		public void Bind(string luaName)
		{
            throw new Exception(luaName);
		}
	}
}