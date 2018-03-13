﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Intrinsic
{
	/// <summary>
	/// FrameIRet
	/// </summary>
	internal class InterruptReturn : IIntrinsicPlatformMethod
	{
		void IIntrinsicPlatformMethod.ReplaceIntrinsicCall(Context context, MethodCompiler methodCompiler)
		{
			Operand v0 = context.Operand1;

			Operand esp = Operand.CreateCPURegister(methodCompiler.TypeSystem.BuiltIn.I4, GeneralPurposeRegister.ESP);

			context.SetInstruction(X86.Mov32, esp, v0);
			context.AppendInstruction(X86.Popad);
			context.AppendInstruction(X86.AddConst32, esp, esp, Operand.CreateConstant(8, methodCompiler.TypeSystem));
			context.AppendInstruction(X86.Sti);
			context.AppendInstruction(X86.IRetd);

			// future - common code (refactor opportunity)
			context.GotoNext();

			// Remove all remaining instructions in block and clear next block list
			while (!context.IsBlockEndInstruction)
			{
				if (!context.IsEmpty)
				{
					context.SetInstruction(X86.Nop);
				}
				context.GotoNext();
			}

			var nextBlocks = context.Block.NextBlocks;

			foreach (var next in nextBlocks)
			{
				next.PreviousBlocks.Remove(context.Block);
			}

			nextBlocks.Clear();
		}
	}
}
