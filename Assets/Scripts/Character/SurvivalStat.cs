using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class SurvivalStat : MonoBehaviour
{
	public Slider gui;
	public float value;
	public float max_value = 100f;
	public float min_value = 0f;
	public float loss_frequency;
	public float loss_timer;
	protected bool _loss_over_time = false;
	protected bool _enabled = true;

	// enable/disable stat loss over time
	public bool loss_over_time {
		get { return  _loss_over_time; }
		set { _loss_over_time = value; }
	}

	// enable/disable the survival stat
	public bool Enabled {
		get { return _enabled;  }
	 	set { _enabled = value; }
	}

	// shield for health, waterskin for water, etc.
	public Slider buffer_gui;
	public Image buffer_fill;
	public float buffer = 0f;
	public float max_buffer = 50f;
	public float min_buffer = 0f;
	private bool _buffer_enabled = false;

	public bool buffer_enabled {
		get { return _buffer_enabled; }
	}

	public void EnableBuffer() {
		_buffer_enabled = true;
		if (buffer_gui == null || buffer_fill == null) return;
		buffer_gui.enabled = true;
		buffer_fill.enabled = true;
	}

	public void DisableBuffer() {
		_buffer_enabled = false;
		if (buffer_gui == null || buffer_fill == null) return;
		buffer_gui.enabled = false;
		buffer_fill.enabled = false;
	}

	// update occurs once per frame
	void Update() {
		if (_enabled) {
			UpdateRoutine ();
			if (_loss_over_time) {
				loss_timer -= Time.deltaTime;
				if (loss_timer <= 0) Decrease ();
			}
		}
		if (gui != null) gui.value = value;
		if (buffer_enabled && buffer_gui != null) buffer_gui.value = buffer;
	}

	public void Increase(float v) {
		if (buffer_enabled) {
			float d = value + v - max_value;
			if (d > 0f) IncreaseBuffer (d);
		}  
		value = Mathf.Min (max_value, value + v);
		loss_timer = loss_frequency;
	}

	public void Increase() {
		Increase (10f);
	}

	public void Decrease(float v) {
		if (buffer_enabled && !BufferIsZero())
			v = DecreaseBuffer (v);
		value = Mathf.Max (min_value, value - v);
		loss_timer = loss_frequency;
	}

	public void Decrease() {
		Decrease (10f);
	}

	public bool IsMax() {
		return value == max_value;
	}

	public bool IsMin() {
		return value == min_value;
	}

	public bool IsZero() {
		return value == 0f;
	}

	protected void IncreaseBuffer(float v) {
		buffer = Mathf.Min (max_buffer, buffer + v);
	}

	protected float DecreaseBuffer(float v) {
		if (buffer - v < min_buffer) {
			buffer = min_buffer;
			return v - buffer;
		} else {
			buffer -= v;
			return 0f;
		}
	}

	public bool BufferIsMax() {
		return buffer == max_buffer;
	}

	public bool BufferIsMin() {
		return buffer == min_buffer;
	}

	public bool BufferIsZero() {
		return buffer == 0f;
	}

	protected virtual void UpdateRoutine() {
	}
}

