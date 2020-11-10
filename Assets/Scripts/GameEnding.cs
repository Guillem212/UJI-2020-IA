﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnding : MonoBehaviour
{
    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;
    public GameObject player;
    public CanvasGroup exitBackgroundImageCanvasGroup;
    public AudioSource exitAudio;
    public CanvasGroup caughtBackgroundImageCanvasGroup;
    public AudioSource caughtAudio;
    public PPEffects pp;
    GameObject cam;

    //jumpscare
    public GameObject m_waluigi;
    public GameObject m_jumpScareWaluigiCamera;    


    bool m_IsPlayerAtExit;
    bool m_IsPlayerCaught;
    float m_Timer = -1.5f;
    bool m_HasAudioPlayed;

    private void Start()
    {
        pp = FindObjectOfType<PPEffects>();
        cam = Camera.main.gameObject;
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject == player)
        {
            if (other.gameObject.GetComponent<ViewController>().m_haveKey)
            {
                m_IsPlayerAtExit = true;
                other.gameObject.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("PauseMenu");
            }
        }
    }

    public void JumpScare()
    {
        //fix camera rotation
        cam.transform.eulerAngles = new Vector3(0f, cam.transform.rotation.eulerAngles.y, cam.transform.rotation.eulerAngles.z); 
        //lock player control
        player.GetComponent<InputManager>().playerInput.SwitchCurrentActionMap("PauseMenu");
        //change pp
        pp.EndChangePP();
        //fade out waluigi
        m_waluigi.GetComponent<Animator>().SetTrigger("FadeOut");
        //fade in waluigi scare        
        m_jumpScareWaluigiCamera.GetComponent<Animator>().SetBool("FadeIn", true);
        //sound
        AudioManager.instance.PlayerCaught();
        //fade out screen
        m_IsPlayerCaught = true;
    }        
    
    public void CaughtPlayer ()
    {
        m_IsPlayerCaught = true;
    }

    void Update ()
    {
        /*if (Input.GetKeyDown(KeyCode.T))
        {
            JumpScare();
        }*/

        if (m_IsPlayerAtExit)
        {
            EndLevel (exitBackgroundImageCanvasGroup, false);
        }
        else if (m_IsPlayerCaught)
        {
            EndLevel (caughtBackgroundImageCanvasGroup, true);
            cam.transform.eulerAngles = new Vector3(0f, cam.transform.rotation.eulerAngles.y, cam.transform.rotation.eulerAngles.z);
        }
    }

    void EndLevel (CanvasGroup imageCanvasGroup, bool doRestart)
    {                    
        m_Timer += Time.deltaTime;
        imageCanvasGroup.alpha = m_Timer / fadeDuration;

        if (m_Timer > fadeDuration + displayImageDuration)
        {           
            /*if (doRestart)
            {
                SceneManager.LoadScene (0);
            }
            else
            {
                Application.Quit ();
            }*/
        }
    }

}
