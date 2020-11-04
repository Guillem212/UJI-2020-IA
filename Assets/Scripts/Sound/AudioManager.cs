using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    GameObject m_waluigi;

    public Sound[] sounds;

    public AudioMixerGroup audioMixerMaster;

    public static AudioManager instance;

    public bool playAmbiente = true;

    Sound[] m_footsteps = new Sound[3];    
    [HideInInspector] public Sound m_breathingSound;

    bool createWaluigiAudioSources = false;

    private const int neutralSounds = 5;
    private const int alertSounds = 2;
    private const int detectedSounds = 2;

    private Sound[] m_waluigiNeutral = new Sound[neutralSounds];    
    private Sound[] m_waluigiAlert = new Sound[alertSounds];    
    private Sound[] m_waluigiDetected = new Sound[detectedSounds];    


    // Use this for initialization
    void Awake()
    {
        if (GameObject.Find("Waluigi") != null) //evitar error
        {
            createWaluigiAudioSources = true;
            m_waluigi = GameObject.Find("Waluigi");
        }
        else
        {
            Debug.LogWarning("Waluigi not detected in actual scene");
        }


        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        //some indexers
        var n = 0;
        var a = 0;
        var d = 0;
        var f = 0;

        foreach (Sound s in sounds)
        {
            if (s.name.Contains("Waluigi"))
            {
                if (!createWaluigiAudioSources) return;
                s.source = m_waluigi.AddComponent<AudioSource>();
                s.source.spatialBlend = 1f;

                if (s.name.Contains("Neutral"))
                {
                    m_waluigiNeutral[n] = s;
                    n++;
                } else if (s.name.Contains("Alert"))
                {
                    m_waluigiAlert[a] = s;
                    a++;
                } else
                {
                    m_waluigiDetected[d] = s;
                    d++;
                }

            }
            else
            {
                s.source = gameObject.AddComponent<AudioSource>();
            }

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = audioMixerMaster; //to control volume

            if (s.name.Contains("Ambient") && playAmbiente) Play(s.name);
            if (s.name.Contains("Footstep")) { m_footsteps[f] = s; f++; }

            //initiate tension sounds with 0 volume
            if (s.name.Contains("Tension")) { Play(s.name);  s.source.volume = 0f; }

        }                

        StartCoroutine(RandomNoises());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) WaluigiRandomSound();
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Stop();
    }    

    public void ReproduceFootsteps()
    {
        Play(m_footsteps[UnityEngine.Random.Range(0, 3)].name);        
    }    

    /// <summary>
    /// Applies to a sound a certain volume in a smooth way due to a lerp factor amount
    /// </summary>
    /// <param name="soundName"></param>
    /// <param name="soundVolume"></param>
    public void SetVolumeSmooth(string soundName, float soundVolume, float lerpAmount)
    {
        StartCoroutine(SmoothSound(soundName, soundVolume, lerpAmount));
    }
    
    private IEnumerator SmoothSound(string soundName, float desiredVolume, float lerpFactor) 
    {
        float lerpAmount = 0f;
        Sound desiredSound = Array.Find(sounds, sound => sound.name == soundName);
        float initialVolume = desiredSound.source.volume;//stores background music volume              
        while (lerpAmount < 1f)
        {
            desiredSound.source.volume = Mathf.Lerp(initialVolume, desiredVolume, lerpAmount);
            lerpAmount += lerpFactor * Time.deltaTime;
            yield return null;
        }
        yield return null;
    }


    /// <summary>
    /// Reproduces a sound each "Random" time
    /// </summary>
    /// <returns></returns>
    private IEnumerator RandomNoises()
    {        
        yield return new WaitForSeconds(UnityEngine.Random.Range(25f, 45f));
        if (UnityEngine.Random.Range(0f, 1f) > 0.5f) PlaySoundWithRandomPitch(0); //wood sound        
        yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 10f));
        if (UnityEngine.Random.Range(0f, 1f) > 0.5f) WaluigiRandomSound(); //waluigi sound        

        StartCoroutine(RandomNoises());
        yield return null;        
    }


    /// <summary>
    /// Reproduce one random neutral sound
    /// </summary>
    private void WaluigiRandomSound()
    {
        Play(m_waluigiNeutral[UnityEngine.Random.Range(0, neutralSounds)].name);
    }

    /// <summary>
    /// Reproduce one random alert sound
    /// </summary>
    public void WaluigiAngrySound()
    {
        Play(m_waluigiAlert[UnityEngine.Random.Range(0, alertSounds)].name);
    }

    /// <summary>
    /// Reproduce one random detected sound
    /// </summary>
    private void WaluigiDetectedSound()
    {
        Play(m_waluigiDetected[UnityEngine.Random.Range(0, detectedSounds)].name);
    }

    private void PlaySoundWithRandomPitch(int index)
    {
        if (index == 0) //Cracking wood
        {
            float rdmPitch = UnityEngine.Random.Range(0.85f, 1.15f); //pitch range
            Sound wood = Array.Find(sounds, sound => sound.name == "Wood");
            wood.source.pitch = rdmPitch;
            Play("Wood");
        }        
    }

    public void SetDetected(bool state)
    {
        if (state) StartCoroutine(DetectedIn());
        else StartCoroutine(DetectedOut());
    }

    private IEnumerator DetectedIn()
    {
        //DetectedMusicOut / In / Loop

        //play in
        Play("DetectedMusicIn");
        yield return new WaitForSeconds(0.5f);
        WaluigiDetectedSound();
        SetVolumeSmooth("DetectedMusicIn", 0f, 1f);
        Play("DetectedMusicLoop");
        SetVolumeSmooth("DetectedMusicLoop", 1f, 1f);
        yield return null;
    }

    private IEnumerator DetectedOut()
    {
        //DetectedMusicOut / In / Loop
        SetVolumeSmooth("DetectedMusicLoop", 0f, 1f);
        //play in
        yield return new WaitForSeconds(0.1f);
        Play("DetectedMusicOut");
        yield return new WaitForSeconds(0.2f);
        Stop("DetectedMusicLoop");
        yield return null;
    }

    #region PORSI
    /*
    //FROM THIS POINT THOSE ARE MY OWN FUNCTIONS, PLEASE NOTICE ME IF YOU CHANGE SOMETHING

    public void SetPitch(string name, float value) //changes pitch value of a sound
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.pitch = value;
    }

    public void ReduceMusicVolume(float time) //only background
    {
        StartCoroutine(TemporalSilence(time));
    }

    public void deathSound() //called from AttackCalculate.SetDeathState();
    {
        for (int s = 1; s < sounds.Length; s++) //stop all sounds except background
        {
            Stop(sounds[s].name);
        }
        FindObjectOfType<AudioManager>().Play("kallumDeath");
        ReduceMusicVolume(4.5f);//mute background sound
    }

    private IEnumerator TemporalSilence(float timeToWait) //stops soundtrack for x time and restore it again with a fade in
    {
        Sound backgroundSound = sounds[0];
        float originalVolume = backgroundSound.source.volume;//stores background music volume
        backgroundSound.source.volume = 0;
        yield return new WaitForSeconds(timeToWait);
        while (backgroundSound.source.volume < originalVolume)
        {
            //print("iteration");
            backgroundSound.source.volume += 0.005f;
            yield return null;
        }
    }
    */
    #endregion

}
