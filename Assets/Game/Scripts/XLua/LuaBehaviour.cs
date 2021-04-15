using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Wanderer.GameFramework
{
    public class LuaBehaviour : MonoBehaviour
    {
        [SerializeField]
        private TextAsset _luaScript;

        [SerializeField]
        private UnityEngine.Object[] _injections;

        private Action _luaOnStart;
        private Action _luaOnDestroy;
        private Action _luaOnUpdate;
        private Action _luaOnEnable;
        private Action _luaOnDisable;

        private LuaTable _scriptEnv;

        public Action<string> OnLuaCallCSharp;


		private void Awake()
		{
            //创建table
            _scriptEnv = GameMode.XLua.NewTable(this);
            //外部参数
            foreach (var item in _injections)
			{
                _scriptEnv.Set(item.name, item);
            }
            //运行lua脚本
            GameMode.XLua.DoString(_luaScript.text,$"LuaBehaviour_{name}_{_luaScript.name}", _scriptEnv);
            //事件回调
            Action luaOnAwake = _scriptEnv.Get<Action>("OnAwake");
            _scriptEnv.Get("OnStart", out _luaOnStart);
            _scriptEnv.Get("OnDestroy", out _luaOnDestroy);
            _scriptEnv.Get("OnUpdate", out _luaOnUpdate);
            _scriptEnv.Get("OnEnable", out _luaOnEnable);
            _scriptEnv.Get("OnDisable", out _luaOnDisable);

            //初始画回调
            luaOnAwake?.Invoke();
        }

		// Start is called before the first frame update
		void Start()
        {
          //  GetComponent<RenderHeads.Media.AVProVideo.MediaPlayer>().OpenVideoFromFile(RenderHeads.Media.AVProVideo.MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder,true);
            _luaOnStart?.Invoke();
        }

        // Update is called once per frame
        void Update()
        {
            _luaOnUpdate?.Invoke();
        }

		private void OnDisable()
		{
            _luaOnDisable?.Invoke();
        }
		private void OnEnable()
		{
            _luaOnEnable?.Invoke();
        }

		private void OnDestroy()
		{
            OnLuaCallCSharp = null;
            _luaOnUpdate = null;
            _luaOnStart = null;
            _luaOnEnable = null;
            _luaOnDisable = null;
            _luaOnDestroy?.Invoke();
            _luaOnDestroy = null;
        }

        /// <summary>
        /// lua 调用 csharp
        /// </summary>
        /// <param name="data"></param>
        public void LuaCallCSharp(string data)
        {
            OnLuaCallCSharp?.Invoke(data);
        }
    }
}