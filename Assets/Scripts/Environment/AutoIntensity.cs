using UnityEngine;
using System.Collections;

public class AutoIntensity : MonoBehaviour {

	public Gradient nightDayColor;

	public float maxIntensity = 3f;
	public float minIntensity = 0f;
	public float minPoint = -0.2f;

	public float maxAmbient = 1f;
	public float minAmbient = 0f;
	public float minAmbientPoint = -0.2f;


	public Gradient nightDayFogColor;
	public AnimationCurve fogDensityCurve;
	public float fogScale = 1f;

	public float dayAtmosphereThickness = 0.4f;
	public float nightAtmosphereThickness = 0.87f;

	public Vector3 dayRotateSpeed;
	public Vector3 nightRotateSpeed;
	public readonly Vector3 base_day_rs = new Vector3(-0.5f, 0f, 0f);
	public readonly Vector3 base_ngt_rs = new Vector3(-2.0f, 0f, 0f);

	float skySpeed = 1;


	Light mainLight;
	Skybox sky;
	Material skyMat;
	WeatherController the_weather;

	string changing_ToD = "";


	public Vector3 day_rotate_speed {
		get { return this.dayRotateSpeed; }
		set { this.dayRotateSpeed = value; }
	}

	public Vector3 ngt_rotate_speed {
		get { return this.nightRotateSpeed; }
		set { this.nightRotateSpeed = value; }
	}

	public Quaternion sun_position {
		set { this.transform.rotation = value; }
	}
		
	void Start () 
	{

		mainLight = GetComponent<Light>();
		skyMat = RenderSettings.skybox;
		the_weather = GameObject.Find ("WorldContainer").GetComponent<WeatherController> ();

	}

	void Update () 
	{
		CheckTimeOfDay ();

		float tRange = 1 - minPoint;
		float dot = Mathf.Clamp01 ((Vector3.Dot (mainLight.transform.forward, Vector3.down) - minPoint) / tRange);
		float i = ((maxIntensity - minIntensity) * dot) + minIntensity;

		mainLight.intensity = i;

		tRange = 1 - minAmbientPoint;
		dot = Mathf.Clamp01 ((Vector3.Dot (mainLight.transform.forward, Vector3.down) - minAmbientPoint) / tRange);
		i = ((maxAmbient - minAmbient) * dot) + minAmbient;
		RenderSettings.ambientIntensity = i;

		mainLight.color = nightDayColor.Evaluate(dot);
		RenderSettings.ambientLight = mainLight.color;

		RenderSettings.fogColor = nightDayFogColor.Evaluate(dot);
		RenderSettings.fogDensity = fogDensityCurve.Evaluate(dot) * fogScale;

		i = ((dayAtmosphereThickness - nightAtmosphereThickness) * dot) + nightAtmosphereThickness;
		skyMat.SetFloat ("_AtmosphereThickness", i);

		if (dot > 0) {
			transform.Rotate (dayRotateSpeed * Time.deltaTime * skySpeed);
			the_weather.SetTimeOfDay("DAY");
		} else {
			transform.Rotate (nightRotateSpeed * Time.deltaTime * skySpeed);
			the_weather.SetTimeOfDay("NIGHT");
		}

		//if (Input.GetKeyDown (KeyCode.J)) skySpeed *= 0.5f;
		//if (Input.GetKeyDown (KeyCode.K)) skySpeed *= 2f;
	}

	public void ChangeTimeOfDay(string t) {
		changing_ToD = t;
		skySpeed *= 10f;
	}

	private void ResetSkySpeed() {
		changing_ToD = "";
		skySpeed = 1f;
	}

	private void CheckTimeOfDay() {
		if (!changing_ToD.Equals ("")) {
			if (changing_ToD.Equals ("DAY")) {
				if (the_weather.GetTimeOfDay ().Equals ("DAY"))
					ResetSkySpeed ();
			} else {
				if (the_weather.GetTimeOfDay ().Equals ("NIGHT"))
					ResetSkySpeed ();
			}
		}
	}
}

