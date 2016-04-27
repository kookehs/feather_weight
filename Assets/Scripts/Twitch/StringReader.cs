using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

public class StringReader : MonoBehaviour {

	private Dictionary<string, float> HEXCOUNT = new Dictionary<string, float> ();
	private Dictionary<string, float> MODCOUNT = new Dictionary<string, float> ();
	private Dictionary<string, float> COMMANDCOUNT = new Dictionary<string, float> ();
	private float currentInfluenceMult = 0f;
	//all commandsmust have a float at the begining of the command to be valid.
	//working on a block read later, right not it's not important
	//hexes must be named as a capital letter, followed IMMEDIATELY by at least one number.
	private string ishex = (@"([0-9]+\.[0-9]+)(?:.*?)([a-z][0-9]+)");
	//modifiers separated by vertical lines, dictates or. Takes in the LAST modifier given.
	private string ismod = (@"([0-9]+\.[0-9]+)(?:.*?)(spawn|buff|hide)");
	//commands same as modifiers.
	private string iscommand = (@"([0-9]+\.[0-9]+)(?:.*?)(bear|chicken|raise|lower|wall|rotate|swap)");
	// Use this for initialization
	private float totalHexInfluence = 0.0f;
	private float totalModInfluence = 0.0f;
	private float totalComInfluence = 0.0f;
	public float threshold = .3f; // this value controls how much of the chat needs to be on the same idea for ANYTHING to happen.
	public string majorityHex = null;
	public string majorityMod = null;
	public string majorityCom = null;

	void ReadStrings(IList<string> twitchstrings){
		HEXCOUNT.Clear ();
		MODCOUNT.Clear ();
		COMMANDCOUNT.Clear ();
		totalHexInfluence = 0.0f;
		totalModInfluence = 0.0f;
		totalComInfluence = 0.0f;
		majorityHex = null;
		majorityMod = null;
		majorityCom = null;
		IEnumerator<string> liste = twitchstrings.GetEnumerator ();
		while (liste.MoveNext ()) {
			string line = liste.Current;
			foreach (Match match in Regex.Matches (line, ishex)) {
				currentInfluenceMult = float.Parse (match.Groups [1].ToString ());
				string currentHex = match.Groups [2].ToString ();
				if (HEXCOUNT.ContainsKey (currentHex)) {
					HEXCOUNT [currentHex] += currentInfluenceMult;
				} else {
					HEXCOUNT.Add (currentHex, currentInfluenceMult);
				}
			}
			foreach (Match match in Regex.Matches (line, ismod)) {
				currentInfluenceMult = float.Parse (match.Groups [1].ToString ());
				string currentMod = match.Groups [2].ToString ();
				if (MODCOUNT.ContainsKey (currentMod)) {
					MODCOUNT [currentMod] += currentInfluenceMult;
				}
				else{
					MODCOUNT.Add(currentMod, currentInfluenceMult);
				}
			}
			foreach (Match match in Regex.Matches (line, iscommand)) {
				currentInfluenceMult = float.Parse (match.Groups [1].ToString ());
				string currentCom = match.Groups [2].ToString ();
				if (COMMANDCOUNT.ContainsKey (currentCom)) {
					COMMANDCOUNT [currentCom] += currentInfluenceMult;
				} else {
					COMMANDCOUNT.Add (currentCom, currentInfluenceMult);
				}
			}
		}
		float highest = 0.0f;
		foreach (KeyValuePair<string,float> kvp in HEXCOUNT){
			if (kvp.Value > highest && (kvp.Value / totalHexInfluence) > threshold){
				majorityHex = kvp.Key;
			}
		}
		highest = 0.0f;
		foreach (KeyValuePair<string,float> kvp in MODCOUNT){
			if (kvp.Value > highest && (kvp.Value / totalModInfluence) > threshold){
				majorityMod = kvp.Key;
			}
		}
		highest = 0.0f;
		foreach (KeyValuePair<string,float> kvp in COMMANDCOUNT){
			if (kvp.Value > highest && (kvp.Value / totalComInfluence) > threshold){
				majorityCom = kvp.Key;
			}
		}
	}
}
