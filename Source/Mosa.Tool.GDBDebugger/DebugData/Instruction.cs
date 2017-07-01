﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Tool.GDBDebugger.DebugData
{
	public class Instruction
	{
		public ulong Address { get; set; }
		public uint Size { get; set; }
		public string ASM { get; set; }
		public string Compiler { get; set; }
	}
}
