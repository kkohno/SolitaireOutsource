using UnityEngine;

namespace ui.translatable
{
	public enum Case
	{
		Original = 0,
		Upper,
		Lower
	}

	public abstract class Translatable :MonoBehaviour
	{
		public string phraseId;
		public string defaultValue;
		public Case Case = Case.Original;

		protected int curLang = -1;
		protected string curPhrase = string.Empty;

		public abstract void TranslateActions (string tag);

		/// <summary>
		/// Translate current object with specified customPhraseId.
		/// </summary>
		/// <param name="customPhraseId">Custom phrase identifier.</param>
		public void Translate (string customPhraseId = "")
		{
		
			// if no custom tag was specified we use tag which set from inspector	
			if (customPhraseId == string.Empty || customPhraseId == null)
				customPhraseId = this.phraseId;

			if (IsMustTranslate (phraseId)) {		
				TranslateActions (phraseId);	
			}

			//remember tag and language to filter useless calls in future
			curLang = LocalizationManager.Instance.CurrentLangId;
			curPhrase = phraseId;
		}

		/// <summary>
		/// Determines whether this instance is must be translated with the specified phraseId.
		/// </summary>
		/// <returns><c>true</c> if this instance is must be translated with phraseId; otherwise, <c>false</c>.</returns>
		/// <param name="phraseId">Phrase identifier.</param>
		protected bool IsMustTranslate (string phraseId)
		{
			//check if our object is already translated to this language by this tag
			return curLang != LocalizationManager.Instance.CurrentLangId ||
			       curLang < 0 ||
			       curPhrase != phraseId;
		}
	}
}