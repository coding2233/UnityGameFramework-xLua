using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class UGUIViewCallXLua: ICSharpCallXLua
    {
        //ui初始化
        public delegate void OnInit(Transform transform, IUIContext uiContext);
        public OnInit LuaOnInit;

        //更新
        public delegate void OnUpdate(IUIContext uiContext, float deltaTime);
        public OnUpdate LuaOnUpdate;

        //进入
        public delegate void OnEnter(Transform transform,IUIContext uiContext, Action<string> callback, object userData);
        public OnEnter LuaOnEnter;

        //退出
        public delegate void OnExit(IUIContext uiContext);
        public OnExit LuaOnExit;

        //暂停
        public delegate void OnPause(IUIContext uiContext);
        public OnPause LuaPause;

        //恢复
        public delegate void OnResume(IUIContext uiContext);
        public OnResume LuaOnResume;

        //动画开始
        public delegate void OnAnimationStart(IUIContext uiContext,IUIAnimation uiAnimation);
        public OnAnimationStart LuaOnAnimationStart;

        //动画结束
        public delegate void OnAnimationComplete(IUIContext uiContext,IUIAnimation uiAnimation);
        public OnAnimationComplete LuaOnAnimationComplete;

		public void Bind(string luaName)
		{
            LuaOnInit = GameMode.XLua.GetInPath<OnInit>($"{luaName}.OnInit");
            LuaOnUpdate = GameMode.XLua.GetInPath<OnUpdate>($"{luaName}.OnUpdate");
            LuaOnEnter = GameMode.XLua.GetInPath<OnEnter>($"{luaName}.OnEnter");
            LuaOnExit = GameMode.XLua.GetInPath<OnExit>($"{luaName}.OnExit");
            LuaPause = GameMode.XLua.GetInPath<OnPause>($"{luaName}.OnPause");
            LuaOnResume = GameMode.XLua.GetInPath<OnResume>($"{luaName}.OnResume");
            LuaOnAnimationStart = GameMode.XLua.GetInPath<OnAnimationStart>($"{luaName}.OnAnimationStart");
            LuaOnAnimationComplete = GameMode.XLua.GetInPath<OnAnimationComplete>($"{luaName}.OnAnimationComplete");
        }
	}
}