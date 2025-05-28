using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BasicLog : MessageHandler {

	public Text text;
	public ScrollRect rect;
	public Scrollbar bar;

	public override void AddMsg(string msg){
		text.text += string.Format ("{0}: {1} \n", DateTime.Now.ToShortTimeString (), msg);
		bar.value = 0;
	}
}
