using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Wanderer.GameFramework
{
	public class XLuaEventTrigger : EventTrigger
	{
		private XLuaUGUIView _xluaUGUIView;

		private readonly Dictionary<EventTriggerType,string> _triggerTypes = new Dictionary<EventTriggerType,string>();

		//是否拖拽
		private bool _isDrag = false;

		private void OnEnable()
		{
			_triggerTypes.Clear();
			for (int i = 0; i < triggers.Count; i++)
			{
				EventTriggerType triggerType = triggers[i].eventID;
				_triggerTypes[triggerType] = $"On{triggerType}";
			}
			_isDrag = false;
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.PointerEnter, eventData);
		}
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.PointerExit, eventData);
		}
		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.PointerDown, eventData);
		}
		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.PointerUp, eventData);

			//EndDrag  要比PointerUp晚
			if (_isDrag)
			{
				OnEndDrag(eventData);
			}
		}
		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.PointerClick, eventData);
		}
		public override void OnDrag(PointerEventData eventData)
		{
			base.OnDrag(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.Drag, eventData);
		}
		public override void OnDrop(PointerEventData eventData)
		{
			base.OnDrop(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.Drop, eventData);
		}
		public override void OnScroll(PointerEventData eventData)
		{
			base.OnScroll(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.Scroll, eventData);
		}
		public override void OnUpdateSelected(BaseEventData eventData)
		{
			base.OnUpdateSelected(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.UpdateSelected, eventData);
		}
		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.Select, eventData);
		}
		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.Deselect, eventData);
		}
		public override void OnMove(AxisEventData eventData)
		{
			base.OnMove(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.Move, eventData);
		}
		public override void OnInitializePotentialDrag(PointerEventData eventData)
		{
			base.OnInitializePotentialDrag(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.InitializePotentialDrag, eventData);
		}
		public override void OnBeginDrag(PointerEventData eventData)
		{
			base.OnBeginDrag(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.BeginDrag, eventData);
			_isDrag = true;
		}
		public override void OnEndDrag(PointerEventData eventData)
		{
			if (_isDrag)
			{
				base.OnEndDrag(eventData);
				ExecuteXLuaEventTrigger(EventTriggerType.EndDrag, eventData);
				_isDrag = false;
			}
		}
		public override void OnSubmit(BaseEventData eventData)
		{
			base.OnSubmit(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.Submit, eventData);
		}
		public override void OnCancel(BaseEventData eventData)
		{
			base.OnCancel(eventData);
			ExecuteXLuaEventTrigger(EventTriggerType.Cancel, eventData);
		}
	

		#region 内部函数

		//触发xlua的EventTrigger
		private void ExecuteXLuaEventTrigger(EventTriggerType triggerType, BaseEventData eventData)
		{
			if (_triggerTypes.ContainsKey(triggerType))
			{
				GetXLuaUGUIView()?.OnEventTrigger(transform, _triggerTypes[triggerType], eventData);
			}
		}

		//获取XLuaUGUIView
		private XLuaUGUIView GetXLuaUGUIView()
		{
			if (_xluaUGUIView == null)
			{
				_xluaUGUIView = GetComponentInParent<XLuaUGUIView>();
			}
			return _xluaUGUIView;
		}
		#endregion

	}
}