// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System;
using System.Collections.Generic;
using DroneOsBoot.HAL;
using DroneOsBoot.UI;
using DroneOsBoot.Utilities;
using Mosa.Demo.SVGAWorld.x86;
using Mosa.Demo.SVGAWorld.x86.HAL;
using Mosa.DeviceSystem;
using Mosa.DeviceSystem.Service;
using Mosa.DeviceDriver;
using Mosa.DeviceDriver.ISA;
using Mosa.DeviceDriver.ScanCodeMap;
using Mosa.FileSystem.FAT;
using Mosa.Kernel.x86;
using Mosa.Kernel.x86.Helpers;
using Mosa.Runtime;
using Mosa.Runtime.Plug;
using Keyboard = Mosa.DeviceSystem.Keyboard;
using DeviceDriverSetup = Mosa.DeviceDriver.Setup;
using DeviceSystemSetup = Mosa.DeviceSystem.Setup;
using FileManager = DroneOsBoot.Utilities.FileManager;


namespace DroneOsBoot
{
	public static class Boot
	{
		public static DeviceService DeviceService;
		public static Keyboard Keyboard;
		public static Hardware HAL;
		public static PCService PCService;

		[Plug("Mosa.Runtime.StartUp::SetInitialMemory")]
		public static void SetInitialMemory()
		{
			KernelMemory.SetInitialMemory(Address.GCInitialMemory, 0x01000000);
		}

		public static void Main()
		{
			Mosa.Kernel.x86.Kernel.Setup();

			Serial.SetupPort(Serial.COM1);
			IDT.SetInterruptHandler(ProcessInterrupt);

			Screen.Clear();
			Screen.Goto(0, 0);
			Screen.Color = ScreenColor.White;

			LoggingSystem.Log(LogOutput.Console, "BOOT", "SERIAL STARTED OK");

			try
			{
				HAL = new Hardware();
				DeviceService = new DeviceService();

				var serviceManager = new ServiceManager();
				var partitionService = new PartitionService();

				serviceManager.AddService(DeviceService);
				serviceManager.AddService(new DiskDeviceService());
				serviceManager.AddService(partitionService);
				serviceManager.AddService(new PCService());
				serviceManager.AddService(new PCIControllerService());
				serviceManager.AddService(new PCIDeviceService());

				//DeviceSystem.Setup(); is the call this is doing...
				DeviceSystemSetup.Initialize(HAL, DeviceService.ProcessInterrupt);

				DeviceService.RegisterDeviceDriver(DeviceDriverSetup.GetDeviceDriverRegistryEntries());
				DeviceService.Initialize(new X86System(), null);

				PCService = serviceManager.GetFirstService<PCService>();

				partitionService.CreatePartitionDevices();

				foreach (var partition in DeviceService.GetDevices<IPartitionDevice>())
				{
					LoggingSystem.Log("PARTITION DISCOVERY", "Found partition: " + partition.Name);
					FileManager.Register(new FatFileSystem(partition.DeviceDriver as IPartitionDevice));
				}

				//Start the Keyboard and Mouse graphical stuffs
				var keyboard = DeviceService.GetFirstDevice<StandardKeyboard>().DeviceDriver as StandardKeyboard;

				if (keyboard == null) HAL.Abort("Could not find Keyboard");

				Keyboard = new Keyboard(keyboard, new US());

				//Start the Mouse
				var mouse = DeviceService.GetFirstDevice<StandardMouse>().DeviceDriver as StandardMouse;
				if (mouse == null) HAL.Abort("Could not find Mouse");


				MouseHal.Mouse = mouse;

				LoggingSystem.Log("BOOT", "Starting Graphical System");


				FontManager.Initialize();
				FontManager.AddFont(FileManager.ReadAllBytes("font2.bin"));


				//If there is a font, we'll load it and then start the Graphical system
				if (FontManager.AddFont(FileManager.ReadAllBytes("font.bin")))
				{
					var font = FontManager.GetFont("Microsoft Sans Serif");

					if (font == null)
					{
						HAL.Abort("Could not find Default System Font");
						return;
					}

					Display.DefaultFont = font;
					if (!Display.Initialize())
						LoggingSystem.Log("BOOT", "Display failed to initialize");
				}
			}
			catch (Exception e)
			{
				LoggingSystem.Log("BOOT", "Display failed to initialize");
				LoggingSystem.Log("BOOT", e.Message);
			}

			//From this point on there is not really any crash recovery, so we'll just try to boot the OS


			MouseRender.Initialize();
			MouseHal.Mouse.SetScreenResolution(Display.Width, Display.Height);

			try
			{
				while (true)
				{
					Renderer.Render();
				}
			}
			catch (Exception e)
			{
				GraphicalModePanic.Panic(e, "Graphical Mode Tests");
			}
		}

		public static void ProcessInterrupt(uint interrupt, uint errorCode)
		{
			if (interrupt >= 0x20 && interrupt < 0x30)
				HAL.ProcessInterrupt((byte)(interrupt - 0x20));
		}
	}
}
