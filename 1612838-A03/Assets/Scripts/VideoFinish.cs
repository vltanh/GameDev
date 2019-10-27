using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoFinish : MonoBehaviour
{
    public GameObject mainMenu;
    VideoPlayer videoPlayer;
    RawImage rawImage;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        rawImage = GetComponent<RawImage>();
        videoPlayer.loopPointReached += VideoEnd;
    }

    private void VideoEnd(VideoPlayer source)
    {
        source.enabled = false;
        rawImage.enabled = false;
        mainMenu.SetActive(true);
    }
}