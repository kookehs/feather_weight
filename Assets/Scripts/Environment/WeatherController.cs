using UnityEngine;
using System.Collections;

public class WeatherController : MonoBehaviour
{

	public enum TimeOfDay
	{
		DAY,
		NIGHT,
		NULL

	}

	public enum Weather
	{
		CLEAR,
		NULL

	}

	private AutoIntensity the_sun;
	private Weather weather_condition;
	private TimeOfDay time_of_day;

	// Use this for initialization
	void Start ()
	{
		GameObject sun = GameObject.Find ("Sun");
		if (sun != null)
			the_sun = sun.GetComponent<AutoIntensity> ();
		weather_condition = Weather.CLEAR;
		time_of_day = TimeOfDay.DAY;
	}

	public bool ChangeTimeOfDay (string t)
	{
		if (t.Equals ("DAY")) {
			if (time_of_day == TimeOfDay.NIGHT) {
				the_sun.ChangeTimeOfDay (t);
				return true; //success
			}
		} else {
			if (time_of_day == TimeOfDay.DAY) {
				the_sun.ChangeTimeOfDay (t);
				return true; //success
			}
		}
		return false; //failure
	}

	public string GetTimeOfDay ()
	{
		switch (time_of_day) {
		case TimeOfDay.DAY:
			return "DAY";
		case TimeOfDay.NIGHT:
			return "NIGHT";
		default:
			return null;
		}
	}

	public TimeOfDay GetTimeValue (string t)
	{
		switch (t) {
		case "DAY":
			return TimeOfDay.DAY;
		case "NIGHT":
			return TimeOfDay.NIGHT;
		default:
			return TimeOfDay.NULL;
		}
	}

	public void SetTimeSpeed (string t, float multiplier) {
		if (t.Equals ("DAY"))
			the_sun.day_rotate_speed = the_sun.base_day_rs * multiplier;
		else
			the_sun.ngt_rotate_speed = the_sun.base_ngt_rs * multiplier;
	}

	public void SetTimeOfDay (string t)
	{
		switch (t) {
		case "DAY":
			time_of_day = TimeOfDay.DAY;
			break;
		case "NIGHT":
			time_of_day = TimeOfDay.NIGHT;
			break;
		default:
			break;
		}
	}
}

