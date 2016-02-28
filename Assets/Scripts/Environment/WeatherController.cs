using UnityEngine;
using System.Collections;

public class WeatherController : MonoBehaviour {

	public enum TimeOfDay { DAY, NIGHT, NULL}

	public enum Weather { CLEAR, NULL}

	private AutoIntensity the_sun;
	private Weather weather_condition;
	private TimeOfDay time_of_day;

	// Use this for initialization
	void Start ()
	{
		the_sun = GameObject.Find ("Sun").GetComponent<AutoIntensity> ();
		weather_condition = Weather.CLEAR;
		time_of_day = TimeOfDay.DAY;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void SetTimeOfDay (TimeOfDay t) {
		time_of_day = t;
	}

	public void ChangeTimeOfDay (string t) {
		if (t.Equals ("DAY")) {
			if (time_of_day == TimeOfDay.NIGHT)
				the_sun.ChangeTimeOfDay (t);
		} else {
			if (time_of_day == TimeOfDay.DAY)
				the_sun.ChangeTimeOfDay (t);
		}
	}

	public string GetTimeOfDay () {
		switch (time_of_day) {
		case TimeOfDay.DAY:
			return "DAY";
		case TimeOfDay.NIGHT:
			return "NIGHT";
		default:
			return null;
		}
	}

	public TimeOfDay GetTimeValue (string t) {
		switch (t) {
		case "DAY":
			return TimeOfDay.DAY;
		case "NIGHT":
			return TimeOfDay.NIGHT;
		default:
			return TimeOfDay.NULL;
		}
	}
}

