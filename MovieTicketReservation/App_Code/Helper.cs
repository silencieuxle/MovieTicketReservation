using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Cryptography;

namespace MovieTicketReservation.App_Code {
	public static class Helper {

        #region Generate Unique random string
        /// <summary>
        /// Generate unique random string with provided length parameter, include provided char set
        /// </summary>
        /// <param name="length">Length of the output string</param>
        /// <param name="allowedChars">Char set</param>
        /// <returns>A unique random string</returns>
        public static string GenerateUniqueString(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789") {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "length cannot be less than zero.");
            if (string.IsNullOrEmpty(allowedChars)) throw new ArgumentException("allowedChars may not be empty.");

            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length) throw new ArgumentException(String.Format("allowedChars may contain no more than {0} characters.", byteSize));

            // Guid.NewGuid and System.Random are not particularly random. By using a
            // cryptographically-secure random number generator, the caller is always
            // protected, regardless of use.
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider()) {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < length) {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < length; ++i) {
                        // Divide the byte into allowedCharSet-sized groups. If the
                        // random value falls into the last group and the last group is
                        // too small to choose from the entire allowedCharSet, ignore
                        // the value in order to avoid biasing the result.
                        var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i]) continue;
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }
                return result.ToString();
            }
        } 
        #endregion

        #region Generate SHA1 string
        /// <summary>
        /// Generate a encrypted string with SHA1 Encryption method
        /// </summary>
        /// <param name="data">Input string</param>
        /// <returns>SHA1 encrypted string</returns>
        public static string GenerateSHA1String(string data) {
            //create new instance of md5
            SHA1 sha1 = SHA1.Create();

            //convert the input text to array of bytes
            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(data));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++) {
                returnValue.Append(hashData[i].ToString());
            }

            // return hexadecimal string
            return returnValue.ToString();
        }
        #endregion

        #region Convert C# Timespan to normal time string hh:mm
        /// <summary>
        /// Convert C# Timespan to normal time string (hh:mm)
        /// </summary>
        /// <param name="time">Input time to be converted</param>
        /// <returns>Converted time</returns>
        public static string ToTimeString(this TimeSpan time) {
            // Get hours
            var hours = time.Hours.ToString();

            // Get minutes
            var minutes = time.Minutes.ToString();
            return hours + ":" + minutes;
        }
        #endregion
    }
}