﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class VideoManager : MonoBehaviour {

    public VideoPlayer video;
    float currentTime;
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
      
	}
}
