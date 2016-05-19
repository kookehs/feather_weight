using UnityEngine;
using System.Collections;

public class Net : Weapon {

	private string me = "Weapon_Pickaxe";
	public float strong_knockback = 600f;
	private InventoryController inventoryController;

	void Start() {
		if (GameObject.FindGameObjectWithTag ("InventoryUI") != null)
			inventoryController = GameObject.FindGameObjectWithTag ("InventoryUI").GetComponent<InventoryController> ();
	}

	protected override void OnTriggerEnter (Collider other)
	{
		//Debug.Log ("Weapon Colliding");
		bool killed = false;

		switch (other.tag) {
		case "Bear":
			killed = other.gameObject.GetComponent<BearNMA> ().receiveHit (GetComponent<Collider> (), 5, 350, me);
			break;
		case "Wolf":
			killed = other.gameObject.GetComponent<BearNMA> ().receiveHit (GetComponent<Collider> (), 5, 350, me);
			break;
		case "MountainLion":
			killed = other.gameObject.GetComponent<MountainLion> ().receiveHit (GetComponent<Collider> (), 5, 350, me);
			break;
		case "Chicken":
			inventoryController.AddNewObject (other.gameObject); //collect the object in inventory

			if (inventoryController.inventoryItems.Count < 6) {
				if (gameObject.tag == "Chicken") {
					Chicken chicken = other.gameObject.GetComponent<Chicken>();

					if (chicken.quest_eligible == true) {
						WorldContainer.UpdateCountCount(other.gameObject.tag);
						chicken.quest_eligible = false;
					}
				} else {
					WorldContainer.UpdateCountCount(other.gameObject.tag);
				}
			}
			break;
		case "Bush":
			other.gameObject.GetComponent<Destroyable> ().receiveHit (GetComponent<Collider> (), 1, 0, me);
			Instantiate (Resources.Load ("Debris_Grass"), other.transform.position, Quaternion.identity);
			break;
		case "Rock3D":
			//disableMe ();
			Instantiate (Resources.Load("Particle Effects/Debris_Rock"), transform.position, Quaternion.identity);
			if (other.gameObject.transform.parent.name.Contains("Wall"))
			{
				other.gameObject.transform.parent.GetComponent<Destroyable>().receiveHit(GetComponent<Collider>(), 1, 0, me);
			}
			else other.gameObject.GetComponent<Destroyable>().receiveHit(GetComponent<Collider>(), 1, 0, me);
			break;
		default:
			break;
		}

		if (killed) {
			WorldContainer.UpdateKillCount (other.tag);
		}
	}

	protected override void OnEnable ()
	{
		GetComponent<Animator> ().Play ("sword_swing");
	}
}
