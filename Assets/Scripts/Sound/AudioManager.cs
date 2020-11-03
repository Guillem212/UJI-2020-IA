using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    public AudioMixerGroup audioMixerMaster;

    public static AudioManager instance;

    public bool playAmbiente = true;

    Sound[] m_footsteps = new Sound[3];
    int footindex = 0;


    // Use this for initialization
    void Awake()
    {

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = audioMixerMaster; //to control volume

            if (s.name.Contains("Ambient") && playAmbiente) Play(s.name);
            if (s.name.Contains("Footstep")) { m_footsteps[footindex] = s; footindex++; }
        }

        StartCoroutine(RandomNoises());
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

    private IEnumerator RandomNoises()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(15f, 40f));
        //PlaySoundWithRandomPitch(0);
        Play("Wood");
        StartCoroutine(RandomNoises());
        yield return null;        
    }

    public void PlaySoundWithRandomPitch(int index)
    {
        if (index == 0) //Cracking wood
        {
            float rdmPitch = UnityEngine.Random.Range(0.85f, 1.15f); //pitch range
            Sound wood = Array.Find(sounds, sound => sound.name == "wood");
            wood.source.pitch = rdmPitch;
            Play("Wood");
        }        
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
