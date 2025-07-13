using Buttplug.Client;
using Buttplug.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PeakVibrations
{
	public class DeviceManager
	{
		private static uint semaphore;
		private static double currentIntensity;

		private List<ButtplugClientDevice> ConnectedDevices { get; set; }

		private ButtplugClient ButtplugClient { get; set; }

		public DeviceManager(string clientName)
		{
			ConnectedDevices = new List<ButtplugClientDevice>();
			ButtplugClient = new ButtplugClient(clientName);
			Debug.Log("BP client created for " + clientName);
			ButtplugClient.DeviceAdded += HandleDeviceAdded;
			ButtplugClient.DeviceRemoved += HandleDeviceRemoved;
		}

		public bool IsConnected() => ButtplugClient.Connected;

		public async void ConnectDevices()
		{
			if (ButtplugClient.Connected) { return; }

			try
			{
				Debug.Log($"Attempting to connect to Intiface server at {Config.ServerUri.Value}");
				await ButtplugClient.ConnectAsync(new ButtplugWebsocketConnector(new Uri(Config.ServerUri.Value)));
				Debug.Log("Connection successful. Beginning scan for devices");
				await ButtplugClient.StartScanningAsync();
			}
			catch (ButtplugException exception)
			{
				Debug.LogError($"Attempt to connect to devices failed. Ensure Intiface is running and attempt to reconnect from the 'Devices' section in the mod's in-game settings.");
				Debug.Log($"ButtplugIO error occured while connecting devices: {exception}");
			}
		}

		public void VibrateConnectedDevicesWithDuration(double intensity, float time)
		{
			intensity *= Config.GlobalMultiplier.Value;

			// Skip if already vibrating harder.
			if (intensity < currentIntensity)
			{
				return;
			}

			currentIntensity = intensity;
			semaphore++;
			uint mySemaphore = semaphore;

			async void Action(ButtplugClientDevice device)
			{
				await device.VibrateAsync(Mathf.Clamp((float)intensity, 0f, 1.0f));
				await Task.Delay((int)(time * 1000f));

				// Don't end if something else took over.
				if (mySemaphore == semaphore)
				{
					currentIntensity = 0;
					await device.VibrateAsync(0.0f);
				}
			}

			ConnectedDevices.ForEach(Action);
		}

		/// <summary>
		///  This has to be manually stopped
		/// </summary>
		public void VibrateConnectedDevices(double intensity)
		{
			intensity += Config.GlobalMultiplier.Value;

			async void Action(ButtplugClientDevice device)
			{
				await device.VibrateAsync(Mathf.Clamp((float)intensity, 0f, 1.0f));
			}

			ConnectedDevices.ForEach(Action);
		}

		public void StopConnectedDevices()
		{
			ConnectedDevices.ForEach(async (ButtplugClientDevice device) => await device.Stop());
		}

		internal void CleanUp()
		{
			StopConnectedDevices();
		}

		private void HandleDeviceAdded(object sender, DeviceAddedEventArgs args)
		{
			if (!IsVibratableDevice(args.Device))
			{
				Debug.Log($"{args.Device.Name} was detected but ignored due to it not being vibratable.");
				return;
			}

			Debug.Log($"{args.Device.Name} connected to client {ButtplugClient.Name}");
			ConnectedDevices.Add(args.Device);
		}

		private void HandleDeviceRemoved(object sender, DeviceRemovedEventArgs args)
		{
			if (!IsVibratableDevice(args.Device)) { return; }

			Debug.Log($"{args.Device.Name} disconnected from client {ButtplugClient.Name}");
			ConnectedDevices.Remove(args.Device);
		}


		private bool IsVibratableDevice(ButtplugClientDevice device)
		{
			return device.VibrateAttributes.Count > 0;
		}
	}
}
