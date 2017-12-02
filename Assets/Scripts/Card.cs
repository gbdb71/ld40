﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {

	private bool dragging = false;
	private float dragTime = 0f;
	private Vector3 dragPoint;

	public CardHolder currentHolder;

	private Vector3 startPoint;
	private Vector3 originalScale;

	private Collider2D coll;

	public LayerMask areaMask;
	private SortingLayer defaultLayer;

	public SpriteRenderer sprite;

	private float height = 0f;

	private Vector3 fromPosition, toPosition;
	private float moveDuration = -1f;
	private float moveSpeed = 5f;

	public Transform shadow;

	// Use this for initialization
	void Start () {
		originalScale = transform.localScale;
		coll = GetComponent<Collider2D> ();

//		sprite.color = new Color (Random.Range (0.5f, 1f), Random.Range (0.5f, 1f), Random.Range (0.5f, 1f));
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 lastPos = transform.position;

		if (dragging) {

			dragTime += Time.deltaTime;

			Vector3 mousePos = Input.mousePosition;
			mousePos.z = -Camera.main.transform.position.z + height;
			mousePos = Camera.main.ScreenToWorldPoint (mousePos);

			transform.position = new Vector3 (mousePos.x, mousePos.y, height) + dragPoint;

			if (LeftArea (1f) && currentHolder) {
				currentHolder.RemoveCard (this);
			}
		} else {
			transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.Euler (Vector3.zero), 1f);
		}

		if (moveDuration >= 0f && moveDuration <= 1f) {
			moveDuration += Time.deltaTime * moveSpeed;
			transform.position = Vector3.Lerp (fromPosition, toPosition, moveDuration);
		}

		Tilt (lastPos, transform.position);

		shadow.position = new Vector3 (transform.position.x, transform.position.y, -0.1f);
	}

	private void Tilt(Vector3 prevPos, Vector3 curPos) {
		float maxAngle = 10f;

		float xdiff = curPos.x - prevPos.x;
		xdiff = Mathf.Clamp (xdiff * 1000f, -maxAngle, maxAngle);

		float ydiff = curPos.y - prevPos.y;
		ydiff = Mathf.Clamp (ydiff * 1000f, -maxAngle, maxAngle);

		transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.Euler (new Vector3 (-ydiff, xdiff, 0)), 0.5f);
	}

//	public void OnMouseEnter() {
//		CardManager.Instance.cursor = 1;
//	}
//
//	public void OnMouseExit() {
//		CardManager.Instance.cursor = 0;
//	}

	public void OnMouseDown() {

//		CardManager.Instance.cursor = 1;

		dragging = true;
		dragTime = 0f;

		height = -1f;

		startPoint = transform.position;

		Vector3 point = Camera.main.ScreenPointToRay (Input.mousePosition).GetPoint(-Camera.main.transform.position.z - height);
		point.z = 0;

		dragPoint = transform.position - point;
	}

	public void OnMouseUp() {

//		CardManager.Instance.cursor = 0;

		dragging = false;

		height = 0f;

		if (dragTime < 0.25f && !LeftArea(1.2f)) {
			currentHolder.RemoveCard (this);
			currentHolder.targetHolder.AddCard (this, false);
			return;
		} else {
			
			Collider2D hit = Physics2D.OverlapBox (transform.position, coll.bounds.size, 0, areaMask);

			if (hit) {
				CardHolder holder = hit.GetComponent<CardHolder> ();
				holder.AddCard (this, false);
				return;
			}
		}
			
		currentHolder.AddCard (this, true);
	}

	private bool LeftArea(float distance) {
		return (transform.position - startPoint).magnitude > distance;
	}

	public void Move(Vector3 pos) {
		fromPosition = transform.position;
		toPosition = pos;
		moveDuration = 0f;
	}
}