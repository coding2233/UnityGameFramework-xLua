using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Wanderer.GameFramework
{
	public class XLuaManager : GameFrameworkModule, IUpdate
	{
		public override int Priority => 10000;

		//lua的环境变量
		private LuaEnv _luaEnv;
		public LuaEnv LuaEnv
		{
			get
			{
				return _luaEnv;
			}
		}

		internal float _lastGCTime = 0;
		internal const float _gcInterval = 1;//1 second
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
		//所有的lua的资源路径
		private List<string> _allLuaAssetPaths;
		//csharp call lua的事件绑定
		private Dictionary<Type, ICSharpCallXLua> _allCsharpCallXLua;


		public XLuaManager()
		{
			_resource = GameFrameworkMode.GetModule<ResourceManager>();
			_luaEnv = new LuaEnv();
			_luaEnv.AddLoader(CustomLoader);

			_allCsharpCallXLua = new Dictionary<Type, ICSharpCallXLua>();
		}

		public override void OnInit()
		{
			base.OnInit();
		}

		/// <summary>
		/// 运行 main lua
		/// </summary>
		public void DoMainLua(string main = "main")
		{
			if (_loadMainLua == null)
			{
				//获取所有的lua脚本路径
				GetAllLuaAssetsPath();

				_loadMainLua = _luaEnv.NewTable();

				// 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
				LuaTable meta = _luaEnv.NewTable();
				meta.Set("__index", _luaEnv.Global);
				_loadMainLua.SetMetaTable(meta);
				meta.Dispose();

				_loadMainLua.Set("self", this);
				_luaEnv.DoString($"require \"{main}\"", main, _loadMainLua);
				Action onLuaStart = _loadMainLua.Get<Action>("OnStart");
				_loadMainLua.Get("OnUpdate", out _luaOnUpdate);
				_loadMainLua.Get("OnClose", out _luaOnClose);

				//事件绑定
				CSharpCallXLuaBind();

				//lua 开始
				onLuaStart?.Invoke();
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
			//先调用了Close
			if (_allCsharpCallXLua == null)
				return default(T);

			if (_allCsharpCallXLua.TryGetValue(typeof(T), out ICSharpCallXLua callXLua))
			{
				return (T)callXLua;
			}
			else
			{
				Debug.LogWarning($"The CSharpCallXLua adapter could not be found.  {typeof(T).FullName}");
				//throw new GameException($"The CSharpCallXLua adapter could not be found.  {typeof(T).FullName}");
			}
			return default(T);
		}

		/// <summary>
		/// 创建新的lua table
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public LuaTable NewTable<TValue>(TValue value)
		{
			LuaTable newTable = _luaEnv.NewTable();
			// 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
			LuaTable meta = _luaEnv.NewTable();
			meta.Set("__index", _luaEnv.Global);
			newTable.SetMetaTable(meta);
			meta.Dispose();
			newTable.Set("self", value);
			return newTable;
		}

		/// <summary>
		/// 运行lua脚本的内容
		/// </summary>
		/// <param name="chunk"></param>
		/// <param name="chunkName"></param>
		/// <param name="env"></param>
		/// <returns></returns>
		public object[] DoString(string chunk, string chunkName = "chunk", LuaTable env = null)
		{
			return _luaEnv.DoString(chunk, chunkName, env);
		}

		//渲染函数
		public void OnUpdate()
		{
			_luaOnUpdate?.Invoke(Time.deltaTime);
			if (Time.time - _lastGCTime > _gcInterval)
			{
				_luaEnv.Tick(); 
				_lastGCTime = Time.time;
			}
		}

		/// <summary>
		/// 关闭
		/// </summary>
		public override void OnClose()
		{
			_luaScripts.Clear();
			//清理所有的绑定事件
			if (_allCsharpCallXLua != null)
			{
				foreach (var item in _allCsharpCallXLua)
				{
					item.Value.Clear();
				}
				_allCsharpCallXLua.Clear();
				_allCsharpCallXLua = null;
			}
			//清理绑定事件
			_luaOnUpdate = null;
			_luaOnClose?.Invoke();
			_luaOnClose = null;
			_loadMainLua?.Dispose();
			_loadMainLua = null;
		}


		#region 内部函数
		//自定义加载
		private byte[] CustomLoader(ref string filePath)
		{
			byte[] data;
			if (!_luaScripts.TryGetValue(filePath, out data))
			{
				string luaPath = GetLuaAssetPath(filePath);
				if (!string.IsNullOrEmpty(luaPath))
				{
					TextAsset textAsset = _resource.Asset.LoadAsset<TextAsset>(luaPath);
					data = textAsset.bytes;
					_luaScripts.Add(filePath, data);
				}
				else
				{
					throw new GameException($"Can't find lua script! {filePath} {luaPath}#");
				}
			}
			return data;
		}

		// 事件绑定
		private void CSharpCallXLuaBind()
		{
			UGUIViewCallXLua uGUIViewCallXLua = new UGUIViewCallXLua();
			uGUIViewCallXLua.Bind("UIAdpterSystem");
			_allCsharpCallXLua.Add((typeof(UGUIViewCallXLua)),uGUIViewCallXLua);
		}

		//获取所有的lua脚本的路径
		private void GetAllLuaAssetsPath()
		{
			string luaPathPrefix = _luaPathPrefix.ToLower();
			_allLuaAssetPaths =GameFrameworkMode.GetModule<ResourceManager>().Asset.AllAssetPaths.FindAll(x => x.StartsWith(luaPathPrefix));
		}

		//获取lua的脚本路径
		private string GetLuaAssetPath(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			name = $"/{name.ToLower()}.lua.txt";
			string assetPath= _allLuaAssetPaths.Find(x => x.EndsWith(name));
			return assetPath;
		}

		#endregion

	}
}