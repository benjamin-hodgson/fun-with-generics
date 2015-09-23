using System;

namespace Specifications
{
	class Example
	{
		static ISpecification<BlogContext> userCanCommentOnPost =
			new AndSpecification<BlogContext>(
				new PostIsPublic(),
				new OrSpecification<BlogContext>(
					new UserIsRegistered(),
					new PostAllowsAnonymousComments()
				)
			);

		public static void Main()
		{
			var ctx = new BlogContext
			{
				CurrentUser = new User(),
				Post = new BlogPost()
			};

			if (!userCanCommentOnPost.IsSatisfiedBy(ctx))
			{
				throw new Exception("I can't let you do that, Dave");
			}
		}

		class PostIsPublic : LeafSpecification<BlogContext>
		{
			public override bool IsSatisfiedBy(BlogContext ctx)
			{
				return ctx.Post.IsPublic;
			}
		}

		class UserIsRegistered : LeafSpecification<BlogContext>
		{
			public override bool IsSatisfiedBy(BlogContext ctx)
			{
				return !ctx.CurrentUser.IsAnonymous;
			}
		}

		class PostAllowsAnonymousComments : LeafSpecification<BlogContext>
		{
			public override bool IsSatisfiedBy(BlogContext ctx)
			{
				return ctx.Post.CanCommentAnonymously;
			}
		}

		class BlogContext
		{
			public User CurrentUser { get; set; }
			public BlogPost Post { get; set; }
		}
		class User
		{
			public bool IsAnonymous { get; set; }
		}
		class BlogPost
		{
			public bool IsPublic { get; set; }
			public bool CanCommentAnonymously { get; set; }
		}
	}


	interface ISpecification<in TCandidate>
	{
		TReturn Accept<TReturn>(ISpecificationVisitor<TCandidate, TReturn> visitor);
	    void Accept(ISpecificationVisitor<TCandidate> visitor);
	}
	interface ISpecificationVisitor<out TCandidate, out TReturn>
	{
		TReturn Visit(ILeafSpecification<TCandidate> spec);
		TReturn Visit(IAndSpecification<TCandidate> spec);
		TReturn Visit(IOrSpecification<TCandidate> spec);
		TReturn Visit(INotSpecification<TCandidate> spec);
	}

	// two ways to make a visitor which returns void, both unpleasant
	interface ISpecificationVisitor<out TCandidate>
	{
		void Visit(ILeafSpecification<TCandidate> spec);
		void Visit(IAndSpecification<TCandidate> spec);
		void Visit(IOrSpecification<TCandidate> spec);
		void Visit(INotSpecification<TCandidate> spec);
	}
	class Unit
	{
		private static Unit _default = new Unit();
		public static Unit Default { get { return _default; } }
		private Unit() {}
	}


	// even worse - duplicating interfaces to cajole the compiler into allowing contravariance
	interface ILeafSpecification<in TCandidate> : ISpecification<TCandidate>
	{
		bool IsSatisfiedBy(TCandidate candidate);
	}
	interface IAndSpecification<in TCandidate> : ISpecification<TCandidate>
	{
		ISpecification<TCandidate> Left { get;  }
		ISpecification<TCandidate> Right { get; }

	}
	interface IOrSpecification<in TCandidate> : ISpecification<TCandidate>
	{
		ISpecification<TCandidate> Left { get;  }
		ISpecification<TCandidate> Right { get; }

	}
	interface INotSpecification<in TCandidate> : ISpecification<TCandidate>
	{
		ISpecification<TCandidate> Spec { get;  }

	}
	abstract class LeafSpecification<TCandidate> : ILeafSpecification<TCandidate>
	{
		public abstract bool IsSatisfiedBy(TCandidate candidate);

		public TReturn Accept<TReturn>(ISpecificationVisitor<TCandidate, TReturn> visitor)
		{
			return visitor.Visit(this);
		}
		public void Accept(ISpecificationVisitor<TCandidate> visitor)
		{
			visitor.Visit(this);
		}
	}
	class AndSpecification<TCandidate> : IAndSpecification<TCandidate>
	{
		public ISpecification<TCandidate> Left { get; private set; }
		public ISpecification<TCandidate> Right { get; private set; }

		public AndSpecification(ISpecification<TCandidate> left, ISpecification<TCandidate> right)
		{
			this.Left = left;
			this.Right = right;
		}

		public TReturn Accept<TReturn>(ISpecificationVisitor<TCandidate, TReturn> visitor)
		{
			return visitor.Visit(this);
		}
		public void Accept(ISpecificationVisitor<TCandidate> visitor)
		{
			visitor.Visit(this);
		}
	}
	class OrSpecification<TCandidate> : IOrSpecification<TCandidate>
	{
		public ISpecification<TCandidate> Left { get; private set; }
		public ISpecification<TCandidate> Right { get; private set; }

		public OrSpecification(ISpecification<TCandidate> left, ISpecification<TCandidate> right)
		{
			this.Left = left;
			this.Right = right;
		}

		public TReturn Accept<TReturn>(ISpecificationVisitor<TCandidate, TReturn> visitor)
		{
			return visitor.Visit(this);
		}
		public void Accept(ISpecificationVisitor<TCandidate> visitor)
		{
			visitor.Visit(this);
		}
	}
	class NotSpecification<TCandidate> : INotSpecification<TCandidate>
	{
		public ISpecification<TCandidate> Spec { get; private set; }

		public NotSpecification(ISpecification<TCandidate> spec)
		{
			this.Spec = spec;
		}

		public TReturn Accept<TReturn>(ISpecificationVisitor<TCandidate, TReturn> visitor)
		{
			return visitor.Visit(this);
		}
		public void Accept(ISpecificationVisitor<TCandidate> visitor)
		{
			visitor.Visit(this);
		}
	}



	static class SpecificationExtensions
	{
		class IsSatisfiedByVisitor<TCandidate> : ISpecificationVisitor<TCandidate, bool>
		{
			private TCandidate candidate;

			public IsSatisfiedByVisitor(TCandidate candidate)
			{
				this.candidate = candidate;
			}
			public bool Visit(ILeafSpecification<TCandidate> spec)
			{
				return spec.IsSatisfiedBy(this.candidate);
			}
			public bool Visit(IAndSpecification<TCandidate> spec)
			{
				return spec.Left.Accept(this) && spec.Right.Accept(this);
			}
			public bool Visit(IOrSpecification<TCandidate> spec)
			{
				return spec.Left.Accept(this) || spec.Right.Accept(this);
			}
			public bool Visit(INotSpecification<TCandidate> spec)
			{
				return !spec.Spec.Accept(this);
			}
		}

		public static bool IsSatisfiedBy<T>(this ISpecification<T> spec, T candidate)
		{
			var visitor = new IsSatisfiedByVisitor<T>(candidate);
			return spec.Accept(visitor);
		}
	}
}

