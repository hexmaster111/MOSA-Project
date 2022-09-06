using System;
using Mosa.Kernel.x86;

namespace DroneOsBoot;

public enum LogOutput
{
	Last,
	Com1,
	Com2,
	Com3,
	Com4,
	Console
}

public class LoggingSystem
{
	private static LogOutput lastOutput = LogOutput.Console;

	public static void Log(string senderClass, string senderMethod, string message,
		LogOutput logOutput = LogOutput.Last)
	{
		var sender = "[" + senderClass + "." + senderMethod + "]";

		if (logOutput != LogOutput.Last)
			lastOutput = logOutput;

		Log(sender, message);
	}

	public static void Log(string sender, string message)
	{
		Log(lastOutput, sender, message);
	}

	public static void Log(LogOutput logOutput, string sender, string message)
	{
		string logMessage = "[" + sender + "]" + " " + message;

		lastOutput = logOutput;

		switch (logOutput)
		{
			case LogOutput.Com1:
				Serial.Write(Serial.COM1, logMessage);
				break;

			case LogOutput.Com2:
				Serial.Write(Serial.COM2, logMessage);
				break;

			case LogOutput.Com3:
				Serial.Write(Serial.COM3, logMessage);
				break;

			case LogOutput.Com4:
				Serial.Write(Serial.COM4, logMessage);
				break;

			case LogOutput.Console:
				Screen.WriteLine(logMessage);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(logOutput));
		}
	}
}
