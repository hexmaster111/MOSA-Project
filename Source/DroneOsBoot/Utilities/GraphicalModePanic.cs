using System;
using Mosa.DeviceSystem;
using Mosa.Kernel.x86;
using Mosa.Runtime.x86;

namespace DroneOsBoot.Utilities;

public static class GraphicalModePanic
{
	public static void Panic(string panicMessage)
	{
		DrawPanicMessage(null, panicMessage);
	}

	public static void Panic(Exception exception, string thrower)
	{
		string exceptionMessage = "Exception: " + exception.Message;

		string innerExceptionMessage;
		if (exception.InnerException != null)
			innerExceptionMessage = "Inner Exception: " + exception.InnerException.Message;
		else
			innerExceptionMessage = "No Inner Exception";


		DrawPanicMessage(thrower, exceptionMessage, innerExceptionMessage);
	}


	private static void DrawPanicMessage(string panicThrower, string panicMessage, string panicMessageLower = null)
	{
		DroneOsBoot.Display.Driver.Disable();
		IDT.SetInterruptHandler(null);

		Screen.BackgroundColor = ScreenColor.Blue;

		Screen.Clear();
		Screen.Goto(1, 0);
		Screen.Color = ScreenColor.White;

		if (panicThrower != null)
			Screen.WriteLine("*** " + panicThrower + " Panic ***");
		else
			Screen.WriteLine("*** Panic ***");

		if(panicMessage != null)
			Screen.WriteLine(panicMessage);

		if (panicMessageLower != null)
			Screen.WriteLine(panicMessageLower);


		while (true)
		{
		}
	}
}
