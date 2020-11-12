using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using Wanderer.GameFramework;
using UnityEngine.UI;
using DG.Tweening;

public class TestUIView : UGUIView
{

}


public class TestUIContext : IUIContext
{
	public string Name => "Test";

	public string AssetPath => $"Assets/Game/UI/Canvas-Test.prefab";

	public bool Multiple => false;

	public IUIContext Clone()
	{
		return this;
	}
}


public class TestUIAnimation : IUIAnimation
{
	public UIView TargetUIView { get; set; }
	private RectTransform _uiRoot;

	public async UniTask<int> Run()
	{
		await _uiRoot.DOAnchorPosX(0.0f, 5);
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