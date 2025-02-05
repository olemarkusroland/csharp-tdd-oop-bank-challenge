﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boolean.CSharp.Main.Models.Accounts;

using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Boolean.CSharp.Main.Models
{
    public class BankStatement
    {
        public Account Account { get; set; }

        public BankStatement(Account account)
        {
            Account = account;
        }

        public string GetStatement()
        {
            StringBuilder statementText = new StringBuilder();

            statementText.AppendFormat("{0,10} || {1,10} || {2,10} || {3,10}\n", "Date", "Credit", "Debit", "Balance");
            foreach (Transaction transaction in Account.Transactions.OrderByDescending(t => t.Date))
            {
                statementText.AppendFormat("{0,10} || {1,10} || {2,10} || {3,10}\n",
                    transaction.Date.ToString("dd/MM/yyyy"),
                    transaction.Type == TransactionType.Credit ? transaction.Amount.ToString("0.00") : "",
                    transaction.Type == TransactionType.Debit ? transaction.Amount.ToString("0.00") : "",
                    Account.GetBalanceAfterTransaction(transaction).ToString("0.00")
                );
            }

            return statementText.ToString();
        }

        public string sendStatmentMessage(string statment)
        {
            // Find your Account SID and Auth Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure
            string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: GetStatement(),
                from: new Twilio.Types.PhoneNumber("MY_TWILIO_PHONE_NUMBER"),
                to: new Twilio.Types.PhoneNumber("MY_PHYSICAL_PHONE_NUMBER")
            );

            return message.Sid;
        }
    }
}
