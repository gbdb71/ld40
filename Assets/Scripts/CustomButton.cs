﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour {

	private Vector3 originalScale, targetScale;
	private bool hovering = false;
	private Vector3 hiddenScale = new Vector3(1.1f, 0f, 1f);

	public UnityEvent clickEvent;

	void Start() {
		originalScale = transform.localScale;
		targetScale = hiddenScale;
		transform.localScale = targetScale;
	}

	public void OnMouseEnter() {
		hovering = true;
	}

	public void OnMouseExit() {
		hovering = false;
	}

	public void OnMouseDown() {
	}

	public void OnMouseUp() {
//		Manager.Instance.Calculate ();
		clickEvent.Invoke ();
	}

	void Update() {
		float mod = hovering ? 1.1f : 1.0f;
		transform.localScale = Vector3.MoveTowards (transform.localScale, targetScale * mod, Time.deltaTime * 6f);
	}

	public void ChangeVisibility(bool visible) {
		targetScale = visible ? originalScale : hiddenScale;
	}
}
