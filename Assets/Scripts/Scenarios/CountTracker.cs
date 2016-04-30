using System;
using System.Collections.Generic;

public struct CountsTracker
{
	public Dictionary<string, int> counts;

	public CountsTracker (Dictionary<string, int> c) {
		counts = c;
	}
		
	public int CountCount() {
		int result = 0;
		foreach (var count in counts)
			result += count.Value;
		return result;
	}

	public int CountCount(string what) {
		int result = 0;
		counts.TryGetValue(what, out result);
		return result;
	}
}
