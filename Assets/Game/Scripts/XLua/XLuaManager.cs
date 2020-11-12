using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Wanderer.GameFramework
{
	public class XLuaManager : GameFrameworkModule, IUpdate
	{
		//lua的环境变量
		private LuaEnv _luaEnv;
		//lua main入口
		private LuaTable _loadMainLua;
		// lua 更新函数
		private Action<float> _luaOnUpdate;
		//lua 关闭函数
		private Action _luaOnClose;
		//资源管理器
		private ResourceManager _resource;
		//所有的lua脚本
		private readonly Dictionary<string, byte[]> _luaScripts = new Dictionary<string, byte[]>();

		//lua路径前缀
		private const string _luaPathPrefix = "Assets/Game/XLua";

		private Dictionary<Type, ICSharpCallXLua> _allCsharpCallXLua;


		public XLuaManager()
		{
			_luaEnv = new LuaEnv();
			_luaEnv.AddLoader(CustomLoader);

			_allCsharpCallXLua = new Dictionary<Type, ICSharpCallXLua>();
		}

		public override void OnInit()
		{
			base.OnInit();
			_resource = GameMode.Resource;
		}

		/// <summary>
		/// 运行 main lua
		/// </summary>
		public void DoMainLua(string main = "main")
		{
			if (_loadMainLua == null)
			{
				_loadMainLua = _luaEnv.NewTable();

				// 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
				LuaTable meta = _luaEnv.NewTable();
				meta.Set("__index", _luaEnv.Global);
				_loadMainLua.SetMetaTable(meta);
				meta.Dispose();

				_loadMainLua.Set("self", this);
				_luaEnv.AddLoader(CustomLoader);
				_luaEnv.DoString($"require '{main}'", main, _loadMainLua);
				Action onLuaStart = _loadMainLua.Get<Action>("OnStart");
				_loadMainLua.Get("OnUpdate", out _luaOnUpdate);
				_loadMainLua.Get("OnClose", out _luaOnClose);

				//lua 开始
				onLuaStart?.Invoke();

				//事件绑定
				CSharpCallXLuaBind();
			}
		}

		/// <summary>
		/// 获取lua事件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path"></param>
		/// <returns></returns>
		public T GetInPath<T>(string path)
		{
			return _luaEnv.Global.GetInPath<T>(path);
		}

		/// <summary>
		/// 调用事件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T CallLua<T>() where T : ICSharpCallXLua
		{
			if (_allCsharpCallXLua.TryGetValue(typeof(T), out ICSharpCallXLua callXLua))
			{
				return (T)callXLua;
			}
			return default(T);
		}

		public void CallXLuaInPath()
		{

		}

		public void OnUpdate()
		{
			_luaOnUpdate?.Invoke(Time.deltaTime);
		}

		public override void OnClose()
		{
			_luaOnUpdate = null;
			_luaOnClose?.Invoke();
			_luaOnClose = null;
			_loadMainLua.Dispose();
			_loadMainLua = null;
		}


		#region 内部函数
		//自定义加载
		private byte[] CustomLoader(ref string filePath)
		{
			byte[] data;
			filePath = System.IO.Path.Combine(_luaPathPrefix, $"{filePath}.lua.txt");
			if (!_luaScripts.TryGetValue(filePath, out data))
			{
				TextAsset textAsset = _resource.LoadAssetSync<TextAsset>(filePath);
				data = textAsset.bytes;
				_luaScripts.Add(filePath, data);
			}
			return data;
		}


		// 事件绑定
		private void CSharpCallXLuaBind()
		{
			UGUIViewCallXLua uGUIViewCallXLua = new UGUIViewCallXLua();
			uGUIViewCallXLua.Bind("UISystem");
			_allCsharpCallXLua.Add((typeof(UGUIViewCallXLua)),uGUIViewCallXLua);
		}

		#endregion

	}
}