﻿using Mosa.Kernel.x86.Helpers;
using Mosa.Runtime.x86;

namespace Mosa.Kernel.x86
{
	public static class ThreadScheduler
	{
		public const int MaxThreads = 256;

		private static Thread[] threads = new Thread[MaxThreads];

		public static void Setup()
		{
			for (int i = 0; i < MaxThreads; i++)
			{
				threads[i] = new Thread();
			}

			threads[0].Status = ThreadStatus.Waiting; // special case since thread = 0 is the interrupt
		}

		public static void SchedulerInterrupt(uint stackSate)
		{
			// Save current stack state
			var threadID = GetCurrentThreadID();
			SaveThreadState(threadID, stackSate);

			// TODO:
			// 1. find next thread to execute

			SwitchToThread(threadID);
		}

		public static uint CreateThread(uint ip, uint stackSize)
		{
			Assert.True(stackSize != 0, "CreateThread(): invalid stack size = 0");
			Assert.True(stackSize % 4096 == 0, "CreateThread() invalid stack size % 4096 != 0");
			Assert.True(stackSize != 0, "CreateThread(): invalid thread id = 0");

			uint threadID = FindEmptyThreadSlot();

			if (threadID == 0)
			{
				ResetTerminatedThreads();
				threadID = FindEmptyThreadSlot();
			}

			Assert.True(stackSize != 0, "CreateThread(): invalid thread id = 0");

			Thread thread = threads[threadID];

			var stack = VirtualPageAllocator.Reserve(stackSize);
			uint stackStateSize = 52;
			var stackTop = stack + stackSize;

			// Setup stack state
			Native.Set32(stackTop - 4, 0);          // Zero Sentinel
			Native.Set32(stackTop - 8, 0);          // Zero Sentinel
			Native.Set32(stackTop - 4 - 8, 0);      // EFLAG
			Native.Set32(stackTop - 12 - 8, ip);	// EIP
			Native.Set32(stackTop - 16 - 8, 0);		// ErrorCode - not used
			Native.Set32(stackTop - 20 - 8, 0);		// Interrupt Number - not used
			Native.Set32(stackTop - 24 - 8, 0);     // EAX
			Native.Set32(stackTop - 28 - 8, 0);     // ECX
			Native.Set32(stackTop - 32 - 8, 0);     // EDX
			Native.Set32(stackTop - 36 - 8, 0);     // EBX
			Native.Set32(stackTop - 40 - 8, stackTop - 8); // ESP (original) - not used
			Native.Set32(stackTop - 44 - 8, stackTop - 8); // EBP
			Native.Set32(stackTop - 48 - 8, 0);     // ESI
			Native.Set32(stackTop - 52 - 8, 0);     // EDI

			thread.Status = ThreadStatus.Running;
			thread.StackBottom = stack;
			thread.StackTop = stackTop;
			thread.StackStatePointer = stackTop - stackStateSize;

			return threadID;
		}

		private static void SaveThreadState(uint threadID, uint stackSate)
		{
			Assert.True(threadID < MaxThreads, "SaveThreadState(): invalid thread id > max");

			var thread = threads[threadID];

			Assert.True(thread != null, "SaveThreadState(): thread id = null");

			thread.StackStatePointer = stackSate;
		}

		private static uint GetCurrentThreadID()
		{
			return Native.GetFS();
		}

		private static void SetThreadID(uint threadID)
		{
			Native.SetFS(threadID);
		}

		private static void SwitchToThread(uint threadID)
		{
			var thread = threads[threadID];

			Assert.True(thread != null, "invalid thread id");

			uint stackState = thread.StackStatePointer;

			SetThreadID(threadID);
			Native.InterruptReturn(stackState);
		}

		private static uint FindEmptyThreadSlot()
		{
			for (uint i = 0; i < MaxThreads; i++)
			{
				if (threads[i].Status == ThreadStatus.Empty)
					return i;
			}

			return 0;
		}

		private static void ResetTerminatedThreads()
		{
			for (uint i = 0; i < MaxThreads; i++)
			{
				if (threads[i].Status == ThreadStatus.Terminated)
				{
					threads[i].Status = ThreadStatus.Empty;
				}
			}
		}
	}
}
