// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x64.Instructions
{
	/// <summary>
	/// CMov64
	/// </summary>
	/// <seealso cref="Mosa.Platform.x64.X64Instruction" />
	public sealed class CMov64 : X64Instruction
	{
		internal CMov64()
			: base(1, 1)
		{
		}

		public override string AlternativeName { get { return "CMov"; } }

		public override bool IsZeroFlagUsed { get { return true; } }

		public override bool IsCarryFlagUsed { get { return true; } }

		public override bool IsSignFlagUsed { get { return true; } }

		public override bool IsOverflowFlagUsed { get { return true; } }

		public override bool IsParityFlagUsed { get { return true; } }

		public override bool AreFlagUseConditional { get { return true; } }

		public override void Emit(InstructionNode node, BaseCodeEmitter emitter)
		{
			System.Diagnostics.Debug.Assert(node.ResultCount == 1);
			System.Diagnostics.Debug.Assert(node.OperandCount == 1);

			emitter.OpcodeEncoder.Append8Bits(0x0F);
			emitter.OpcodeEncoder.SuppressByte(0x40);
			emitter.OpcodeEncoder.Append4Bits(0b0100);
			emitter.OpcodeEncoder.Append1Bit(0b1);
			emitter.OpcodeEncoder.Append1Bit((node.Result.Register.RegisterCode >> 3) & 0x1);
			emitter.OpcodeEncoder.Append1Bit(0b0);
			emitter.OpcodeEncoder.Append1Bit((node.Operand1.Register.RegisterCode >> 3) & 0x1);
			emitter.OpcodeEncoder.Append4Bits(0b0100);
			emitter.OpcodeEncoder.Append4Bits(GetConditionCode(node.ConditionCode));
			emitter.OpcodeEncoder.Append2Bits(0b11);
			emitter.OpcodeEncoder.Append3Bits(node.Result.Register.RegisterCode);
			emitter.OpcodeEncoder.Append3Bits(node.Operand1.Register.RegisterCode);
		}
	}
}
