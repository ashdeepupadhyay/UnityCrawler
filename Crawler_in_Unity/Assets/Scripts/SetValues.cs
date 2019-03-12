using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetValues : MonoBehaviour {
    //FEATUREID:Set_Values
    public Text My;
    public Text name;
   
	// Use this for initialization
	void Start () {
        // "My" LOCTAG:MY
        My.text = "My";
        // "Name" LOCTAG:NAME
        name.text = "Name";  
	}	
}
