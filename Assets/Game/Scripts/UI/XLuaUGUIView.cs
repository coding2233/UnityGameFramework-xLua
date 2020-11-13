using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class XLuaUGUIView : UGUIView
    {
        private IUIContext _uiContext;

        public override void OnInit(IUIContext uiContext)
		{
			base.OnInit(uiContext);
            _uiContext = uiContext;
            //初始化
            GameMode.XLua.CallLua<UGUIViewCallXLua>().LuaOnInit(transform, uiContext);
        }
	

        /// <summary>
        /// 更新界面
        /// </summary>
        /// <param name="uiContext"></param>
        public override void OnUpdate(IUIContext uiContext, float deltaTime)
        {
            base.OnUpdate(uiContext, deltaTime);
            GameMode.XLua.CallLua<UGUIViewCallXLua>().LuaOnUpdate(uiContext, Time.deltaTime);
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="parameters">不确定参数</param>
        public override void OnEnter(IUIContext uiConext, Action<string> callBack = null, params object[] parameters)
        {
            base.OnEnter(uiConext, callBack, parameters);
            GameMode.XLua.CallLua<UGUIViewCallXLua>().LuaOnEnter(transform,uiConext, callBack, parameters);
        }
        /// <summary>
        /// 退出界面
        /// </summary>
        public override void OnExit(IUIContext uiConext)
        {
            base.OnExit(uiConext);
            GameMode.XLua.CallLua<UGUIViewCallXLua>().LuaOnExit(uiConext);
        }
        /// <summary>
        /// 暂停界面
        /// </summary>
        public override void OnPause(IUIContext uiConext)
        {
            base.OnPause(uiConext);
            GameMode.XLua.CallLua<UGUIViewCallXLua>().LuaPause(uiConext);
        }
        /// <summary>
        /// 恢复界面
        /// </summary>
        public override void OnResume(IUIContext uiConext)
        {
            base.OnResume(uiConext);
            GameMode.XLua.CallLua<UGUIViewCallXLua>().LuaOnResume(uiConext);
        }

        /// <summary>
        /// 动画开始
        /// </summary>
        /// <param name="uiAnim"></param>
        public override void OnAnimationStart(IUIAnimation uiAnim)
        {
            base.OnAnimationStart(uiAnim);
            GameMode.XLua.CallLua<UGUIViewCallXLua>().LuaOnAnimationStart(_uiContext, uiAnim);
        }
        /// <summary>
        /// 动画结束
        /// </summary>
        /// <param name="uiAnim"></param>
        public override void OnAnimationComplete(IUIAnimation uiAnim)
        {
            base.OnAnimationComplete(uiAnim);
            GameMode.XLua.CallLua<UGUIViewCallXLua>().LuaOnAnimationComplete(_uiContext, uiAnim);
        }

        /// <summary>
        /// 设置深度
        /// </summary>
        /// <param name="depth"></param>
		public override void SetDepth(int depth)
        {
            base.SetDepth(depth);
        }
    }


}
