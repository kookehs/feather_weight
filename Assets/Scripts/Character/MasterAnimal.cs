using UnityEngine;
using System.Collections;

public class MasterAnimal : Animal
{
	public override void performStateCheck() {
	}

	protected override void Initialize() {
		state = AnimalState.NULL;
		if (nma != null) nma.enabled = false;
	}
}

