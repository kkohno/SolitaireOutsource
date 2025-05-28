using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] bgSounds;
    public GameObject soundPrefab;
    private static SoundManager instance;
    private List<AudioSource> sourcesTotal;
    private List<AudioSource> sourcesFree;
    private Dictionary<string, AudioSource> sourcesBg;
    private bool onPause;
    private float[] pitches;
    private float[] songPitches;
    private int songCursor;
    private int[] androidSounds;
    private AudioClip[] sounds;

    public static SoundManager Instance
    {
        get { return instance; }
    }

#if UNITY_ANDROID
    private IEnumerator LoadSoundsAndroid(string path, int[] array)
    {
        yield break; // todo вырезал
        /*for (int i = 1; i < array.Length + 1; i++) {	
            array [i - 1] = AndroidNativeAudio.load (string.Format ("{0}/{1}.wav", path, i.ToString ()));	
            yield return null;
        }*/
    }
#endif

    private IEnumerator LoadSoundsIOS(string path, AudioClip[] array)
    {
        WWW www;
        for (int i = 1; i < array.Length + 1; i++) {
            www = new WWW("file://" + string.Format("{0}/{1}/{2}.wav", Application.streamingAssetsPath, path, i.ToString()));
            while (!www.isDone) {
                if (www.error != null)
                    yield break;

                yield return null;
            }
            array[i - 1] = www.GetAudioClip();
        }
    }

    /// <summary>
    /// Loads the sounds dependent on runtime platform.
    /// </summary>
    /// <param name="callback">Callback.</param>
    public async Task LoadSounds(Action callback = null)
    {

        /*
#if UNITY_IPHONE || UNITY_EDITOR
        if (sounds == null) {
			sounds = new AudioClip[14];
			yield return  StartCoroutine (LoadSoundsIOS ("sounds", sounds));			
		}	
		#elif UNITY_ANDROID
		if(androidSounds == null)
		{
			AndroidNativeAudio.makePool(10);
			androidSounds = new int[14];
			yield return StartCoroutine(LoadSoundsAndroid("sounds",androidSounds));
		}
		#endif

		if (callback != null)
			callback ();*/
    }

    /// <summary>
    /// Gets audio soure instance from pool or create new
    /// </summary>
    /// <returns>The source.</returns>
    public AudioSource GetSource()
    {
        AudioSource res;
        if (sourcesFree == null)
            sourcesFree = new List<AudioSource>();

        if (sourcesFree.Count > 1) {

            //get from pool
            res = sourcesFree[0];
            sourcesFree.RemoveAt(0);
            return res;

        }
        else {

            //if pool is empty create new instance
            res = Instantiate(soundPrefab).GetComponent<AudioSource>();
            res.transform.SetParent(gameObject.transform);

            if (sourcesTotal == null)
                sourcesTotal = new List<AudioSource>();

            sourcesTotal.Add(res);
            return res;

        }
    }

    /// <summary>
    /// Resets the song cursor.
    /// </summary>
    public void ResetSongCursor()
    {
        songCursor = 0;
    }

    /// <summary>
    /// Plays the song note of current cursor.
    /// </summary>
    public void PlaySongNote()
    {
        if (songCursor == songPitches.Length)
            ResetSongCursor();

        PlaySound(8, 0.4f, songPitches[songCursor++]);
    }

    /// <summary>
    /// Gets random pitch from preset array.
    /// </summary>
    /// <returns>The pitch.</returns>
    public float RandomPitch()
    {
        //get random pitch ratio from array
        return pitches[UnityEngine.Random.Range(0, pitches.Length)];
    }

    /// <summary>
    /// Returns the AudioSource to pool.
    /// </summary>
    /// <param name="source">Source.</param>
    public void ReturnSourceToPool(AudioSource source)
    {
        sourcesFree.Add(source);
    }

    void Awake()
    {
        instance = this;

        //init sound pitches to diversify similar sounds
        pitches = new float[] { 0.92f, 0.96f, 1f, 1.04f, 1.08f };

        //song pitches
        songPitches = new float[] { 0.95f, 0.95f, 1.01f, 1.12f, 1.12f, 1.01f, 0.95f, 0.86f,
            0.76f, 0.76f, 0.86f, 0.95f, 0.95f, 0.86f, 0.86f,
            0.95f, 0.95f, 1.01f, 1.12f, 1.12f, 1.01f, 0.95f, 0.86f,
            0.76f, 0.76f, 0.86f, 0.95f, 0.86f, 0.76f, 0.76f
        };

        onPause = false;
        songCursor = 0;
        sourcesBg = new Dictionary<string, AudioSource>();
    }

    void OnEnable()
    {
        AdsManager.AdStarted += PauseSounds;
        AdsManager.AdFinished += ResumeSounds;
    }

    void OnDisable()
    {
        AdsManager.AdStarted -= PauseSounds;
        AdsManager.AdFinished -= ResumeSounds;
    }

    public void PauseSounds()
    {
        onPause = true;
    }

    public void ResumeSounds()
    {
        onPause = false;
    }


    public void PlaySound(int soundId, float volume, float delay, float pitch = 1f)
    {

        StartCoroutine(CoroutineExtension.ExecuteAfterTime(delay, () => {
            PlaySound(soundId, volume, pitch);
        }));

    }

    public void PlaySound(int soundId, float volume, float pitch = 1f)
    {
        return; // todo вырезал
        /*if (!GameSettings.Instance.Data.SoundEnabled || onPause)
            return;		
        
        if (soundId < 0)
            return;
        
        #if UNITY_IPHONE || UNITY_EDITOR
        if (soundId >= sounds.Length)
            return;
        #elif UNITY_ANDROID		
            if (soundId >= androidSounds.Length)
                return;
        #endif
    
        #if UNITY_IPHONE || UNITY_EDITOR

        AudioSource current = GetSource ();
        current.loop = false;
        current.volume = volume * (float)GameSettings.Instance.Data.Volume;
        current.pitch = pitch;
        current.clip = sounds [soundId];
        current.Play ();

        //when sound is ended return source to pool
        StartCoroutine (CoroutineExtension.ExecuteAfterTime (sounds [soundId].length, () => {
            ReturnSourceToPool (current);
        }));

        #else
            AndroidNativeAudio.play(androidSounds[soundId], volume * (float)GameSettings.Instance.Data.Volume, -1,1,0,pitch);
        #endif*/

    }

    public void PlaySound(int soundId)
    {
        PlaySound(soundId, 1f);
    }

    public void StopBackgroundSound(string name)
    {
        return; // todo вырезал
        /*if (!sourcesBg.ContainsKey (name))
			return;
		
		sourcesBg [name].Stop ();
		ReturnSourceToPool (sourcesBg [name]);
		sourcesBg.Remove (name);*/

    }

    public void PlayBackgroundSound(int soundId, float volume, string name)
    {
        return; // todo вырезал
        /*if (soundId < 0)
			return;
		
		if (soundId >= bgSounds.Length)
			return;

		if (sourcesBg.ContainsKey (name))
			return;

		AudioSource src = GetSource ();
		src.volume = volume;
		src.clip = bgSounds [soundId];
		src.loop = true;

		src.Play ();
		sourcesBg.Add (name, src);*/

    }

}
