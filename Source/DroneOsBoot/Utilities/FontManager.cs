using System.Collections.Generic;
using Mosa.DeviceSystem;

namespace DroneOsBoot.Utilities;

public static class FontManager
{
	private static List<ISimpleFont> Fonts;

	public static int Count => Fonts.Count;

	public static void Initialize()
	{
		Fonts = new List<ISimpleFont>();
	}

	public static string[] GetFontNames()
	{
		string[] names = new string[Fonts.Count];
		for (int i = 0; i < Fonts.Count; i++)
		{
			names[i] = Fonts[i].Name;
		}
		return names;
	}

	public static void AddFont(ISimpleFont font)
	{
		Fonts.Add(font);
	}

	public static ISimpleFont GetFont(string name)
	{
		foreach (ISimpleFont font in Fonts)
		{
			if (font.Name == name)
				return font;
		}
		return null;
	}

	public static bool AddFont(byte[] fontData)
	{
		if (fontData == null)
		{
			LoggingSystem.Log("FontManager", "AddFont", "fontData is null, not loading");
			return false;
		}
		var font = Load(fontData);
		LoggingSystem.Log("FontManager", "Loaded font: " + font.Name);
		Fonts.Add(font);
		return true;
	}

	public static SimpleBitFont Load(byte[] data)
	{
		var stream = new DataStream(data);

		var name = string.Empty;
		var charset = string.Empty;

		// Name
		for (;;)
		{
			var ch = stream.ReadByte();
			if (ch == byte.MaxValue)
				break;

			name += (char)ch;
		}

		// Charset
		for (;;)
		{
			var ch = stream.ReadByte();
			if (ch == byte.MaxValue)
				break;

			charset += (char)ch;
		}

		var width = stream.ReadByte();
		var height = stream.ReadByte();
		var size = stream.ReadByte();

		_ = stream.ReadByte(); // Skip end byte

		var fontData = stream.ReadEnd();

		return new SimpleBitFont(name, width, height, size, charset, fontData);
	}

}
