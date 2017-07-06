using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour{
	List<Player> players;

	Player impulsivo;
	Player exigente;
	Player cauteloso;
	Player aleatorio;

	Property[] propertiesInput;

	int roundNumber;
	int matchNumber;

	int countWinnerOverThousandRounds = 0;
	int roundsByMatch = 0;
	int countWinnerImpulsivo = 0;
	int countWinnerExigente = 0;
	int countWinnerCauteloso = 0;
	int countWinnerAleatorio = 0;

	public Text timeout;
	public Text mediaTurn;
	public Text resultImpulsivo;
	public Text resultExigente;
	public Text resultCauteloso;
	public Text resultAleatorio;
	public Text biggestWinnerText;

	Property[] ReadPropertiesFile(){
		string[] propertiesFile = File.ReadAllLines (Application.dataPath+"/gameConfig.txt");
		string regex = @"(\d+)\s+(\d+)";

		for(int i=0; i<20; i++){
			foreach (Match m in Regex.Matches(propertiesFile[i], regex)) {
				int costRead = System.Int32.Parse(m.Groups[1].Value);
				int rentRead = System.Int32.Parse(m.Groups[2].Value);

				propertiesInput.SetValue (new Property (costRead, rentRead), i);
			}
		}
		return propertiesInput;
	}


	List<Player> randomOrderRound(){
		impulsivo = new Impulsivo ();
		exigente = new Exigente ();
		cauteloso = new Cauteloso ();
		aleatorio = new Aleatorio ();

		players.Add (impulsivo);
		players.Add (exigente);
		players.Add (cauteloso);
		players.Add (aleatorio);

		List<Player> randomizedList = new List<Player>();
		System.Random rnd = new System.Random();
		while (players.Count > 0){
			int index = rnd.Next(0, players.Count); //pick a random item from the master list
			randomizedList.Add(players[index]); //place it at the end of the randomized list
			players.RemoveAt(index);
		}

		return randomizedList;
	}

	public void execute(){
		countWinnerOverThousandRounds = 0;
		roundsByMatch = 0;
		countWinnerImpulsivo = 0;
		countWinnerExigente = 0;
		countWinnerCauteloso = 0;
		countWinnerAleatorio = 0;

		players = new List<Player> ();

		propertiesInput = new Property[20];
		propertiesInput = ReadPropertiesFile ();

		Debug.Log ("-----Start------");
		matchNumber = 0;

		while (matchNumber < 300) {
			List<Player> orderByRound = new List<Player> ();
			orderByRound = randomOrderRound ();

			roundNumber = 0;

			Player winner = null;
			float maxCoins = 0;

			while (roundNumber < 1000) {
				foreach (Player currentPlayer in orderByRound) {
					int currentPropertyPosition = currentPlayer.getCurrentPropertyPosition ();
					int dice = currentPlayer.playDice ();

//					Debug.Log ("Count: "+ orderByRound.Count);
//
//
//					Debug.Log ("Round nº: "+ roundNumber);
//					Debug.Log ("\nPlayer: "+ currentPlayer);
//					Debug.Log ("\nNº dado: "+ dice);
//					Debug.Log ("\nNº casa antes: "+ currentPropertyPosition);

					if ((currentPropertyPosition + dice) < 20) {
						currentPropertyPosition = currentPropertyPosition + dice;
					} else {
						currentPropertyPosition = (currentPropertyPosition + dice) - 20;
					}
						
//					Debug.Log ("\nNº casa depois: "+ currentPropertyPosition);
//					Debug.Log ("\nDono da casa: "+ propertiesInput [currentPropertyPosition].getOwner());

					if (!(propertiesInput[currentPropertyPosition].hasOwner ())) {
						if(currentPlayer.authorizePayment((propertiesInput [currentPropertyPosition].getCost()))){
							currentPlayer.BuyProperty (propertiesInput [currentPropertyPosition]);
//							Debug.Log ("\nComprou: "+ propertiesInput [currentPropertyPosition].getCost());
						}
					} else {
						if (currentPlayer.authorizePayment ((propertiesInput [currentPropertyPosition].getRent ()))) {
							currentPlayer.payRent (propertiesInput [currentPropertyPosition]);
//							Debug.Log ("\nPagou: "+ propertiesInput [currentPropertyPosition].getRent());
						} else {

							currentPlayer.setCoins (currentPlayer.getCoins () - propertiesInput [currentPropertyPosition].getRent ());
							currentPlayer.lostProperties (currentPlayer);
//							Debug.Log ("\nZerou: "+ currentPlayer.getCoins());



						}
					}
					currentPlayer.setCurrentPropertyPosition (currentPropertyPosition);
				}

				if (orderByRound.Count == 1) {
					
					winner = orderByRound[0];
					countWinner (winner);
					Debug.Log ("Vencedor: " + winner);
					break;
				}

				for (int i =0;i<orderByRound.Count;i++) {
//					Debug.Log ("Coins: " +orderByRound [i].getCoins ());
					if (orderByRound [i].getCoins () < 0) {
						orderByRound.RemoveAt (i);
					} else {
						orderByRound [i].bonusRound ();
					}

				}
				roundNumber++;
			}

			roundsByMatch += roundNumber;
			if (orderByRound.Count > 1) {
				foreach (Player finalist in orderByRound) {
					if (finalist.getCoins () > maxCoins) {
						maxCoins = finalist.getCoins ();
						winner = finalist;
					}

				}
				Debug.Log ("Vencedor: " + winner);
				Debug.Log ("Vencedor Coins: " + winner.getCoins());
				countWinner (winner);
				countWinnerOverThousandRounds++;
			}
			matchNumber++;
		}

		Debug.Log ("-------------------------------------------------");
		Debug.Log ("Rounds por turno: " + (roundsByMatch/matchNumber));
		mediaTurn.text = (roundsByMatch / matchNumber).ToString();
		Debug.Log ("Rounds com timeout: " + countWinnerOverThousandRounds);
		timeout.text = countWinnerOverThousandRounds.ToString ();

		Debug.Log ("Nº Impulsivo: " + countWinnerImpulsivo);
		resultImpulsivo.text = ((float)countWinnerImpulsivo/(float)matchNumber)*100+"%";
		Debug.Log ("Nº Exigente: " + countWinnerExigente);
		resultExigente.text = ((float)countWinnerExigente/(float)matchNumber)*100+"%";
		Debug.Log ("Nº Cauteloso: " + countWinnerCauteloso);
		resultCauteloso.text = ((float)countWinnerCauteloso/(float)matchNumber)*100+"%";
		Debug.Log ("Nº Aleatório: " + countWinnerAleatorio);
		resultAleatorio.text = ((float)countWinnerAleatorio/(float)matchNumber)*100+"%";

		string biggestWinner = setBiggestWinner ();
		Debug.Log (biggestWinner);
		if (biggestWinner.Contains ("impulsivo")) {
			biggestWinnerText.text = "Impulsivo";
		} else if(biggestWinner.Contains ("exigente")){
			biggestWinnerText.text = "Exigente";
		} else if(biggestWinner.Contains ("cauteloso")){
			biggestWinnerText.text = "Cauteloso";
		}else if(biggestWinner.Contains ("aleatorio")){
			biggestWinnerText.text = "Aleatório";
		}
	}

	void countWinner(Player winner){
		if (winner.GetType ().ToString ().Equals ("Impulsivo")) {
			countWinnerImpulsivo++;
		}else if(winner.GetType ().ToString().Equals("Cauteloso")){
			countWinnerCauteloso++;
		}else if(winner.GetType ().ToString().Equals("Aleatorio")){
			countWinnerAleatorio++;
		}else if(winner.GetType ().ToString().Equals("Exigente")){
			countWinnerExigente++;
		}
	}

	string setBiggestWinner(){
		Dictionary<int, string> dictionaryScores = new Dictionary<int,string> ();

		dictionaryScores.Add (countWinnerImpulsivo, "impulsivo");
		dictionaryScores.Add (countWinnerExigente, "exigente");
		dictionaryScores.Add (countWinnerCauteloso, "cauteloso");
		dictionaryScores.Add (countWinnerAleatorio, "aleatorio");

		int[] counts = new int[4] {countWinnerAleatorio, countWinnerCauteloso, countWinnerExigente, countWinnerImpulsivo};
		int biggestVictories = Mathf.Max(counts);

		string theBiggestWinner;
		bool hasValue = dictionaryScores.TryGetValue(biggestVictories, out theBiggestWinner);
		Debug.Log ("Big: " + theBiggestWinner);
		if (hasValue) {
			return theBiggestWinner;
		}
		return "";
	}
}
