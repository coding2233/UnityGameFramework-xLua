using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Wanderer.GameFramework
{
    public class UGUIViewCallXLua : ICSharpCallXLua
    {
        //ui初始化
        public delegate void OnInit(Transform transform, IUIContext uiContext);
        public OnInit LuaOnInit;

        //ui销毁
        public delegate void OnFree(Transform transform, IUIContext uiContext);
        public OnFree LuaOnFree;

        //更新
        public delegate void OnUpdate(IUIContext uiContext, float deltaTime);
        public OnUpdate LuaOnUpdate;

        //进入
        public delegate void OnEnter(Transform transform, IUIContext uiContext, Action<string> callback, object userData);
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
        public delegate void OnAnimationStart(IUIContext uiContext, IUIAnimation uiAnimation);
        public OnAnimationStart LuaOnAnimationStart;

        //动画结束
        public delegate void OnAnimationComplete(IUIContext uiContext, IUIAnimation uiAnimation);
        public OnAnimationComplete LuaOnAnimationComplete;

        //UI事件
        public delegate void OnEventTrigger(IUIContext uiContext, Transform target, string callMethod, BaseEventData eventData);
        public OnEventTrigger LuaOnEventTrigger;

        //物理碰撞事件
        public delegate void OnColliderTrigger(IUIContext uIContext, string callMethod, Transform self, Transform other);
        public OnColliderTrigger LuaOnColliderTrigger;

        public void Bind(string luaName)
		{
            LuaOnInit = GameMode.XLua.GetInPath<OnInit>($"{luaName}.OnInit");
            LuaOnFree = GameMode.XLua.GetInPath<OnFree>($"{luaName}.OnFree");
            LuaOnUpdate = GameMode.XLua.GetInPath<OnUpdate>($"{luaName}.OnUpdate");
            LuaOnEnter = GameMode.XLua.GetInPath<OnEnter>($"{luaName}.OnEnter");
            LuaOnExit = GameMode.XLua.GetInPath<OnExit>($"{luaName}.OnExit");
            LuaPause = GameMode.XLua.GetInPath<OnPause>($"{luaName}.OnPause");
            LuaOnResume = GameMode.XLua.GetInPath<OnResume>($"{luaName}.OnResume");
            LuaOnAnimationStart = GameMode.XLua.GetInPath<OnAnimationStart>($"{luaName}.OnAnimationStart");
            LuaOnAnimationComplete = GameMode.XLua.GetInPath<OnAnimationComplete>($"{luaName}.OnAnimationComplete");
            LuaOnEventTrigger = GameMode.XLua.GetInPath<OnEventTrigger>($"{luaName}.OnEventTrigger");
            LuaOnColliderTrigger = GameMode.XLua.GetInPath<OnColliderTrigger>($"{luaName}.OnColliderTrigger");
        }

        public void Clear()
        {
            LuaOnInit = null;
            LuaOnFree = null;
            LuaOnUpdate = null;
            LuaOnEnter = null;
            LuaOnExit = null;
            LuaPause = null;
            LuaOnResume = null;
            LuaOnAnimationStart = null;
            LuaOnAnimationComplete = null;
            LuaOnEventTrigger = null;
            LuaOnColliderTrigger = null;
        }
	}
}