using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player {
	protected float _coins = 300;

	private int _currentPropertyPosition;

	protected List<Property> properties = new List<Property> ();

	public abstract void BuyProperty (Property property);

	public void buy(Property property){
		_coins -= property.getCost();
		property.setOwner (this);
		properties.Add (property);
	}

	public void payRent(Property property){
		_coins -= property.getRent();
		property.getOwner()._coins += property.getRent();
	}

	public void lostProperties(Player player){
		foreach (Property property in player.properties) {
			property.setOwner (null);
		}
		player.properties.Clear();
	}


	//colocar em controller
	public int playDice(){
		return Random.Range (1, 6);
	}

	public int getCurrentPropertyPosition(){
		return _currentPropertyPosition;
	}

	public void setCurrentPropertyPosition(int currentPropertyPosition){
		_currentPropertyPosition = currentPropertyPosition;
	}

	public bool authorizePayment(float value){
		if (_coins < value) {
			return false;
		}else{
			return true;
		}
	}

	public void bonusRound(){
		_coins += 100;
	}

	public float getCoins(){
		return _coins;
	}

	public void setCoins(float coins){
		_coins = coins;
	}
}


public class Impulsivo : Player{
	public override void BuyProperty (Property property){
		if (property.getCost () < _coins) {
			buy (property);
		}
	}
}
	
public class Exigente : Player{
	public override void BuyProperty (Property property){
		if (property.getCost () >= 50) {
			buy (property);
		}
	}
}

public class Cauteloso : Player{
	public override void BuyProperty (Property property){
		if (_coins - property.getCost () >= 80) {
			buy (property);
		}
	}
}

public class Aleatorio : Player{
	public override void BuyProperty (Property property){
		int random = Random.Range (0, 100);
		if (random < 50) {
			buy (property);
		}
	}
}

