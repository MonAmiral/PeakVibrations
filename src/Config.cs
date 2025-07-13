using BepInEx;
using BepInEx.Configuration;

namespace PeakVibrations
{
	internal class Config
	{
		private static ConfigFile ConfigFile { get; set; }

		internal static ConfigEntry<string> ServerUri { get; set; }

		internal static ConfigEntry<float> GlobalMultiplier { get; set; }

		internal static ConfigEntry<float> InjuryMultiplier { get; set; }
		internal static ConfigEntry<float> HungerMultiplier { get; set; }
		internal static ConfigEntry<float> ColdMultiplier { get; set; }
		internal static ConfigEntry<float> PoisonMultiplier { get; set; }
		internal static ConfigEntry<float> DrowsyMultiplier { get; set; }
		internal static ConfigEntry<float> HotMultiplier { get; set; }

		internal static ConfigEntry<float> StaminaDamageBaseDuration { get; set; }
		internal static ConfigEntry<float> StaminaDamageDurationMultiplier { get; set; }

		internal static ConfigEntry<float> OutOfStaminaMultiplier { get; set; }
		internal static ConfigEntry<float> OutOfStaminaIncrease { get; set; }

		static Config()
		{
			ConfigFile = new ConfigFile(Paths.ConfigPath + "\\PeakVibrations.cfg", true);

			ServerUri = ConfigFile.Bind(
				"Devices",
				"Server Uri",
				"ws://localhost:12345",
				"URI of the Intiface server."
			);

			GlobalMultiplier = ConfigFile.Bind("Global", "GlobalMultiplier", 1.0f, "Change the strength of all vibration");

			InjuryMultiplier = ConfigFile.Bind("Stamina Damage", "InjuryMultiplier", 1.0f, "Vibration strength when suffering an injury (multiplied by the new amount of damage in % of maximum vibration)");
			HungerMultiplier = ConfigFile.Bind("Stamina Damage", "HungerMultiplier", 0.0f, "Vibration strength when getting hungry (multiplied by the new amount of damage in % of maximum vibration)");
			ColdMultiplier = ConfigFile.Bind("Stamina Damage", "ColdMultiplier", 1.0f, "Vibration strength when suffering from cold (multiplied by the new amount of damage in % of maximum vibration)");
			PoisonMultiplier = ConfigFile.Bind("Stamina Damage", "PoisonMultiplier", 1.0f, "Vibration strength when suffering from poison (multiplied by the new amount of damage in % of maximum vibration)");
			DrowsyMultiplier = ConfigFile.Bind("Stamina Damage", "DrowsyMultiplier", 1.0f, "Vibration strength when getting drowsy (multiplied by the new amount of damage in % of maximum vibration)");
			HotMultiplier = ConfigFile.Bind("Stamina Damage", "HotMultiplier", 1.0f, "Vibration strength when getting burned (multiplied by the new amount of damage in % of maximum vibration)");
			
			StaminaDamageBaseDuration = ConfigFile.Bind("Stamina Damage", "StaminaDamageBaseDuration", 1.0f, "Base duration in seconds of Stamina damage vibrations");
			StaminaDamageDurationMultiplier = ConfigFile.Bind("Stamina Damage", "StaminaDamageDurationMultiplier", 4.0f, "Additional duration in seconds of Stamina damage vibrations multiplied by the new amount of damage");

			OutOfStaminaMultiplier = ConfigFile.Bind("Out Of Stamina", "OutOfStaminaMultiplier", 1.0f, "Vibration strength when out of stamina");
			OutOfStaminaIncrease = ConfigFile.Bind("Out Of Stamina", "OutOfStaminaIncrease", 0.2f, "How fast vibration increases when out of stamina");
		}
	}
}