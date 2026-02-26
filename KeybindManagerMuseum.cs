using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200012E RID: 302
public class KeybindManagerMuseum : MonoBehaviour
{
	// Token: 0x0600082A RID: 2090 RVA: 0x00052558 File Offset: 0x00050758
	private void Update()
	{
		if (UIFunctions.globaluifunctions.helpmanager.gameObject.activeSelf)
		{
			return;
		}
		if (Input.anyKeyDown)
		{
			if (!KeybindManagerMuseum.selectionScreen)
			{
				if (InputManager.globalInputManager.GetButtonDown("Cancel or Quit", false))
				{
					UIFunctions.globaluifunctions.levelloadmanager.CloseMuseum();
				}
				else if (InputManager.globalInputManager.GetButtonDown("Museum Next", false))
				{
					this.GetNextMuseumItem(true);
				}
				else if (InputManager.globalInputManager.GetButtonDown("Museum Previous", false))
				{
					this.GetNextMuseumItem(false);
				}
				else if (InputManager.globalInputManager.GetButtonDown("Continue", false) && UIFunctions.globaluifunctions.levelloadmanager.shipDebugModeOn)
				{
					if (UIFunctions.globaluifunctions.levelloadmanager.currentShipDebugIndex == 10)
					{
						if (!GameDataManager.enemyvesselsonlevel[0].isSubmarine && !GameDataManager.enemyvesselsonlevel[0].damagesystem.destroyedMesh)
						{
							GameDataManager.enemyvesselsonlevel[0].damagesystem.SetMeshes();
						}
					}
					else
					{
						GameDataManager.enemyvesselsonlevel[0].bouyancyCompartments[UIFunctions.globaluifunctions.levelloadmanager.currentShipDebugIndex].ApplyDamageDecal(true, -1);
						UIFunctions.globaluifunctions.levelloadmanager.currentShipDebugIndex++;
					}
				}
			}
			else if (!InputManager.globalInputManager.GetButtonDown("Cancel or Quit", false))
			{
				if (InputManager.globalInputManager.GetButtonDown("Museum Next", false) && !UIFunctions.globaluifunctions.missionmanager.assignedShip)
				{
					this.GetNextMuseumItem(true);
				}
				else if (InputManager.globalInputManager.GetButtonDown("Museum Previous", false) && !UIFunctions.globaluifunctions.missionmanager.assignedShip)
				{
					this.GetNextMuseumItem(false);
				}
			}
		}
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x00052738 File Offset: 0x00050938
	public void GetNextMuseumItem(bool next)
	{
		if (!KeybindManagerMuseum.selectionScreen)
		{
			if (next)
			{
				int num = ++UIFunctions.globaluifunctions.levelloadmanager.currentMuseumItem;
				int num2 = UIFunctions.globaluifunctions.levelloadmanager.currentFilteredVessels.Count + UIFunctions.globaluifunctions.levelloadmanager.currentFilteredAircraft.Count + UIFunctions.globaluifunctions.levelloadmanager.currentFilteredWeapons.Count;
				if (num == num2)
				{
					UIFunctions.globaluifunctions.levelloadmanager.currentMuseumItem = 0;
				}
				UIFunctions.globaluifunctions.levelloadmanager.BackgroundMuseumRender(false);
			}
			else
			{
				int num3 = --UIFunctions.globaluifunctions.levelloadmanager.currentMuseumItem;
				if (num3 < 0)
				{
					int num4 = UIFunctions.globaluifunctions.levelloadmanager.currentFilteredVessels.Count + UIFunctions.globaluifunctions.levelloadmanager.currentFilteredAircraft.Count + UIFunctions.globaluifunctions.levelloadmanager.currentFilteredWeapons.Count;
					UIFunctions.globaluifunctions.levelloadmanager.currentMuseumItem = num4 - 1;
				}
				UIFunctions.globaluifunctions.levelloadmanager.BackgroundMuseumRender(false);
			}
		}
		else if (next)
		{
			if (UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay.Length == 1)
			{
				return;
			}
			this.currentShipForSelection++;
			if (this.currentShipForSelection > this.unitsToDisplay.Length - 1)
			{
				this.currentShipForSelection = 0;
			}
			UIFunctions.globaluifunctions.levelloadmanager.currentMuseumItem = this.unitsToDisplay[this.currentShipForSelection];
			UIFunctions.globaluifunctions.levelloadmanager.BackgroundMuseumRender(false);
		}
		else
		{
			if (UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay.Length == 1)
			{
				return;
			}
			this.currentShipForSelection--;
			if (this.currentShipForSelection < 0)
			{
				this.currentShipForSelection = this.unitsToDisplay.Length - 1;
			}
			UIFunctions.globaluifunctions.levelloadmanager.currentMuseumItem = this.unitsToDisplay[this.currentShipForSelection];
			UIFunctions.globaluifunctions.levelloadmanager.BackgroundMuseumRender(false);
		}
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x00052950 File Offset: 0x00050B50
	public void GetNextFilteredMuseumItem(bool next)
	{
	}

	// Token: 0x04000BDD RID: 3037
	public static bool selectionScreen;

	// Token: 0x04000BDE RID: 3038
	public int[] unitsToDisplay;

	// Token: 0x04000BDF RID: 3039
	public int currentShipForSelection;

	// Token: 0x04000BE0 RID: 3040
	public Button selectShipButton;

	// Token: 0x04000BE1 RID: 3041
	public Button randomShipButton;

	// Token: 0x04000BE2 RID: 3042
	public Button nextPrevButton;
}
