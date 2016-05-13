using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Destroyable : Strikeable
{

	public GameObject collectable;

	public int totalDropNum = 2;

	protected char[] separator = { '_' };

	private GameObject _chicken;
	private bool _has_chicken = false;

	public GameObject chicken {
		get { return this._chicken; }
		set { this._chicken = value; }
	}

	public bool has_chicken {
		get { return this._has_chicken; }
		set { this._has_chicken = value; }
	}

	public void
        HideChicken (GameObject chicken)
	{
		_chicken = chicken;
		_has_chicken = true;
		_chicken.SetActive (false);
	}

	public void
        ReleaseChicken ()
	{
		if (_chicken != null) {
			_has_chicken = false;
			_chicken.SetActive (true);
		}
	}

	protected override bool AfterHit (string hitter)
	{
		invincible = false;
		Health health = GetComponent<Health> ();
		// DropCollectable (hitter);
		if (health != null && health.IsDead ()) {
			ReleaseChicken ();
			Destroy (gameObject);
		}
		return false;
	}
}
