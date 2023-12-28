using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Schizo.Audio
{
	public enum ClipType { Sfx, Voice, Background, Music };
	public class AudioSystem : MonoBehaviour
	{
		private static AudioSystem instance;
		public static AudioSystem Instance => instance;

		/// <summary>
		/// Count of main audio tracks (Sound effects)
		/// </summary>
		[SerializeField]
		private int sfxTrackCount = 3;

		[SerializeField]
		[Range(0f, 1.0f)]
		private float masterVolume = 1.0f;

		[SerializeField]
		[Range(0f, 1.0f)]
		private float sfxVolume = 0.8f;

		[SerializeField]
		[Range(0f, 1.0f)]
		private float voiceVolume = 1.0f;

		[SerializeField]
		[Range(0f, 1.0f)]
		private float backgroundVolume = 0.6f;

		[SerializeField]
		[Range(0f, 1.0f)]
		private float musicVolume = 0.75f;

		/// <summary>
		/// Main tracks array, used for sound effects
		/// </summary>
		private AudioSource[] sfxTracks;
		/// <summary>
		/// Lead audio track, used for prompts and voice overs
		/// </summary>
		private AudioSource voiceTrack;
		/// <summary>
		/// Sub track used for background ambient noise
		/// </summary>
		private AudioSource backgroundTrack;
		/// <summary>
		/// Music track
		/// </summary>
		private AudioSource musicTrack;

		private int currentSfx = 0;

		private void Awake()
		{
			DontDestroyOnLoad(this.gameObject);

			if (instance != null && instance != this)
			{
				Destroy(this);
			}
			else
			{
				instance = this;
			}

			CreateSources();
		}

		private void CreateSources()
		{
			sfxTracks = new AudioSource[sfxTrackCount];

			for (int i = 0; i < sfxTrackCount; i++)
			{
				AudioSource audio = gameObject.AddComponent<AudioSource>();
				sfxTracks[i] = audio;
			}

			voiceTrack = gameObject.AddComponent<AudioSource>();
			backgroundTrack = gameObject.AddComponent<AudioSource>();
			musicTrack = gameObject.AddComponent<AudioSource>();

			ResetVolumes();
		}

		/// <summary>
		/// Call to reload volume values
		/// To be called after volume value changes
		/// </summary>
		public void ResetVolumes()
		{
			for (int i = 0; i < sfxTrackCount; i++)
			{
				SetupSource(sfxTracks[i], sfxVolume);
			}

			SetupSource(voiceTrack, voiceVolume);
			SetupSource(backgroundTrack, backgroundVolume);
			SetupSource(musicTrack, musicVolume, true);
		}


		/// <summary>
		/// Play a sound effect; loops through the entirety of available tracks and cycles back to the first track when it runs out
		/// Will allow to play N sound effects at a time <see cref="sfxTrackCount"/>
		/// </summary>
		/// <param name="clip">Audio clip to play</param>
		private void PlaySfx(AudioClip clip)
		{
			sfxTracks[currentSfx].Stop(); // Kill whatever this track was playing
			sfxTracks[currentSfx].clip = clip;
			sfxTracks[currentSfx].Play();

			currentSfx++;
			if (currentSfx > masterVolume) { currentSfx = 0; }
		}

		public void Play(AudioClip clip, ClipType at)
		{
			AudioSource source = voiceTrack;
			switch (at)
			{
				case ClipType.Sfx:
					PlaySfx(clip);
					return;
					break;
				case ClipType.Voice:
					source = voiceTrack;
					break;
				case ClipType.Background:
					source = backgroundTrack;
					break;
				case ClipType.Music:
					source = musicTrack;
					break;
			}

			source.clip = clip;
			source.Play();
		}

		/// <summary>
		/// Compounds a desired volume by the value of <see cref="masterVolume"/>
		/// </summary>
		/// <param name="audio">Source to affect</param>
		/// <param name="volume">Value of this source's volume</param>
		private void SetupSource(AudioSource audio, float volume, bool loop = false)
		{
			audio.volume = volume * masterVolume;
			audio.spatialBlend = 0.0f;
			audio.loop = loop;
		}

	}
}
