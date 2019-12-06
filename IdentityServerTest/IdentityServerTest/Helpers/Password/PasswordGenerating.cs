using System;
using System.Text;
using IdentityServerTest.Constants;
using Microsoft.AspNetCore.Identity;

namespace IdentityServerTest.Helpers.Password
{
    public static class PasswordGenerating
    {
        public static string GeneratePassword()
        {
           bool nonAlphanumeric = true;
            bool digit = true;
            bool lowercase = true;
            bool uppercase = true;

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            while (password.Length < PasswordConstants.PASSWORD_LENGTH)
            {
                char c = (char)random.Next(PasswordConstants.PASSWORD_INDEX_MIN, PasswordConstants.PASSWORD_INDEX_MAX);

                password.Append(c);

                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }
            
            if (nonAlphanumeric)
                password.Append((char)random.Next(PasswordConstants.NON_ALPHANUMERICAL_INDEX_MIN, PasswordConstants.NON_ALPHANUMERICAL_INDEX_MAX));
            if (digit)
                password.Append((char)random.Next(PasswordConstants.DIGIT_INDEX_MIN, PasswordConstants.DIGIT_INDEX_MAX));
            if (lowercase)
                password.Append((char)random.Next(PasswordConstants.LOWERCASE_INDEX_MIN, PasswordConstants.LOWERCASE_INDEX_MAX));
            if (uppercase)
                password.Append((char)random.Next(PasswordConstants.UPPERCASE_INDEX_MIN, PasswordConstants.UPPERCASE_INDEX_MAX));

            return password.ToString();
        }
    }
}