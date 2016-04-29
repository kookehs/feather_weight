using UnityEngine;
using System.Collections;

public enum BossState
{
	LIFTING,
	MOVING,
	DUNKING,
	HOLD,
	ACQUIRE,
	PREDUNK,
}


public class Boss : MonoBehaviour {

	GameObject left_hand;
	GameObject right_hand;
	GameObject totem;
	GameObject player;
	GameObject current_hand;

	public BossState state = BossState.HOLD;

	//public ArrayList ClockDirection = new ArrayList{15f, 45f, 75f, 105f, 1235f, 165f, 195f, 225f, 255f, 285f, 315f, 345f};
	//public MeshRenderer[] ClockBlocks;
	public float move_rate = 8f;
	public float move_rate_two = 18f;
	public float move_rate_dunk = 10f;
	public float fire_rate = .5f;
	//public float lightning_fire_rate = 17.1f;
	float next_action = 0.0f;
	//float next_lightning = 0.0f;
	public float dunk_delay = .25f;
	float dunk_time = 0.0f;
	private Quaternion reset_roatation;

	//Color lightning_purple = new Color(154f,11f,216f,120f);
	//Color lightning_yellow = new Color(255f,255f,0f,120f);
	//Color lightning_prehit = new Color (160f, 0f, 0f, 0f);

	//private string attack_lightning = "BOSS_LIGHTNING";

	//private float damage_lightning = 10f;

	Vector3 moveto;
	//Vector3 totem_pos;
	//Transform totem_trans;
	// Use this for initialization
	void Start () {
		left_hand = transform.FindChild ("lefthand").gameObject as GameObject;
		right_hand = transform.FindChild ("righthand").gameObject as GameObject;
		//totem = transform.FindChild ("totem").gameObject as GameObject;
		player = GameObject.Find ("Player") as GameObject;
		next_action = Time.time;
		//next_lightning = Time.time + lightning_fire_rate;
		//totem_pos = totem.transform.position;
		//totem_trans = totem.transform;
		/*ClockBlocks = new MeshRenderer[]{
			totem_trans.FindChild ("Hour0").FindChild ("Cube").gameObject.GetComponent<MeshRenderer> (),
			totem_trans.FindChild ("Hour1").FindChild ("Cube").gameObject.GetComponent<MeshRenderer> (),
			totem_trans.FindChild ("Hour2").FindChild ("Cube").gameObject.GetComponent<MeshRenderer> (),
			totem_trans.FindChild ("Hour3").FindChild ("Cube").gameObject.GetComponent<MeshRenderer> (),
			totem_trans.FindChild ("Hour4").FindChild ("Cube").gameObject.GetComponent<MeshRenderer> (),
			totem_trans.FindChild ("Hour5").FindChild ("Cube").gameObject.GetComponent<MeshRenderer> (),
			totem_trans.FindChild ("Hour6").FindChild ("Cube").gameObject.GetComponent<MeshRenderer> (),
			totem_trans.FindChild ("Hour7").FindChild ("Cube").gameObject.GetComponent<MeshRenderer> (),
			totem_trans.FindChild ("Hour8").FindChild ("Cube").gameObject.GetComponent<MeshRenderer> (),
			totem_trans.FindChild ("Hour9").FindChild ("Cube").gameObject.GetComponent<MeshRenderer> (),
			totem_trans.FindChild ("Hour10").FindChild ("Cube").gameObject.GetComponent<MeshRenderer> (),
			totem_trans.FindChild ("Hour11").FindChild ("Cube").gameObject.GetComponent<MeshRenderer> ()
		};*/
	}

	// Update is called once per frame
	void Update () {
		/*if (Time.time > next_lightning) {
			next_lightning = Time.time + lightning_fire_rate;
			StartCoroutine ("FireLightning");
		}*/
		switch (state) {
		case BossState.HOLD:
			if (Time.time > next_action)
				state = BossState.ACQUIRE;
			break;
		case BossState.ACQUIRE:
			if (Vector3.Distance (player.transform.position, transform.position) < 17) {
				ChooseHand ();
			} else {
				next_action = Time.time + fire_rate;
				state = BossState.HOLD;
			}
			break;
		case BossState.LIFTING:
			Move (move_rate);
			if (current_hand.transform.position == moveto)
				state = BossState.MOVING;
			break;
		case BossState.MOVING:
			moveto = new Vector3 (
				player.transform.position.x,
				player.transform.position.y + 4f,
				player.transform.position.z);
			Move (move_rate_two);
			if (current_hand.transform.position == moveto) {
				state = BossState.PREDUNK;
				dunk_time = Time.time + dunk_delay;
			}
			break;
		case BossState.PREDUNK:
			if (Time.time > dunk_time) {
				state = BossState.DUNKING;
				moveto = new Vector3 (player.transform.position.x,1f,player.transform.position.z);
			}
			break;
		case BossState.DUNKING:
			Move (move_rate_dunk);
			if (current_hand.transform.position == moveto) {
				next_action = Time.time + fire_rate;
				state = BossState.HOLD;
			}
			break;
		}
	}

