using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour {

    public static Sound me;
    public GameObject sourcePrefab;
    public AudioSource[] sources;
    public int sourceNum;
    bool interrupt;

	// Use this for initialization
	void Start () {
        interrupt = false;
        me = this;
        sources = new AudioSource[sourceNum];
        for (int i = 0; i < sourceNum; i++)
        {
            sources[i] = ((GameObject)Instantiate(sourcePrefab, Vector3.zero, Quaternion.identity)).GetComponent<AudioSource>();
        }
	}
	public void Stop(AudioClip clip){
		foreach (AudioSource a in sources) {
			if (a.clip == clip) {
				a.Stop ();
				break;
			}
		}

	}
	public void Play(AudioClip clip, float volume, bool randoPitch, float point = 0)
    {
        AudioSource source = null;
        for (int i = 0; i < sourceNum; i++)
        {
            if (!sources[i].isPlaying)
            {
                source = sources[i];
                break;
            }
        }
        Debug.Log(source + " is source");
        source.volume = volume;
        source.clip = clip;
        if (randoPitch)
        {
            source.pitch = Random.Range(.8f, 1.2f);
        } else {
            source.pitch = 1f;
        }

		if (point > 0) {
			source.time = point;
		}
        source.Play();

    }

    public void Play(AudioClip clip)
    {
        Play(clip, 1f, false);
    }

}
