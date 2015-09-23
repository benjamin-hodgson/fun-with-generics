using System;
using System.Collections.Generic;
using System.Linq;

namespace Vec
{
	class Example
	{
		public static void Main()
		{
			Vec<int, S<S<S<Z>>>> vec1 = new Cons<S<S<Z>>, int>(1, new Cons<S<Z>, int>(2, new Cons<Z, int>(3, new Nil<int>())));
			vec1.First();
//			var vec2 = new Nil<int>();
//			vec2.First();
		}
	}


	abstract class Vec<T, N> {}
	class Nil<T> : Vec<T, Z> {}
	class Cons<N, T> : Vec<T, S<N>>
	{
		public T Head { get; private set; }
		public Vec<T, N> Tail { get; private set; }

		public Cons(T head, Vec<T, N> tail)
		{
			this.Head = head;
			this.Tail = tail;
		}
	}

	static class VecExt
	{
		public static T First<T, N>(this Vec<T, S<N>> vec)
		{
			return ((Cons<N, T>)vec).Head;
		}
	}

	sealed class Z {}
	sealed class S<N> {}
}

