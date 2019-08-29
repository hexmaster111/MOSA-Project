// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

namespace Mosa.Compiler.Framework.IR
{
	/// <summary>
	/// LoadParamFloatR8
	/// </summary>
	/// <seealso cref="Mosa.Compiler.Framework.IR.BaseIRInstruction" />
	public sealed class LoadParamFloatR8 : BaseIRInstruction
	{
		public LoadParamFloatR8()
			: base(1, 1)
		{
		}

		public override bool IsMemoryRead { get { return true; } }

		public override bool IsParameterLoad { get { return true; } }
	}
}
