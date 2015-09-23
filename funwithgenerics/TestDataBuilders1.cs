using System;

namespace TestDataBuilders1
{
	class Example
	{
		public static void Main()
		{
			new SalaryPaymentTestDataBuilder().Build();

			new SalaryPaymentTestDataBuilder()
				.WithDateIssued(new DateTime(2015, 9, 24))
				.Build();

			new SalaryPaymentTestDataSaver(new EmployeeTestDataBuilder().Save())
				.Save();

			new SalaryPaymentTestDataSaver(new EmployeeTestDataBuilder().Save())
				.WithAmount(2500)
				.Save();
			
//			new SalaryPaymentTestDataBuilder()
//				.Save();  // boom
		}
	}

                                                               // F-bound
	abstract class SalaryPaymentTestDataBuilder<TSelf> where TSelf : SalaryPaymentTestDataBuilder<TSelf>
	{
		protected Employee recipient = new Employee();
		protected DateTime dateIssued = new DateTime(2015, 9, 17);
		protected long amount = 750;


		public SalaryPayment Build()
		{
            return new SalaryPayment(recipient, dateIssued, amount); 
		}

		public TSelf WithRecipient(Employee recipient)
		{
			this.recipient = recipient;
			return Self();
		}
		public TSelf WithDateIssued(DateTime dateIssued)
		{
			this.dateIssued = dateIssued;
			return Self();
		}
		public TSelf WithAmount(long amount)
		{
			this.amount = amount;
			return Self();
		}
		protected abstract TSelf Self();
	}

	class SalaryPaymentTestDataBuilder : SalaryPaymentTestDataBuilder<SalaryPaymentTestDataBuilder>
	{
		protected override SalaryPaymentTestDataBuilder Self() { return this; }
	}

	class SalaryPaymentTestDataSaver : SalaryPaymentTestDataBuilder<SalaryPaymentTestDataSaver>
	{
		public SalaryPaymentTestDataSaver(Employee recipient)
		{
			this.recipient = recipient;
		}
		
		public SalaryPayment Save()
		{

			var payment = this.Build();

			// ... SQL to save the entity ...

			return payment;

		}

		protected override SalaryPaymentTestDataSaver Self() {return this;}
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

