﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

	public SpriteRenderer numberSprite;
	public SpriteRenderer colorSprite;
	public Animator anim;
	public Face face;

	private int number = 0;

	public void SetNumber(int num) {
		numberSprite.sprite = ProgressManager.Instance.numberSprites [num];
		colorSprite.color = BlockColor(num);
	}

	public static Color BlockColor(int idx) {
		float baseSat = 0.6f;
		float baseVal = 0.9f;

		Color[] blockColors = {
			new Color(0.6f, 0.6f, 0.6f),
			Color.HSVToRGB(0f, baseSat, baseVal),
			Color.HSVToRGB(0.1f, baseSat, baseVal),
			Color.HSVToRGB(0.2f, baseSat, baseVal),
			Color.HSVToRGB(0.3f, baseSat, baseVal),
			Color.HSVToRGB(0.4f, baseSat, baseVal),
			Color.HSVToRGB(0.5f, baseSat, baseVal),
			Color.HSVToRGB(0.6f, baseSat, baseVal),
			Color.HSVToRGB(0.7f, baseSat, baseVal),
			Color.HSVToRGB(0.8f, baseSat, baseVal)
		};

		return blockColors [idx];
	}

	public static Color TextColor(int idx) {
		float baseSat = 0.6f;
		float baseVal = 0.8f;

		Color[] blockColors = {
			new Color(0.5f, 0.5f, 0.5f),
			Color.HSVToRGB(0f, baseSat, baseVal),
			Color.HSVToRGB(0.1f, baseSat, baseVal),
			Color.HSVToRGB(0.2f, baseSat, baseVal),
			Color.HSVToRGB(0.3f, baseSat, baseVal),
			Color.HSVToRGB(0.4f, baseSat, baseVal),
			Color.HSVToRGB(0.5f, baseSat, baseVal),
			Color.HSVToRGB(0.6f, baseSat, baseVal),
			Color.HSVToRGB(0.7f, baseSat, baseVal),
			Color.HSVToRGB(0.8f, baseSat, baseVal)
		};

		return blockColors [idx];
	}

	public static string HexColor(int idx) {
		Color c = BlockColor (idx);
		string hiliteColor = "#" + ColorUtility.ToHtmlStringRGB (c);
		return hiliteColor;
	}

	public void Pulse() {
		anim.speed = 0.5f;
		anim.SetBool ("pulse", true);
		face.Emote (Face.Emotion.Happy);
	}

	public void PulseOnce() {
		anim.SetBool ("pulse", true);
		face.Emote (Face.Emotion.Happy, Face.Emotion.Default, 2f);
		Invoke ("CancelPulse", 0.1f);
	}

	public void CancelPulse() {
		anim.SetBool ("pulse", false);
	}

	public void NumberPulse(float delay, int num) {
		number = num;
		Invoke ("DoNumberPulse", delay);
	}

	void DoNumberPulse() {
		AudioManager.Instance.PlayEffectAt (14, transform.position, 0.25f);
		anim.SetTrigger ("numberpulse");
		SetNumber (number);
	}

	public void PulseSound() {
		AudioManager.Instance.PlayEffectAt (9, Vector3.zero, 0.15f);
	}
}
