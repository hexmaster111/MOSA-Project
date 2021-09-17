﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Runtime;

namespace Mosa.DeviceSystem
{
	/// <summary>
	/// Implementation of FrameBuffer with 24 Bits Per Pixel
	/// </summary>
	public sealed class FrameBuffer24bpp : FrameBuffer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FrameBuffer24bpp"/> class.
		/// </summary>
		/// <param name="buffer">The memory.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="depth">The depth.</param>
		public FrameBuffer24bpp(ConstrainedPointer buffer, uint width, uint height, uint offset, uint depth, bool doubleBuffering = true)
		{
			this.buffer = buffer;
			this.width = width;
			this.height = height;
			this.offset = offset;
			this.depth = depth;

			if (doubleBuffering)
				secondBuffer = new ConstrainedPointer(GC.AllocateObject(buffer.Size), buffer.Size);
		}

		/// <summary>
		/// Gets the offset.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns></returns>
		protected override uint GetOffset(uint x, uint y)
		{
			return offset + (y * depth) + (x * 3);
		}

		/// <summary>
		/// Gets the pixel.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns></returns>
		public override uint GetPixel(uint x, uint y)
		{
			return doubleBuffering ? secondBuffer.Read24(GetOffset(x, y)) : buffer.Read24(GetOffset(x, y));
		}

		/// <summary>
		/// Sets the pixel.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		public override void SetPixel(uint color, uint x, uint y)
		{
			if (doubleBuffering)
				secondBuffer.Write24(GetOffset(x, y), (ushort)color);
			else
				buffer.Write24(GetOffset(x, y), (ushort)color);
		}

		/// <summary>
		/// Fills a rectangle with color.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <param name="x">X of the top left of the rectangle.</param>
		/// <param name="y">Y of the top left of the rectangle.</param>
		/// <param name="w">Width of the rectangle.</param>
		/// <param name="h">Width of the rectangle.</param>
		public override void FillRectangle(uint color, uint x, uint y, uint w, uint h)
		{
			uint startAddress = GetOffset(x, y);

			for (uint offsetY = 0; offsetY < h; offsetY++)
			{
				for (uint offsetX = 0; offsetX < w; offsetX++)
				{
					if (doubleBuffering)
						secondBuffer.Write24(startAddress + (offsetX * 3), color);
					else
						buffer.Write24(startAddress + (offsetX * 3), color);
				}

				startAddress += depth;
			}
		}
	}
}
