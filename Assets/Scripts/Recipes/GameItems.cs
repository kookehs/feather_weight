using UnityEngine;
using System.Collections;

[SerializeField]
public class GameItems {

	public string name;
	public int teir;
	public string description;
	public int cost;

	public GameItems(string name, int tier, string desc, int cost){
		this.name = name;
		this.teir = tier;
		this.description = desc;
		this.cost = cost;
	}

}
