using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Wanderer.GameFramework
{
	public class XLuaUIAnimation:IUIAnimation
	{
		public UIView TargetUIView { get; set; }

		private Tween _dotweenTweener;
		private IEnumerator _enumerator;

		public void SetTarget(GameObject target)
		{
			TargetUIView.GetComponent<CanvasGroup>().blocksRaycasts = false;
			//target.GetComponent<RectTransform>().anchoredPosition=.DOAnchorPos(Vector2.zero,0.5f)
		}

		public async UniTask<int> Run()
		{
			if (_dotweenTweener != null)
			{
				await _dotweenTweener;
			}
			if (_enumerator != null)
			{
				await _enumerator;
			}
			return 1;
		}

		public XLuaUIAnimation SetTargetUIView(UIView targetUIView)
		{
			TargetUIView = targetUIView;
			return this;
		}
		/// <summary>
		/// 优先支持dotween动画
		/// </summary>
		/// <param name="tweener"></param>
		/// <returns></returns>
		public XLuaUIAnimation SetAnimation(Tween tweener)
		{
			_dotweenTweener = tweener;
			return this;
		}
		/// <summary>
		/// 支持协程动画
		/// </summary>
		/// <param name="enumerator"></param>
		/// <returns></returns>
		public XLuaUIAnimation SetAnimation(IEnumerator enumerator)
		{
			_enumerator = enumerator;
			return this;
		}

		public void Flush()
		{
			_dotweenTweener = null;
			_enumerator = null;
			TargetUIView = null;
		}

		public void OnUITweenComplete()
		{
			Flush();
			XLuaUIAnimPool.Release(this);
		}
	}

	public class XLuaUIAnimPool
	{
		private static ObjectPool<XLuaUIAnimation> _xluaAnimPool = new ObjectPool<XLuaUIAnimation>(null, null);

		public static XLuaUIAnimation Get()
		{
			return _xluaAnimPool.Get();
		}

		public static void Release(XLuaUIAnimation animation)
		{
			animation.Flush();
			_xluaAnimPool.Release(animation);
		}

		public static void Release(XLuaUIAnimation[] animations)
		{
			if (animations != null)
			{
				foreach (var item in animations)
				{
					item.Flush();
					_xluaAnimPool.Release(item);
				}
			}
		}
	}
}