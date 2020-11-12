using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wanderer.GameFramework;
using DG.Tweening;

public class MainUIView : UGUIView
{
	public override void OnEnter(IUIContext uiConext, Action<string> callBack = null, params object[] parameters)
	{
		base.OnEnter(uiConext, callBack, parameters);

		transform.Find("Panel/Button").GetComponent<Button>().onClick.AddListener(
			() => {
				Call("Next");
			}
			);
	}

	public override void OnExit(IUIContext uiConext)
	{
		base.OnExit(uiConext);
		transform.Find("Panel/Button").GetComponent<Button>().onClick.RemoveAllListeners();
	}

}

public class MainUIContext : IUIContext
{
	public string Name => "Main";

	public string AssetPath => $"Assets/Game/UI/Canvas-Main.prefab";

	public bool Multiple => false;

	public IUIContext Clone()
	{
		return this;
	}
}


public class MainUIAnimation : IUIAnimation
{
	public UIView TargetUIView { get; set; }
	private RectTransform _uiRoot;

	public async UniTask<int> Run()
	{
		await _uiRoot.DOAnchorPosX(1920.0f, 5);
		//float localX = _uiRoot.transform.localPosition.x;
		//await _uiRoot.DOLocalMoveX(2340, 1.0f);
		//await TestEnumerator();
		//await _uiRoot.DOLocalMoveX(localX, 1.0f);
		//_uiRoot.GetComponent<CanvasGroup>().alpha = 0.15f;
		//await System.Threading.Tasks.Task.Delay(1000 * 3);
		//await UniTask.DelayFrame(1000);
		//_uiRoot.GetComponent<CanvasGroup>().alpha = 0.98f;
		return 1;
	}

	IEnumerator TestEnumerator()
	{
		yield return new WaitForSeconds(1.0f);
	}

	public void SetTarget(GameObject target)
	{
		_uiRoot = target.GetComponent<RectTransform>();
	}

	
}