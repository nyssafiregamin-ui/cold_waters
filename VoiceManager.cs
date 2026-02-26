using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200016F RID: 367
public class VoiceManager : MonoBehaviour
{
	// Token: 0x06000B37 RID: 2871 RVA: 0x000A4A3C File Offset: 0x000A2C3C
	private void PlayAudioArray()
	{
		this.currentArrayIndex = 0;
		this.voiceSource.clip = this.audioClipsToPlay[this.currentArrayIndex];
		this.voiceSource.Play();
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x000A4A78 File Offset: 0x000A2C78
	private void PlayDelayedAudioArray()
	{
		this.currentArrayIndex = 0;
		this.voiceSource.clip = this.audioClipsToPlay[this.currentArrayIndex];
		this.voiceSource.PlayDelayed(1f);
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x000A4AB0 File Offset: 0x000A2CB0
	public void StopAudioArray()
	{
		this.currentArrayIndex = -1;
		this.currentMessagePriority = -1;
		this.voiceSource.Stop();
		this.audioClipsToPlay = new List<AudioClip>();
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x000A4AE4 File Offset: 0x000A2CE4
	private void Update()
	{
		if (this.currentArrayIndex >= 0 && !this.voiceSource.isPlaying)
		{
			this.currentArrayIndex++;
			if (this.currentArrayIndex >= this.audioClipsToPlay.Count)
			{
				this.StopAudioArray();
			}
			else
			{
				this.voiceSource.Stop();
				this.voiceSource.clip = this.audioClipsToPlay[this.currentArrayIndex];
				this.voiceSource.Play();
			}
		}
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x000A4B70 File Offset: 0x000A2D70
	private void GetSonarAudioClip(string tag)
	{
		switch (tag)
		{
		case "SONARBRG":
		{
			string text = UIFunctions.globaluifunctions.playerfunctions.sensormanager.bearingToContacts[this.currentVesselIndex].ToString("000");
			for (int i = 0; i < 3; i++)
			{
				string key = "SONAR" + text[i];
				if (LanguageManager.messageLogVoiceDictionary.ContainsKey(key))
				{
					this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary[key]));
				}
			}
			return;
		}
		case "SONARNOISEMAKERBRG":
			for (int j = 0; j < 3; j++)
			{
				string key2 = "SONAR" + this.currentNoisemakerBearing[j];
				if (LanguageManager.messageLogVoiceDictionary.ContainsKey(key2))
				{
					this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary[key2]));
				}
			}
			return;
		case "SONARTORPEDOBRG":
			for (int k = 0; k < 3; k++)
			{
				string key3 = "SONAR" + this.currentTorpedoBearing[k] + "E";
				if (LanguageManager.messageLogVoiceDictionary.ContainsKey(key3))
				{
					this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary[key3]));
				}
			}
			return;
		case "SONARDESIGNATE":
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialDetectedByPlayerName[this.currentVesselIndex] != null)
			{
				if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialDetectedByPlayerName[this.currentVesselIndex].Contains("S"))
				{
					if (LanguageManager.messageLogVoiceDictionary.ContainsKey("SONARDESIGNATES"))
					{
						this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary["SONARDESIGNATES"]));
					}
				}
				else if (LanguageManager.messageLogVoiceDictionary.ContainsKey("SONARDESIGNATEM"))
				{
					this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary["SONARDESIGNATEM"]));
				}
				string text2 = UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialDetectedByPlayerName[this.currentVesselIndex];
				text2 = text2.Replace("S", string.Empty);
				text2 = text2.Replace("M", string.Empty);
				text2 = text2.Replace("R", string.Empty);
				text2 = text2.Replace("E", string.Empty);
				text2 = text2.Replace("V", string.Empty);
				for (int l = 0; l < text2.Length; l++)
				{
					if (LanguageManager.messageLogVoiceDictionary.ContainsKey("SONAR" + text2[l]))
					{
						this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary["SONAR" + text2[l]]));
					}
				}
			}
			return;
		case "SONARCONTACT":
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialDetectedByPlayerName[this.currentVesselIndex] != null)
			{
				if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialDetectedByPlayerName[this.currentVesselIndex].Contains("S"))
				{
					if (LanguageManager.messageLogVoiceDictionary.ContainsKey("SONARCONTACTS"))
					{
						this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary["SONARCONTACTS"]));
					}
				}
				else if (LanguageManager.messageLogVoiceDictionary.ContainsKey("SONARCONTACTM"))
				{
					this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary["SONARCONTACTM"]));
				}
				string text3 = UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialDetectedByPlayerName[this.currentVesselIndex];
				text3 = text3.Replace("S", string.Empty);
				text3 = text3.Replace("M", string.Empty);
				text3 = text3.Replace("R", string.Empty);
				text3 = text3.Replace("E", string.Empty);
				text3 = text3.Replace("V", string.Empty);
				for (int m = 0; m < text3.Length; m++)
				{
					if (LanguageManager.messageLogVoiceDictionary.ContainsKey("SONAR" + text3[m]))
					{
						this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary["SONAR" + text3[m]]));
					}
				}
			}
			return;
		case "SONARTYPE":
		{
			string str = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.sensormanager.classifiedByPlayerAsClass[this.currentVesselIndex]].shipType.ToUpper();
			if (LanguageManager.messageLogVoiceDictionary.ContainsKey("SONARTYPE" + str))
			{
				this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary["SONARTYPE" + str]));
			}
			return;
		}
		}
		if (LanguageManager.messageLogVoiceDictionary.ContainsKey(tag))
		{
			this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary[tag]));
		}
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x000A51E8 File Offset: 0x000A33E8
	public void PlayMessageLogVoice(string lookupValue)
	{
		if (!this.voiceSource.enabled)
		{
			return;
		}
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialRun)
		{
			return;
		}
		if (this.voiceSource.volume == 0f)
		{
			return;
		}
		if (!LanguageManager.messageLogVoiceDictionary.ContainsKey(lookupValue))
		{
			return;
		}
		string[] array = LanguageManager.messageLogVoiceDictionary[lookupValue].Split(new char[]
		{
			'|'
		});
		if (this.voiceSource.isPlaying)
		{
			if (!array[2].Contains("DELAY"))
			{
				if (int.Parse(array[1]) >= this.currentMessagePriority)
				{
					return;
				}
			}
			else if (int.Parse(array[1]) > this.currentMessagePriority)
			{
				return;
			}
		}
		this.audioClipsToPlay = new List<AudioClip>();
		this.currentMessagePriority = int.Parse(array[1]);
		if (array[2] != "FALSE")
		{
			if (array[2].Contains("SONAR"))
			{
				if (!LanguageManager.messageLogVoiceDictionary.ContainsKey(lookupValue))
				{
					return;
				}
				if (array[2].Contains("SONARCONTACT") && UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialDetectedByPlayerName[this.currentVesselIndex] != null && !UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialDetectedByPlayerName[this.currentVesselIndex].Contains("S") && !UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialDetectedByPlayerName[this.currentVesselIndex].Contains("M"))
				{
					return;
				}
				string[] array2 = array[2].Split(new char[]
				{
					'_'
				});
				this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(array[0]));
				for (int i = 0; i < array2.Length; i++)
				{
					this.GetSonarAudioClip(array2[i]);
				}
				this.PlayAudioArray();
				return;
			}
			else if (array[2].Contains("WANTEDDEPTH"))
			{
				if (LanguageManager.messageLogVoiceDictionary.ContainsKey(lookupValue))
				{
					this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(array[0]));
					string text = this.currentWantedDepth;
					for (int j = 0; j < this.currentWantedDepth.Length; j++)
					{
						string key = "DIVEOFFICER" + this.currentWantedDepth[j];
						if (LanguageManager.messageLogVoiceDictionary.ContainsKey(key))
						{
							this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary[key]));
						}
					}
					if (LanguageManager.messageLogVoiceDictionary.ContainsKey("DIVEOFFICERFEET"))
					{
						this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary["DIVEOFFICERFEET"]));
					}
					if (!LanguageManager.messageLogVoiceDictionary.ContainsKey("DIVEOFFICERAYE"))
					{
						return;
					}
					this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary["DIVEOFFICERAYE"]));
				}
			}
			else
			{
				if (array[2].Contains("COMETOHEADING"))
				{
					if (LanguageManager.messageLogVoiceDictionary.ContainsKey(this.currentLeftOrRight))
					{
						this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary[this.currentLeftOrRight]));
					}
					for (int k = 0; k < this.currentCourse.Length; k++)
					{
						string key2 = "HELM" + this.currentCourse[k];
						if (LanguageManager.messageLogVoiceDictionary.ContainsKey(key2))
						{
							this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary[key2]));
						}
					}
					if (LanguageManager.messageLogVoiceDictionary.ContainsKey("HELMAYE"))
					{
						this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary["HELMAYE"]));
					}
					this.PlayAudioArray();
					return;
				}
				if (array[2].Contains("WANTEDSPEED"))
				{
					this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(array[0]));
					for (int l = 0; l < this.currentSpeed.Length; l++)
					{
						string key3 = "MANEUVERING" + this.currentSpeed[l];
						if (LanguageManager.messageLogVoiceDictionary.ContainsKey(key3))
						{
							this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary[key3]));
						}
					}
					if (LanguageManager.messageLogVoiceDictionary.ContainsKey("MANEUVERINGAYE"))
					{
						this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary["MANEUVERINGAYE"]));
					}
					this.PlayDelayedAudioArray();
					return;
				}
				if (array[2].Contains("ATSPEED"))
				{
					this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(array[0]));
					for (int m = 0; m < this.currentSpeed.Length; m++)
					{
						string key4 = "MANEUVERING" + this.currentSpeed[m];
						if (LanguageManager.messageLogVoiceDictionary.ContainsKey(key4))
						{
							this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary[key4]));
						}
					}
					if (LanguageManager.messageLogVoiceDictionary.ContainsKey("MANEUVERINGKNOTS"))
					{
						this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary["MANEUVERINGKNOTS"]));
					}
					this.PlayDelayedAudioArray();
					return;
				}
				if (array[2].Contains("FLOODEDCOMPARTMENT"))
				{
					if (LanguageManager.messageLogVoiceDictionary.ContainsKey("FLOODING" + this.currentFloodedCompartment))
					{
						this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary["FLOODING" + this.currentFloodedCompartment]));
					}
					this.PlayAudioArray();
					return;
				}
				if (array[2].Contains("DCCOMPARTMENT"))
				{
					if (LanguageManager.messageLogVoiceDictionary.ContainsKey("DCPARTY" + this.currentDCCompartment))
					{
						this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary["DCPARTY" + this.currentDCCompartment]));
					}
					this.PlayAudioArray();
					return;
				}
				if (array[2].Contains("DEPTH"))
				{
					if (!LanguageManager.messageLogVoiceDictionary.ContainsKey(lookupValue + this.currentDepthTier))
					{
						return;
					}
					array = LanguageManager.messageLogVoiceDictionary[lookupValue + this.currentDepthTier].Split(new char[]
					{
						'|'
					});
				}
				else if (array[2].Contains("TUBENUMBER"))
				{
					if (!LanguageManager.messageLogVoiceDictionary.ContainsKey(lookupValue + this.currentTubeNumber))
					{
						return;
					}
					array = LanguageManager.messageLogVoiceDictionary[lookupValue + this.currentTubeNumber].Split(new char[]
					{
						'|'
					});
				}
				else if (array[2].Contains("SUBSYSTEM"))
				{
					if (!LanguageManager.messageLogVoiceDictionary.ContainsKey(lookupValue + this.currentSubsystem))
					{
						return;
					}
					array = LanguageManager.messageLogVoiceDictionary[lookupValue + this.currentSubsystem].Split(new char[]
					{
						'|'
					});
				}
				else if (array[2].Contains("WATCHOFFICERBRG"))
				{
					string text2 = UIFunctions.globaluifunctions.playerfunctions.sensormanager.bearingToContacts[this.currentVesselIndex].ToString("000");
					for (int n = 0; n < 3; n++)
					{
						string key5 = "WATCHOFFICER" + text2[n];
						if (LanguageManager.messageLogVoiceDictionary.ContainsKey(key5))
						{
							this.audioClipsToPlay.Add(UIFunctions.globaluifunctions.textparser.GetAudioClip(LanguageManager.messageLogVoiceDictionary[key5]));
						}
					}
					return;
				}
			}
		}
		this.currentArrayIndex = 0;
		UIFunctions.globaluifunctions.playerfunctions.voicemanager.voiceSource.clip = UIFunctions.globaluifunctions.textparser.GetAudioClip(array[0]);
		UIFunctions.globaluifunctions.playerfunctions.voicemanager.voiceSource.Play();
	}

	// Token: 0x04001221 RID: 4641
	public AudioSource voiceSource;

	// Token: 0x04001222 RID: 4642
	public string currentDepthTier;

	// Token: 0x04001223 RID: 4643
	public string currentTubeNumber;

	// Token: 0x04001224 RID: 4644
	public string currentSubsystem;

	// Token: 0x04001225 RID: 4645
	public string currentNoisemakerBearing;

	// Token: 0x04001226 RID: 4646
	public string currentTorpedoBearing;

	// Token: 0x04001227 RID: 4647
	public string currentWantedDepth;

	// Token: 0x04001228 RID: 4648
	public string currentLeftOrRight;

	// Token: 0x04001229 RID: 4649
	public string currentCourse;

	// Token: 0x0400122A RID: 4650
	public string currentSpeed;

	// Token: 0x0400122B RID: 4651
	public string currentFloodedCompartment;

	// Token: 0x0400122C RID: 4652
	public string currentDCCompartment;

	// Token: 0x0400122D RID: 4653
	public int currentVesselIndex;

	// Token: 0x0400122E RID: 4654
	public int currentMessagePriority;

	// Token: 0x0400122F RID: 4655
	public int currentArrayIndex;

	// Token: 0x04001230 RID: 4656
	public List<AudioClip> audioClipsToPlay;
}
