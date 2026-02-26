using System;
using UnityEngine;

// Token: 0x0200012D RID: 301
public class KeybindManagerMenu : MonoBehaviour
{
	// Token: 0x06000828 RID: 2088 RVA: 0x00052050 File Offset: 0x00050250
	private void Update()
	{
		if (Input.anyKeyDown && InputManager.globalInputManager.GetButtonDown("Cancel or Quit", false) && UIFunctions.globaluifunctions.helpmanager.gameObject.activeSelf)
		{
			UIFunctions.globaluifunctions.helpmanager.gameObject.SetActive(!UIFunctions.globaluifunctions.helpmanager.gameObject.activeSelf);
			return;
		}
		if (UIFunctions.globaluifunctions.helpmanager.gameObject.activeSelf)
		{
			return;
		}
		if (Input.anyKeyDown)
		{
			if (InputManager.globalInputManager.GetButtonDown("Continue", false))
			{
				if (UIFunctions.globaluifunctions.menuSystem[2].activeSelf)
				{
					UIFunctions.globaluifunctions.CloseActionReport();
					return;
				}
				if (UIFunctions.globaluifunctions.menuSystem[8].activeSelf)
				{
					UIFunctions.globaluifunctions.campaignmanager.eventManager.ContinueButton();
					return;
				}
			}
			if (UIFunctions.globaluifunctions.menuSystem[7].activeSelf || UIFunctions.globaluifunctions.menuSystem[11].activeSelf)
			{
				return;
			}
			if (InputManager.globalInputManager.GetButtonDown("Damage Report", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.OpenContextualPanel(2);
			}
			else if (InputManager.globalInputManager.GetButtonDown("Stores", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.OpenContextualPanel(3);
			}
			else if (InputManager.globalInputManager.GetButtonDown("Weapon Homing", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.SetWeaponHoming();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Weapon Attack", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.SetWeaponAttack();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Weapon Depth", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.SetWeaponDepth();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Next Tube", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.GetNextTube();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Load Tube", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.ReloadTube();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Context Menu Set", false))
			{
				if (UIFunctions.globaluifunctions.playerfunctions.damagecontrol.timeToRepairReadout.text != string.Empty)
				{
					UIFunctions.globaluifunctions.playerfunctions.damagecontrol.SelectDamageControlSubsystem(UIFunctions.globaluifunctions.database.databasesubsystemsdata[UIFunctions.globaluifunctions.playerfunctions.damagecontrol.currentSubsystem].subsystem);
				}
			}
			else if (InputManager.globalInputManager.GetButtonDown("Context Menu Next", false) || InputManager.globalInputManager.GetButtonDown("Context Menu Previous", false))
			{
				if (UIFunctions.globaluifunctions.playerfunctions.currentOpenPanel == 2)
				{
					int num = UIFunctions.globaluifunctions.playerfunctions.damagecontrol.currentSubsystem;
					if (InputManager.globalInputManager.GetButtonDown("Context Menu Next", false))
					{
						do
						{
							num++;
							if (num == UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlLabels.Length)
							{
								num = 0;
							}
						}
						while (!UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlLabels[num].gameObject.activeSelf);
					}
					else
					{
						do
						{
							num--;
							if (num < 0)
							{
								num = UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlLabels.Length - 1;
							}
						}
						while (!UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlLabels[num].gameObject.activeSelf);
					}
					UIFunctions.globaluifunctions.playerfunctions.damagecontrol.BoldText(num);
					UIFunctions.globaluifunctions.playerfunctions.damagecontrol.GetTimeToRepair();
					UIFunctions.globaluifunctions.playerfunctions.damagecontrol.currentCompartment = UIFunctions.globaluifunctions.playerfunctions.damagecontrol.GetSubsystemCompartment();
				}
				else
				{
					if (InputManager.globalInputManager.GetButtonDown("Context Menu Next", false))
					{
						UIFunctions.globaluifunctions.portRearm.currentWeapon++;
					}
					else
					{
						UIFunctions.globaluifunctions.portRearm.currentWeapon--;
					}
					if (UIFunctions.globaluifunctions.portRearm.currentWeapon == UIFunctions.globaluifunctions.playerfunctions.playerVessel.vesselmovement.weaponSource.torpedoTypes.Length)
					{
						UIFunctions.globaluifunctions.portRearm.currentWeapon = 0;
					}
					else if (UIFunctions.globaluifunctions.portRearm.currentWeapon < 0)
					{
						UIFunctions.globaluifunctions.portRearm.currentWeapon = UIFunctions.globaluifunctions.playerfunctions.playerVessel.vesselmovement.weaponSource.torpedoTypes.Length - 1;
					}
					UIFunctions.globaluifunctions.portRearm.SetLoadoutStats();
				}
			}
		}
	}
}
