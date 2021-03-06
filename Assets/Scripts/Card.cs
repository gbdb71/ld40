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

	private float moveSpeed = 3.5f;
	private float normalMoveSpeed = 3.5f;

	public Transform shadow;

	private BlockMatrix blockMatrix;

	public bool isMatrix = true;
	private int operation = -1;
	public SpriteRenderer operationSprite;

	private Vector3 shadowScale;

	// Use this for initialization
	void Start () {
		originalScale = transform.localScale;
		coll = GetComponent<Collider2D> ();

		if (isMatrix) {
			blockMatrix = GetComponentInChildren<BlockMatrix> ();

			if (Manager.Instance.StarterMatrix ()) {
				blockMatrix.FillStarter ();
			} else {
				blockMatrix.FillNormal ();
			}
		} else {
			operation = Random.Range (0, Manager.Instance.OperatorMax());
			operationSprite.sprite = Manager.Instance.operationSprites [operation];

			if (operation == 2) {
				ProgressManager.Instance.MultiTutorial();
			}
		}

		shadowScale = shadow.localScale;

//		sprite.color = new Color (Random.Range (0.5f, 1f), Random.Range (0.5f, 1f), Random.Range (0.5f, 1f));
	}

	public void SetSpeed(float s) {
		moveSpeed = s;
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
		float offset = dragging ? 0.1f : 0f;

		shadow.position = new Vector3 (transform.position.x, transform.position.y, dragging ? -0.1f : 0f);
		shadow.localScale = dragging ? shadowScale * 1.1f : shadowScale;
	}

	private void Tilt(Vector3 prevPos, Vector3 curPos) {
		float maxAngle = 10f;

		float xdiff = curPos.x - prevPos.x;
		xdiff = Mathf.Clamp (xdiff * 1000f, -maxAngle, maxAngle);

		float ydiff = curPos.y - prevPos.y;
		ydiff = Mathf.Clamp (ydiff * 1000f, -maxAngle, maxAngle);

		transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.Euler (new Vector3 (-ydiff, xdiff, 0)), 0.5f);
	}

	public void OnMouseEnter() {
		CursorManager.Instance.hovering = true;
		PulseTarget (true);
	}

	public void OnMouseExit() {
		CursorManager.Instance.hovering = false;
		PulseTarget (false);
	}

	private void PulseTarget(bool state) {

		if (currentHolder.cardType == -1) {
			currentHolder.targetHolders [isMatrix ? 0 : 1].Pulse (state);
		}
	}

	public void OnMouseDown() {

		moveSpeed = normalMoveSpeed + Random.Range(-0.5f, 0.5f);

		if (!Manager.Instance.CanInteract ()) {
			return;
		}

		AudioManager.Instance.PlayEffectAt (3, transform.position, 0.5f);

		dragging = true;
		dragTime = 0f;

		height = -1f;

		startPoint = transform.position;

		Vector3 point = Camera.main.ScreenPointToRay (Input.mousePosition).GetPoint(-Camera.main.transform.position.z - height);
		point.z = 0;

		dragPoint = transform.position - point;
	}

	public void OnMouseUp() {

		if (!Manager.Instance.CanInteract ()) {
			return;
		}

		dragging = false;

		height = 0f;

		int type = isMatrix ? 0 : 1;

		if (dragTime < 0.25f && !LeftArea(1.2f)) {
			UseCard ();
			return;
		} else {
			
			Collider2D hit = Physics2D.OverlapBox (transform.position, coll.bounds.size, 0, areaMask);

			if (hit) {
				CardHolder holder = hit.GetComponent<CardHolder> ();
				if (holder.Allows (type)) {
					holder.AddCard (this, false);
					return;
				}
			}

			AudioManager.Instance.PlayEffectAt (6, transform.position, 0.5f);
		}
			
		currentHolder.AddCard (this, true);
	}

	public void UseCard(float delay = 0f) {
		Invoke ("UseCard", delay);
		Invoke ("UseSound", delay);
	}

	public void UseSound() {
		AudioManager.Instance.PlayEffectAt (3, transform.position, 0.5f);
	}

	public void UseCard() {
		int type = isMatrix ? 0 : 1;
		currentHolder.RemoveCard (this);
		currentHolder.targetHolders[type].AddCard (this, false);

		AudioManager.Instance.PlayEffectAt (6, transform.position, 0.5f);
	}

	private bool LeftArea(float distance) {
		return (transform.position - startPoint).magnitude > distance;
	}

	public void Move(Vector3 pos) {
		fromPosition = transform.position;
		toPosition = pos;
		moveDuration = 0f;
	}

	public Matrix GetMatrix() {
		return blockMatrix.GetMatrix ();
	}

	public int GetOperation() {
		return operation;
	}
}
