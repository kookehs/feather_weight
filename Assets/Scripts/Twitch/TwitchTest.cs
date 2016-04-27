using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TwitchTest : MonoBehaviour {
    void Start () {
        List<string> messages = new List<string>();
        messages.Add("H1 Spawn chicken");
        messages.Add("H1 Spawn chicken");
        messages.Add("H2 Spawn bear");

        StringReader.ReadStrings(messages);
        //Debug.Log(StringReader.majorityHex);
        //Debug.Log(StringReader.majorityMod);
        //Debug.Log(StringReader.majorityCom);
    }
}
