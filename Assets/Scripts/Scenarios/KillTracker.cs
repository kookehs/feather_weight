using System;
using System.Collections.Generic;

public struct KillsTracker
{
	public Dictionary<string, int> bounties;

	public KillsTracker (Dictionary<string, int> b) {
		bounties = b;
	}
		
	public int KillCount() {
		int result = 0;
		foreach (var bounty in bounties)
			result += bounty.Value;
		return result;
	}

	public int KillCount(string what) {
		int result = 0;
		bounties.TryGetValue(what, out result);
		return result;
	}
}


