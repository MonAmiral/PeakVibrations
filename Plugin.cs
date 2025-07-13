using UnityEngine;
using BepInEx;
using HarmonyLib;

namespace PeakVibrations
{
	[BepInPlugin("monamiral.peakvibrations", "Peak Vibrations", "1.0.0")]
	public partial class Plugin : BaseUnityPlugin
	{
		internal static DeviceManager DeviceManager { get; private set; }
		private static float OutOfStaminaTime;

		public class Patcher
		{
			[HarmonyPatch(typeof(CharacterAfflictions), "AddStatus")]
			[HarmonyPostfix]
			public static void CharacterAfflictionsAddStatusPostfix(CharacterAfflictions __instance, CharacterAfflictions.STATUSTYPE statusType, float amount)
			{
				if (!__instance.character.IsLocal)
				{
					return;
				}

				float statusValue = __instance.currentStatuses[(int)statusType];
				float duration = PeakVibrations.Config.StaminaDamageBaseDuration.Value + PeakVibrations.Config.StaminaDamageDurationMultiplier.Value * statusValue;

				switch (statusType)
				{
					case CharacterAfflictions.STATUSTYPE.Injury:
						statusValue *= PeakVibrations.Config.InjuryMultiplier.Value;
						break;

					case CharacterAfflictions.STATUSTYPE.Hunger:
						statusValue *= PeakVibrations.Config.HungerMultiplier.Value;
						break;

					case CharacterAfflictions.STATUSTYPE.Cold:
						statusValue *= PeakVibrations.Config.ColdMultiplier.Value;
						break;

					case CharacterAfflictions.STATUSTYPE.Poison:
						statusValue *= PeakVibrations.Config.PoisonMultiplier.Value;
						break;

					case CharacterAfflictions.STATUSTYPE.Drowsy:
						statusValue *= PeakVibrations.Config.DrowsyMultiplier.Value;
						break;

					case CharacterAfflictions.STATUSTYPE.Hot:
						statusValue *= PeakVibrations.Config.HotMultiplier.Value;
						break;
				}

				if (statusValue > 0)
				{
					DeviceManager.VibrateConnectedDevicesWithDuration(statusValue, duration);
				}
			}

			[HarmonyPatch(typeof(Character), "Update")]
			[HarmonyPrefix]
			public static void CharacterUpdatePreFix(Character __instance)
			{
				if (!__instance.IsLocal
				 || !__instance.data.fullyConscious
				 || __instance.data.passedOutOnTheBeach > 0
				 || Time.timeSinceLevelLoad < 3)
				{
					return;
				}

				// Vibrate when below 5% stamina (excluding extra stamina).
				if (__instance.data.currentStamina < 0.05f && __instance.data.staminaDelta <= 0)
				{
					OutOfStaminaTime += Time.deltaTime * PeakVibrations.Config.OutOfStaminaIncrease.Value;
					DeviceManager.VibrateConnectedDevicesWithDuration(OutOfStaminaTime * PeakVibrations.Config.OutOfStaminaMultiplier.Value, 0.1f);
				}
				else if (__instance.data.staminaDelta > 0)
				{
					OutOfStaminaTime = 0;
				}
			}
		}

		public void Awake()
		{
			Debug.Log("[MonAmiral] Connecting devices.");

			DeviceManager = new DeviceManager("PeakVibrations");
			DeviceManager.ConnectDevices();

			new Harmony("monamiral.peakvibrations").PatchAll(typeof(Patcher));
		}
	}
}