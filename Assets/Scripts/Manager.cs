using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using Vuforia;

public class Manager : MonoBehaviour
{
  int index = 0;
  bool firstTimeDetected = true;
  // public List<Texture2D> images;
  readonly List<VideoClip> videos = new();
  readonly List<string> stepList = new() { "Connect the other end of the resistor using jumper wire", "Connect the resistor to digital pin 13 of the Arduino", "Connect the short leg of the LED (cathode) using a jumper wire", "Connect the LED to the GND (Ground) pin of the Arduino", "Demo" };
  public SpriteRenderer arrowPin;
  public SpriteRenderer arrowGnd;
  public Button nextButton;
  public Button prevButton;
  public RawImage imageFrame;
  public TextMeshProUGUI textStep;
  public VideoPlayer videoPlayer;

  public ObserverBehaviour modelTargetBehaviour;

  public void OnModelDetected()
  {
    Debug.Log("Model Detected");
    if (firstTimeDetected)
    {
      firstTimeDetected = false;
      videoPlayer.clip = videos[index];
    }
    videoPlayer.Play();
  }

  public void OnModelLost()
  {
    Debug.Log("Model Lost");
    if (!firstTimeDetected)
    {
      videoPlayer.Pause();
    }
  }

  void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
  {
    Debug.Log("Target Status Changed");
    if (targetStatus.Status == Status.TRACKED)
    {
      OnModelDetected();
    }
    else
    {
      OnModelLost();
    }
  }

  void Start()
  {
    modelTargetBehaviour = GameObject.FindWithTag("ModelTarget").GetComponent<ObserverBehaviour>();
    modelTargetBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;

    Debug.Log("Manager Script Started");
    arrowPin = GameObject.FindWithTag("ArrowPin").GetComponent<SpriteRenderer>();
    arrowGnd = GameObject.FindWithTag("ArrowGnd").GetComponent<SpriteRenderer>();
    // move arrow sprite left and right
    LeanTween.moveX(arrowPin.gameObject, arrowPin.transform.position.x + 0.1f, 0.5f).setEaseInOutSine().setLoopPingPong();
    LeanTween.moveX(arrowGnd.gameObject, arrowGnd.transform.position.x - 0.1f, 0.5f).setEaseInOutSine().setLoopPingPong();

    arrowPin.enabled = false;
    arrowGnd.enabled = false;

    textStep = GameObject.FindWithTag("TextStep").GetComponent<TextMeshProUGUI>();
    textStep.text = stepList[index];

    GameObject nextButtonGameObject = GameObject.FindWithTag("NextButton");
    GameObject prevButtonGameObject = GameObject.FindWithTag("PrevButton");

    nextButton = nextButtonGameObject.GetComponent<Button>();
    prevButton = prevButtonGameObject.GetComponent<Button>();
    if (nextButton != null)
    {
      nextButton.onClick.AddListener(() =>
      {
        NextVideo();
        Debug.Log("Next Button Clicked");
      });
    }
    else
    {
      Debug.LogError("Next Button not found");
    }

    if (prevButton != null)
    {
      prevButton.onClick.AddListener(() =>
      {
        PrevVideo();
        Debug.Log("Prev Button Clicked");
      });
      prevButton.interactable = false;
    }
    else
    {
      Debug.LogError("Prev Button not found");
    }

    for (int i = 1; i <= 5; i++)
    {
      VideoClip video = Resources.Load<VideoClip>("Videos/" + i);
      videos.Add(video);
    }

    // Debug.Log(videos.Count);

    // for (int i = 0; i < 5; i++)
    // {
    //   Debug.Log(videos[i].name);
    // }

    // GameObject imageGameObject = GameObject.FindWithTag("ImageFrame");
    // imageFrame = imageGameObject?.GetComponent<RawImage>(); // Add null check here

    // if (imageFrame != null)
    // {
    //   Debug.Log("ImageFrame found");
    // }
    // else
    // {
    //   Debug.LogError("ImageFrame not found");
    // }


    GameObject videoPlayerGameObject = GameObject.FindWithTag("VideoPlayer");
    videoPlayer = videoPlayerGameObject.GetComponent<VideoPlayer>(); // Add null check here

    if (videoPlayer != null)
    {
      Debug.Log("VideoPlayer found");
    }
    else
    {
      Debug.LogError("VideoPlayer not found");
    }
  }

  // void nextImage()
  // {
  //   index++;
  //   if (index >= images.Count)
  //   {
  //     index = 0;
  //   }
  //   imageFrame.texture = images[index];
  // }

  // void prevImage()
  // {
  //   index--;
  //   if (index < 0)
  //   {
  //     index = images.Count - 1;
  //   }
  //   imageFrame.texture = images[index];
  // }

  void NextVideo()
  {
    index++;
    if (index >= videos.Count)
    {
      index = 0;
    }
    CheckButtonState();
    SwitchVideo();
  }

  void PrevVideo()
  {
    index--;
    if (index < 0)
    {
      index = videos.Count - 1;
    }
    CheckButtonState();
    SwitchVideo();
  }

  void CheckButtonState()
  {
    if (index == 0)
    {
      prevButton.interactable = false;
    }
    else
    {
      prevButton.interactable = true;
    }

    if (index == videos.Count - 1)
    {
      nextButton.interactable = false;
    }
    else
    {
      nextButton.interactable = true;
    }

    if (index == 1)
    {
      Debug.Log("Arrow Enabled");
      arrowPin.enabled = true;
    }
    else
    {
      Debug.Log("Arrow Disabled");
      arrowPin.enabled = false;
    }

    if (index == 3)
    {
      arrowGnd.enabled = true;
    }
    else
    {
      arrowGnd.enabled = false;
    }
  }

  void SwitchVideo()
  {
    videoPlayer.Stop();
    textStep.text = stepList[index];
    videoPlayer.clip = videos[index];
    videoPlayer.Play();
  }

  void FixedUpdate()
  {
    if (arrowPin == null || arrowGnd == null)
    {
      return;
    }

    arrowPin.enabled = false;
    arrowGnd.enabled = false;

    if (index == 1)
    {
      arrowPin.enabled = true;
    }

    if (index == 3)
    {
      arrowGnd.enabled = true;
    }
  }

}