	bool PlayerOnLeft(){
		return (player.transform.position.x > transform.position.x);
	}

	void ChooseHand(){
		if (PlayerOnLeft ()) {
			current_hand = left_hand;
		} else {
			current_hand = right_hand;
		}
		moveto = new Vector3 (
			current_hand.transform.position.x,
			current_hand.transform.position.y + 2f,
			current_hand.transform.position.z);
		state = BossState.LIFTING;
	}

	void Move(float rate){
		float step = rate * Time.deltaTime;
		current_hand.transform.transform.position = Vector3.MoveTowards (current_hand.transform.position, moveto, step);
	}

	/*IEnumerator FireLightning(){
		int hour1 = Random.Range (0, 12);
		int hour2 = Random.Range (0, 12);
		int hour3 = Random.Range (0, 12);
		Lightning (hour1, lightning_prehit);
		Lightning (hour2, lightning_prehit);
		Lightning (hour3, lightning_prehit);
		yield return new WaitForSeconds (2f);
		Lightning (hour1, lightning_yellow);
		Lightning (hour2, lightning_yellow);
		Lightning (hour3, lightning_yellow);
		if (PlayerDetect(hour1)) player.GetComponent<PlayerMovementRB> ().receiveHit (GetComponent<Collider> (), damage_lightning, 0, attack_lightning);
		if (PlayerDetect(hour2)) player.GetComponent<PlayerMovementRB> ().receiveHit (GetComponent<Collider> (), damage_lightning, 0, attack_lightning);
		if (PlayerDetect(hour3)) player.GetComponent<PlayerMovementRB> ().receiveHit (GetComponent<Collider> (), damage_lightning, 0, attack_lightning);
		yield return new WaitForSeconds (.1f);
		ResetLightning (hour1);
		ResetLightning (hour2);
		ResetLightning (hour3);
	}

        public void FireLightningTwitchHelper(int hour) {
                StartCoroutine ("FireLightningTwitch", hour);
        }

	IEnumerator FireLightningTwitch(int hour){
                Debug.Log("Hour: " + hour);
		Lightning (hour, lightning_prehit);
		yield return new WaitForSeconds (2f);
		Lightning (hour, lightning_purple);
		if (PlayerDetect(hour)) player.GetComponent<PlayerMovementRB> ().receiveHit (GetComponent<Collider> (), damage_lightning, 0, attack_lightning);
                yield return new WaitForSeconds (.1f);
		ResetLightning (hour);
	}

	void Lightning(int hour, Color color){
		int hour2;
		if (hour < 11)
			hour2 = (++hour);
		else
			hour2 = 0;
		ClockBlocks [hour].enabled = true;
		ClockBlocks [hour2].enabled = true;
		ClockBlocks [hour].material.color = color;
		ClockBlocks [hour2].material.color = color;
	}

	void ResetLightning(int hour){
		int hour2;
		if (hour < 11)
			hour2 = (++hour);
		else
			hour2 = 0;
		ClockBlocks [hour].enabled = false;
		ClockBlocks [hour2].enabled = false;
	}

	bool PlayerDetect(int hour){
		totem_trans.rotation = Quaternion.AngleAxis ((float)ClockDirection [hour], Vector3.up);
		Vector3 start_check = (((new Vector3 (
			totem_pos.x,
			0f,
			totem_pos.z)) + totem_trans.forward) - (new Vector3 (
			totem_pos.x,
			0f,
			totem_pos.z)));
		Vector3 player_angle = ((new Vector3 (
			player.transform.position.x,
			0f,
			player.transform.position.z)) - (new Vector3 (
			totem_pos.x,
			0f,
			totem_pos.z)));
		float angle = Vector3.Angle (start_check, player_angle);
		totem_trans.rotation = Quaternion.Euler(0f,0f,0f);
		return (angle <= 15f);
	}*/
}
