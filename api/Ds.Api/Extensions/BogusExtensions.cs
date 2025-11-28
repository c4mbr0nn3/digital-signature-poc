using Bogus;

namespace Ds.Api.Extensions;

public static class BogusExtensions
{
    extension(Faker faker)
    {
        public string Isin()
        {
            var countryCode = faker.Address.CountryCode();
            var nsin = faker.Random.AlphaNumeric(9).ToUpper();
            var baseIsin = countryCode + nsin;
            var checkDigit = CalculateLuhnCheckDigit(baseIsin);
            return baseIsin + checkDigit;
        }
    }

    private static int CalculateLuhnCheckDigit(string baseIsin)
    {
        // Convert letters to numbers (A=10, B=11, ..., Z=35)
        var digits = string.Concat(baseIsin.Select(c =>
            char.IsLetter(c) ? (c - 'A' + 10).ToString() : c.ToString()));

        // Luhn algorithm
        var sum = 0;
        var alternate = true;

        for (var i = digits.Length - 1; i >= 0; i--)
        {
            var n = digits[i] - '0';
            if (alternate)
            {
                n *= 2;
                if (n > 9) n -= 9;
            }
            sum += n;
            alternate = !alternate;
        }

        return (10 - sum % 10) % 10;
    }
}