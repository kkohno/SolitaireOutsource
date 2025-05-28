using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeManager : MonoBehaviour {

	private static ThemeManager instance;

	public Sprite[] cardSprites;
	public Sprite cardBack;
	public Sprite slotSprite;
	//public Sprite shadow;
	public Sprite glow;

	public static Sprite BackSprite{
		get{
			return instance.cardBack;
		}
	}

	public static Sprite SlotSprite{
		get{
			return instance.slotSprite;
		}
	}

	/*public static Sprite ShadowSprite{
		get{
			return instance.shadow;
		}
	}*/


	public static Sprite GlowSprite{
		get{
			return instance.glow;
		}
	}

	public static Sprite FaceSprite(int index){		
		return instance.cardSprites [index];
	}


	public static void Init(){
		instance.LoadDefaultTheme ();
	}

	private void LoadDefaultTheme(){
		cardSprites = Resources.LoadAll<Sprite> ("defaultTheme/cards");
		cardBack = Resources.Load<Sprite> ("defaultTheme/back");
		slotSprite = Resources.Load<Sprite> ("defaultTheme/glow");
		//shadow = Resources.Load<Sprite> ("defaultTheme/shadow");
		glow = Resources.Load<Sprite> ("defaultTheme/glow2");
	}

	public ThemeManager():base(){
		instance = this;
	}

}
