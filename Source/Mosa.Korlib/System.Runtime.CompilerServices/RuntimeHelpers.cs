﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Collections.Generic;
using System.Reflection;

namespace System.Runtime.CompilerServices
{
	public static class RuntimeHelpers
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void InitializeArray(Array array, RuntimeFieldHandle fldHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetHashCode(object o);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public new static extern bool Equals(object o1, object o2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern T UnsafeCast<T>(object o) where T : class;

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IEnumerable<Assembly> GetAssemblies();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object CreateInstance(Type type, params object[] args);

		// TODO: Implement?
		[Intrinsic]
        internal static bool EnumEquals<T>(T x, T y) where T : Enum
        {
            // The body of this function will be replaced by the EE with unsafe code
            // See getILIntrinsicImplementation for how this happens.
            return x.Equals(y);
        }

		[Intrinsic]
		public static bool IsReferenceOrContainsReferences<T>()
		{
			throw new InvalidOperationException();
		}
	}
}
