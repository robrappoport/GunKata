using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound : MonoBehaviour {

    public static Sound me;
    public GameObject sourcePrefab;
    public AudioSource[] sources;
    public AudioMixerGroup SFXMixer;
    public int sourceNum;
    bool interrupt;
    string defaultName;
	// Use this for initialization
	void Start () {
        interrupt = false;
        me = this;
        sources = new AudioSource[sourceNum];
        for (int i = 0; i < sourceNum; i++)
        {
            sources[i] = ((GameObject)Instantiate(sourcePrefab, Vector3.zero, Quaternion.identity)).GetComponent<AudioSource>();
        }
        defaultName = sources[0].name;

        foreach(AudioSource a in sources)
        {
            a.outputAudioMixerGroup = SFXMixer;
        }
	}

	public void ToggleMuteAllSound(){
		foreach (AudioSource a in sources) {
			a.mute = !a.mute;
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

    public bool IsPlaying(AudioClip clip, float pitch = 10, string myName = "Audio Source(Clone)"){
        foreach(AudioSource a in sources){

            if (myName != defaultName)
            {
                if (a.clip == clip)
                {

                    if (a.isPlaying)
                    {
                        if (pitch != 10)
                        {
                            if (a.pitch == pitch)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }else{
                if(a.name == myName){
                    if(a.isPlaying)
                    
                    {
                        return true;
                    }else{
                        return false;
                    }
                }
            }
        }
        return false;
    }
    public void Play(AudioClip clip, float volume, bool randoPitch, float point = 0, float pitch = 1, string myName = "Audio Source(Clone)")
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
        source.pitch = pitch;

        if (myName != defaultName) {
            source.name = myName;
        }
    }

    public void Play(AudioClip clip)
    {
        Play(clip, 1f, false);
    }

    public void Play(AudioClip clip, float pitch)
    {
        Play(clip, 1f, false, 0, pitch);
    }

    public void Play(AudioClip clip, string myName)
	{
        Play(clip, 1f, false, 0, 1, myName);
	}
}
