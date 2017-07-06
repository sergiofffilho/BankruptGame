using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Property {

	private float _cost;
	private float _rent;

	public Property(float cost, float rent){
		_cost = cost;
		_rent = rent;
	}

	private Player _owner = null;

	public bool hasOwner(){
		if (_owner == null) {
			return false;
		} else {
			return true;
		}
	}

	public float getCost(){
		return _cost;
	}

	public float getRent(){
		return _rent;
	}

	public Player getOwner(){
		return _owner;
	}

	public void setOwner(Player owner){
		_owner = owner;
	}
}
