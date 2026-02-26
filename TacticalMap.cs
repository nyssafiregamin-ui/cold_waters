using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

// Token: 0x02000164 RID: 356
public class TacticalMap : MonoBehaviour
{
	// Token: 0x06000A45 RID: 2629 RVA: 0x0007D994 File Offset: 0x0007BB94
	private void Start()
	{
		this.tacticalMap.transform.position = new Vector3(0f, 0f, 1050f);
		this.tacActiveShip = 0;
		this.tacPlayerShip = 0;
		this.orthFactor = 1.7f;
		this.textMinMaxSize = this.orthFactor * this.textSize;
		this.zoomNumber = 4;
		this.zoomText.text = "1 " + LanguageManager.interfaceDictionary["KiloYard"];
		this.scalerValue = 2f;
		this.canvasScaler.dynamicPixelsPerUnit = 2f;
		this.backgroundsize = 47f;
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x0007DA44 File Offset: 0x0007BC44
	public void TacticalMapInit()
	{
		this.autoCentreMap = false;
		this.numberOfShips = GameDataManager.playerNumberofShips + GameDataManager.enemyNumberofShips;
		this.mapObjects = new GameObject[GameDataManager.enemyNumberofShips];
		this.mapContact = new MapContact[GameDataManager.enemyNumberofShips];
		this.mapContactButtons = new Button[GameDataManager.enemyNumberofShips];
		this.allVesselsList = new Vessel[this.numberOfShips];
		for (int i = 0; i < GameDataManager.playerNumberofShips; i++)
		{
			Vessel activeVessel = GameDataManager.playervesselsonlevel[i];
			this.CreateContact(activeVessel, i, this.navyColors[0]);
		}
		this.playerMapContact = this.mapContact[0];
		this.playerMapContact.shipDisplayIcon.sprite = this.sensormanager.sonarPaintImages[5];
		this.playerMapObject = this.mapContact[0].gameObject;
		this.playerMapObject.name = "P";
		for (int j = 0; j < GameDataManager.enemyNumberofShips; j++)
		{
			Vessel activeVessel2 = GameDataManager.enemyvesselsonlevel[j];
			this.CreateContact(activeVessel2, j, this.navyColors[2]);
		}
		this.allVesselsList[0] = GameDataManager.playervesselsonlevel[0];
		for (int k = 0; k < GameDataManager.enemyNumberofShips; k++)
		{
			Vessel vessel = GameDataManager.enemyvesselsonlevel[k];
			this.allVesselsList[k + 1] = vessel;
		}
		this.playerMapContact.gameObject.SetActive(true);
		TacticalMap.tacMapEnabled = false;
		this.zoomText.gameObject.SetActive(false);
		this.tacMapCamera.gameObject.SetActive(false);
		this.tacticalMap.SetActive(false);
		this.updateTimer = 0f;
		this.waypointMarker.gameObject.SetActive(false);
		this.dumbfireMarker.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.campaignmanager.eventManager.sealsReleased = false;
		if (this.levelloadmanager.levelloaddata.missionPosition.x != -10000f)
		{
			this.missionMarker.color = UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.missionMarkerColors[UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.missionMarkerCurrentColor];
			this.missionMarker.gameObject.SetActive(true);
			this.missionMarker.gameObject.transform.localPosition = new Vector3(this.levelloadmanager.levelloaddata.missionPosition.x * this.zoomFactor, this.levelloadmanager.levelloaddata.missionPosition.y * this.zoomFactor, -5f);
			this.uifunctions.playerfunctions.inMissionZoneTimer = 55f;
		}
		else
		{
			this.missionMarker.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x0007DD1C File Offset: 0x0007BF1C
	public void TacticalMapCleanup()
	{
		for (int m = 0; m < this.sensormanager.torpedoObjects.Length; m++)
		{
			this.sensormanager.torpedoObjects[m].destroyMe = true;
		}
		for (int j = 0; j < this.sensormanager.noisemakerObjects.Length; j++)
		{
			if (this.sensormanager.noisemakerObjects[j] != null)
			{
				this.sensormanager.noisemakerObjects[j].DestroyCountermeasure();
			}
		}
		int i;
		for (i = 0; i < this.mapObjects.Length; i++)
		{
			this.mapContactButtons[i].onClick.RemoveListener(delegate()
			{
				this.uifunctions.playerfunctions.MapContactButton(i, false);
			});
			this.ClearMapContactTrail(this.mapContact[i], false);
			ObjectPoolManager.DestroyPooled(this.mapObjects[i]);
		}
		this.mapObjects = new GameObject[0];
		this.mapContact = new MapContact[0];
		this.allVesselsList = new Vessel[0];
		if (this.playerMapObject != null)
		{
			this.ClearMapContactTrail(this.playerMapContact, false);
			ObjectPoolManager.DestroyPooled(this.playerMapObject);
			this.playerMapObject = null;
			this.playerMapContact = null;
		}
		int childCount = this.hazardIcons.childCount;
		for (int k = 0; k < childCount; k++)
		{
			UnityEngine.Object.Destroy(this.hazardIcons.GetChild(k).gameObject);
		}
		childCount = this.noisemakerLayer.transform.childCount;
		for (int l = 0; l < childCount; l++)
		{
			UnityEngine.Object.Destroy(this.noisemakerLayer.transform.GetChild(l).gameObject);
		}
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x0007DF04 File Offset: 0x0007C104
	private void ScaleHazards()
	{
		float num = 8f * this.orthFactor;
		int childCount = this.hazardIcons.childCount;
		for (int i = 0; i < childCount; i++)
		{
			this.hazardIcons.GetChild(i).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(num, num);
		}
	}

	// Token: 0x06000A49 RID: 2633 RVA: 0x0007DF60 File Offset: 0x0007C160
	public void PlaceHazardIconOnMap(Vector3 hazardPosition, int iconType)
	{
		Vector3 b = new Vector3(hazardPosition.x * this.zoomFactor, hazardPosition.z * this.zoomFactor, -5f);
		int childCount = this.hazardIcons.childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (Vector3.Distance(this.hazardIcons.GetChild(i).gameObject.transform.localPosition, b) < 10f)
			{
				return;
			}
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.blankTransform, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.SetParent(this.hazardIcons);
		gameObject.transform.localPosition = new Vector3(hazardPosition.x * this.zoomFactor, hazardPosition.z * this.zoomFactor, -5f);
		gameObject.AddComponent<CanvasRenderer>();
		Image image = gameObject.AddComponent<Image>();
		image.sprite = this.hazardSprites[iconType];
		image.color = this.hazardColors[iconType];
		this.ScaleHazards();
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x0007E084 File Offset: 0x0007C284
	private void FixedUpdate()
	{
		if (!GameDataManager.HUDActive)
		{
			return;
		}
		this.updateTimer += Time.deltaTime;
		this.positionTimer += Time.deltaTime;
		this.positionTorpedoTimer += Time.deltaTime;
		if (this.updateTimer > 0.1f)
		{
			this.TacticalMapRefresh();
			this.updateTimer -= 0.1f;
			if (this.autoCentreMap)
			{
				this.CentreMap();
			}
		}
		if (this.positionTimer > 5f)
		{
			this.RecordAllPositions(this.playerMapContact);
			for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
			{
				if (this.sensormanager.solutionQualityOfContacts[i] > this.qualityToDrawTails && this.sensormanager.detectedByPlayer[i])
				{
					this.RecordAllPositions(this.mapContact[i]);
				}
			}
			this.positionTimer -= 5f;
		}
		if (this.positionTorpedoTimer > 1f)
		{
			this.RecordTorpedoPositions();
			this.positionTorpedoTimer -= 1f;
		}
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x0007E1B4 File Offset: 0x0007C3B4
	public void ClearMapContactTrail(MapContact mapcontactToClear, bool resetTrail)
	{
		while (mapcontactToClear.positionMarkerQueue.Count > 0)
		{
			Transform transform = mapcontactToClear.positionMarkerQueue.Dequeue();
			ObjectPoolManager.DestroyPooled(transform.gameObject);
		}
		if (resetTrail)
		{
			mapcontactToClear.contactNumberOfMoves = 0;
			mapcontactToClear.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x0007E208 File Offset: 0x0007C408
	public void DrawPingLine(Transform pingTransform, Color lineColor)
	{
		if (TacticalMap.tacMapEnabled)
		{
			float num = Vector3.Distance(GameDataManager.playervesselsonlevel[0].transform.position, pingTransform.position) * GameDataManager.yardsScale;
			num = Mathf.Clamp(num, 0f, 20000f);
			if (num >= 5000f)
			{
				if (num >= 10000f)
				{
					if (num < 15000f)
					{
					}
				}
			}
			GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.LookAt(pingTransform.position);
			float y = GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.eulerAngles.y;
			GameObject gameObject = ObjectPoolManager.CreatePooled(this.sonarPingLine.gameObject, this.playerMapContact.transform.position, Quaternion.Euler(0f, 180f, y));
			FadeOverTime component = gameObject.GetComponent<FadeOverTime>();
			component.lineColor = lineColor;
			component.lineColorEnd = component.lineColor;
			component.lineColorEnd.a = 0f;
			component.linerenderer.SetColors(component.lineColor, component.lineColorEnd);
			component.currentColor = component.lineColor;
			component.transform.SetParent(this.tacmapscroller, true);
		}
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x0007E378 File Offset: 0x0007C578
	private void RecordAllPositions(MapContact mapcontacttorecord)
	{
		Transform transform;
		if (mapcontacttorecord.contactNumberOfMoves < this.numberOfMovesInTail)
		{
			transform = ObjectPoolManager.CreatePooled(this.positionMarker, mapcontacttorecord.transform.position, Quaternion.Euler(45f, 90f, -90f)).GetComponent<Transform>();
			transform.SetParent(this.shipPositionLayer, false);
			transform.gameObject.name = mapcontacttorecord.contactNumberOfMoves.ToString();
			mapcontacttorecord.contactNumberOfMoves++;
		}
		else
		{
			transform = mapcontacttorecord.positionMarkerQueue.Dequeue();
		}
		transform.localPosition = mapcontacttorecord.transform.localPosition;
		float num = this.positionSize * this.orthFactor;
		transform.localScale = new Vector3(num, num, num);
		mapcontacttorecord.positionMarkerQueue.Enqueue(transform);
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x0007E444 File Offset: 0x0007C644
	private void RecordTorpedoPositions()
	{
		for (int i = 0; i < this.sensormanager.torpedoObjects.Length; i++)
		{
			if (!this.sensormanager.torpedoObjects[i].databaseweapondata.isMissile && !this.sensormanager.torpedoObjects[i].isAirborne)
			{
				MapContact tacMapTorpedoIcon = this.sensormanager.torpedoObjects[i].tacMapTorpedoIcon;
				if (tacMapTorpedoIcon != null)
				{
					bool flag = this.sensormanager.CheckSingleSonar(GameDataManager.playervesselsonlevel[0].gameObject, this.sensormanager.torpedoObjects[i].gameObject, GameDataManager.playervesselsonlevel[0].databaseshipdata.passiveSonarID, GameDataManager.playervesselsonlevel[0].databaseshipdata.activeSonarID, 0, null, false, null);
					if (this.sensormanager.torpedoObjects[i].databaseweapondata.weaponType == "DECOY" && this.sensormanager.torpedoObjects[i].whichNavy == 0)
					{
						flag = true;
					}
					if (flag)
					{
						tacMapTorpedoIcon.gameObject.SetActive(true);
					}
					else
					{
						tacMapTorpedoIcon.gameObject.SetActive(false);
					}
					if (tacMapTorpedoIcon.gameObject.activeSelf)
					{
						Transform transform;
						if (tacMapTorpedoIcon.contactNumberOfMoves < this.numberOfMovesInTail)
						{
							GameObject prefab = this.positionMarkerTorpedo_enemy;
							if (this.sensormanager.torpedoObjects[i].whichNavy == 0)
							{
								prefab = this.positionMarkerTorpedo_player;
							}
							transform = ObjectPoolManager.CreatePooled(prefab, tacMapTorpedoIcon.transform.position, Quaternion.Euler(45f, 90f, -90f)).GetComponent<Transform>();
							transform.SetParent(this.torpedoLayer.transform, false);
							transform.gameObject.name = tacMapTorpedoIcon.contactNumberOfMoves.ToString();
							tacMapTorpedoIcon.contactNumberOfMoves++;
						}
						else
						{
							transform = tacMapTorpedoIcon.positionMarkerQueue.Dequeue();
						}
						transform.localPosition = tacMapTorpedoIcon.transform.localPosition;
						float num = this.positionSize * this.orthFactor;
						transform.localScale = new Vector3(num, num, num);
						tacMapTorpedoIcon.positionMarkerQueue.Enqueue(transform);
					}
				}
			}
		}
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x0007E670 File Offset: 0x0007C870
	public void TacticalMapRefresh()
	{
		if (GameDataManager.HUDActive)
		{
			this.playerMapObject.transform.localPosition = new Vector3(GameDataManager.playervesselsonlevel[0].transform.position.x * this.zoomFactor, GameDataManager.playervesselsonlevel[0].transform.position.z * this.zoomFactor, -5f);
			Quaternion rotation = Quaternion.Euler(0f, 180f, GameDataManager.playervesselsonlevel[0].transform.eulerAngles.y);
			if (!GameDataManager.playervesselsonlevel[0].isSinking)
			{
				this.playerMapContact.shipDisplayIcon.transform.rotation = rotation;
			}
			else
			{
				this.playerMapContact.shipDisplayIcon.transform.rotation = Quaternion.identity;
			}
			this.dumbfireMarker.gameObject.SetActive(false);
			for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
			{
				Vessel activeVessel = GameDataManager.enemyvesselsonlevel[i];
				this.RefreshContact(activeVessel, i);
			}
			bool flag = false;
			for (int j = 0; j < this.sensormanager.torpedoObjects.Length; j++)
			{
				if (this.sensormanager.torpedoObjects[j] != null && this.sensormanager.torpedoObjects[j].databaseweapondata != null && !this.sensormanager.torpedoObjects[j].databaseweapondata.isMissile)
				{
					this.RefreshTorpedoContact(this.sensormanager.torpedoObjects[j]);
					if (this.sensormanager.torpedoObjects[j].targetTransform != null && this.sensormanager.torpedoObjects[j].targetTransform == GameDataManager.playervesselsonlevel[0].transform)
					{
						flag = true;
						if (!this.sensormanager.torpedoObjects[j].calledOutToPlayer)
						{
							this.sensormanager.torpedoObjects[j].calledOutToPlayer = true;
							string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "TorpedoInTheWater");
							GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.LookAt(this.sensormanager.torpedoObjects[j].transform.position);
							text = text.Replace("<BRG>", string.Format("{0:0}", GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.eulerAngles.y));
							UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentTorpedoBearing = GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.eulerAngles.y.ToString("000");
							UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["Sonar"], "TorpedoInTheWater", false);
						}
					}
				}
			}
			if (flag != UIFunctions.globaluifunctions.playerfunctions.statusIcons[9].gameObject.activeSelf)
			{
				UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("torpedo", flag);
			}
		}
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x0007E9BC File Offset: 0x0007CBBC
	public bool IsMouseOverTacMiniMap()
	{
		return UIFunctions.globaluifunctions.playerfunctions.tacMapMaximisedGraphic.enabled && Input.mousePosition.x < UIFunctions.globaluifunctions.optionsmanager.miniMapUpperRight.x && Input.mousePosition.x > UIFunctions.globaluifunctions.optionsmanager.minimapLowerLeft.x && Input.mousePosition.y < UIFunctions.globaluifunctions.optionsmanager.miniMapUpperRight.y && Input.mousePosition.y > UIFunctions.globaluifunctions.optionsmanager.minimapLowerLeft.y;
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x0007EA80 File Offset: 0x0007CC80
	public void ZoomIn()
	{
		bool flag = false;
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.background.activeSelf)
		{
			flag = true;
		}
		else if (this.IsMouseOverTacMiniMap())
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		if (this.zoomNumber > 3)
		{
			this.zoomNumber--;
			this.SetZoomLevel();
			return;
		}
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x0007EAF4 File Offset: 0x0007CCF4
	public void ZoomOut()
	{
		bool flag = false;
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.background.activeSelf)
		{
			flag = true;
		}
		else if (this.IsMouseOverTacMiniMap())
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		if (this.zoomNumber < 9)
		{
			this.zoomNumber++;
			this.SetZoomLevel();
			return;
		}
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x0007EB68 File Offset: 0x0007CD68
	private void SetZoomLevel()
	{
		float num = 0.5f;
		switch (this.zoomNumber)
		{
		case 3:
			this.orthFactor = 0.85f;
			num = 0.5f;
			this.scalerValue = 4f;
			break;
		case 4:
			this.orthFactor = 1.7f;
			num = 1f;
			this.scalerValue = 2f;
			break;
		case 5:
			this.orthFactor = 3.4f;
			num = 2f;
			this.scalerValue = 1f;
			break;
		case 6:
			this.orthFactor = 6.8f;
			num = 4f;
			this.scalerValue = 0.5f;
			break;
		case 7:
			this.orthFactor = 13.6f;
			num = 8f;
			this.scalerValue = 0.25f;
			break;
		case 8:
			this.orthFactor = 27.2f;
			num = 16f;
			this.scalerValue = 0.125f;
			break;
		case 9:
			this.orthFactor = 54.4f;
			num = 32f;
			this.scalerValue = 0.0625f;
			break;
		}
		switch ((int)UIFunctions.globaluifunctions.gamedatamanager.worldYardsScale)
		{
		case 1:
			num /= 2f;
			break;
		case 3:
			num *= 1.5f;
			break;
		case 4:
			num *= 2f;
			break;
		}
		this.zoomText.text = num.ToString() + " " + LanguageManager.interfaceDictionary["KiloYard"];
		this.TacticalMapZoom();
		this.ScaleHazards();
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x0007ED24 File Offset: 0x0007CF24
	private void TacticalMapZoom()
	{
		this.textMinMaxSize = this.orthFactor * this.textSize;
		this.canvasScaler.dynamicPixelsPerUnit = 2f;
		this.tacMapCamera.orthographicSize = 100f * this.orthFactor;
		if (UIFunctions.globaluifunctions.playerfunctions.tacMapMaximisedGraphic.enabled)
		{
			this.tacMapCamera.orthographicSize = this.tacMapCamera.orthographicSize / 3.6f;
		}
		float num = this.orthFactor;
		if (!GameDataManager.optionsBoolSettings[17])
		{
			this.orthFactor *= 0.9144f;
		}
		this.background.transform.localScale = new Vector3(this.backgroundsize * num * 2f, this.backgroundsize * num, this.backgroundsize * num);
		this.waypointMarker.sizeDelta = new Vector2(this.iconSize * this.orthFactor, this.iconSize * this.orthFactor);
		UIFunctions.globaluifunctions.playerfunctions.helmmanager.navWaypointMarker.sizeDelta = new Vector2(this.iconSize * this.orthFactor, this.iconSize * this.orthFactor);
		this.dumbfireMarker.sizeDelta = new Vector2(this.iconSize * this.orthFactor, this.iconSize * this.orthFactor);
		this.playerMapContact.contactText.transform.localScale = new Vector3(this.textMinMaxSize, this.textMinMaxSize, this.textMinMaxSize);
		this.playerMapContact.contactText.transform.localPosition = new Vector3(0f, this.textYOffset * this.orthFactor, 0f);
		this.waypointLine.SetWidth(this.waypointLineSize * this.orthFactor, this.waypointLineSize * this.orthFactor);
		UIFunctions.globaluifunctions.playerfunctions.helmmanager.navWaypointLine.SetWidth(this.waypointLineSize * this.orthFactor, this.waypointLineSize * this.orthFactor);
		float num2 = this.iconSize * this.orthFactor;
		float num3 = this.positionSize * this.orthFactor;
		this.playerMapContact.shipRectTransform.sizeDelta = new Vector2(num2, num2);
		foreach (Transform transform in this.playerMapContact.positionMarkerQueue.ToArray())
		{
			transform.localScale = new Vector3(num3, num3, num3);
		}
		for (int j = 0; j < this.mapContact.Length; j++)
		{
			this.mapContact[j].contactText.transform.localScale = new Vector3(this.textMinMaxSize, this.textMinMaxSize, this.textMinMaxSize);
			this.mapContact[j].contactText.transform.localPosition = new Vector3(0f, this.textYOffset * this.orthFactor, 0f);
			this.mapContact[j].shipRectTransform.sizeDelta = new Vector2(num2, num2);
			foreach (Transform transform2 in this.mapContact[j].positionMarkerQueue.ToArray())
			{
				transform2.localScale = new Vector3(num3, num3, num3);
			}
		}
		for (int l = 0; l < this.sensormanager.torpedoObjects.Length; l++)
		{
			if (!this.sensormanager.torpedoObjects[l].databaseweapondata.isMissile)
			{
				this.sensormanager.torpedoObjects[l].tacMapTorpedoIcon.shipRectTransform.sizeDelta = new Vector2(num2, num2);
				foreach (Transform transform3 in this.sensormanager.torpedoObjects[l].tacMapTorpedoIcon.positionMarkerQueue.ToArray())
				{
					transform3.localScale = new Vector3(num3, num3, num3);
				}
				for (int n = 0; n < this.sensormanager.torpedoObjects[l].tacMapTorpedoIcon.sensorConeLines.Length; n++)
				{
					this.sensormanager.torpedoObjects[l].tacMapTorpedoIcon.sensorConeLines[n].SetWidth(this.waypointLineSize * this.orthFactor, this.waypointLineSize * this.orthFactor);
				}
				if (this.sensormanager.torpedoObjects[l].tacMapTorpedoIcon.contactText.gameObject.activeSelf)
				{
					this.sensormanager.torpedoObjects[l].tacMapTorpedoIcon.contactText.transform.localScale = new Vector3(this.textMinMaxSize, this.textMinMaxSize, this.textMinMaxSize);
					this.sensormanager.torpedoObjects[l].tacMapTorpedoIcon.contactText.transform.localPosition = new Vector3(0f, this.textYOffset * this.orthFactor, 0f);
				}
			}
		}
		for (int num4 = 0; num4 < this.sensormanager.noisemakerObjects.Length; num4++)
		{
			this.sensormanager.noisemakerObjects[num4].tacMapNoisemakerIcon.shipRectTransform.sizeDelta = new Vector2(num2, num2);
		}
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x0007F288 File Offset: 0x0007D488
	public void SetTacticalMap()
	{
		if (this.minimapIsOpen && !this.background.activeSelf)
		{
			UIFunctions.globaluifunctions.playerfunctions.HUDTacMap();
			this.minimapIsOpen = true;
			TacticalMap.tacMapEnabled = false;
		}
		else if (!this.background.activeSelf)
		{
			TacticalMap.tacMapEnabled = false;
		}
		this.tacMapCamera.gameObject.SetActive(true);
		if (!TacticalMap.tacMapEnabled)
		{
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.zoomText.text = string.Empty;
			UIFunctions.globaluifunctions.playerfunctions.binocularZoomText.gameObject.SetActive(false);
			this.tacPlayerShip = this.GetTacPlayerShip();
			this.levelloadmanager.rain.SetActive(false);
			this.tacticalMap.SetActive(true);
			this.uifunctions.bearingMarker.gameObject.SetActive(false);
			this.background.SetActive(true);
			this.tacMapCamera.depth = 0f;
			TacticalMap.tacMapEnabled = true;
			this.zoomText.gameObject.SetActive(true);
			this.SetZoomLevel();
			this.TacticalMapRefresh();
			this.TacticalMapZoom();
			if (ManualCameraZoom.binoculars)
			{
				UIFunctions.globaluifunctions.playerfunctions.playerVessel.submarineFunctions.LeavePeriscopeView();
			}
			if (GameDataManager.isNight)
			{
				this.levelloadmanager.hudmaterial.color = Color.white;
			}
			if (!UIFunctions.globaluifunctions.playerfunctions.wireData[0].gameObject.activeSelf)
			{
				this.waypointMarker.gameObject.SetActive(false);
			}
			else
			{
				UIFunctions.globaluifunctions.playerfunctions.DisplayWireWaypoint();
			}
			UIFunctions.globaluifunctions.playerfunctions.tacMapMiniMapButtons[0].interactable = false;
			UIFunctions.globaluifunctions.playerfunctions.tacMapMiniMapButtons[1].interactable = false;
			GC.Collect();
		}
		else
		{
			UIFunctions.globaluifunctions.playerfunctions.binocularZoomText.gameObject.SetActive(false);
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.zoomText.text = string.Empty;
			if (this.minimapIsOpen)
			{
				TacticalMap.tacMapEnabled = true;
				UIFunctions.globaluifunctions.playerfunctions.tacMapMiniMapButtons[1].interactable = true;
				UIFunctions.globaluifunctions.playerfunctions.HUDTacMap();
				this.tacMapCamera.depth = 2f;
			}
			else
			{
				UIFunctions.globaluifunctions.playerfunctions.tacMapMiniMapButtons[0].interactable = true;
				UIFunctions.globaluifunctions.playerfunctions.tacMapMiniMapButtons[1].interactable = true;
				this.tacMapCamera.depth = -1f;
			}
			this.background.SetActive(false);
			this.uifunctions.bearingMarker.gameObject.SetActive(true);
			this.zoomText.gameObject.SetActive(false);
			if (LevelLoadManager.isRaining && !ManualCameraZoom.underwater)
			{
				this.levelloadmanager.rain.SetActive(true);
			}
			if (ManualCameraZoom.binoculars)
			{
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.SetPeriscopeMask(true);
			}
			if (GameDataManager.isNight)
			{
				this.levelloadmanager.hudmaterial.color = new Color(0.351f, 0.214f, 0.214f, 1f);
			}
		}
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x0007F5F4 File Offset: 0x0007D7F4
	public void SetTacMiniMap(bool open)
	{
		float scaleFactor = UIFunctions.globaluifunctions.playerfunctions.hudScaler.scaleFactor;
		if (open)
		{
			this.tacMapCamera.rect = this.tacMapCameraRects[0];
			this.tacMapCamera.depth = 2f;
			this.tacMapCameraOverlay.enabled = false;
			this.background.SetActive(false);
			this.SetZoomLevel();
		}
		else
		{
			this.tacMapCamera.rect = this.tacMapCameraRects[1];
			this.tacMapCamera.depth = -1f;
			this.tacMapCameraOverlay.enabled = true;
		}
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x0007F6A4 File Offset: 0x0007D8A4
	private void ActivateShips(bool on)
	{
		foreach (Vessel vessel in this.allVesselsList)
		{
			if (vessel.transform.position.y < 1010f)
			{
				vessel.gameObject.SetActive(on);
			}
		}
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x0007F6FC File Offset: 0x0007D8FC
	private void CreateContact(Vessel activeVessel, int i, Color iconColor)
	{
		this.tacticalMap.transform.position = new Vector3(0f, 0f, 1050f);
		Vector3 position = new Vector3(activeVessel.transform.position.x * this.zoomFactor, activeVessel.transform.position.z * this.zoomFactor, 1045f);
		Quaternion rotation = Quaternion.Euler(0f, 180f, activeVessel.transform.eulerAngles.y);
		this.mapObjects[i] = ObjectPoolManager.CreatePooled(this.shipContact, position, Quaternion.identity);
		this.mapObjects[i].name = i.ToString();
		this.mapObjects[i].transform.SetParent(this.tacmapscroller, false);
		MapContact component = this.mapObjects[i].GetComponent<MapContact>();
		component.positionMarkerQueue = new Queue<Transform>();
		this.mapContact[i] = component;
		component.shipDisplayIcon.transform.rotation = rotation;
		component.shipDisplayIcon.color = iconColor;
		component.contactText.color = iconColor;
		component.contactText.text = activeVessel.databaseshipdata.shipclass;
		this.mapContactButtons[i] = this.mapContact[i].contactButton;
		this.mapContactButtons[i].onClick.AddListener(delegate()
		{
			this.uifunctions.playerfunctions.MapContactButton(i, activeVessel.playercontrolled);
		});
		component.gameObject.SetActive(false);
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x0007F8D4 File Offset: 0x0007DAD4
	private void SetLeadPositionMarker(int i)
	{
		int num = GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.weaponInTube[GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.activeTube];
		if (num > -1 && UIFunctions.globaluifunctions.database.databaseweapondata[num].weaponType == "TORPEDO")
		{
			float num2 = Vector3.Distance(GameDataManager.enemyvesselsonlevel[i].transform.position, GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.torpedoTubes[GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.activeTube].position);
			float d = num2 / (UIFunctions.globaluifunctions.database.databaseweapondata[num].runSpeed / 10f * GameDataManager.globalTranslationSpeed);
			Vector3 vector = GameDataManager.enemyvesselsonlevel[i].transform.position + GameDataManager.enemyvesselsonlevel[i].transform.forward * GameDataManager.enemyvesselsonlevel[i].vesselmovement.shipSpeed.z * GameDataManager.globalTranslationSpeed * d;
			this.dumbfireMarker.gameObject.transform.localPosition = new Vector3(vector.x * this.zoomFactor, vector.z * this.zoomFactor, -5f);
			this.dumbfireMarker.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x0007FA48 File Offset: 0x0007DC48
	private void RefreshContact(Vessel activeVessel, int i)
	{
		if (activeVessel.isSinking || activeVessel.isCapsizing)
		{
			this.mapContact[i].shipDisplayIcon.sprite = this.sensormanager.sonarPaintImages[4];
			this.mapContact[i].shipDisplayIcon.color = this.sunkColor;
			this.mapContact[i].shipDisplayIcon.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			return;
		}
		if (!this.sensormanager.detectedByPlayer[i])
		{
			return;
		}
		if (this.sensormanager.solutionQualityOfContacts[i] >= this.qualityToDrawTails)
		{
			this.mapObjects[i].transform.localPosition = new Vector3(activeVessel.transform.position.x * this.zoomFactor, activeVessel.transform.position.z * this.zoomFactor, -5f);
			if (i == UIFunctions.globaluifunctions.playerfunctions.currentTargetIndex)
			{
				this.SetLeadPositionMarker(i);
			}
		}
		else
		{
			this.mapObjects[i].transform.localPosition = new Vector3(this.sensormanager.enemyPositions[i].x * this.zoomFactor, this.sensormanager.enemyPositions[i].z * this.zoomFactor, -5f);
		}
		Quaternion rotation = Quaternion.identity;
		if (this.sensormanager.solutionQualityOfContacts[i] < this.qualityToCourse)
		{
			if (this.sensormanager.identifiedByPlayer[i])
			{
				this.mapContact[i].shipDisplayIcon.sprite = this.sensormanager.sonarPaintImages[this.sensormanager.shipTypes[i]];
			}
			else
			{
				this.mapContact[i].shipDisplayIcon.sprite = this.sensormanager.sonarPaintImages[0];
			}
		}
		else
		{
			rotation = Quaternion.Euler(0f, 180f, activeVessel.transform.eulerAngles.y);
			this.mapContact[i].shipDisplayIcon.sprite = this.sensormanager.sonarPaintImages[5];
		}
		this.mapContact[i].shipDisplayIcon.transform.rotation = rotation;
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x0007FCA0 File Offset: 0x0007DEA0
	private void RefreshTorpedoContact(Torpedo torpedo)
	{
		if (torpedo != null)
		{
			if (torpedo.isAirborne)
			{
				torpedo.tacMapTorpedoIcon.gameObject.SetActive(false);
			}
			else
			{
				torpedo.tacMapTorpedoIcon.transform.localPosition = new Vector3(torpedo.transform.position.x * this.zoomFactor, torpedo.transform.position.z * this.zoomFactor, -5f);
				Quaternion rotation = Quaternion.Euler(0f, 180f, torpedo.transform.eulerAngles.y);
				torpedo.tacMapTorpedoIcon.shipRectTransform.transform.rotation = rotation;
			}
		}
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x0007FD64 File Offset: 0x0007DF64
	private int GetTacPlayerShip()
	{
		return GameDataManager.activePlayerSlot;
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x0007FD78 File Offset: 0x0007DF78
	public float GetBearingToWeaponWaypoint()
	{
		GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.LookAt(this.waypointWorldMarker.transform.position);
		return GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.eulerAngles.y;
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x0007FDD8 File Offset: 0x0007DFD8
	public float GetDistanceToWeaponWaypoint()
	{
		return Vector3.Distance(GameDataManager.playervesselsonlevel[0].transform.position, this.waypointWorldMarker.transform.position) * GameDataManager.yardsScale;
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x0007FE14 File Offset: 0x0007E014
	public void SetWaypointMarker()
	{
		this.waypointMarker.gameObject.transform.localPosition = new Vector3(this.waypointMarker.gameObject.transform.localPosition.x, this.waypointMarker.gameObject.transform.localPosition.y, 0f);
		this.waypointMarker.gameObject.SetActive(true);
		this.waypointWorldMarker.transform.position = new Vector3(this.waypointMarker.transform.localPosition.x / this.zoomFactor, 1000f, this.waypointMarker.transform.localPosition.y / this.zoomFactor);
		string[] waypointDetails = this.GetWaypointDetails();
		string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "WaypointSet");
		text = text.Replace("<BRG>", waypointDetails[0]);
		text = text.Replace("<RANGE>", waypointDetails[1]);
		text = text.Replace("<KILOYARD>", LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "KiloYard"));
		UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["FireControl"], "WaypointSet", false);
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x0007FF64 File Offset: 0x0007E164
	public string[] GetWaypointDetails()
	{
		float num = this.GetBearingToWeaponWaypoint();
		if (num < 0f)
		{
			num = 360f + num;
		}
		int num2 = Mathf.RoundToInt(num);
		if (num2 == 360)
		{
			num2 = 0;
		}
		float num3 = this.GetDistanceToWeaponWaypoint();
		num3 /= 1000f;
		return new string[]
		{
			num2.ToString("000"),
			string.Format("{0:0.0}", num3)
		};
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x0007FFD8 File Offset: 0x0007E1D8
	public void CentreMap()
	{
		this.tacmapscroller.transform.localPosition = new Vector3(UIFunctions.globaluifunctions.playerfunctions.playerVessel.transform.position.x * -5f, UIFunctions.globaluifunctions.playerfunctions.playerVessel.transform.position.z * -5f, this.tacmapscroller.transform.localPosition.z);
	}

	// Token: 0x04000FE2 RID: 4066
	public UIFunctions uifunctions;

	// Token: 0x04000FE3 RID: 4067
	public LevelLoadManager levelloadmanager;

	// Token: 0x04000FE4 RID: 4068
	public SensorManager sensormanager;

	// Token: 0x04000FE5 RID: 4069
	public GameObject[] mapObjects;

	// Token: 0x04000FE6 RID: 4070
	public MapContact[] mapContact;

	// Token: 0x04000FE7 RID: 4071
	public GameObject playerMapObject;

	// Token: 0x04000FE8 RID: 4072
	public MapContact playerMapContact;

	// Token: 0x04000FE9 RID: 4073
	public Vessel[] allVesselsList;

	// Token: 0x04000FEA RID: 4074
	public GameObject shipContact;

	// Token: 0x04000FEB RID: 4075
	public GameObject torpedoContact;

	// Token: 0x04000FEC RID: 4076
	public Color disengageColor;

	// Token: 0x04000FED RID: 4077
	public Color activeColor;

	// Token: 0x04000FEE RID: 4078
	public Color radarColor;

	// Token: 0x04000FEF RID: 4079
	public Color sonarColor;

	// Token: 0x04000FF0 RID: 4080
	public Color[] navyColors;

	// Token: 0x04000FF1 RID: 4081
	public Vector3 cameraStart;

	// Token: 0x04000FF2 RID: 4082
	public bool mapInitialized;

	// Token: 0x04000FF3 RID: 4083
	public static bool tacMapEnabled = true;

	// Token: 0x04000FF4 RID: 4084
	public int numberOfShips;

	// Token: 0x04000FF5 RID: 4085
	public float zoomFactor;

	// Token: 0x04000FF6 RID: 4086
	public float orthFactor;

	// Token: 0x04000FF7 RID: 4087
	public float minOrth;

	// Token: 0x04000FF8 RID: 4088
	public float maxOrth;

	// Token: 0x04000FF9 RID: 4089
	public int zoomNumber;

	// Token: 0x04000FFA RID: 4090
	public GameObject tacticalMap;

	// Token: 0x04000FFB RID: 4091
	public RectTransform waypointMarker;

	// Token: 0x04000FFC RID: 4092
	public RectTransform dumbfireMarker;

	// Token: 0x04000FFD RID: 4093
	public GameObject background;

	// Token: 0x04000FFE RID: 4094
	public float backgroundsize;

	// Token: 0x04000FFF RID: 4095
	public int tacActiveShip;

	// Token: 0x04001000 RID: 4096
	public int tacPlayerShip;

	// Token: 0x04001001 RID: 4097
	public ManualCameraZoom manualcamerazoom;

	// Token: 0x04001002 RID: 4098
	public Camera tacMapCamera;

	// Token: 0x04001003 RID: 4099
	public bool zoom;

	// Token: 0x04001004 RID: 4100
	public float textYOffset;

	// Token: 0x04001005 RID: 4101
	public float iconSize;

	// Token: 0x04001006 RID: 4102
	public float positionSize;

	// Token: 0x04001007 RID: 4103
	public float waypointLineSize;

	// Token: 0x04001008 RID: 4104
	public float textSize;

	// Token: 0x04001009 RID: 4105
	public float textMinMaxSize;

	// Token: 0x0400100A RID: 4106
	public Text zoomText;

	// Token: 0x0400100B RID: 4107
	public Clock[] xyclock;

	// Token: 0x0400100C RID: 4108
	public CanvasScaler canvasScaler;

	// Token: 0x0400100D RID: 4109
	public Canvas tacmapCanvas;

	// Token: 0x0400100E RID: 4110
	public float scalerValue;

	// Token: 0x0400100F RID: 4111
	public int numberOfMovesInTail;

	// Token: 0x04001010 RID: 4112
	public GameObject positionMarker;

	// Token: 0x04001011 RID: 4113
	public GameObject positionMarkerTorpedo_enemy;

	// Token: 0x04001012 RID: 4114
	public GameObject positionMarkerTorpedo_player;

	// Token: 0x04001013 RID: 4115
	public GameObject torpedoLayer;

	// Token: 0x04001014 RID: 4116
	public GameObject noisemakerLayer;

	// Token: 0x04001015 RID: 4117
	public float updateTimer;

	// Token: 0x04001016 RID: 4118
	public float positionTimer;

	// Token: 0x04001017 RID: 4119
	public float positionTorpedoTimer;

	// Token: 0x04001018 RID: 4120
	public Transform shipPositionLayer;

	// Token: 0x04001019 RID: 4121
	public GameObject waypointWorldMarker;

	// Token: 0x0400101A RID: 4122
	public Transform tacmapscroller;

	// Token: 0x0400101B RID: 4123
	public float qualityToRange;

	// Token: 0x0400101C RID: 4124
	public float qualityToSpeed;

	// Token: 0x0400101D RID: 4125
	public float qualityToCourse;

	// Token: 0x0400101E RID: 4126
	public float qualityToDrawTails;

	// Token: 0x0400101F RID: 4127
	public Button[] mapContactButtons;

	// Token: 0x04001020 RID: 4128
	public LineRenderer waypointLine;

	// Token: 0x04001021 RID: 4129
	public Color sensorCone;

	// Token: 0x04001022 RID: 4130
	public Color sensorConeTracking;

	// Token: 0x04001023 RID: 4131
	public Text waypointReadout;

	// Token: 0x04001024 RID: 4132
	public Color waypointMapColor;

	// Token: 0x04001025 RID: 4133
	public Color waypointReadoutColor;

	// Token: 0x04001026 RID: 4134
	public FadeOverTime sonarPingLine;

	// Token: 0x04001027 RID: 4135
	public GameObject torpedoMapContactObject;

	// Token: 0x04001028 RID: 4136
	public Rect[] tacMapCameraRects;

	// Token: 0x04001029 RID: 4137
	public ScreenOverlay tacMapCameraOverlay;

	// Token: 0x0400102A RID: 4138
	public bool minimapIsOpen;

	// Token: 0x0400102B RID: 4139
	public Color[] pingLines;

	// Token: 0x0400102C RID: 4140
	public Color[] torpedoColors;

	// Token: 0x0400102D RID: 4141
	public Color sunkColor;

	// Token: 0x0400102E RID: 4142
	public Material[] mapPositionMarkers;

	// Token: 0x0400102F RID: 4143
	public Image missionMarker;

	// Token: 0x04001030 RID: 4144
	public Texture2D landOverlay;

	// Token: 0x04001031 RID: 4145
	public GameObject landOverlayObject;

	// Token: 0x04001032 RID: 4146
	public Material landOverlayMaterial;

	// Token: 0x04001033 RID: 4147
	public float[] landOverlayThresholds;

	// Token: 0x04001034 RID: 4148
	public float[] landOverlayLerps;

	// Token: 0x04001035 RID: 4149
	public Color landOverlayLandColor;

	// Token: 0x04001036 RID: 4150
	public Color landOverlayCoastColor;

	// Token: 0x04001037 RID: 4151
	public Color landOverlayWaterColor;

	// Token: 0x04001038 RID: 4152
	public bool landOverlayFilter;

	// Token: 0x04001039 RID: 4153
	public Transform hazardIcons;

	// Token: 0x0400103A RID: 4154
	public Sprite[] hazardSprites;

	// Token: 0x0400103B RID: 4155
	public Color[] hazardColors;

	// Token: 0x0400103C RID: 4156
	public bool autoCentreMap;
}
