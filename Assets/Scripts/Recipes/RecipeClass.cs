using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class RecipeClass {

	public string craftItemName;
	public SortedDictionary<string, int> produces;
	public SortedDictionary<string, int> consumes;

}
