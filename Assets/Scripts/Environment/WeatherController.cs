using UnityEngine;
using System.Collections;

public class WeatherController : MonoBehaviour
{
	enum Weather { CLEAR }

	enum Time { DAY, NIGHT }

	private Weather weather_condition;
	private Time time_of_day;

	// Use this for initialization
	void Start ()
	{
		weather_condition = Weather.CLEAR;
		time_of_day = Time.DAY;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

