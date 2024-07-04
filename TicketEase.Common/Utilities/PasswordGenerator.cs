using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TicketEase.Common.Utilities
{
	public class PasswordGenerator
	{
		public static string GeneratePassword(string email, string companyName)
		{

			email = email.Split("@")[0];
			if (email.Length > 5)
			{
				email = email.Substring(0,5).Trim();
			}
			if (companyName.Length > 4)
			{
				companyName = companyName.Substring(0,4).Trim();
			}

			companyName = char.ToUpper(companyName[0]) + companyName.Substring(1);
			string password = companyName +  GenerateRandomString(4) + email;

			return  password;

		}

		//static string HashPassword(string password)
		//{
		//	// Generate a random salt
		//	byte[] salt = new byte[16];
		//	using (var rng = new RNGCryptoServiceProvider())
		//	{
		//		rng.GetBytes(salt);
		//	}

		//	// Hash the password using PBKDF2 with 10000 iterations
		//	using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
		//	{
		//		byte[] hash = pbkdf2.GetBytes(32); // 256 bits
		//		byte[] combined = new byte[salt.Length + hash.Length];
		//		Array.Copy(salt, 0, combined, 0, salt.Length);
		//		Array.Copy(hash, 0, combined, salt.Length, hash.Length);
		//		return Convert.ToBase64String(combined);
		//	}
		//}

		//static bool ValidateLogin(string username, string password, string hashedPassword)
		//{
		//	// Retrieve the salt from the stored hashed password
		//	byte[] combined = Convert.FromBase64String(hashedPassword);
		//	byte[] salt = new byte[16];
		//	Array.Copy(combined, 0, salt, 0, salt.Length);

		//	// Hash the entered password with the retrieved salt
		//	using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
		//	{
		//		byte[] hash = pbkdf2.GetBytes(32); // 256 bits
		//		byte[] combinedHash = new byte[salt.Length + hash.Length];
		//		Array.Copy(salt, 0, combinedHash, 0, salt.Length);
		//		Array.Copy(hash, 0, combinedHash, salt.Length, hash.Length);

		//		// Compare the generated hash with the stored hashed password
		//		return Convert.ToBase64String(combinedHash) == hashedPassword;
		//	}
		//}
		static string GenerateRandomString(int length)
		{
			//lenght = length of desired output
			// Characters to use for the random string
			const string chars = "$.ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#abcdefghijklmnopqrstuvwxyz0123456789%&()";

			// Random number generator
			Random random = new Random();

			// Generate the random string
			var randomString = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				randomString.Append(chars[random.Next(chars.Length)]);
			}

			return randomString.ToString();
		}


	}
}
