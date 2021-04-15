#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Wanderer.GameFramework
{
	public class XLuaUIViewRunInUnityEditor:MonoBehaviour
	{
		private XLuaUGUIView _uiView;
		private UIContextBase _uiContext;

		[SerializeField]
		private string _prefixPath = "Assets/Game/UI";

		public bool RunInEditor = true;

		private void Awake()
		{
			if (!RunInEditor)
			{
				Destroy(this);
			}

			_uiView = GetComponent<XLuaUGUIView>();
			if (_uiView == null)
			{
				throw new GameException($"Can't GetComponent XLuaUGUIView!");
			}

			//资源和lua的代码执行
			GameMode.Resource = GameFrameworkMode.GetModule<ResourceManager>();
			GameMode.Audio = GameFrameworkMode.GetModule<AudioManager>();
			GameFrameworkMode.GetModule<ResourceManager>().SetResourceHelper(new EditorAssetsHelper());
		}

		private void Start()
		{
			if (!RunInEditor)
			{
				return;
			}

			//设置默认UI
			_uiContext = new UIContextBase() { AssetPath = $"{_prefixPath}/{name}.prefab", Name = $"{name}", Multiple = false };
			//加载UIAdpterSystem
			GameFrameworkMode.GetModule<XLuaManager>().DoMainLua("UIAdpterSystem");
			//添加一些默认的全局变量
			GameFrameworkMode.GetModule<XLuaManager>().DoString(
				$"require \"class\"\n" +
				$"json = require \"json\"\n" +
				$"require \"UIView\"\n" +
				$"cs_coroutine = require \"cs_coroutine\"\n" +
				$"require \"SpineSystem\"\n" +
				$"Vector2 = CS.UnityEngine.Vector2\n" +
				$"Vector3 = CS.UnityEngine.Vector3\n" +
				$"Color = CS.UnityEngine.Color\n" +
				$"Input = CS.UnityEngine.Input\n" +
				$"RectTransform = CS.UnityEngine.RectTransform\n" +
				$"GameObject = CS.UnityEngine.GameObject\n" +
				$"Transform = CS.UnityEngine.Transform\n"+
				$"GE =  CS.UnityEngine\n" +
				$"UGUI =  CS.UnityEngine.UI\n" +
				$"GF = CS.Wanderer.GameFramework\n" +
				$"GameMode = GF.GameMode\n"+
				$"RectTransformUtility = CS.UnityEngine.RectTransformUtility\n" +
				$"require \"Resource\"\n" +
				$"Image = CS.UnityEngine.UI.Image\n"+
				$"require \"Utility\"\n"+
				$"require \"AudioSystem\"\n"+
				$"", "XLuaUIViewRunInUnityEditor_PreRequire");
			//UIView的生命周期回调
			_uiView.OnInit(_uiContext);
			_uiView.OnEnter(_uiContext, null, null);
		}

		private void OnDestroy()
		{
			if (!RunInEditor)
			{
				return;
			}

			_uiView.OnExit(_uiContext);
			GameFrameworkMode.GetModule<XLuaManager>().OnClose();
			GameFrameworkMode.GetModule<ResourceManager>().OnClose();
		}

		private void Update()
		{
			if (!RunInEditor)
			{
				return;
			}

			if (_uiContext != null)
			{
				_uiView.OnUpdate(_uiContext, Time.deltaTime);
			}
		}

	}
}
#endif