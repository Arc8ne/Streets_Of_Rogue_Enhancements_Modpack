using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SOR_Test_Modpack
{
	public class HoldAndShootModule : ISORModpackModule
	{
		public static HoldAndShootModule instance = new HoldAndShootModule();

		public HoldAndShootModule()
		{

		}

		public void Init()
		{
			SORTestModpackCore.instance.harmony.Patch(
				original: AccessTools.Method(
					typeof(Gun),
					nameof(Gun.GunUpdate)
				),
				prefix: new HarmonyMethod(
					AccessTools.Method(
						typeof(HoldAndShootModule),
						nameof(Gun_GunUpdate_PrefixPatch)
					)
				)
			);
		}

		public static bool Gun_GunUpdate_PrefixPatch(Gun __instance)
		{
			bool flag = false;
			bool flag2 = false;
			__instance.pressedFire = false;
			__instance.heldFire = false;
			bool dead = __instance.agent.oma._dead;
			if (!__instance.gc.serverPlayer)
			{
				dead = __instance.agent.dead;
			}
			if (__instance.agent.isPlayer > 0 && __instance.agent.localPlayer)
			{
				if (__instance.gc.playerControl.keyCheck(buttonType.Attack, __instance.agent) && !__instance.agent.ghost)
				{
					flag = true;
					__instance.holdingAttack = false;
					__instance.pressedFire = true;
					if (__instance.agent.inventory.equippedWeapon.rapidFire && __instance.mainGUI.invInterface.thrownItem == null && !__instance.mainGUI.invInterface.justThrewItem)
					{
						__instance.holdingAttack = true;
					}
				}
				else if (__instance.gc.playerControl.keyCheckHeld(buttonType.Attack, __instance.agent))
				{
					flag = true;
				}
				else if (__instance.gc.playerControl.releasedAttack[__instance.agent.isPlayer - 1])
				{
					__instance.holdingAttack = false;
					__instance.mainGUI.invInterface.justThrewItem = false;
				}
				else if (__instance.agent.inventory.equippedWeapon != null && __instance.agent.inventory.equippedWeapon.rapidFire && __instance.holdingAttack && !__instance.mainGUI.showingTarget && __instance.gc.playerControl.keyCheckHeld(buttonType.Attack, __instance.agent))
				{
					__instance.heldFire = true;
					flag = true;
				}
				if (__instance.agent.agentInvDatabase.equippedSpecialAbility != null && __instance.agent.agentInvDatabase.equippedSpecialAbility.isWeapon)
				{
					if (__instance.gc.playerControl.keyCheck(buttonType.SpecialAbility, __instance.agent) && !__instance.agent.ghost)
					{
						if (__instance.agent.agentInvDatabase.equippedSpecialAbility.invItemCount == 0 && __instance.agent.agentInvDatabase.equippedSpecialAbility.weaponCode == weaponType.WeaponProjectile)
						{
							if (((!__instance.mainGUI.overInterface && !__instance.worldSpaceGUI.overInterface) || !(__instance.agent.controllerType == "Keyboard")) && !__instance.mainGUI.showingTarget && !__instance.worldSpaceGUI.openedObjectButtons && !__instance.worldSpaceGUI.preventClicks && !__instance.mainGUI.showingInterface && !__instance.mainGUI.preventClicks && __instance.mainGUI.invInterface.thrownItem == null && __instance.mainGUI.invInterface.draggedInvItem == null && !__instance.agent.isOperating && !__instance.agent.dead && (!__instance.gc.mainGUI.menuGUI.onMenu || __instance.gc.multiplayerMode) && !(Time.timeScale == 0f & __instance.gc.freezeFrame == -1f))
							{
								__instance.gc.audioHandler.Play(__instance.agent, "LaserGunFireEmpty");
							}
						}
						else
						{
							flag = true;
							__instance.holdingAttackSpecial = false;
							flag2 = true;
							if (__instance.agent.agentInvDatabase.equippedSpecialAbility.rapidFire)
							{
								__instance.holdingAttackSpecial = true;
							}
						}
					}
					else if (__instance.gc.playerControl.releasedSpecialAbility[__instance.agent.isPlayer - 1])
					{
						if (__instance.holdingAttackSpecial)
						{
							__instance.holdingAttackSpecial = false;
							if (__instance.mostRecentGunUsed != "LaserGun")
							{
								__instance.HideGun();
							}
						}
					}
					else if (__instance.agent.agentInvDatabase.equippedSpecialAbility.rapidFire && __instance.holdingAttackSpecial && !__instance.mainGUI.showingTarget && __instance.agent.agentInvDatabase.equippedSpecialAbility.invItemCount > 0 && __instance.gc.playerControl.keyCheckHeld(buttonType.SpecialAbility, __instance.agent))
					{
						__instance.heldFire = true;
						flag2 = true;
						flag = true;
					}
				}
				if (__instance.thrownItemTarget != null)
				{
					if (__instance.agent.inventory.equippedWeapon.weaponCode == weaponType.WeaponThrown && !__instance.agent.mainGUI.showingTarget && __instance.agent.controllerType == "Keyboard" && !dead && !__instance.gc.cinematic && !__instance.gc.menuGUI.onMenu && Time.timeScale != 0f)
					{
						if (!__instance.agent.mainGUI.showingInterface && (float)__instance.agent.inventory.equippedWeapon.throwDistance < Vector2.Distance(__instance.agent.tr.position, __instance.mainGUI.agent.agentCamera.actualCamera.ScreenCamera.ScreenToWorldPoint(Input.mousePosition)))
						{
							__instance.thrownItemTarget.SetActive(true);
							__instance.thrownItemTargetTr.position = __instance.agent.tr.position;
							__instance.thrownItemTargetMovement.RotateToMouseTr(__instance.mainGUI.agent.agentCamera.actualCamera);
							__instance.thrownItemTargetMovement.MoveForwardTransform((float)__instance.agent.inventory.equippedWeapon.throwDistance);
							__instance.thrownItemTargetTr.eulerAngles = Vector3.zero;
						}
						else if (__instance.thrownItemTarget.activeSelf)
						{
							__instance.thrownItemTarget.SetActive(false);
						}
					}
					else if (__instance.thrownItemTarget.activeSelf)
					{
						__instance.thrownItemTarget.SetActive(false);
					}
				}
				if (__instance.agent.stomping || __instance.agent.stomping2)
				{
					flag = false;
				}
			}
			if (__instance.gc.multiplayerMode && !__instance.agent.localPlayer && __instance.visibleTime < 3.94f && __instance.gunAnim.enabled && __instance.usingSpecialGun && __instance.mostRecentGunUsed != "LaserGun")
			{
				__instance.HideGun();
			}
			if (__instance.dontAttackOneFrame)
			{
				flag = false;
			}
			if (__instance.agent.mindControlling && __instance.agent.localPlayer)
			{
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				if (__instance.gc.playerControl.keyCheck(buttonType.Attack, __instance.agent))
				{
					flag3 = true;
					flag4 = true;
				}
				else if (__instance.gc.playerControl.keyCheckHeld(buttonType.Attack, __instance.agent))
				{
					flag3 = true;
					flag5 = true;
				}
				if (flag3)
				{
					for (int i = 0; i < __instance.gc.agentList.Count; i++)
					{
						Agent agent = __instance.gc.agentList[i];
						if (agent.oma.mindControlled && agent.mindControlAgent == __instance.agent && !agent.relationships.MindControlPause())
						{
							InvItem equippedWeapon = agent.inventory.equippedWeapon;
							if (flag4 || (flag5 && agent.inventory.equippedWeapon.rapidFire))
							{
								if (equippedWeapon.weaponCode == weaponType.WeaponMelee)
								{
									agent.melee.CheckAttack(false);
								}
								else if (equippedWeapon.weaponCode == weaponType.WeaponProjectile)
								{
									agent.gun.CheckAttack(false);
								}
								else if (equippedWeapon.weaponCode == weaponType.WeaponThrown)
								{
									agent.gun.CheckAttack(false);
								}
							}
						}
					}
				}
			}

			if (flag)
			{
				if (!flag2)
				{
					if (!__instance.agent.mindControlling)
					{
						__instance.CheckAttack(false);
						if (!__instance.justSwitchedWeapon)
						{
							__instance.agent.melee.CheckAttack(false);
						}
					}
				}
				else
				{
					__instance.CheckAttack(true);
					if (!__instance.justSwitchedWeapon && __instance.agent.statusEffects.CanSpecialAttack(true) != null)
					{
						__instance.agent.melee.CheckAttack(true);
					}
				}
				__instance.justSwitchedWeapon = false;
				if (__instance.mainGUI.invInterface.thrownItem != null)
				{
					__instance.ThrowItem();
				}
				if (__instance.agent.target != null)
				{
					__instance.TargetObject();
				}
			}
			if (__instance.agent.weaponCooldown > 0f)
			{
				__instance.agent.weaponCooldown -= Time.deltaTime;
			}
			if ((__instance.agent.isPlayer == 0 || __instance.agent.outOfControl) && __instance.visibleGun == null && !dead)
			{
				InvItem equippedWeapon2 = __instance.agent.inventory.equippedWeapon;
			}
			if (__instance.visibleTime > 0f)
			{
				__instance.visibleTime -= Time.deltaTime;
				if (__instance.visibleTime <= 0f || __instance.agent.inventory.equippedWeapon != __instance.visibleGun || ((__instance.agent.isPlayer == 0 || __instance.agent.outOfControl) && __instance.agent.mostRecentGoalCode != goalType.Battle && __instance.agent.mostRecentGoalCode != goalType.Flee && __instance.agent.mostRecentGoalCode != goalType.RobotFollow && !__instance.agent.oma.mindControlled))
				{
					__instance.HideGun();
				}
			}
			if (__instance.agent.target != null && __instance.agent.controllerType != "Keyboard")
			{
				__instance.mainGUI.invInterface.TargetAnywhere(__instance.agent.target.tr.position, flag);
			}
			if (!__instance.agent.ghost && __instance.agent.controllerType != "Keyboard" && !__instance.mainGUI.showingInterface)
			{
				if (__instance.mainGUI.invInterface.thrownItem != null && __instance.gc.playerControl.keyCheck(buttonType.Interact, __instance.agent))
				{
					__instance.ThrowItem();
				}
				if (__instance.agent.target != null && !__instance.agent.mainGUI.showingInterface && __instance.gc.playerControl.keyCheck(buttonType.Interact, __instance.agent) && !__instance.agent.mainGUI.justClosedInventory && !__instance.agent.worldSpaceGUI.justClosedObjectButtons)
				{
					__instance.TargetObject();
					__instance.mainGUI.invInterface.TargetAnywhere(__instance.agent.target.tr.position, true);
				}
			}
			if (__instance.timeSinceLastExtendedFire < 5f)
			{
				__instance.timeSinceLastExtendedFire += Time.deltaTime;
			}
			if (__instance.timeSinceLastBulletSpawn < 1f)
			{
				__instance.timeSinceLastBulletSpawn += Time.deltaTime;
			}
			if (__instance.fleeingAndCanFire > 0f)
			{
				__instance.fleeingAndCanFire -= Time.deltaTime;
			}

			return false;
		}
	}
}
