﻿using UnityEngine;
using System.Collections;
using MicrosoftResearch.Infer.Models;
using MicrosoftResearch.Infer;

public class GameHandler : MonoBehaviour {

	public Player p1;
	public Player p2;
	
	Player activePlayer;
	Player inactivePlayer;

	public GUIText endMessage;
	bool gameOver = false;

	public static GameHandler Instance;

	/*
	 * 3*16*16 matrix
	 * [context, attacker, defender]
	 * 0 : Challenge not possible, cannot be initiated
	 * 1 : Attacker wins, defender's card is discarded
	 * 2 : Defender wins, attacker's card is discarded
	 * 3 : Both cards are discarded
	 */
	static int[,,] challengeTable = new int[,,] {{ 
		//Field to Field
		//                            |P|    
		//                            |E|    
		//        |P|       |D|       |R|    
		//        |O|   |C|C|I|   |P|P|S|
		//        |S|A| |Y|E|O| |M|A|E|E|P|S|
		//    |A| |E|P|G|C|N|N|H|E|N|G|P|Y|I|
		//  |Z|R|H|I|O|I|L|T|Y|A|D|D|A|H|T|R|
		//  |E|G|E|D|L|A|O|A|S|D|U|O|S|O|H|E|
		//  |U|U|R|O|L|N|P|U|U|E|S|R|U|N|I|N|
		//  |S|S|O|N|O|T|S|R|S|S|A|A|S|E|A|S|
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, //ZEUS
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, //ARGUS
			{0,1,3,2,2,2,2,2,0,0,1,3,1,0,1,0}, //HERO
			{0,1,1,3,1,1,1,1,0,0,2,3,1,0,1,0}, //POSEIDON
			{0,1,1,2,3,1,1,1,0,0,2,3,1,0,1,0}, //APOLLO
			{0,1,1,2,2,3,1,1,0,0,2,3,1,0,1,0}, //GIANT
			{0,1,1,2,2,2,3,1,0,0,2,3,1,0,1,0}, //CYCLOPS
			{0,1,1,2,2,2,2,3,0,0,2,3,1,0,1,0}, //CENTAUR
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, //DIONYSUS
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, //HADES
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, //MEDUSA
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, //PANDORA
			{0,1,2,2,2,2,2,2,0,0,2,3,3,0,1,0}, //PEGASUS
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, //PERSEPHONE
			{0,3,2,2,2,2,2,2,0,0,2,3,2,0,3,0}, //PYTHIA
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}},//SIRENS
		
		   {{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},//Hand to Hand
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,3,2,2,2,2,2,2,3,3,3,3,3,3,3,3}, //If PEGASUS picks the cards with strength 3-7, opponent places the picked card in a 1st row face up. If there are no field spot to put it, it is discarded.
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{2,2,2,3,2,2,2,2,2,2,2,2,2,2,2,2},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}},
		
		   {{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},//Hand to Field
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{3,2,2,2,2,2,2,2,0,0,3,2,3,2,2,2},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}},

		   {{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},//Field to Hand
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}}};
	
	// Use this for initialization
	void Start () {
		Instance = this;
		activePlayer = p2;
		inactivePlayer = p1;
		endMessage.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {
		if (gameOver){
			if (Input.GetMouseButtonDown(0)){
				Application.LoadLevel(Application.loadedLevel);
			}
		}
		else{
			if (p1.setupPhase || p2.setupPhase){
				p1.SetupField();
				p2.SetupField();
			}
			else if (activePlayer.actionPoints <= 0)
				SwitchPlayer();
			else activePlayer.CheckInput();

			if (activePlayer.actionPoints > 0){//check to see if the active player has remaining action points and no moves
				if (activePlayer.hand.Count ==0 && activePlayer.drawPile.Count == 0){
					bool canAttack = false;
					foreach (FieldSpot spot in activePlayer.playField){
						if (spot.row == 0 && spot.card != null){
							if (spot.card.type != CardType.MEDUSA && spot.card.type != CardType.PANDORA && spot.card.type != CardType.ZEUS){
								canAttack = true;
							}
						}
					}
					if (!canAttack) EndGame(inactivePlayer);
				}
			}
		}
	}

	public void EndGame(Player winner){
		endMessage.enabled = true;
		endMessage.text = (winner.name + "\nWins!");
		gameOver = true;
	}

	public void SwitchPlayer(){
		if (activePlayer == p1){
			p2.selectedCard = null;
			activePlayer = p2;
			inactivePlayer = p1;
			p2.BeginTurn();
		}
		else {
			p1.selectedCard = null;
			activePlayer = p1;
			inactivePlayer = p2;
			p1.BeginTurn();
		}

		if (activePlayer.actionPoints < 1){
			EndGame(inactivePlayer);
		}

	}

	public int Challenge(int context, Card attacker, Card defender){
		int result = challengeTable[context, (int)attacker.type, (int)defender.type];
		Debug.Log ("Challenge!" + "\nAttacker: " + attacker + "  Defender: " + defender + "  Resolution: ");

		//SPECIAL CASES
		if (defender.type == CardType.PANDORA){
			Debug.Log("Special Case: Pandora is Challenged");
			//if pandora is on the field, discard all from that column
			if (inactivePlayer.FindOnField(defender)){
				while(inactivePlayer.playField[0,defender.spot.col].card != null){
					inactivePlayer.Discard(inactivePlayer.playField[0,defender.spot.col].card);
				}
				while(activePlayer.playField[0,2-defender.spot.col].card != null){
					activePlayer.Discard(activePlayer.playField[0,2-defender.spot.col].card);
				}
			}
			
			//if pandora is in hand, discard all in hand
			else if (inactivePlayer.hand.Contains(defender)){
				while(inactivePlayer.hand.Count > 0){
					inactivePlayer.Discard(inactivePlayer.hand[0]);
				}
			}
		}

		if (attacker.type == CardType.PYTHIA && context == 1){
			Debug.Log("Special Case: Pythia reveals opponent's hand");
			activePlayer.actionPoints ++;
			Card target = null;
			activePlayer.phase = MythPhase.PYTHIA;
			foreach (Card c in inactivePlayer.hand){
				if (c.type == CardType.POSEIDON){
					target = c;
				}
				c.Flip(true);
				//c.Reveal(true);

			}
			if (target!=null){
				inactivePlayer.Discard(target);
			}
		}

		if (defender.type == CardType.ARGUS && result != 0){
			Debug.Log("Special Case: Argus has been attacked");
			EndGame(activePlayer);
		}


		switch(result) {
		case 0:
			Debug.Log("Invalid Challenge");
			break;
		case 1:
			Debug.Log(attacker + " Wins");
			inactivePlayer.Discard(defender);
			break;
		case 2:
			Debug.Log(defender + " Wins");
			activePlayer.Discard(attacker);
			break;
		case 3:
			Debug.Log("Both Discarded");
			inactivePlayer.Discard(defender);
			activePlayer.Discard(attacker);
			break;
		
		default:
			Debug.Log("Something is very wrong");
			break;
		}



		return result;
	}

	public void EndPythiaPhase(){
		foreach (Card c in inactivePlayer.hand){
			c.Flip(inactivePlayer.showHand);
			//Debug.Log(inactivePlayer.showHand);
		}
		Player.Shuffle(inactivePlayer.hand);
		inactivePlayer.ArrangeHand();
		//inactivePlayer
	}
}
