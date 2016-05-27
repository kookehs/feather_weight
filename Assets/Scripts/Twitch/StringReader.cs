using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

public class StringReader {

	static Dictionary<string, float> HEXCOUNT;
	static Dictionary<string, float> MODCOUNT;
	static Dictionary<string, float> COMMANDCOUNT;
	static float currentInfluenceMult;
	//all commandsmust have a float at the begining of the command to be valid.
	//working on a block read later, right not it's not important
	//hexes must be named as a letter, followed IMMEDIATELY by at least one number.
	static string ishex;
	//modifiers separated by vertical lines, dictates or. Takes in the LAST modifier given.
	static string ismod;
	//commands same as modifiers.
	static string iscommand;
	// Use this for initialization
	static float totalHexInfluence;
	static float totalModInfluence;
	static float totalComInfluence;
	static float _threshold; // this value controls how much of the chat needs to be on the same idea for ANYTHING to happen.
	static string majorityHex;
	static string majorityMod;
	static string majorityCom;

	// static constructor to initialize variables without having to create an instance of StringReader
	static StringReader() {
		HEXCOUNT = new Dictionary<string, float> ();
		MODCOUNT = new Dictionary<string, float> ();
		COMMANDCOUNT = new Dictionary<string, float> ();
		currentInfluenceMult = 0f;
		ishex = (@"([0-9]+\.[0-9]+)(?:.*?)([h][0-9]+)");
		ismod = (@"([0-9]+\.[0-9]+)(?:.*?)(craze|fall|faster|shrink|smite|stronger|spawn|raise|lower|wall)");
		iscommand = (@"([0-9]+\.[0-9]+)(?:.*?)(bear|boulder|chicken|tree|monster|wolf|ice|lava|\[.*?\])");
		totalHexInfluence = 0.0f;
		totalModInfluence = 0.0f;
		totalComInfluence = 0.0f;
		_threshold = .3f; 
		majorityHex = null;
		majorityMod = null;
		majorityCom = null;
	}

	public static float threshold {
		get { return _threshold;  }
		set { _threshold = value; }
	}

	public static string command {
		get { return majorityCom; }
	}

	public static string effect {
		get { return majorityMod; }
	}

	public static string hex {
		get { return majorityHex; }
	}

	public static void ReadStrings(IList<string> twitchstrings){
		HEXCOUNT.Clear ();
		MODCOUNT.Clear ();
		COMMANDCOUNT.Clear ();
		totalHexInfluence = 0.0f;
		totalModInfluence = 0.0f;
		totalComInfluence = 0.0f;
		majorityHex = "random";
		majorityMod = "random";
		majorityCom = "hex";
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
			if (kvp.Value > highest && (kvp.Value / totalHexInfluence) > _threshold){
				majorityHex = kvp.Key;
			}
		}
		highest = 0.0f;
		foreach (KeyValuePair<string,float> kvp in MODCOUNT){
			if (kvp.Value > highest && (kvp.Value / totalModInfluence) > _threshold){
				majorityMod = kvp.Key;
			}
		}
		highest = 0.0f;
		foreach (KeyValuePair<string,float> kvp in COMMANDCOUNT){
			if (kvp.Value > highest && (kvp.Value / totalComInfluence) > _threshold){
				majorityCom = kvp.Key;
			}
		}
	}
}
