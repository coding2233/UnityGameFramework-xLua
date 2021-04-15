using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public class XLuaColliderTrigger : MonoBehaviour
	{
		#region field
		[SerializeField]
		private bool _enableTriggerEnter2D=true;

		[SerializeField]
		private bool _enableTriggerStay2D=true;

		[SerializeField]
		private bool _enableTriggerExit2D = true;

		[SerializeField]
		private bool _enableCollisionEnter2D = true;

		[SerializeField]
		private bool _enableCollisionStay2D = true;

		[SerializeField]
		private bool _enableCollisionExit2D = true;
		#endregion

		private XLuaUGUIView _xluaUGUIView;
		//获取XLuaUGUIView
		private XLuaUGUIView GetXLuaUGUIView()
		{
			if (_xluaUGUIView == null)
			{
				_xluaUGUIView = GetComponentInParent<XLuaUGUIView>();
			}
			return _xluaUGUIView;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!_enableTriggerEnter2D)
				return;
			GetXLuaUGUIView()?.OnColliderTrigger("OnTriggerEnter2D", transform, collision.transform);
			// Vector2
		}

		private void OnTriggerStay2D(Collider2D collision)
		{
			if (!_enableTriggerStay2D)
				return;
			GetXLuaUGUIView()?.OnColliderTrigger("OnTriggerStay2D", transform, collision.transform);
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (!_enableTriggerExit2D)
				return;
			GetXLuaUGUIView()?.OnColliderTrigger("OnTriggerExit2D", transform, collision.transform);
		}


		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (!_enableCollisionEnter2D)
				return;
			GetXLuaUGUIView()?.OnColliderTrigger("OnCollisionEnter2D", transform, collision.transform);
		}

		private void OnCollisionStay2D(Collision2D collision)
		{
			if (!_enableCollisionStay2D)
				return;
			GetXLuaUGUIView()?.OnColliderTrigger("OnCollisionStay2D", transform, collision.transform);
		}

		private void OnCollisionExit2D(Collision2D collision)
		{
			if (!_enableCollisionExit2D)
				return;
			GetXLuaUGUIView()?.OnColliderTrigger("OnCollisionExit2D", transform, collision.transform);
		}
	}
}