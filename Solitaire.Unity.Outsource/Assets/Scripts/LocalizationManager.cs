using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.UI;
using System.IO;
using System.Threading.Tasks;
using ui.windows;

public enum LangType
{
	none = -1,
	normal = 0,
	china,
	japan,
	korean,
	arabic}
;

public class LocalizationManager : MonoBehaviour
{
	private static LocalizationManager instance;
	private Hashtable currentHashTable = null;
	private string currentLanguage = "";
	private int langId = -1;
	private List<string[]> langNames;
	private Dictionary<string, Hashtable> langHashes;

	public delegate void LocalizationEventHandler ();

	public static event LocalizationEventHandler LanguageLoaded;

	void Awake ()
	{		
		if (instance == null) {
			langHashes = new Dictionary<string, Hashtable> ();
			instance = this;				
		}
	}

	public int CurrentLangId {
		get{ return langId; }   
	}

	private const string FILE_NAME = "languages";
	private XmlDocument _doc;

	public static LocalizationManager Instance {
		get {
			return instance;
		}
	}

	/// <summary>
	/// Gets the available langs names.
	/// </summary>
	/// <value>The available langs names list.</value>
	public List<string[]> AvailableLangs {
		get{ return langNames; }
	}

	/// <summary>
	/// Gets full path to localization XML file.
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="fileName">File name.</param>
	private static string FullXMLPath (string fileName)
	{
       
		string fullPath = Path.Combine (Application.streamingAssetsPath, fileName + ".xml"); 

		#if !UNITY_ANDROID || UNITY_EDITOR
		fullPath = "file://" + fullPath;
		#endif

		return fullPath;
	}

	/// <summary>
	/// Applies given hashtable to manager and notifies everybody about language changed.
	/// </summary>
	/// <param name="table">Table.</param>
	private void ApplyHashtable (Hashtable table)
	{
		currentHashTable = table;

		currentLanguage = Instance.name = currentHashTable ["name"].ToString ();
	
		if (GameSettings.Instance.Data.Language == string.Empty) {			
			GameSettings.Instance.Data.Language = currentLanguage;
			GameSettings.Instance.Save ();
		}

		langId = int.Parse (currentHashTable ["id"].ToString ());

		RaiseEvent (LanguageLoaded);
	}

	/// <summary>
	/// Reads the XML with localization strings and applies specified language.
	/// </summary>
	/// <param name="language">Language name.</param>
	public async Task ReadLangXml (string language)
	{		
		if (language == string.Empty) {			
			language = Application.systemLanguage.ToString ();
		}

		if (langHashes.ContainsKey (language)) {			
			ApplyHashtable (langHashes [language]);
            return;
        }

		if (langNames == null)
			langNames = new List<string[]> ();

		string path = FullXMLPath (FILE_NAME);
	
		byte[] bytes = null;


		using (WWW www = new WWW (path)) {

			while (!www.isDone) {
                Task.Yield();
            }

			if (www.error != null) {
                return;
            } 

			bytes = www.bytes;
		}

		MemoryStream ms = new MemoryStream (bytes);
		ms.Flush ();
		ms.Position = 0;

		_doc = new XmlDocument ();	
		_doc.Load (ms);
     
		bool found = false;

		foreach (XmlNode node in _doc.SelectNodes("texts/language")) {
			string name = node.Attributes ["name"].Value;
			Hashtable hashTable = new Hashtable ();      
			langNames.Add (new string[]{ name, node.Attributes ["caption"].Value });

			hashTable.Add ("name", node.Attributes ["name"].Value);
			hashTable.Add ("caption", node.Attributes ["caption"].Value);
			hashTable.Add ("flag", node.Attributes ["flag"].Value);
			hashTable.Add ("id", node.Attributes ["id"].Value);

			foreach (XmlNode child in node.ChildNodes) {
				if (child.Name == "phrase") {
					hashTable.Add (child.Attributes ["name"].Value, child.Attributes ["text"].Value.Replace ("\\n", "\n"));
				}
			}
			if (!langHashes.ContainsKey (name))
				langHashes.Add (name, hashTable);

			if (node.Attributes ["name"].Value.StartsWith (language)) {				
				found = true;
				ApplyHashtable (hashTable);
			}
            Task.Yield();
        }

		if (found) {
            return;
        }
		
		//если не удалось загрузить требуемый язык - пробуем английский по умолчанию
		if (language != "English") {			
			await ReadLangXml ("English");		
		}
	}

	/// <summary>
	/// Gets the localized text by given key. Returns default value if no value found for key.
	/// </summary>
	/// <returns>The localized text.</returns>
	/// <param name="key">Key.</param>
	/// <param name="def">Default value.</param>
	public string GetText (string key, string def = "")
	{
		if (string.IsNullOrEmpty (key))
			return string.Empty;

		if (currentHashTable != null && currentHashTable.ContainsKey (key)) {
			return (string)currentHashTable [key];
		} else
			return def.Equals (string.Empty) ? key : def;
	}

	/// <summary>
	/// Translate the specified text component using key a. Uses default value if no value found for key.
	/// </summary>
	/// <param name="text">Text.</param>
	/// <param name="key">Key.</param>
	/// <param name="def">Def.</param>
	public void Translate (Text text, string key, string def = "")
	{		
		if (text != null) {
			if (key != null && !key.Equals (string.Empty))
				text.text = LocalizationManager.Instance.GetText (key, def);
		}
	}


	private void RaiseEvent (LocalizationEventHandler handler)
	{	
		if (handler != null)
			handler ();
	}
}
