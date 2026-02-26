using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x0200016D RID: 365
public class VesselBuilder : MonoBehaviour
{
	// Token: 0x06000B1C RID: 2844 RVA: 0x0009D310 File Offset: 0x0009B510
	private void Start()
	{
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x0009D314 File Offset: 0x0009B514
	public void CreateWeaponPrefabObject(int weaponID)
	{
		string weaponPrefabName = UIFunctions.globaluifunctions.database.databaseweapondata[weaponID].weaponPrefabName;
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load("template_objects/torpedoTemplate"));
		UIFunctions.globaluifunctions.database.databaseweapondata[weaponID].weaponObject = gameObject;
		UIFunctions.globaluifunctions.database.databaseweapondata[weaponID].weaponObject.name = weaponPrefabName;
		this.CreateAndPlaceWeaponMeshes(gameObject, weaponID, weaponPrefabName);
		gameObject.transform.SetParent(UIFunctions.globaluifunctions.prePlacedObjects);
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x0009D3A0 File Offset: 0x0009B5A0
	private void CreateAndPlaceWeaponMeshes(GameObject weaponTemplate, int weaponID, string weaponPrefabRef)
	{
		Torpedo component = UIFunctions.globaluifunctions.database.databaseweapondata[weaponID].weaponObject.GetComponent<Torpedo>();
		Vector3 localPosition = Vector3.zero;
		Vector3 vector = Vector3.zero;
		Material material = null;
		AudioSource audioSource = null;
		Transform transform = weaponTemplate.transform;
		this.currentMesh = null;
		float speed = 0f;
		int num = 0;
		if (UIFunctions.globaluifunctions.database.databaseweapondata[weaponID].weaponType == "MISSILE")
		{
			component.boxcollider.size = new Vector3(0.02f, 0.02f, 0.5f);
		}
		bool flag = false;
		bool flag2 = false;
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile("weapons");
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			if (array2[0].Trim() == "WeaponObjectReference")
			{
				if (array2[1].Trim() == weaponPrefabRef)
				{
					flag = true;
				}
			}
			else if (array2[0].Trim() == "[Model]")
			{
				flag2 = true;
			}
			else if (array2[0].Trim() == "[/Model]" && flag)
			{
				return;
			}
			if (flag2 && flag)
			{
				string text = array2[0];
				switch (text)
				{
				case "ModelFile":
					this.allMeshes = Resources.LoadAll<Mesh>(array2[1].Trim());
					break;
				case "MeshPosition":
					localPosition = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
					break;
				case "MeshRotation":
					vector = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
					break;
				case "Material":
					material = (Resources.Load(array2[1].Trim()) as Material);
					break;
				case "MaterialTextures":
					if (material != null)
					{
						string[] array3 = array2[1].Trim().Split(new char[]
						{
							','
						});
						material.SetTexture("_MainTex", UIFunctions.globaluifunctions.textparser.GetTexture(array3[0]));
						if (array3.Length > 1)
						{
							material.SetTexture("_SpecTex", UIFunctions.globaluifunctions.textparser.GetTexture(array3[1]));
						}
						if (array3.Length > 2)
						{
							material.SetTexture("_BumpMap", UIFunctions.globaluifunctions.textparser.GetTexture(array3[2]));
						}
					}
					break;
				case "MeshWeapon":
					component.torpedoMeshes[0].GetComponent<MeshFilter>().mesh = this.GetMesh(array2[1].Trim());
					component.torpedoMeshes[0].GetComponent<MeshRenderer>().sharedMaterial = material;
					component.torpedoMeshes[0].transform.localPosition = localPosition;
					break;
				case "MeshWeaponCanister":
					component.torpedoMeshes[1].GetComponent<MeshFilter>().mesh = this.GetMesh(array2[1].Trim());
					component.torpedoMeshes[1].GetComponent<MeshRenderer>().sharedMaterial = material;
					component.torpedoMeshes[1].transform.localPosition = localPosition;
					component.torpedoMeshes[1].gameObject.SetActive(true);
					component.torpedoMeshes[0].layer = 17;
					break;
				case "MeshWeaponPropRotation":
					speed = float.Parse(array2[1].Trim());
					break;
				case "MeshWeaponProp":
					component.torpedoPropMeshes[num].GetComponent<MeshFilter>().mesh = this.GetMesh(array2[1].Trim());
					component.torpedoPropMeshes[num].GetComponent<MeshRenderer>().sharedMaterial = material;
					component.torpedoPropMeshes[num].transform.localPosition = localPosition;
					component.torpedoPropMeshes[num].transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(-90f, 0f, 0f), 1f);
					component.propRotations[num].speed = speed;
					num++;
					break;
				case "MeshMissileBooster":
					component.boosterMesh.GetComponent<MeshFilter>().mesh = this.GetMesh(array2[1].Trim());
					component.boosterMesh.GetComponent<MeshRenderer>().sharedMaterial = material;
					component.boosterMesh.transform.localPosition = localPosition;
					break;
				case "AudioSource":
				{
					string text2 = array2[1].Trim();
					switch (text2)
					{
					case "TorpedoSonarPing":
						audioSource = component.activePingAudioSource;
						break;
					case "TorpedoEngine":
						audioSource = component.cavitationAudioSource;
						break;
					case "MissileLaunch":
						audioSource = component.launchAudioSource;
						break;
					case "MissileEngine":
						audioSource = component.engineAudioSource;
						break;
					}
					break;
				}
				case "AudioClip":
					audioSource.clip = UIFunctions.globaluifunctions.textparser.GetAudioClip(array2[1].Trim());
					break;
				case "AudioRollOff":
					if (array2[1].Trim() != "LOGARITHMIC")
					{
						audioSource.rolloffMode = AudioRolloffMode.Linear;
					}
					else
					{
						audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
					}
					break;
				case "AudioDistance":
				{
					string[] array4 = array2[1].Trim().Split(new char[]
					{
						','
					});
					audioSource.minDistance = float.Parse(array4[0]);
					audioSource.maxDistance = float.Parse(array4[1]);
					break;
				}
				case "AudioPitch":
					audioSource.pitch = float.Parse(array2[1].Trim());
					break;
				case "AudioLoop":
					if (array2[1].Trim() == "TRUE")
					{
						audioSource.loop = true;
					}
					else
					{
						audioSource.loop = false;
					}
					break;
				case "CavitationParticle":
					component.cavitationTransform.localPosition = localPosition;
					break;
				case "MissileTrailParticle":
					component.missileTrailTransform.localPosition = localPosition;
					break;
				case "BoosterParticle":
					component.boosterParticleTransform.localPosition = localPosition;
					break;
				case "ParachuteParticle":
					component.parachuteTransform.localPosition = localPosition;
					break;
				}
			}
		}
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x0009DB60 File Offset: 0x0009BD60
	public GameObject CreateAircraft(int aircraftID, Vector3 spawnPosition, Quaternion spawnRotation, bool isHelicopter)
	{
		string aircraftPrefabName = UIFunctions.globaluifunctions.database.databaseaircraftdata[aircraftID].aircraftPrefabName;
		GameObject gameObject;
		if (isHelicopter)
		{
			gameObject = (UnityEngine.Object.Instantiate((GameObject)Resources.Load("template_objects/helicopterTemplate"), spawnPosition, spawnRotation) as GameObject);
		}
		else
		{
			gameObject = (UnityEngine.Object.Instantiate((GameObject)Resources.Load("template_objects/fixed_wing_aircraftTemplate"), spawnPosition, spawnRotation) as GameObject);
		}
		gameObject.name = aircraftPrefabName;
		this.CreateAndPlaceAircraftMeshes(gameObject, aircraftID, isHelicopter, aircraftPrefabName);
		return gameObject;
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0009DBE0 File Offset: 0x0009BDE0
	private void CreateAndPlaceAircraftMeshes(GameObject aircraftTemplate, int aircraftID, bool isHelicopter, string aircraftPrefabRef)
	{
		Aircraft aircraft = null;
		Helicopter helicopter = null;
		if (isHelicopter)
		{
			helicopter = aircraftTemplate.GetComponent<Helicopter>();
			helicopter.databaseaircraftdata = UIFunctions.globaluifunctions.database.databaseaircraftdata[aircraftID];
			if (helicopter.databaseaircraftdata.passiveSonarID >= 0 || helicopter.databaseaircraftdata.passiveSonarID >= 0)
			{
				helicopter.sonarLine.gameObject.SetActive(true);
			}
		}
		else
		{
			aircraft = aircraftTemplate.GetComponent<Aircraft>();
			aircraft.databaseaircraftdata = UIFunctions.globaluifunctions.database.databaseaircraftdata[aircraftID];
		}
		Transform vesselMesholder;
		if (isHelicopter)
		{
			vesselMesholder = aircraftTemplate.transform;
		}
		else
		{
			vesselMesholder = aircraft.meshHolder;
		}
		List<GameObject> list = new List<GameObject>();
		Vector3 meshPosition = Vector3.zero;
		Vector3 meshRotation = Vector3.zero;
		Material material = null;
		this.currentMesh = null;
		float speed = 0f;
		bool flag = false;
		bool flag2 = false;
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile("aircraft");
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			if (array2[0].Trim() == "AircraftObjectReference")
			{
				if (array2[1].Trim() == aircraftPrefabRef)
				{
					flag = true;
				}
			}
			else if (array2[0].Trim() == "[Model]")
			{
				flag2 = true;
			}
			else if (array2[0].Trim() == "[/Model]" && flag)
			{
				return;
			}
			if (flag2 && flag)
			{
				string text = array2[0];
				switch (text)
				{
				case "ModelFile":
					this.allMeshes = Resources.LoadAll<Mesh>(array2[1].Trim());
					break;
				case "MeshPosition":
					meshPosition = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
					break;
				case "MeshRotation":
					meshRotation = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
					break;
				case "Material":
					material = (Resources.Load(array2[1].Trim()) as Material);
					break;
				case "MaterialTextures":
					if (material != null)
					{
						string[] array3 = array2[1].Trim().Split(new char[]
						{
							','
						});
						material.SetTexture("_MainTex", UIFunctions.globaluifunctions.textparser.GetTexture(array3[0]));
						if (array3.Length > 1)
						{
							material.SetTexture("_SpecTex", UIFunctions.globaluifunctions.textparser.GetTexture(array3[1]));
						}
						if (array3.Length > 2)
						{
							material.SetTexture("_BumpMap", UIFunctions.globaluifunctions.textparser.GetTexture(array3[2]));
						}
					}
					break;
				case "MeshAircraftBody":
				{
					GameObject gameObject = this.SetupMesh(vesselMesholder, meshPosition, meshRotation, material, array2[1].Trim());
					if (isHelicopter)
					{
						helicopter.helibody = gameObject.transform;
						helicopter.sonarLine.transform.SetParent(gameObject.transform);
						helicopter.raycastPosition.SetParent(gameObject.transform);
						helicopter.raycastPosition.localPosition = Vector3.zero;
						helicopter.raycastPosition.localRotation = Quaternion.Euler(-5f, 0f, 0f);
					}
					break;
				}
				case "DippingSonarPosition":
					if (isHelicopter)
					{
						helicopter.sonarLine.transform.localRotation = Quaternion.identity;
						helicopter.sonarLine.transform.localPosition = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
					}
					break;
				case "HoverParticle":
					if (isHelicopter)
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load(array2[1].Trim()), helicopter.transform.position, helicopter.transform.rotation) as GameObject;
						gameObject2.transform.SetParent(helicopter.transform);
						gameObject2.transform.localPosition = Vector3.zero;
						gameObject2.transform.localRotation = Quaternion.identity;
						helicopter.hoverparticle = gameObject2.GetComponent<ParticleSystem>();
						helicopter.hoverparticle.Stop();
					}
					break;
				case "MeshSpeed":
					speed = float.Parse(array2[1].Trim());
					break;
				case "MeshAircraftProp":
				{
					GameObject gameObject = this.SetupMesh(vesselMesholder, meshPosition, meshRotation, material, array2[1].Trim());
					Radar radar = gameObject.AddComponent<Radar>();
					radar.speed = speed;
					if (isHelicopter)
					{
						gameObject.transform.SetParent(helicopter.helibody);
					}
					break;
				}
				case "AudioClip":
					if (helicopter != null)
					{
						helicopter.audiosource.clip = UIFunctions.globaluifunctions.textparser.GetAudioClip(array2[1].Trim());
					}
					else
					{
						aircraft.audiosource.clip = UIFunctions.globaluifunctions.textparser.GetAudioClip(array2[1].Trim());
					}
					break;
				case "AudioRollOff":
				{
					AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
					if (array2[1].Trim() != "LOGARITHMIC")
					{
						rolloffMode = AudioRolloffMode.Linear;
					}
					if (helicopter != null)
					{
						helicopter.audiosource.rolloffMode = rolloffMode;
					}
					else
					{
						aircraft.audiosource.rolloffMode = rolloffMode;
					}
					break;
				}
				case "AudioDistance":
				{
					string[] array4 = array2[1].Trim().Split(new char[]
					{
						','
					});
					if (helicopter != null)
					{
						helicopter.audiosource.minDistance = float.Parse(array4[0]);
						helicopter.audiosource.maxDistance = float.Parse(array4[1]);
					}
					else
					{
						aircraft.audiosource.minDistance = float.Parse(array4[0]);
						aircraft.audiosource.maxDistance = float.Parse(array4[1]);
					}
					break;
				}
				case "AudioPitch":
					if (helicopter != null)
					{
						helicopter.audiosource.pitch = float.Parse(array2[1].Trim());
					}
					else
					{
						aircraft.audiosource.pitch = float.Parse(array2[1].Trim());
					}
					break;
				case "MeshHardpoint":
				{
					GameObject gameObject = this.SetupMesh(vesselMesholder, meshPosition, meshRotation, material, array2[1].Trim());
					list.Add(gameObject);
					if (isHelicopter)
					{
						helicopter.torpedoHardpoints = list.ToArray();
						gameObject.transform.SetParent(helicopter.helibody);
					}
					break;
				}
				}
			}
		}
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x0009E348 File Offset: 0x0009C548
	public GameObject CreateVessel(int vesselID, bool playerControlled, Vector3 spawnPosition, Quaternion spawnRotation)
	{
		string shipPrefabName = UIFunctions.globaluifunctions.database.databaseshipdata[vesselID].shipPrefabName;
		GameObject gameObject = UnityEngine.Object.Instantiate((GameObject)Resources.Load("template_objects/vesselTemplate"), spawnPosition, spawnRotation) as GameObject;
		gameObject.name = shipPrefabName;
		Vessel component = gameObject.GetComponent<Vessel>();
		component.databaseshipdata = UIFunctions.globaluifunctions.database.databaseshipdata[vesselID];
		component.acoustics.playerHasDetectedWith = new bool[4];
		this.CreateAndPlaceMeshes(gameObject, component, playerControlled, shipPrefabName);
		float num = 1.0936f;
		float num2 = UIFunctions.globaluifunctions.database.databaseshipdata[vesselID].length * num / 75.13f / 5f;
		float num3 = UIFunctions.globaluifunctions.database.databaseshipdata[vesselID].beam * num / 75.13f / 4f;
		component.vesselmovement.sternFoam.startSize = num3 * 20f;
		component.vesselmovement.hullFoam.startSize = num3 * 17f;
		component.bouyancyCompartments = new Compartment[10];
		GameObject gameObject2 = UnityEngine.Object.Instantiate((GameObject)Resources.Load("template_objects/blank"), spawnPosition, Quaternion.identity) as GameObject;
		gameObject2.transform.SetParent(gameObject.transform, true);
		gameObject2.transform.localRotation = Quaternion.identity;
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.name = "Compartments";
		component.compartmentvolume = UIFunctions.globaluifunctions.database.databaseshipdata[vesselID].length * UIFunctions.globaluifunctions.database.databaseshipdata[vesselID].beam / 15f + 300f;
		float num4 = num2 * 2f;
		float num5 = -1f;
		for (int i = 0; i < component.bouyancyCompartments.Length; i++)
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate((GameObject)Resources.Load("template_objects/vesselTemplateCompartment"), spawnPosition, Quaternion.identity) as GameObject;
			gameObject3.transform.SetParent(gameObject2.transform, true);
			gameObject3.transform.localRotation = Quaternion.identity;
			gameObject3.transform.localPosition = new Vector3(num3 * num5, 0f, num4);
			gameObject3.transform.localScale = new Vector3(num3, component.databaseshipdata.hullHeight, num2 / 2f);
			num5 *= -1f;
			if (i == 1 || i == 3 || i == 5 || i == 7)
			{
				num4 -= num2;
			}
			Compartment component2 = gameObject3.GetComponent<Compartment>();
			gameObject3.name = this.GetCompartmentName(i, component2);
			component2.activeVessel = component;
			component.bouyancyCompartments[i] = component2;
		}
		gameObject2 = (UnityEngine.Object.Instantiate((GameObject)Resources.Load("template_objects/blank"), spawnPosition, Quaternion.identity) as GameObject);
		gameObject2.transform.SetParent(gameObject.transform, true);
		gameObject2.transform.localRotation = Quaternion.identity;
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.name = "Components";
		if (component.databaseshipdata.shipType != "SUBMARINE")
		{
			GameObject gameObject4 = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.navRadius, base.transform.position, Quaternion.identity) as GameObject;
			gameObject4.transform.SetParent(gameObject2.transform, true);
			gameObject4.transform.localPosition = Vector3.zero;
			VesselAINav component3 = gameObject4.GetComponent<VesselAINav>();
			component3.vesselai = component.vesselai;
			gameObject4.GetComponent<SphereCollider>().radius = component.databaseshipdata.length / 30f;
		}
		return gameObject;
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x0009E710 File Offset: 0x0009C910
	private Mesh GetMesh(string meshName)
	{
		for (int i = 0; i < this.allMeshes.Length; i++)
		{
			if (this.allMeshes[i].name == meshName)
			{
				return this.allMeshes[i];
			}
		}
		Debug.Log("MESH not found " + meshName);
		return null;
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x0009E768 File Offset: 0x0009C968
	private GameObject SetupMesh(Transform vesselMesholder, Vector3 meshPosition, Vector3 meshRotation, Material meshMaterial, string meshName)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate((GameObject)Resources.Load("template_objects/meshTemplate"), vesselMesholder.position, Quaternion.identity) as GameObject;
		gameObject.transform.SetParent(vesselMesholder, false);
		gameObject.transform.localPosition = meshPosition;
		gameObject.transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(meshRotation), 1f);
		gameObject.GetComponent<MeshRenderer>().sharedMaterial = meshMaterial;
		this.currentMesh = this.GetMesh(meshName);
		gameObject.GetComponent<MeshFilter>().mesh = this.currentMesh;
		gameObject.name = meshName;
		return gameObject;
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x0009E808 File Offset: 0x0009CA08
	private void CreateAndPlaceMeshes(GameObject vesselTemplate, Vessel activeVessel, bool playerControlled, string vesselPrefabRef)
	{
		Transform meshHolder = activeVessel.meshHolder;
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		Material material = null;
		this.currentMesh = null;
		GameObject gameObject = null;
		if (!playerControlled)
		{
			UnityEngine.Object.Destroy(activeVessel.submarineFunctions.gameObject);
			activeVessel.submarineFunctions = null;
			activeVessel.vesselmovement.hasWeaponSource = false;
			activeVessel.vesselmovement.weaponSource = null;
			if (activeVessel.databaseshipdata.numberofnoisemakers > 0)
			{
				activeVessel.vesselai.hasNoiseMaker = true;
			}
			else
			{
				UnityEngine.Object.Destroy(activeVessel.vesselai.enemynoisemaker.gameObject);
				activeVessel.vesselai.enemynoisemaker = null;
			}
			if (activeVessel.databaseshipdata.missileGameObject != null)
			{
				activeVessel.vesselai.hasMissile = true;
				activeVessel.vesselai.enemymissile.missileLaunchers = new Transform[activeVessel.databaseshipdata.missilesPerLauncher.Length];
				activeVessel.vesselai.enemymissile.missileLaunchParticlePositions = new Vector3[activeVessel.databaseshipdata.missilesPerLauncher.Length];
				activeVessel.vesselai.enemymissile.missileLaunchParticles = new ParticleSystem[activeVessel.databaseshipdata.missilesPerLauncher.Length];
			}
			else
			{
				UnityEngine.Object.Destroy(activeVessel.vesselai.enemymissile.gameObject);
				activeVessel.vesselai.enemymissile = null;
			}
			if (activeVessel.databaseshipdata.torpedotypes != null)
			{
				activeVessel.vesselai.hasTorpedo = true;
				int num = activeVessel.databaseshipdata.torpedoConfig.Length;
				activeVessel.vesselai.enemytorpedo.torpedoMounts = new Transform[num];
				activeVessel.vesselai.enemytorpedo.torpedoSpawnPositions = new Transform[num];
				activeVessel.vesselai.enemytorpedo.torpedoParticlePositions = new Vector3[num];
				activeVessel.vesselai.enemytorpedo.torpedoClouds = new ParticleSystem[num];
				activeVessel.vesselai.enemytorpedo.numberOfTorpedoes = new int[num];
				activeVessel.vesselai.enemytorpedo.torpedoMountAngles = new float[num];
				activeVessel.vesselai.enemytorpedo.torpedoStatus = new int[num];
				if (activeVessel.databaseshipdata.shipType == "SUBMARINE")
				{
					activeVessel.vesselai.enemytorpedo.fixedTubes = true;
					activeVessel.vesselai.enemytorpedo.submergedTubes = true;
					activeVessel.isSubmarine = true;
					activeVessel.vesselmovement.isSubmarine = true;
					activeVessel.vesselmovement.planes = new Transform[2];
				}
			}
			else
			{
				UnityEngine.Object.Destroy(activeVessel.vesselai.enemytorpedo.gameObject);
				activeVessel.vesselai.enemytorpedo = null;
			}
			if (activeVessel.databaseshipdata.gunProbability > 0f)
			{
				activeVessel.vesselai.hasMissileDefense = true;
				int num2 = activeVessel.databaseshipdata.gunFiringArcStart.Length;
				activeVessel.vesselai.enemymissiledefense.turrets = new GameObject[num2];
				if (activeVessel.databaseshipdata.gunRadarRestAngles != null)
				{
					activeVessel.vesselai.enemymissiledefense.trackingRadars = new GameObject[activeVessel.databaseshipdata.gunRadarRestAngles.Length];
				}
				activeVessel.vesselai.enemymissiledefense.barrels = new Transform[num2];
				activeVessel.vesselai.enemymissiledefense.directionFinders = new Transform[num2];
			}
			else
			{
				UnityEngine.Object.Destroy(activeVessel.vesselai.enemymissiledefense.gameObject);
				activeVessel.vesselai.enemymissiledefense = null;
			}
			if (activeVessel.databaseshipdata.navalGunTypes != null)
			{
				activeVessel.vesselai.hasNavalGuns = true;
				int num3 = activeVessel.databaseshipdata.navalGunTypes.Length;
				activeVessel.vesselai.enemynavalguns.turrets = new Transform[num3];
				activeVessel.vesselai.enemynavalguns.barrels = new Transform[num3];
				activeVessel.vesselai.enemynavalguns.muzzlePositions = new Transform[num3];
			}
			else
			{
				UnityEngine.Object.Destroy(activeVessel.vesselai.enemynavalguns.gameObject);
				activeVessel.vesselai.enemynavalguns = null;
			}
			if (activeVessel.databaseshipdata.rbuLauncherTypes != null)
			{
				activeVessel.vesselai.hasRBU = true;
				int num4 = activeVessel.databaseshipdata.rbuLauncherTypes.Length;
				activeVessel.vesselai.enemyrbu.rbuLaunchers = new Transform[num4];
				activeVessel.vesselai.enemyrbu.rbuPositions = new Transform[num4];
				activeVessel.vesselai.enemyrbu.rbuHubs = new Transform[num4];
				activeVessel.vesselai.enemyrbu.rbuLaunchPositions = new Transform[num4];
				activeVessel.vesselai.enemyrbu.rbuLaunchEffects = new ParticleSystem[num4];
				activeVessel.vesselai.enemyrbu.salvosFired = new int[num4];
			}
			else
			{
				UnityEngine.Object.Destroy(activeVessel.vesselai.enemyrbu.gameObject);
				activeVessel.vesselai.enemyrbu = null;
			}
		}
		else
		{
			activeVessel.vesselai = null;
			UnityEngine.Object.Destroy(activeVessel.weaponsHolder.gameObject);
			activeVessel.vesselmovement.weaponSource.torpedoTubes = new Transform[activeVessel.databaseshipdata.torpedoConfig.Length];
			activeVessel.vesselmovement.weaponSource.tubeParticleEffects = new Vector3[activeVessel.databaseshipdata.torpedoConfig.Length];
			activeVessel.playercontrolled = true;
			activeVessel.isSubmarine = true;
			activeVessel.vesselmovement.isSubmarine = true;
			activeVessel.vesselmovement.planes = new Transform[2];
			activeVessel.vesselmovement.telegraphValue = 2;
		}
		int num5 = 0;
		activeVessel.vesselmovement.props = new Transform[0];
		if (activeVessel.databaseshipdata.proprotationspeed.Length > 0)
		{
			activeVessel.vesselmovement.props = new Transform[activeVessel.databaseshipdata.proprotationspeed.Length];
		}
		List<Transform> list = new List<Transform>();
		int num6 = 0;
		float speed = 0f;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		int num11 = 0;
		int num12 = 0;
		List<MeshCollider> list2 = new List<MeshCollider>();
		List<Radar> list3 = new List<Radar>();
		List<Mesh> list4 = new List<Mesh>();
		List<MeshFilter> list5 = new List<MeshFilter>();
		List<GameObject> list6 = new List<GameObject>();
		activeVessel.vesselmovement.submarineFoamDurations = new float[2];
		string str = string.Empty;
		bool flag = false;
		string filename = Path.Combine("vessels", vesselPrefabRef);
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filename);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			if (array2[0].Trim() == "[Model]")
			{
				flag = true;
			}
			if (flag)
			{
				string text = array2[0];
				switch (text)
				{
				case "ModelFile":
				{
					this.allMeshes = Resources.LoadAll<Mesh>(array2[1].Trim());
					string[] array3 = array2[1].Trim().Split(new char[]
					{
						'/'
					});
					str = array3[array3.Length - 1];
					break;
				}
				case "MeshPosition":
					vector = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
					break;
				case "MeshRotation":
					vector2 = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
					break;
				case "Material":
					material = (Resources.Load(array2[1].Trim()) as Material);
					break;
				case "MaterialTextures":
					if (material != null)
					{
						string[] array4 = array2[1].Trim().Split(new char[]
						{
							','
						});
						material.SetTexture("_MainTex", UIFunctions.globaluifunctions.textparser.GetTexture(array4[0]));
						if (array4.Length > 1)
						{
							material.SetTexture("_SpecTex", UIFunctions.globaluifunctions.textparser.GetTexture(array4[1]));
						}
						if (array4.Length > 2)
						{
							material.SetTexture("_BumpMap", UIFunctions.globaluifunctions.textparser.GetTexture(array4[2]));
						}
					}
					break;
				case "Mesh":
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array5 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array5[0].Trim());
						if (array5[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array5[1].Trim()));
						}
					}
					if (gameObject.name.Contains("biologic"))
					{
						gameObject.GetComponent<MeshRenderer>().receiveShadows = false;
						gameObject.transform.parent.parent.gameObject.AddComponent<Whale_AI>().parentVessel = activeVessel;
					}
					break;
				case "MeshLights":
					if (GameDataManager.isNight)
					{
						if (!array2[1].Trim().Contains(","))
						{
							gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
						}
						else
						{
							string[] array6 = array2[1].Trim().Split(new char[]
							{
								','
							});
							gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array6[0].Trim());
							if (array6[1] == "HIDE")
							{
								list6.Add(gameObject);
							}
							else
							{
								list5.Add(gameObject.GetComponent<MeshFilter>());
								list4.Add(this.GetMesh(array6[1].Trim()));
							}
						}
					}
					break;
				case "MeshHullCollider":
					activeVessel.hullCollider.sharedMesh = this.GetMesh(array2[1].Trim());
					break;
				case "MeshSuperstructureCollider":
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.blankTransform, meshHolder.position, Quaternion.identity) as GameObject;
					gameObject2.transform.SetParent(meshHolder);
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localRotation = Quaternion.identity;
					MeshCollider meshCollider = gameObject2.AddComponent<MeshCollider>();
					meshCollider.convex = true;
					meshCollider.isTrigger = true;
					meshCollider.sharedMesh = this.GetMesh(array2[1].Trim());
					list2.Add(meshCollider);
					break;
				}
				case "MeshHullNumber":
				{
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array7 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array7[0].Trim());
						if (array7[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array7[1].Trim()));
						}
					}
					activeVessel.vesselmovement.hullNumberRenderer = gameObject.GetComponent<MeshRenderer>();
					int num14 = UnityEngine.Random.Range(0, activeVessel.databaseshipdata.hullnumbers.Length);
					string texturePath = "ships/materials/hullnumbers/" + activeVessel.databaseshipdata.hullnumbers[num14];
					Material material2 = activeVessel.vesselmovement.hullNumberRenderer.material;
					material2.SetTexture("_MainTex", UIFunctions.globaluifunctions.textparser.GetTexture(texturePath));
					break;
				}
				case "MeshRudder":
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array8 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array8[0].Trim());
						if (array8[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array8[1].Trim()));
						}
					}
					list.Add(gameObject.transform);
					break;
				case "MeshProp":
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array9 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array9[0].Trim());
						if (array9[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array9[1].Trim()));
						}
					}
					activeVessel.vesselmovement.props[num5] = gameObject.transform;
					num5++;
					break;
				case "MeshBowPlanes":
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array10 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array10[0].Trim());
						if (array10[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array10[1].Trim()));
						}
					}
					activeVessel.vesselmovement.planes[0] = gameObject.transform;
					break;
				case "MeshSternPlanes":
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array11 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array11[0].Trim());
						if (array11[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array11[1].Trim()));
						}
					}
					activeVessel.vesselmovement.planes[1] = gameObject.transform;
					break;
				case "MastHeight":
					if (playerControlled)
					{
						float y = float.Parse(array2[1].Trim());
						activeVessel.submarineFunctions.peiscopeStops[num6].y = y;
						activeVessel.submarineFunctions.mastHeads[num6].transform.localPosition = new Vector3(0f, y, 0f);
					}
					break;
				case "MeshMast":
					if (playerControlled)
					{
						if (!array2[1].Trim().Contains(","))
						{
							activeVessel.submarineFunctions.mastTransforms[num6].GetComponent<MeshFilter>().mesh = this.GetMesh(array2[1].Trim());
						}
						else
						{
							string[] array12 = array2[1].Trim().Split(new char[]
							{
								','
							});
							activeVessel.submarineFunctions.mastTransforms[num6].GetComponent<MeshFilter>().mesh = this.GetMesh(array12[0].Trim());
							if (array12[1] == "HIDE")
							{
								list6.Add(activeVessel.submarineFunctions.mastTransforms[num6].gameObject);
							}
							else
							{
								list5.Add(activeVessel.submarineFunctions.mastTransforms[num6].GetComponent<MeshFilter>());
								list4.Add(this.GetMesh(array12[1].Trim()));
							}
						}
						activeVessel.submarineFunctions.mastTransforms[num6].GetComponent<MeshRenderer>().sharedMaterial = material;
						activeVessel.submarineFunctions.mastTransforms[num6].localPosition = vector;
						activeVessel.submarineFunctions.mastTransforms[num6].localRotation = Quaternion.identity;
						gameObject = activeVessel.submarineFunctions.mastTransforms[num6].gameObject;
						activeVessel.submarineFunctions.mastTransforms[num6] = gameObject.transform;
						activeVessel.submarineFunctions.peiscopeStops[num6].x = gameObject.transform.localPosition.y;
						Vector2[] peiscopeStops = activeVessel.submarineFunctions.peiscopeStops;
						int num15 = num6;
						peiscopeStops[num15].y = peiscopeStops[num15].y + activeVessel.submarineFunctions.peiscopeStops[num6].x;
						num6++;
					}
					break;
				case "ChildMesh":
				{
					Transform transform = gameObject.transform;
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array13 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array13[0].Trim());
						if (array13[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array13[1].Trim()));
						}
					}
					gameObject.transform.SetParent(transform, false);
					gameObject.transform.localPosition = vector;
					gameObject.transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(vector2), 1f);
					break;
				}
				case "MeshMainFlag":
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array14 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array14[0].Trim());
						if (array14[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array14[1].Trim()));
						}
					}
					material.color = global::Environment.whiteLevel;
					gameObject.layer = 17;
					activeVessel.vesselmovement.flagRenderer = gameObject.GetComponent<MeshRenderer>();
					break;
				case "MeshOtherFlags":
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array15 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array15[0].Trim());
						if (array15[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array15[1].Trim()));
						}
					}
					material.color = global::Environment.whiteLevel;
					gameObject.layer = 17;
					break;
				case "RADARSpeed":
					speed = float.Parse(array2[1].Trim());
					break;
				case "RADARDirection":
				{
					float num16 = float.Parse(array2[1].Trim());
					break;
				}
				case "MeshRADAR":
				{
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array16 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array16[0].Trim());
						if (array16[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array16[1].Trim()));
						}
					}
					Radar radar = gameObject.AddComponent<Radar>();
					radar.speed = speed;
					list3.Add(radar);
					break;
				}
				case "MeshNoisemakerMount":
					if (activeVessel.vesselai != null)
					{
						activeVessel.vesselai.enemynoisemaker.noisemakerTubes.transform.localPosition = vector;
					}
					else
					{
						activeVessel.vesselmovement.weaponSource.noisemakerTubes.transform.localPosition = vector;
					}
					break;
				case "MeshTorpedoMount":
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array17 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array17[0].Trim());
						if (array17[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array17[1].Trim()));
						}
					}
					activeVessel.vesselai.enemytorpedo.torpedoMounts[num7] = gameObject.transform;
					break;
				case "TorpedoSpawnPosition":
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.blankTransform, meshHolder.position, Quaternion.identity) as GameObject;
					if (!activeVessel.playercontrolled)
					{
						if (activeVessel.vesselai.enemytorpedo != null)
						{
							if (!activeVessel.vesselai.enemytorpedo.fixedTubes)
							{
								gameObject3.transform.SetParent(gameObject.transform, false);
								gameObject3.transform.localPosition = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
								gameObject3.transform.localRotation = Quaternion.identity;
							}
							else
							{
								gameObject3.transform.SetParent(meshHolder.transform, false);
								gameObject3.transform.localPosition = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
								gameObject3.transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(vector2), 1f);
							}
						}
					}
					else
					{
						gameObject3.transform.SetParent(meshHolder.transform, false);
						gameObject3.transform.localPosition = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
						gameObject3.transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(vector2), 1f);
					}
					if (activeVessel.vesselai != null)
					{
						activeVessel.vesselai.enemytorpedo.torpedoSpawnPositions[num7] = gameObject3.transform;
						if (activeVessel.isSubmarine)
						{
							activeVessel.vesselai.enemytorpedo.torpedoMounts[num7] = gameObject3.transform;
						}
					}
					else
					{
						activeVessel.vesselmovement.weaponSource.torpedoTubes[num7] = gameObject3.transform;
					}
					break;
				}
				case "TorpedoEffectPosition":
					if (activeVessel.vesselai != null)
					{
						activeVessel.vesselai.enemytorpedo.torpedoParticlePositions[num7] = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
					}
					else
					{
						activeVessel.vesselmovement.weaponSource.tubeParticleEffects[num7] = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
					}
					num7++;
					break;
				case "MeshMissileMount":
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array18 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array18[0].Trim());
						if (array18[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array18[1].Trim()));
						}
					}
					gameObject.name = "missileLauncher";
					activeVessel.vesselai.enemymissile.missileLaunchers[num8] = gameObject.transform;
					break;
				case "MissileEffectPosition":
					activeVessel.vesselai.enemymissile.missileLaunchParticlePositions[num8] = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
					num8++;
					break;
				case "MeshNavalGun":
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array19 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array19[0].Trim());
						if (array19[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array19[1].Trim()));
						}
					}
					activeVessel.vesselai.enemynavalguns.turrets[num12] = gameObject.transform;
					break;
				case "MeshNavalGunBarrel":
				{
					Transform transform2 = gameObject.transform;
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array20 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array20[0].Trim());
						if (array20[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array20[1].Trim()));
						}
					}
					gameObject.transform.SetParent(transform2, false);
					gameObject.transform.localPosition = vector;
					gameObject.transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(vector2), 1f);
					activeVessel.vesselai.enemynavalguns.barrels[num12] = gameObject.transform;
					break;
				}
				case "NavalGunSpawnPosition":
				{
					GameObject gameObject4 = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.blankTransform, meshHolder.position, Quaternion.identity) as GameObject;
					if (activeVessel.vesselai.enemynavalguns != null)
					{
						gameObject4.transform.SetParent(gameObject.transform, false);
						gameObject4.transform.localPosition = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
						gameObject4.transform.localRotation = Quaternion.identity;
					}
					if (activeVessel.vesselai != null)
					{
						activeVessel.vesselai.enemynavalguns.muzzlePositions[num12] = gameObject4.transform;
					}
					num12++;
					break;
				}
				case "MeshCIWSGun":
				{
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array21 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array21[0].Trim());
						if (array21[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array21[1].Trim()));
						}
					}
					activeVessel.vesselai.enemymissiledefense.turrets[num9] = gameObject;
					GameObject gameObject5 = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.blankTransform, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
					gameObject5.transform.SetParent(gameObject.transform, false);
					gameObject5.name = "directionfinder";
					gameObject5.transform.localPosition = Vector3.zero;
					activeVessel.vesselai.enemymissiledefense.directionFinders[num9] = gameObject5.transform;
					GameObject gameObject6 = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.blankTransform, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
					gameObject6.transform.SetParent(gameObject.transform, false);
					gameObject6.transform.localPosition = Vector3.zero;
					gameObject6.transform.localRotation = Quaternion.identity;
					gameObject6.name = "barrel";
					activeVessel.vesselai.enemymissiledefense.barrels[num9] = gameObject6.transform;
					num9++;
					break;
				}
				case "MeshCIWSRADAR":
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array22 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array22[0].Trim());
						if (array22[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array22[1].Trim()));
						}
					}
					activeVessel.vesselai.enemymissiledefense.trackingRadars[num10] = gameObject;
					num10++;
					break;
				case "MeshRBULauncher":
				{
					GameObject gameObject7 = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.blankTransform, meshHolder.position, Quaternion.identity) as GameObject;
					gameObject7.transform.SetParent(activeVessel.meshHolder, false);
					gameObject7.transform.localPosition = vector;
					gameObject7.transform.localRotation = Quaternion.identity;
					gameObject7.name = "rbuMount";
					activeVessel.vesselai.enemyrbu.rbuPositions[num11] = gameObject7.transform;
					if (!array2[1].Trim().Contains(","))
					{
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array2[1].Trim());
					}
					else
					{
						string[] array23 = array2[1].Trim().Split(new char[]
						{
							','
						});
						gameObject = this.SetupMesh(meshHolder, vector, vector2, material, array23[0].Trim());
						if (array23[1] == "HIDE")
						{
							list6.Add(gameObject);
						}
						else
						{
							list5.Add(gameObject.GetComponent<MeshFilter>());
							list4.Add(this.GetMesh(array23[1].Trim()));
						}
					}
					gameObject.transform.SetParent(gameObject7.transform, false);
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(vector2), 1f);
					activeVessel.vesselai.enemyrbu.rbuLaunchers[num11] = gameObject.transform;
					GameObject gameObject8 = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.blankTransform, meshHolder.position, Quaternion.identity) as GameObject;
					gameObject8.transform.SetParent(gameObject.transform, false);
					gameObject8.transform.localPosition = Vector3.zero;
					gameObject8.transform.localRotation = Quaternion.identity;
					gameObject8.name = "muzzlehub";
					activeVessel.vesselai.enemyrbu.rbuHubs[num11] = gameObject8.transform;
					GameObject gameObject9 = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.blankTransform, meshHolder.position, Quaternion.identity) as GameObject;
					gameObject9.transform.SetParent(gameObject8.transform, false);
					gameObject9.transform.localPosition = new Vector3(UIFunctions.globaluifunctions.database.databasedepthchargedata[activeVessel.databaseshipdata.rbuLauncherTypes[num11]].firingPositions.x, 0f, UIFunctions.globaluifunctions.database.databasedepthchargedata[activeVessel.databaseshipdata.rbuLauncherTypes[num11]].firingPositions.y);
					gameObject9.transform.localRotation = Quaternion.identity;
					activeVessel.vesselai.enemyrbu.rbuLaunchPositions[num11] = gameObject9.transform;
					GameObject gameObject10 = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.rbuLaunchFlare, gameObject8.transform.position, gameObject8.transform.rotation) as GameObject;
					gameObject10.transform.SetParent(gameObject8.transform);
					gameObject10.transform.localPosition = Vector3.zero;
					gameObject10.transform.localRotation = Quaternion.Slerp(gameObject10.transform.localRotation, Quaternion.Euler(0f, 180f, 0f), 1f);
					activeVessel.vesselai.enemyrbu.rbuLaunchEffects[num11] = gameObject10.GetComponent<ParticleSystem>();
					num11++;
					break;
				}
				case "MeshRBUMount":
					gameObject = this.SetupMesh(activeVessel.vesselai.enemyrbu.rbuPositions[num11 - 1], vector, vector2, material, array2[1].Trim());
					break;
				case "BowWaveParticle":
				{
					GameObject gameObject11 = UnityEngine.Object.Instantiate((GameObject)Resources.Load(array2[1].Trim()), vector, Quaternion.identity) as GameObject;
					gameObject11.transform.SetParent(activeVessel.vesselmovement.bowwaveHolder.transform);
					gameObject11.transform.localPosition = Vector3.zero;
					gameObject11.transform.localRotation = Quaternion.identity;
					activeVessel.vesselmovement.bowwave = gameObject11.GetComponent<ParticleSystem>();
					gameObject11.layer = 28;
					break;
				}
				case "PropWashParticle":
				{
					GameObject gameObject12 = UnityEngine.Object.Instantiate((GameObject)Resources.Load(array2[1].Trim()), vector, Quaternion.identity) as GameObject;
					gameObject12.transform.SetParent(activeVessel.vesselmovement.wakeObject.transform);
					gameObject12.transform.localPosition = vector;
					gameObject12.transform.localRotation = Quaternion.identity;
					activeVessel.vesselmovement.propwash = gameObject12.GetComponent<ParticleSystem>();
					gameObject12.layer = 28;
					break;
				}
				case "FunnelSmokeParticle":
				{
					GameObject gameObject13 = UnityEngine.Object.Instantiate((GameObject)Resources.Load(array2[1].Trim()), vector, Quaternion.identity) as GameObject;
					gameObject13.transform.SetParent(meshHolder.transform);
					gameObject13.transform.localPosition = Vector3.zero;
					gameObject13.transform.localRotation = Quaternion.identity;
					activeVessel.damagesystem.funnelSmoke = gameObject13.GetComponent<ParticleSystem>();
					break;
				}
				case "EmergencyBlowParticle":
				{
					GameObject gameObject14 = UnityEngine.Object.Instantiate((GameObject)Resources.Load(array2[1].Trim()), vector, Quaternion.identity) as GameObject;
					gameObject14.transform.SetParent(meshHolder.transform);
					gameObject14.transform.localPosition = Vector3.zero;
					gameObject14.transform.localRotation = Quaternion.identity;
					activeVessel.damagesystem.emergencyBlow = gameObject14.GetComponent<ParticleSystem>();
					gameObject14.GetComponent<AudioSource>().playOnAwake = false;
					break;
				}
				case "CavitationParticle":
				{
					GameObject gameObject15 = UnityEngine.Object.Instantiate((GameObject)Resources.Load(array2[1].Trim()), vector, Quaternion.identity) as GameObject;
					gameObject15.transform.SetParent(meshHolder.transform);
					gameObject15.transform.localPosition = Vector3.zero;
					gameObject15.transform.localRotation = Quaternion.identity;
					activeVessel.vesselmovement.cavBubbles = gameObject15.GetComponent<ParticleSystem>();
					break;
				}
				case "KelvinWaves":
				{
					Vector3 vector3 = UIFunctions.globaluifunctions.textparser.PopulateVector2(array2[1].Trim());
					activeVessel.vesselmovement.kelvinWaveOverlay.width = vector3.x;
					activeVessel.vesselmovement.kelvinWaveOverlay.height = vector3.y;
					break;
				}
				case "ParticleBowWavePosition":
					if (array2[1].Trim() != "FALSE")
					{
						activeVessel.vesselmovement.bowwave.transform.parent.transform.localPosition = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
					}
					break;
				case "ParticlePropWashPosition":
					if (array2[1].Trim() != "FALSE")
					{
						activeVessel.vesselmovement.propwash.transform.localPosition = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
					}
					break;
				case "ParticleHullFoamPosition":
					if (array2[1].Trim() != "FALSE")
					{
						activeVessel.vesselmovement.foamTrails[0].transform.localPosition = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
					}
					else
					{
						UnityEngine.Object.Destroy(activeVessel.vesselmovement.foamTrails[0].gameObject);
					}
					break;
				case "ParticleHullFoamParameters":
				{
					string[] array24 = array2[1].Trim().Split(new char[]
					{
						','
					});
					activeVessel.vesselmovement.foamTrails[0].duration = float.Parse(array24[0]);
					activeVessel.vesselmovement.foamTrails[0].size = float.Parse(array24[1]);
					activeVessel.vesselmovement.foamTrails[0].spacing = float.Parse(array24[2]);
					activeVessel.vesselmovement.foamTrails[0].expansion = float.Parse(array24[3]);
					activeVessel.vesselmovement.foamTrails[0].momentum = float.Parse(array24[4]);
					activeVessel.vesselmovement.foamTrails[0].spin = float.Parse(array24[5]);
					activeVessel.vesselmovement.foamTrails[0].jitter = float.Parse(array24[6]);
					activeVessel.vesselmovement.submarineFoamDurations[0] = activeVessel.vesselmovement.foamTrails[0].duration;
					break;
				}
				case "ParticleSternFoamPosition":
					activeVessel.vesselmovement.foamTrails[1].transform.localPosition = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1].Trim());
					break;
				case "ParticleSternFoamParameters":
				{
					string[] array25 = array2[1].Trim().Split(new char[]
					{
						','
					});
					activeVessel.vesselmovement.foamTrails[1].duration = float.Parse(array25[0]);
					activeVessel.vesselmovement.foamTrails[1].size = float.Parse(array25[1]);
					activeVessel.vesselmovement.foamTrails[1].spacing = float.Parse(array25[2]);
					activeVessel.vesselmovement.foamTrails[1].expansion = float.Parse(array25[3]);
					activeVessel.vesselmovement.foamTrails[1].momentum = float.Parse(array25[4]);
					activeVessel.vesselmovement.foamTrails[1].spin = float.Parse(array25[5]);
					activeVessel.vesselmovement.foamTrails[1].jitter = float.Parse(array25[6]);
					activeVessel.vesselmovement.submarineFoamDurations[1] = activeVessel.vesselmovement.foamTrails[1].duration;
					break;
				}
				case "EngineAudioClip":
					activeVessel.vesselmovement.engineSound.clip = UIFunctions.globaluifunctions.textparser.GetAudioClip(array2[1].Trim());
					break;
				case "EngineAudioRollOff":
					if (array2[1].Trim() != "LOGARITHMIC")
					{
						activeVessel.vesselmovement.engineSound.rolloffMode = AudioRolloffMode.Linear;
					}
					else
					{
						activeVessel.vesselmovement.engineSound.rolloffMode = AudioRolloffMode.Logarithmic;
					}
					break;
				case "EngineAudioDistance":
				{
					string[] array26 = array2[1].Trim().Split(new char[]
					{
						','
					});
					activeVessel.vesselmovement.engineSound.minDistance = float.Parse(array26[0]);
					activeVessel.vesselmovement.engineSound.maxDistance = float.Parse(array26[1]);
					break;
				}
				case "EngineAudioPitchRange":
					activeVessel.vesselmovement.enginePitchRange = UIFunctions.globaluifunctions.textparser.PopulateVector2(array2[1].Trim());
					break;
				case "PropAudioClip":
					activeVessel.vesselmovement.propSound.clip = UIFunctions.globaluifunctions.textparser.GetAudioClip(array2[1].Trim());
					activeVessel.vesselmovement.propSound.transform.localPosition = new Vector3(0f, 0f, activeVessel.vesselmovement.props[0].transform.localPosition.z);
					break;
				case "PropAudioRollOff":
					if (array2[1].Trim() != "LOGARITHMIC")
					{
						activeVessel.vesselmovement.propSound.rolloffMode = AudioRolloffMode.Linear;
					}
					else
					{
						activeVessel.vesselmovement.propSound.rolloffMode = AudioRolloffMode.Logarithmic;
					}
					break;
				case "PropAudioDistance":
				{
					string[] array27 = array2[1].Trim().Split(new char[]
					{
						','
					});
					activeVessel.vesselmovement.propSound.minDistance = float.Parse(array27[0]);
					activeVessel.vesselmovement.propSound.maxDistance = float.Parse(array27[1]);
					break;
				}
				case "PropAudioPitchRange":
					activeVessel.vesselmovement.propPitchRange = UIFunctions.globaluifunctions.textparser.PopulateVector2(array2[1].Trim());
					break;
				case "PingAudioClip":
					activeVessel.vesselmovement.pingSound.enabled = true;
					activeVessel.vesselmovement.pingSound.clip = UIFunctions.globaluifunctions.textparser.GetAudioClip(array2[1].Trim());
					activeVessel.vesselmovement.pingSound.loop = false;
					activeVessel.vesselmovement.pingSound.transform.localPosition = new Vector3(0f, 0f, activeVessel.vesselmovement.bowwaveHolder.transform.localPosition.z);
					break;
				case "PingAudioRollOff":
					if (array2[1].Trim() != "LOGARITHMIC")
					{
						activeVessel.vesselmovement.pingSound.rolloffMode = AudioRolloffMode.Linear;
					}
					else
					{
						activeVessel.vesselmovement.pingSound.rolloffMode = AudioRolloffMode.Logarithmic;
					}
					break;
				case "PingAudioDistance":
				{
					string[] array28 = array2[1].Trim().Split(new char[]
					{
						','
					});
					activeVessel.vesselmovement.pingSound.minDistance = float.Parse(array28[0]);
					activeVessel.vesselmovement.pingSound.maxDistance = float.Parse(array28[1]);
					break;
				}
				case "PingAudioPitch":
					activeVessel.vesselmovement.pingSound.pitch = float.Parse(array2[1].Trim());
					break;
				case "BowwaveAudioClip":
					activeVessel.vesselmovement.bowwaveSound.enabled = true;
					activeVessel.vesselmovement.bowwaveSound.clip = UIFunctions.globaluifunctions.textparser.GetAudioClip(array2[1].Trim());
					activeVessel.vesselmovement.bowwaveSound.loop = true;
					break;
				case "BowwaveAudioRollOff":
					if (array2[1].Trim() != "LOGARITHMIC")
					{
						activeVessel.vesselmovement.bowwaveSound.rolloffMode = AudioRolloffMode.Linear;
					}
					else
					{
						activeVessel.vesselmovement.bowwaveSound.rolloffMode = AudioRolloffMode.Logarithmic;
					}
					break;
				case "BowwaveAudioDistance":
				{
					string[] array29 = array2[1].Trim().Split(new char[]
					{
						','
					});
					activeVessel.vesselmovement.bowwaveSound.minDistance = float.Parse(array29[0]);
					activeVessel.vesselmovement.bowwaveSound.maxDistance = float.Parse(array29[1]);
					break;
				}
				case "BowwaveAudioPitch":
					activeVessel.vesselmovement.bowwaveSound.pitch = float.Parse(array2[1].Trim());
					break;
				}
			}
		}
		activeVessel.vesselmovement.rudder = list.ToArray();
		if (!activeVessel.playercontrolled && activeVessel.vesselai.hasTorpedo)
		{
			activeVessel.vesselai.enemytorpedo.launcherPositions = new int[activeVessel.vesselai.enemytorpedo.torpedoMounts.Length];
			if (!activeVessel.vesselai.enemytorpedo.fixedTubes)
			{
				for (int j = 0; j < activeVessel.vesselai.enemytorpedo.torpedoMounts.Length; j++)
				{
					if (activeVessel.vesselai.enemytorpedo.torpedoMounts[j].localPosition.x < 0f)
					{
						activeVessel.vesselai.enemytorpedo.launcherPositions[j] = -1;
					}
					else if (activeVessel.vesselai.enemytorpedo.torpedoMounts[j].localPosition.x > 0f)
					{
						activeVessel.vesselai.enemytorpedo.launcherPositions[j] = 1;
					}
					else
					{
						activeVessel.vesselai.enemytorpedo.launcherPositions[j] = 0;
					}
				}
			}
		}
		if (activeVessel.databaseshipdata.shipType != "BIOLOGIC" && activeVessel.databaseshipdata.shipType != "OILRIG")
		{
			activeVessel.damagesystem.hullDamageMeshes = new Mesh[10];
			activeVessel.damagesystem.hullDamageMeshes[0] = this.GetMesh(str + "_damage_11");
			activeVessel.damagesystem.hullDamageMeshes[1] = this.GetMesh(str + "_damage_12");
			activeVessel.damagesystem.hullDamageMeshes[2] = this.GetMesh(str + "_damage_21");
			activeVessel.damagesystem.hullDamageMeshes[3] = this.GetMesh(str + "_damage_22");
			activeVessel.damagesystem.hullDamageMeshes[4] = this.GetMesh(str + "_damage_31");
			activeVessel.damagesystem.hullDamageMeshes[5] = this.GetMesh(str + "_damage_32");
			activeVessel.damagesystem.hullDamageMeshes[6] = this.GetMesh(str + "_damage_41");
			activeVessel.damagesystem.hullDamageMeshes[7] = this.GetMesh(str + "_damage_42");
			activeVessel.damagesystem.hullDamageMeshes[8] = this.GetMesh(str + "_damage_51");
			activeVessel.damagesystem.hullDamageMeshes[9] = this.GetMesh(str + "_damage_52");
		}
		activeVessel.damagesystem.damageMeshFilters = list5.ToArray();
		activeVessel.damagesystem.damageMeshes = list4.ToArray();
		activeVessel.damagesystem.objectMeshesToHide = list6.ToArray();
		activeVessel.damagesystem.radars = list3.ToArray();
		if (list2.Count > 0)
		{
			activeVessel.superstructureColliders = list2.ToArray();
		}
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x000A1B48 File Offset: 0x0009FD48
	private string GetCompartmentName(int i, Compartment currentCompartment)
	{
		string result = string.Empty;
		switch (i)
		{
		case 0:
			result = "11";
			currentCompartment.compartmentMaximumDamage = 0.09f;
			currentCompartment.compartmentPosition = "FRONT";
			break;
		case 1:
			result = "12";
			currentCompartment.compartmentMaximumDamage = 0.09f;
			currentCompartment.compartmentPosition = "FRONT";
			break;
		case 2:
			result = "21";
			currentCompartment.compartmentMaximumDamage = 0.315f;
			currentCompartment.compartmentPosition = "FORE";
			break;
		case 3:
			result = "22";
			currentCompartment.compartmentMaximumDamage = 0.315f;
			currentCompartment.compartmentPosition = "FORE";
			break;
		case 4:
			result = "31";
			currentCompartment.compartmentMaximumDamage = 0.9f;
			currentCompartment.compartmentPosition = "MID";
			break;
		case 5:
			result = "32";
			currentCompartment.compartmentMaximumDamage = 0.9f;
			currentCompartment.compartmentPosition = "MID";
			break;
		case 6:
			result = "41";
			currentCompartment.compartmentMaximumDamage = 0.315f;
			currentCompartment.compartmentPosition = "AFT";
			break;
		case 7:
			result = "42";
			currentCompartment.compartmentMaximumDamage = 0.315f;
			currentCompartment.compartmentPosition = "AFT";
			break;
		case 8:
			result = "51";
			currentCompartment.compartmentMaximumDamage = 0.09f;
			currentCompartment.compartmentPosition = "REAR";
			break;
		case 9:
			result = "52";
			currentCompartment.compartmentMaximumDamage = 0.09f;
			currentCompartment.compartmentPosition = "REAR";
			break;
		}
		currentCompartment.sinkRate = UnityEngine.Random.value * 0.05f / 2f + 0.001f;
		return result;
	}

	// Token: 0x040011C6 RID: 4550
	public GameObject vesselTemplateParent;

	// Token: 0x040011C7 RID: 4551
	public GameObject vesselTemplateCompartment;

	// Token: 0x040011C8 RID: 4552
	public GameObject meshTemplate;

	// Token: 0x040011C9 RID: 4553
	public GameObject meshTemplateNoShadows;

	// Token: 0x040011CA RID: 4554
	public Mesh[] allMeshes;

	// Token: 0x040011CB RID: 4555
	public Mesh currentMesh;
}
