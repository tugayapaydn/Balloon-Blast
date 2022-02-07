using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager Instance = null;

    public List<AudioClip> baloonPopClipList;
	public AudioClip musicClip1;

	public AudioSource audioSource;
	public AudioSource musicSource;

	public float volume = 0.5f;

    private void Awake()
    {
		// If there is not already an instance of SoundManager, set it to this.
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		//If an instance already exists, destroy whatever this object is to enforce the singleton.
		else
		{
			Instance = this;
		}

		//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
		DontDestroyOnLoad(gameObject);
		audioSource.volume = volume;
		musicSource.volume = volume;
	}

	// Play a single clip through the sound effects source.
	public void PlayClip(AudioClip clip)
	{
		audioSource.clip = clip;
		audioSource.Play();
	}

	public void PlayMusic(AudioClip clip)
    {
		musicSource.clip = clip;
		musicSource.Play();
    }

	// Play a single clip through the music source.
	public void PlayAudioList(AudioClip[] clip)
	{
		foreach(AudioClip ac in clip)
        {
			audioSource.clip = ac;
			audioSource.Play();
        }
	}
}
