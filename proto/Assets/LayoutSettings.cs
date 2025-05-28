using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutSettings : MonoBehaviour {

	private static LayoutSettings instance;



	void Awake(){
		instance = this;
	}
}
