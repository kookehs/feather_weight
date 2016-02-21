using UnityEngine;
using System.Collections;

public abstract class Consumable {
	public virtual void Consume () {}

	public virtual void Fill(){}

	protected virtual void SetEmpty(){}
}
