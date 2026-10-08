using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	public static AudioManager me;

	public AudioMixer mixer;

	public AudioClip BattleLoop;

	public AudioClip SpiritLoop;

	private AudioSource ambienceSource;

	public List<AudioClip> CancelSadness;

	public List<AudioClip> Click;

	public List<AudioClip> SpiritTransitionExit;

	public List<AudioClip> SpiritTransitionEnter;

	public List<AudioClip> CitiesTransitionEnter;

	public List<AudioClip> CitiesTransitionExit;

	public List<AudioClip> Eat;

	public List<AudioClip> GetSick;

	public List<AudioClip> BecomeTeenager;

	public List<AudioClip> BecomeAdult;

	public List<AudioClip> BecomeOld;

	public List<AudioClip> ConsumeHappiness;

	public List<AudioClip> Unhappy;

	public List<AudioClip> Angry;

	public AudioMixerGroup SfxGroup;

	public AudioMixerGroup MusicGroup;

	public List<AudioClip> DropOnStack;

	public List<AudioClip> CardPickup;

	public List<AudioClip> CardDrop;

	public List<AudioClip> CardDestroy;

	public AudioClip QuestComplete;

	public List<AudioClip> MediumPickup;

	public List<AudioClip> HeavyPickup;

	public AudioClip EndOfMoon;

	public List<AudioClip> CardCreate;

	public List<AudioClip> OpenBooster;

	public List<AudioClip> Coin;

	public List<AudioClip> Dollar;

	public List<AudioClip> AnimalMove;

	public List<AudioClip> Miss;

	public List<AudioClip> Block;

	public List<AudioClip> HitMelee;

	public List<AudioClip> HitRanged;

	public List<AudioClip> HitMagic;

	public List<AudioClip> HitFoot;

	public List<AudioClip> HitArmour;

	public List<AudioClip> HitAir;

	public List<AudioClip> MissCities;

	public List<AudioClip> Buff;

	public List<AudioClip> MagicRelease;

	public List<AudioClip> MagicCharge;

	public List<AudioClip> RangedRelease;

	public List<AudioClip> Crit;

	public List<AudioClip> ShamanSpawn;

	public List<AudioClip> Bleed;

	public List<AudioClip> Poison;

	[Header("Cities")]
	public AudioClip EnergyConnected;

	public AudioClip EnergyStrech;

	public AudioClip EnergyStart;

	public AudioClip SewerConnected;

	public AudioClip SewerStrech;

	public AudioClip SewerStart;

	public AudioClip TransportConnected;

	public AudioClip TransportStrech;

	public AudioClip TransportStart;

	public AudioClip AddWellbeing;

	public AudioClip LostWellbeing;

	public AudioClip WellbeingCounter;

	public AudioClip PowerOutage;

	public AudioClip LandmarkBuild;

	public AudioClip ExtinguishCardSound;

	public AudioClip RepairCardSound;

	public AudioClip DamagedCardSound;

	public AudioClip OnFireCardSound;

	public AudioClip DroughtStart;

	public AudioClip DroughtSolved;

	public AudioClip IndustrialRevolutionCreate;

	public AudioClip PositiveEventSpawn;

	public AudioClip NegativeEventSpawn;

	public AudioClip ColliderRunningSound;

	public AudioClip AdvisorAppears;

	public AudioClip AdvisorWarning;

	public AudioClip AdvisorTalking;

	public AudioClip ClearPollution;

	public AudioClip SpawnPollution;

	public AudioClip LandfillOverflow;

	private AudioSource songSource;

	private AudioSource battleSource;

	private AudioSource spiritSource;

	private List<AudioClip> playedSongs = new List<AudioClip>();

	private float currentSongTargetVolume;

	private float newSongTimer;

	private bool muteSong;

	private List<AudioSource> sources = new List<AudioSource>();

	private List<Transform> targets = new List<Transform>();

	private void Awake()
	{
		me = this;
		InitializeAudioSources();
	}

	private void Start()
	{
		ambienceSource = GetSource(null, claim: true);
		ambienceSource.spatialBlend = 0f;
		ambienceSource.clip = DetermineCurrentAmbience();
		ambienceSource.volume = DetermineCurrentAmbienceVolume();
		ambienceSource.reverbZoneMix = 0f;
		ambienceSource.outputAudioMixerGroup = MusicGroup;
		ambienceSource.loop = true;
		ambienceSource.Play();
		songSource = GetSource(null, claim: true);
		songSource.spatialBlend = 0f;
		songSource.reverbZoneMix = 0f;
		songSource.outputAudioMixerGroup = MusicGroup;
		songSource.volume = 0f;
		songSource.time = 0f;
		AudioClipWithVolume audioClipWithVolume = DetermineCurrentSong();
		songSource.clip = audioClipWithVolume.Clip;
		currentSongTargetVolume = audioClipWithVolume.Volume;
		songSource.Play();
		battleSource = GetSource(null, claim: true);
		battleSource.spatialBlend = 0f;
		battleSource.reverbZoneMix = 0f;
		battleSource.outputAudioMixerGroup = SfxGroup;
		battleSource.volume = 0f;
		battleSource.clip = BattleLoop;
		battleSource.loop = true;
		battleSource.Play();
		spiritSource = GetSource(null, claim: true);
		spiritSource.spatialBlend = 0f;
		spiritSource.reverbZoneMix = 0f;
		spiritSource.outputAudioMixerGroup = SfxGroup;
		spiritSource.volume = 0f;
		spiritSource.clip = SpiritLoop;
		spiritSource.loop = true;
		spiritSource.Play();
	}

	public void SkipSong()
	{
		songSource.time = songSource.clip.length - 5.1f;
	}

	private AudioClipWithVolume DetermineCurrentSong()
	{
		GameBoard curBoard = WorldManager.instance.GetCurrentBoardSafe();
		if (curBoard.BoardOptions.Songs.Count == 1)
		{
			return curBoard.BoardOptions.Songs.FirstOrDefault();
		}
		List<AudioClipWithVolume> list = curBoard.BoardOptions.Songs.FindAll((AudioClipWithVolume x) => !playedSongs.Contains(x.Clip));
		if (list.Count < 1)
		{
			playedSongs.RemoveAll((AudioClip x) => curBoard.BoardOptions.Songs.Any((AudioClipWithVolume v) => v.Clip == x) && x != songSource.clip);
			list = curBoard.BoardOptions.Songs.FindAll((AudioClipWithVolume x) => !playedSongs.Contains(x.Clip));
		}
		AudioClipWithVolume audioClipWithVolume = ((list.Count > 0) ? list.Choose() : null);
		if (audioClipWithVolume != null && !playedSongs.Contains(audioClipWithVolume.Clip))
		{
			playedSongs.Add(audioClipWithVolume.Clip);
		}
		return audioClipWithVolume;
	}

	private AudioClip DetermineCurrentAmbience()
	{
		return WorldManager.instance.GetCurrentBoardSafe().BoardOptions.Ambience;
	}

	private float DetermineCurrentAmbienceVolume()
	{
		return WorldManager.instance.GetCurrentBoardSafe().BoardOptions.AmbienceVolume;
	}

	private bool CurrentSongShouldStop()
	{
		AudioClip currentSong = songSource.clip;
		if (WorldManager.instance.CurrentBoard != null && !WorldManager.instance.CurrentBoard.BoardOptions.Songs.Any((AudioClipWithVolume x) => x.Clip == currentSong))
		{
			return true;
		}
		return false;
	}

	private bool AnyCardInConflict()
	{
		foreach (GameCard allCard in WorldManager.instance.AllCards)
		{
			if (allCard.MyBoard.IsCurrent && allCard.Combatable != null && allCard.InConflict)
			{
				return true;
			}
		}
		return false;
	}

	private bool SpiritOnBoard()
	{
		return WorldManager.instance.GetCard<Spirit>() != null;
	}

	private void LateUpdate()
	{
		MusicGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log(OptionsScreen.MusicVol) * 20f);
		SfxGroup.audioMixer.SetFloat("SfxVolume", Mathf.Log(OptionsScreen.SfxVol) * 20f);
		float b = currentSongTargetVolume;
		if (songSource.clip != null)
		{
			if (songSource.time >= songSource.clip.length - 5f || CurrentSongShouldStop())
			{
				b = 0f;
				newSongTimer += Time.deltaTime;
			}
			if (WorldManager.instance.GetCardCount<Spirit>() > 0)
			{
				b = 0f;
			}
		}
		if (WorldManager.instance.CurrentGameState == WorldManager.GameState.GameOver)
		{
			b = 0f;
		}
		if (muteSong)
		{
			b = 0f;
		}
		if (!songSource.isPlaying)
		{
			newSongTimer += Time.deltaTime;
		}
		battleSource.volume = Mathf.Lerp(battleSource.volume, AnyCardInConflict() ? 0.01f : 0f, Time.unscaledDeltaTime);
		spiritSource.volume = Mathf.Lerp(spiritSource.volume, SpiritOnBoard() ? 0.5f : 0f, Time.unscaledDeltaTime);
		float b2 = 1f;
		if (WorldManager.instance.SpeedUp == 0f)
		{
			b2 = 0.8f;
		}
		else if (WorldManager.instance.SpeedUp == 5f)
		{
			b2 = 1.2f;
		}
		songSource.pitch = Mathf.Lerp(songSource.pitch, b2, Time.deltaTime * 12f);
		if (newSongTimer >= 4f)
		{
			newSongTimer = 0f;
			AudioClipWithVolume audioClipWithVolume = DetermineCurrentSong();
			songSource.clip = audioClipWithVolume.Clip;
			currentSongTargetVolume = audioClipWithVolume.Volume;
			songSource.time = 0f;
			songSource.Play();
		}
		songSource.volume = Mathf.Lerp(songSource.volume, b, Time.unscaledDeltaTime);
		UpdateAmbience();
		for (int i = 0; i < sources.Count; i++)
		{
			if (targets[i] != null)
			{
				sources[i].transform.position = targets[i].transform.position;
			}
		}
	}

	private void UpdateAmbience()
	{
		AudioClip audioClip = DetermineCurrentAmbience();
		if (ambienceSource.clip != audioClip)
		{
			ambienceSource.volume = Mathf.Lerp(ambienceSource.volume, 0f, Time.unscaledDeltaTime);
			if (ambienceSource.volume < 0.001f)
			{
				ambienceSource.clip = audioClip;
				ambienceSource.Play();
			}
		}
		else
		{
			ambienceSource.volume = Mathf.Lerp(ambienceSource.volume, DetermineCurrentAmbienceVolume(), Time.unscaledDeltaTime);
		}
	}

	private void InitializeAudioSources()
	{
		for (int i = 0; i < 100; i++)
		{
			GameObject obj = new GameObject("Audio " + i);
			obj.transform.SetParent(base.transform);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;
			obj.transform.localScale = Vector3.zero;
			AudioSource audioSource = obj.AddComponent<AudioSource>();
			audioSource.outputAudioMixerGroup = SfxGroup;
			audioSource.loop = false;
			audioSource.playOnAwake = false;
			audioSource.spatialBlend = 0f;
			audioSource.minDistance = 20f;
			audioSource.priority = 10;
			sources.Add(audioSource);
			targets.Add(null);
		}
	}

	public AudioSource GetSource(Transform target = null, bool claim = false)
	{
		AudioSource audioSource = null;
		for (int i = 0; i < sources.Count; i++)
		{
			if (!sources[i].isPlaying)
			{
				audioSource = sources[i];
				targets[i] = target;
				break;
			}
		}
		if (claim)
		{
			sources.Remove(audioSource);
		}
		return audioSource;
	}

	public void PlaySound(List<AudioClip> clips, Vector3 pos, float pitch = 1f, float vol = 1f)
	{
		PlaySound(clips[Random.Range(0, clips.Count)], pos, pitch, vol);
	}

	public void PlaySound(AudioClip clip, Vector3 pos, float pitch = 1f, float vol = 1f)
	{
		AudioSource source = GetSource();
		if (!(source == null))
		{
			source.pitch = pitch;
			source.clip = clip;
			source.volume = vol;
			source.transform.position = pos;
			source.spatialBlend = 1f;
			source.reverbZoneMix = 1f;
			source.bypassListenerEffects = false;
			source.Play();
		}
	}

	public void PlaySound(AudioClip clip, Transform t, float pitch = 1f, float vol = 1f)
	{
		AudioSource source = GetSource(t);
		if (!(source == null))
		{
			source.pitch = pitch;
			source.clip = clip;
			source.volume = vol;
			source.reverbZoneMix = 0f;
			source.transform.localPosition = Vector3.zero;
			source.spatialBlend = 1f;
			source.bypassListenerEffects = false;
			source.Play();
		}
	}

	public void PlaySound2D(List<AudioClip> clips, float pitch = 1f, float vol = 1f)
	{
		PlaySound2D(clips[Random.Range(0, clips.Count)], pitch, vol);
	}

	public void PlaySound2D(AudioClip clip, float pitch, float vol)
	{
		AudioSource source = GetSource();
		if (!(source == null))
		{
			source.pitch = pitch;
			source.clip = clip;
			source.volume = vol;
			source.reverbZoneMix = 0f;
			source.spatialBlend = 0f;
			source.bypassListenerEffects = false;
			source.Play();
		}
	}

	public AudioSource PlayLoop2D(AudioClip clip, float pitch, float vol)
	{
		AudioSource source = GetSource();
		if (source == null)
		{
			return null;
		}
		source.pitch = pitch;
		source.clip = clip;
		source.volume = vol;
		source.reverbZoneMix = 0f;
		source.spatialBlend = 0f;
		source.bypassListenerEffects = false;
		source.loop = true;
		source.Play();
		return source;
	}

	public void ReleaseLoopedSource(AudioSource source)
	{
		if (source != null)
		{
			source.loop = false;
			source.Stop();
		}
	}

	public List<AudioClip> GetSoundForPickupSoundGroup(PickupSoundGroup group)
	{
		return group switch
		{
			PickupSoundGroup.Medium => MediumPickup, 
			PickupSoundGroup.Heavy => HeavyPickup, 
			_ => CardPickup, 
		};
	}
}
