using UnityEngine;
using System.Collections;

public class Torchick : Animal
{
	protected override void Initialize() {
	}

	public override void performStateCheck() {
		state = AnimalState.HOSTILE;
	}

	public override void performHostile ()
	{
		
	}

	protected override bool DamagePlayerOnCollision() {
		return true;
	}
}

