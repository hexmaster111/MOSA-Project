﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Utility.DebugEngine
{
	public delegate void SenderMesseageDelegate(DebugMessage response);

	public class DebugMessage
	{
		public int ID { get; internal set; }

		public int Code { get; private set; }

		public byte[] CommandData { get; private set; }

		public byte[] ResponseData { get; internal set; }

		public object Other { get; private set; }

		public object Sender { get; protected set; }

		public SenderMesseageDelegate SenderMethod { get; protected set; }

		public int Checksum { get { return 0; } } // TODO

		public DebugMessage(int code, byte[] data)
		{
			Code = code;
			CommandData = data;
		}

		public DebugMessage(int code, int[] data)
		{
			Code = code;
			CommandData = new byte[data.Length * 4];

			int index = 0;
			foreach (int i in data)
			{
				CommandData[index++] = (byte)(i & 0xFF);
				CommandData[index++] = (byte)((i >> 8) & 0xFF);
				CommandData[index++] = (byte)((i >> 16) & 0xFF);
				CommandData[index++] = (byte)((i >> 24) & 0xFF);
			}
		}

		public DebugMessage(int code, uint[] data)
		{
			Code = code;
			CommandData = new byte[data.Length * 4];

			int index = 0;
			foreach (int i in data)
			{
				CommandData[index++] = (byte)(i & 0xFF);
				CommandData[index++] = (byte)((i >> 8) & 0xFF);
				CommandData[index++] = (byte)((i >> 16) & 0xFF);
				CommandData[index++] = (byte)((i >> 24) & 0xFF);
			}
		}

		public DebugMessage(int code, byte[] data, object sender, SenderMesseageDelegate senderMethod)
			: this(code, data)
		{
			Sender = sender;
			SenderMethod = senderMethod;
		}

		public DebugMessage(int code, int[] data, object sender, SenderMesseageDelegate senderMethod)
			: this(code, data)
		{
			Sender = sender;
			SenderMethod = senderMethod;
		}

		public DebugMessage(int code, uint[] data, object sender, SenderMesseageDelegate senderMethod)
		: this(code, data)
		{
			Sender = sender;
			SenderMethod = senderMethod;
		}

		public int GetInt32(int index)
		{
			return (ResponseData[index + 3] << 24) | (ResponseData[index + 2] << 16) | (ResponseData[index + 1] << 8) | ResponseData[index];
		}

		public uint GetUInt32(int index)
		{
			return (uint)GetInt32(index);
		}
	}
}
