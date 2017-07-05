using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player {
	protected float coins = 300;

	protected List<Property> properties = new List<Property> ();

	protected abstract void BuyProperty(Property property){}

	protected void buy(Property property){
		coins -= property.getCost();
		property.setOwner (this);
		properties.Add (property);
	}
//
//	public abstract void checkCoinsPay(Property property, float value){
//		if (value < coins) {
//			pay (value);
//		} else {
//			lostProperties (properties);
//		}
//	}
//
	protected void pay(Property property){
		coins -= property.getRent();
		property.getOwner().coins += property.getRent();
	}

	protected void lostProperties(Player player){
		player.properties.Clear();
	}


}


public class Impulsivo : Player{
	public override void BuyProperty (Property property){
		if (property.getCost () < coins) {
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
		if (coins - property.getCost () >= 80) {
			buy (property);
		}
	}
}

public class Alearotio : Player{
	public override void BuyProperty (Property property){
		int random = Random.Range (0, 100);
		if (random < 50) {
			buy (property);
		}
	}
}

