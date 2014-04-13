﻿using UnityEngine;
using System;
using System.Collections;

public enum CardType {
	HERA, IO, AMAZON, NEMESIS, ARTEMIS, HYDRA, HARPY, FURY,		// HERA cards
	ZEUS, ARGUS, HERO, POSEIDON, APOLLO, GIANT, CYCLOPS, CENTAUR,	// ZEUS cards
	DIONYSUS, HADES, MEDUSA, PANDORA, PEGASUS, PERSEPHONE, PYTHIA, SIRENS,	// common cards
	NONE
}

public class Card : MonoBehaviour {
	public static float width = 5;
	public static float height = 7;

	public CardType type;
	public string title;
	public int strength;
	public bool special;

	public bool moving;
	public bool inField = false;
	public bool flipping;
	public bool isFlipped = false;

	public bool typeSet = false;
	Vector3 goalRotation;
	Vector3 destination;
	public bool revealed = false;

	public FieldSpot spot;

	public TextMesh titleText;
	public TextMesh strengthText;
	public TextMesh specialText;

	// Use this for initialization
	void Start () {
		//type = CardType.NONE;
		name = type.ToString();
		this.transform.eulerAngles = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (moving) {
			this.transform.position = Vector3.Lerp(this.transform.position, destination, 0.1f);
			if (Vector3.Distance(this.transform.position, destination) <= 0.1f) {
				this.transform.position = destination;
				moving = false;
				inField = true;
			}
		}

		if (flipping) {
			if (isFlipped) {
				this.transform.eulerAngles = Vector3.Lerp(this.transform.eulerAngles, new Vector3(0, 0, 0), 0.1f);
				if ((this.transform.eulerAngles - new Vector3(0, 0, 0)).magnitude <= 0.01) {
					SetFlip(false);
				}
			} else {
				this.transform.eulerAngles = Vector3.Lerp(this.transform.eulerAngles, new Vector3(0, 0, 180), 0.1f);
				if ((this.transform.eulerAngles - new Vector3(0, 0, 180)).magnitude <= 0.01) {
					SetFlip(true);
				}
			}
		}

		if (type != CardType.NONE && !typeSet) {
 			SetType(type);
 			typeSet = true;
		}

		DetermineTextVisibility();
	}

	private void DetermineTextVisibility() {
		bool revealed = false;
		if (this.transform.eulerAngles.z > 90 && this.transform.eulerAngles.z < 270) {
			revealed = true;
		}

		titleText.active = revealed;
		strengthText.active = revealed;
		specialText.active = revealed;
	}

	public void MoveTo(Vector3 dest){
		moving = true;
		destination = dest;
	}

	public void Flip() {
		flipping = true;
	}

	public void Flip(bool flipBool) {
		// only flips the card if it isn't in desired
		if (isFlipped != flipBool) {
			flipping = true;
		}
	}

	public void SetFlip(bool flipBool) {
		// immediately flips the card to the desired state
		flipping = false;
		isFlipped = flipBool;
		if (isFlipped) {
			this.transform.eulerAngles = new Vector3(0, 0, 180);
		} else {
			this.transform.eulerAngles = new Vector3(0, 0, 0);
		}
	}

	public void SetType(CardType cardType) {
		this.type = cardType;
		this.name = type.ToString();
		this.title = type.ToString();
		switch(type) {
			case CardType.HERA:
				this.strength = -1;
				this.special = true;
				break;
			case CardType.IO:
				this.strength = 0;
				this.special = true;
				break;
			case CardType.AMAZON:
				this.strength = 2;
				this.special = true;
				break;
			case CardType.NEMESIS:
				this.strength = 7;
				this.special = false;
				break;
			case CardType.ARTEMIS:
				this.strength = 6;
				this.special = false;
				break;
			case CardType.HYDRA:
				this.strength = 5;
				this.special = false;
				break;
			case CardType.HARPY:
				this.strength = 4;
				this.special = false;
				break;
			case CardType.FURY:
				this.strength = 3;
				this.special = false;
				break;
			case CardType.ZEUS:
				this.strength = -1;
				this.special = true;
				break;
			case CardType.ARGUS:
				this.strength = 0;
				this.special = true;
				break;
			case CardType.HERO:
				this.strength = 2;
				this.special = true;
				break;
			case CardType.POSEIDON:
				this.strength = 7;
				this.special = false;
				break;
			case CardType.APOLLO:
				this.strength = 6;
				this.special = false;
				break;
			case CardType.GIANT:
				this.strength = 5;
				this.special = false;
				break;
			case CardType.CYCLOPS:
				this.strength = 4;
				this.special = false;
				break;
			case CardType.CENTAUR:
				this.strength = 3;
				this.special = false;
				break;
			case CardType.DIONYSUS:
				this.strength = -1;
				this.special = true;
				break;
			case CardType.HADES:
				this.strength = -1;
				this.special = true;
				break;
			case CardType.MEDUSA:
				this.strength = 0;
				this.special = true;
				break;
			case CardType.PANDORA:
				this.strength = 0;
				this.special = true;
				break;
			case CardType.PEGASUS:
				this.strength = 1;
				this.special = true;
				break;
			case CardType.PERSEPHONE:
				this.strength = -1;
				this.special = true;
				break;
			case CardType.PYTHIA:
				this.strength = 0;
				this.special = true;
				break;
			case CardType.SIRENS:
				this.strength = -1;
				this.special = true;
				break;
			default:
				this.title = "ERROR";
				this.strength = -1;
				this.special = false;
				break;
		}

		titleText.text = this.title;
		strengthText.text = (this.strength == -1) ? " " : this.strength.ToString();
		specialText.text = (this.special) ? "X" : " ";
	}
}
