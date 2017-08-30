﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common;
using Mosa.Compiler.Framework.IR;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mosa.Compiler.Framework.Stages
{
	/// <summary>
	/// Places phi instructions for the SSA transformation
	/// </summary>
	/// <seealso cref="Mosa.Compiler.Framework.BaseMethodCompilerStage" />
	public class PhiPlacementStage : BaseMethodCompilerStage
	{
		/// <summary>
		/// Gets the assignments.
		/// </summary>
		public Dictionary<Operand, List<BasicBlock>> Assignments { get; } = new Dictionary<Operand, List<BasicBlock>>();

		protected override void Run()
		{
			// Method is empty - must be a plugged method
			if (BasicBlocks.HeadBlocks.Count == 0)
				return;

			if (HasProtectedRegions)
				return;

			CollectAssignments();

			PlacePhiFunctionsMinimal();
		}

		protected override void Finish()
		{
			UpdateCounter("PhiPlacement.IRInstructions", instructionCount);
		}

		/// <summary>
		/// Collects the assignments.
		/// </summary>
		private void CollectAssignments()
		{
			foreach (var block in BasicBlocks)
			{
				for (var context = new Context(block); !context.IsBlockEndInstruction; context.GotoNext())
				{
					if (context.IsEmpty)
						continue;

					instructionCount++;

					if (context.Result == null)
						continue;

					if (context.Result.IsVirtualRegister)
					{
						AddToAssignments(context.Result, block);
					}
				}
			}
		}

		/// <summary>
		/// Adds to assignments.
		/// </summary>
		/// <param name="operand">The operand.</param>
		/// <param name="block">The block.</param>
		private void AddToAssignments(Operand operand, BasicBlock block)
		{
			if (!Assignments.TryGetValue(operand, out List<BasicBlock> blocks))
			{
				blocks = new List<BasicBlock>();
				Assignments.Add(operand, blocks);
			}

			blocks.AddIfNew(block);
		}

		/// <summary>
		/// Inserts the phi instruction.
		/// </summary>
		/// <param name="block">The block.</param>
		/// <param name="variable">The variable.</param>
		private void InsertPhiInstruction(BasicBlock block, Operand variable)
		{
			var context = new Context(block);
			context.AppendInstruction(IRInstruction.Phi, variable);

			//var sourceBlocks = new BasicBlock[block.PreviousBlocks.Count];
			var sourceBlocks = new List<BasicBlock>(block.PreviousBlocks.Count);
			context.PhiBlocks = sourceBlocks;

			for (var i = 0; i < block.PreviousBlocks.Count; i++)
			{
				context.SetOperand(i, variable);
				sourceBlocks.Add(block.PreviousBlocks[i]);
			}

			context.OperandCount = block.PreviousBlocks.Count;

			Debug.Assert(context.OperandCount == context.Block.PreviousBlocks.Count);
		}

		/// <summary>
		/// Places the phi functions minimal.
		/// </summary>
		private void PlacePhiFunctionsMinimal()
		{
			foreach (var headBlock in BasicBlocks.HeadBlocks)
			{
				PlacePhiFunctionsMinimal(headBlock);
			}
		}

		/// <summary>
		/// Places the phi functions minimal.
		/// </summary>
		/// <param name="headBlock">The head block.</param>
		private void PlacePhiFunctionsMinimal(BasicBlock headBlock)
		{
			var analysis = MethodCompiler.DominanceAnalysis.GetDominanceAnalysis(headBlock);

			foreach (var t in Assignments)
			{
				var blocks = t.Value;

				if (blocks.Count < 2)
					continue;

				blocks.AddIfNew(headBlock);

				var idf = analysis.IteratedDominanceFrontier(blocks);

				foreach (var n in idf)
				{
					InsertPhiInstruction(n, t.Key);
				}
			}
		}
	}
}
