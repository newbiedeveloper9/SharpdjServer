using System;
using System.Globalization;
using System.Net;
using System.Security;
using System.Text.RegularExpressions;

namespace SharpDj.Common
{
    public class DataValidation
    {
        public static bool EmailIsValid(string email)
        {
            return Regex.IsMatch(email, @"\A[a-z0-9]+([-._][a-z0-9]+)*@([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,4}\z")
                   && Regex.IsMatch(email, @"^(?=.{1,64}@.{4,64}$)(?=.{6,100}$).*");
        }

        public static bool LengthIsValid(string str, int minLength, int maxLength)
        {
            if (string.IsNullOrEmpty(str)) return true;
            if (str.Length >= minLength && str.Length <= maxLength)
                return true;
            return false;
        }

        public static bool PasswordIsValid(SecureString password, int minLength, int maxLength)
        {
            //new System.Net.NetworkCredential(string.Empty, password).Password; instead of method just for little security
            if (string.IsNullOrEmpty(new System.Net.NetworkCredential(string.Empty, password).Password)) return true;
            if (new System.Net.NetworkCredential(string.Empty, password).Password.Length >= minLength &&
                new System.Net.NetworkCredential(string.Empty, password).Password.Length <= maxLength)
                return true;
            return false;

        }
        public static bool ImageIsValid(string url)
        {
            bool result = Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (!result) return false;

            var req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "HEAD";
            using (var resp = req.GetResponse())
            {
                return resp.ContentType.ToLower(CultureInfo.InvariantCulture)
                    .StartsWith("image/", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
