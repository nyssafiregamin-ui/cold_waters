using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020000ED RID: 237
public class AudioManager : MonoBehaviour
{
	// Token: 0x06000642 RID: 1602 RVA: 0x0002CDCC File Offset: 0x0002AFCC
	public void PlayMusicClip(int clipIndex, string filepath = "")
	{
		if (clipIndex != -1)
		{
			filepath = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(this.musicTrackPaths[clipIndex]);
		}
		AudioClip audioClip = UIFunctions.globaluifunctions.textparser.GetAudioClip(filepath);
		if (this.musicSources[this.currentlyUsingTrack].clip == audioClip && this.musicSources[this.currentlyUsingTrack].isPlaying)
		{
			return;
		}
		float timeToReach = this.fadeTime;
		if (clipIndex == 6)
		{
			timeToReach = 0f;
		}
		if (this.currentlyUsingTrack == 0)
		{
			this.musicSources[1].Stop();
			this.musicSources[1].clip = audioClip;
			this.musicSources[1].Play();
			this.musicTwoSnapshot.TransitionTo(timeToReach);
			this.currentlyUsingTrack = 1;
		}
		else
		{
			this.musicSources[0].Stop();
			this.musicSources[0].clip = audioClip;
			this.musicSources[0].Play();
			this.musicOneSnapshot.TransitionTo(timeToReach);
			this.currentlyUsingTrack = 0;
		}
		base.enabled = true;
		this.fadeTimeTimer = 0f;
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x0002CEEC File Offset: 0x0002B0EC
	public void SetCombatSounds(bool isOn)
	{
		if (!isOn)
		{
			this.combatEffectsMuted.TransitionTo(0.3f);
		}
		else if (ManualCameraZoom.underwater)
		{
			this.GoDeepAudio(0.3f);
		}
		else
		{
			this.GoSurfaceAudio(0.3f);
		}
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x0002CF3C File Offset: 0x0002B13C
	public void InitialiseCombatMusic()
	{
		this.combatSources[0].Stop();
		this.combatSources[1].Stop();
		string filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(this.musicTrackPaths[2]);
		if (!UIFunctions.globaluifunctions.combatai.playerIsAmbushed)
		{
			if (GameDataManager.isNight)
			{
				filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(this.musicTrackPaths[4]);
			}
			else if (UIFunctions.globaluifunctions.levelloadmanager.icePresent || UIFunctions.globaluifunctions.levelloadmanager.packIcePresent)
			{
				filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(this.musicTrackPaths[5]);
			}
			else if (UnityEngine.Random.value < 0.5f)
			{
				filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(this.musicTrackPaths[3]);
			}
		}
		this.currentAmbientFilepath = filePathFromString;
		this.combatSources[0].clip = UIFunctions.globaluifunctions.textparser.GetAudioClip(this.currentAmbientFilepath);
		this.combatSources[0].Play();
		this.combatOneSnapshot.TransitionTo(this.fadeTime);
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x0002D070 File Offset: 0x0002B270
	public void PlayCombatAmbientMusic()
	{
		if (this.combatSources[0].clip == null)
		{
			string clipPath = this.currentAmbientFilepath;
			this.combatSources[0].clip = UIFunctions.globaluifunctions.textparser.GetAudioClip(clipPath);
		}
		if (!this.combatSources[0].isPlaying)
		{
			this.combatSources[0].Play();
		}
		this.combatOneSnapshot.TransitionTo(this.fadeTime);
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x0002D0EC File Offset: 0x0002B2EC
	public void PlayCombatActionMusic()
	{
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x0002D0FC File Offset: 0x0002B2FC
	public void StopMusic()
	{
		if (this.currentlyUsingTrack == 0)
		{
			this.currentlyUsingTrack = 1;
		}
		else
		{
			this.currentlyUsingTrack = 0;
		}
		this.musicMuted.TransitionTo(this.fadeTime);
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x0002D130 File Offset: 0x0002B330
	public void StopCombatMusic()
	{
		this.combatSources[0].Stop();
		this.combatSources[1].Stop();
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x0002D14C File Offset: 0x0002B34C
	public void SwitchToTrackTwo(float time)
	{
		this.musicTwoSnapshot.TransitionTo(time);
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x0002D15C File Offset: 0x0002B35C
	public void SwitchToTrackOne(float time)
	{
		this.musicOneSnapshot.TransitionTo(time);
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x0002D16C File Offset: 0x0002B36C
	public void GoDeepAudio(float time)
	{
		this.underwaterSnapshot.TransitionTo(time);
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x0002D17C File Offset: 0x0002B37C
	public void GoSurfaceAudio(float time)
	{
		this.surfaceSnapshot.TransitionTo(time);
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x0002D18C File Offset: 0x0002B38C
	public void SetMusicVolume(float musicLevel)
	{
		this.masterMixer.SetFloat("musicVolume", musicLevel);
		GameDataManager.currentmusicvolume = musicLevel;
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x0002D1A8 File Offset: 0x0002B3A8
	public void SetEffectsVolume(float effectsLevel)
	{
		this.masterMixer.SetFloat("effectsVolume", effectsLevel);
		GameDataManager.currentvolume = effectsLevel;
	}

	// Token: 0x040006A7 RID: 1703
	public AudioMixer masterMixer;

	// Token: 0x040006A8 RID: 1704
	public static AudioManager audiomanager;

	// Token: 0x040006A9 RID: 1705
	public AudioMixerSnapshot surfaceSnapshot;

	// Token: 0x040006AA RID: 1706
	public AudioMixerSnapshot underwaterSnapshot;

	// Token: 0x040006AB RID: 1707
	public AudioMixerSnapshot combatEffectsMuted;

	// Token: 0x040006AC RID: 1708
	public AudioMixerSnapshot musicMuted;

	// Token: 0x040006AD RID: 1709
	public AudioMixerSnapshot musicOneSnapshot;

	// Token: 0x040006AE RID: 1710
	public AudioMixerSnapshot musicTwoSnapshot;

	// Token: 0x040006AF RID: 1711
	public AudioMixerSnapshot combatOneSnapshot;

	// Token: 0x040006B0 RID: 1712
	public AudioMixerSnapshot combatTwoSnapshot;

	// Token: 0x040006B1 RID: 1713
	public AudioMixerSnapshot combatOffSnapshot;

	// Token: 0x040006B2 RID: 1714
	public AudioSource[] musicSources;

	// Token: 0x040006B3 RID: 1715
	public AudioSource[] combatSources;

	// Token: 0x040006B4 RID: 1716
	public int currentlyUsingTrack;

	// Token: 0x040006B5 RID: 1717
	public int currentlyCombatTrack;

	// Token: 0x040006B6 RID: 1718
	public float fadeTime = 2f;

	// Token: 0x040006B7 RID: 1719
	public float fadeTimeTimer;

	// Token: 0x040006B8 RID: 1720
	public string currentAmbientFilepath;

	// Token: 0x040006B9 RID: 1721
	public string[] musicTrackPaths;
}
