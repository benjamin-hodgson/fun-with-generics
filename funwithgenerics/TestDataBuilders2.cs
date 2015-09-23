using System;

namespace TestDataBuilders2
{
	class Example
	{
		public static void Main()
		{
			new SalaryPaymentTestDataBuilder().Build();

			new SalaryPaymentTestDataBuilder()
				.WithDateIssued(new DateTime(2015, 9, 24))
				.Build();

			new SalaryPaymentTestDataBuilder()
				.WithRecipient(new EmployeeTestDataBuilder().Save())
				.Save();

			new SalaryPaymentTestDataBuilder()
				.WithRecipient(new EmployeeTestDataBuilder().Save())
				.WithAmount(2500)
				.Save();
			
//			new SalaryPaymentTestDataBuilder()
//				.Save();  // boom
		}
	}

	class BOOL {
		private BOOL() {}
		public sealed class True : BOOL {}
		public sealed class False : BOOL {}
	}

//	class Perhaps : BOOL {}
//
//	class NonsenseBuilder :SalaryPaymentTestDataBuilder<Perhaps>
//	{}

	class SalaryPaymentTestDataBuilder<EmployeeWasSet> where EmployeeWasSet : BOOL
	{
		protected Employee recipient = new Employee();
		protected DateTime dateIssued = new DateTime(2015, 9, 17);
		protected long amount = 750;
		protected bool recipientWasSet = false;

		private SalaryPaymentTestDataBuilder(Employee recipient, DateTime dateIssued, long amount)
		{
			this.recipient = recipient;
			this.dateIssued = dateIssued;
			this.amount = amount;
		}

		protected SalaryPaymentTestDataBuilder() {}

		public SalaryPayment Build()
		{
			return new SalaryPayment(recipient, dateIssued, amount); 
		}

		public SalaryPaymentTestDataBuilder<BOOL.True> WithRecipient(Employee recipient)
		{
			this.recipientWasSet = true;
			this.recipient = recipient;
			return new SalaryPaymentTestDataBuilder<BOOL.True>(recipient, dateIssued, amount);
		}
		public SalaryPaymentTestDataBuilder<EmployeeWasSet> WithDateIssued(DateTime dateIssued)
		{
			this.dateIssued = dateIssued;
			return this;
		}
		public SalaryPaymentTestDataBuilder<EmployeeWasSet> WithAmount(long amount)
		{
			this.amount = amount;
			return this;
		}
	}

	class SalaryPaymentTestDataBuilder : SalaryPaymentTestDataBuilder<BOOL.False>
	{
		public SalaryPaymentTestDataBuilder() : base() {}
	}

	static class BuilderExt
	{
		public static SalaryPayment Save(this SalaryPaymentTestDataBuilder<BOOL.True> builder)
		{

			var payment = builder.Build();

			// ... SQL to save the entity ...

			return payment;
			
		}
	}

	class SalaryPayment
	{
		public SalaryPayment(Employee recipient, DateTime dateIssued, long amount)
		{
			this.Recipient = recipient;
			this.DateIssued = dateIssued;
			this.Amount = amount;
		}

		public Employee Recipient { get; private set; }
		public DateTime DateIssued { get; private set; }
		public long Amount { get; private set; }
	}

	class Employee
	{
		// ...
	}

	class EmployeeTestDataBuilder
	{
		public Employee Save()
		{
			return new Employee();
		}
	}
}

