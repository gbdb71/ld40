﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelectBlock : MonoBehaviour {

	private Block block;
	public int number = 0;
	public ColorPicker picker;

	// Use this for initialization
	void Start () {
		block = GetComponent<Block> ();

		block.SetNumber (number);
	}

	public void OnMouseEnter() {
		CursorManager.Instance.pointing = true;
		block.face.Emote (Face.Emotion.Brag);

		if (picker) {
			picker.SetColor (number);
		}

		block.NumberPulse (0f, number);
	}

	public void OnMouseExit() {
		CursorManager.Instance.pointing = false;
	}

	public void OnMouseDown() {
	}

	public void OnMouseUp() {
		if (picker && !picker.locked) {
			AudioManager.Instance.PlayEffectAt (1, transform.position, 0.5f);
			block.PulseOnce ();
			picker.Choose (number);
		}
	}
}
