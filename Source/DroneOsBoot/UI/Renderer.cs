using System.Drawing;
using DroneOsBoot.HAL;
using DroneOsBoot.Utilities;

namespace DroneOsBoot.UI;

public static class Renderer
{
	public static void Render()
	{

		Display.Clear(Color.Aqua);
		Display.DrawString(10, 10, "Hello World!", Display.DefaultFont, Color.Black);
		Display.DrawString(10, 20, "Mouse X: " + MouseHal.Mouse.X, Display.DefaultFont, Color.Black);
		Display.DrawString(10, 30, "Mouse Y: " + MouseHal.Mouse.Y, Display.DefaultFont, Color.Black);
		Display.DrawString(10, 50, "Fonts Loaded: ", Display.DefaultFont, Color.Black);

		var index = 0;
		foreach (var fontName in FontManager.GetFontNames())
		{
			Display.DrawString(10, 60 + (10 * index), fontName, Display.DefaultFont, Color.Black);
			index++;
		}

		MouseRender.Draw();
		Display.Update();
	}
}
