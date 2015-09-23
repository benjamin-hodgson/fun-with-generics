using System;

namespace TestDataBuilders0
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

			// exception when you try to save but you didn't set the recipient
			new SalaryPaymentTestDataBuilder()
				.Save();  // boom
		}
	}

	class SalaryPaymentTestDataBuilder
	{
		protected Employee recipient = new Employee();
		protected DateTime dateIssued = new DateTime(2015, 9, 17);
		protected long amount = 750;
		protected bool recipientWasSet;

		public SalaryPayment Build()
		{
			return new SalaryPayment(recipient, dateIssued, amount); 
		}

		public SalaryPayment Save()
		{
			if (!this.recipientWasSet)
			{
				throw new InvalidOperationException(
					"You have to set the recipient because foreign keys");
			}

			var payment = this.Build();

			// ... SQL to save the entity ...

			return payment;

		}


		public SalaryPaymentTestDataBuilder WithRecipient(Employee recipient)
		{
			this.recipientWasSet = true;
			this.recipient = recipient;
			return this;
		}
		public SalaryPaymentTestDataBuilder WithDateIssued(DateTime dateIssued)
		{
			this.dateIssued = dateIssued;
			return this;
		}
		public SalaryPaymentTestDataBuilder WithAmount(long amount)
		{
			this.amount = amount;
			return this;
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

