using System.Drawing;
using DroneOsBoot.HAL;

namespace DroneOsBoot.UI;

public static class MouseRender
{
	public static Color MouseColor;

	public static void Initialize()
	{
		MouseColor = Color.White;
	}

	public static void Draw()
	{
		Display.DrawRectangle(MouseHal.Mouse.X, MouseHal.Mouse.Y, 10, 10, MouseColor, true);
	}

}
