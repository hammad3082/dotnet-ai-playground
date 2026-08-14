using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace _04_FunctionCalling
{
    internal static class SampleTools
    {
        [Description("Gets the current weather for a specified city.")]
        public static string GetWeather(
        [Description("The name of the city, e.g., Bangalore, London")] string city)
        {
            return city.ToLower() switch
            {
                "bangalore" => "Sunny, 26°C with 60% humidity.",
                "london" => "Rainy, 14°C with heavy cloud cover.",
                "tokyo" => "Clear, 20°C.",
                _ => $"Weather data for '{city}' is currently unavailable."
            };
        }

        [Description("Fetches user profile information from the database by user ID.")]
        public static string GetUserProfile(
            [Description("The unique integer ID of the user")] int userId)
        {
            return userId switch
            {
                101 => "{ 'id': 101, 'name': 'Mohammed', 'role': 'Backend Developer', 'status': 'Active' }",
                102 => "{ 'id': 102, 'name': 'Sarah', 'role': 'Frontend Lead', 'status': 'On Leave' }",
                _ => $"User with ID {userId} not found."
            };
        }

        [Description("Calculates monthly mortgage or loan payment amount.")]
        public static string CalculateLoanPayment(
            [Description("Total loan amount / principal")] double principal,
            [Description("Annual interest rate in percent, e.g., 8.5 for 8.5%")] double annualRatePercent,
            [Description("Loan duration in years")] int years)
        {
            double monthlyRate = (annualRatePercent / 100) / 12;
            int totalPayments = years * 12;

            double monthlyPayment = (principal * monthlyRate * Math.Pow(1 + monthlyRate, totalPayments)) /
                                   (Math.Pow(1 + monthlyRate, totalPayments) - 1);

            return $"Monthly Payment: ₹{monthlyPayment:N2} for {years} years at {annualRatePercent}% interest.";
        }

        [Description("Converts an amount from one currency to another.")]
        public static string ConvertCurrency(
            [Description("Amount of money to convert")] double amount,
            [Description("Source 3-letter currency code, e.g., USD, EUR, INR")] string fromCurrency,
            [Description("Target 3-letter currency code, e.g., INR, USD, EUR")] string toCurrency)
        {
            // Mock exchange rates relative to USD
            double fromRate = fromCurrency.ToUpper() switch { "USD" => 1.0, "INR" => 86.5, "EUR" => 0.92, _ => 1.0 };
            double toRate = toCurrency.ToUpper() switch { "USD" => 1.0, "INR" => 86.5, "EUR" => 0.92, _ => 1.0 };

            double amountInUsd = amount / fromRate;
            double converted = amountInUsd * toRate;

            return $"{amount} {fromCurrency.ToUpper()} = {converted:N2} {toCurrency.ToUpper()}";
        }

        [Description("Sends a notification email to a user. Use this when the user explicitly requests to send an email.")]
        public static string SendEmail(
            [Description("Recipient email address")] string toAddress,
            [Description("Email subject line")] string subject,
            [Description("Main message body of the email")] string body)
        {
            // Mock side-effect call
            Console.WriteLine($"\n[MOCK EMAIL SENT] To: {toAddress} | Subject: {subject}");
            return $"Email successfully queued and sent to {toAddress}.";
        }
    }
}
