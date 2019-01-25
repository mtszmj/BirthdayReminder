using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayReminder.Model.Service.Password
{
    public class LoginHandler : ILoginHandler
    {
        private string PathToPassword;
        private string PathToSalt;

        public LoginHandler(string pathToPassword, string pathToSalt)
        {
            PathToPassword = pathToPassword;
            PathToSalt = pathToSalt;
        }

        public string ReadPassword()
        {
            return ReadPassword(PathToPassword, PathToSalt);
        }

        public bool StorePassword(string password, string passwordPath, string saltPath = null)
        {
            string salt = null;
            if(saltPath != null)
            {
                salt = CreateRandomSalt();
                try
                { 
                    File.WriteAllText(saltPath, salt);
                }
                catch (Exception)
                {
                    return false; // Password was not saved.
                }
            }

            string pwd = ProtectPassword(password, salt, DataProtectionScope.CurrentUser);

            try
            {
                File.WriteAllText(passwordPath, pwd);
                return true;
            }
            catch (Exception)
            {
                return false; // Password was not saved.
            }
        }

        public string ReadPassword(string passwordPath, string saltPath = null)
        {
            string salt = null;
            if(saltPath != null)
            {
                salt = File.ReadAllText(saltPath);
            }
            string pwd = File.ReadAllText(passwordPath);
            return UnprotectPassword(pwd, salt, DataProtectionScope.CurrentUser);
        }

        private string ProtectPassword(string password, string salt = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = string.IsNullOrEmpty(salt) ? null : Encoding.UTF8.GetBytes(salt);
            byte[] encryptedPassword = ProtectedData.Protect(passwordBytes, saltBytes, scope);
            return Convert.ToBase64String(encryptedPassword);
        }

        private string UnprotectPassword(string encryptedPassword, string salt = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            if (encryptedPassword == null)
                throw new ArgumentNullException(nameof(encryptedPassword));
            byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);
            byte[] saltBytes = string.IsNullOrEmpty(salt) ? null : Encoding.UTF8.GetBytes(salt);
            byte[] password = ProtectedData.Unprotect(encryptedBytes, saltBytes, scope);
            return Encoding.UTF8.GetString(password);
        }

        private string CreateRandomSalt()
        {
            byte[] entropy = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(entropy);
            return Encoding.UTF8.GetString(entropy);
        }
    }
}
