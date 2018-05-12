using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
public class VideoManager : MonoBehaviour {

    float timeElapsed = 0;
    public VideoPlayer video;
    float currentTime;
    public GameObject startCanvas;
	// Use this for initialization
	void Start () {
        video = GetComponent<VideoPlayer>();
        currentTime = Time.time;
	}

    // Update is called once per frame
    void Update()
    {
        if (!video.isPlaying){
            if (Time.time - currentTime > video.clip.length)
            {
                gameObject.SetActive(false);
            }
        }

        if (Time.time - currentTime > 3f)
        {
            if (!startCanvas.activeSelf)
            startCanvas.SetActive(true);
        }
      
	}
}
