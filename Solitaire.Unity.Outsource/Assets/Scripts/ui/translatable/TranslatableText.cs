using UnityEngine.UI;

namespace ui.translatable
{
	public class TranslatableText : Translatable
	{
		#region implemented abstract members of Translatable

		public override void TranslateActions (string phraseId)
		{
			var t = GetComponent<Text> ();

			LocalizationManager.Instance.Translate (t, phraseId, defaultValue);

			ApplyCase (t);
		}

		#endregion

		protected void ApplyCase (Text text)
		{
			if (Case == Case.Original)
				return;

			//if font case was specified in inspector we apply it
			switch (Case) {
				case Case.Upper:
					text.text = text.text.ToUpper ();
					break;
				case Case.Lower:
					text.text = text.text.ToLower ();
					break;			
			}
		}


	}
}
