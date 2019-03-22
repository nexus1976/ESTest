using ESTest.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ESTest.Domain
{
    public class SystemUser : IdentityUser, IUser<long>
    {
        internal const int SALT_BYTE_SIZE = 24;

        public new long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Salt { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string MainPhone { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsAuthenticated { get; private set; }
        public string PasswordChangeTo { get; set; }
        public string DisplayName
        {
            get
            {
                return ((string.IsNullOrWhiteSpace(this.FirstName) ? string.Empty : this.FirstName.Trim() + " ")
                        + (string.IsNullOrWhiteSpace(this.LastName) ? string.Empty : this.LastName.Trim())).Trim();
            }
        }

        public SystemUser()
        {
            this.SecurityStamp = Guid.NewGuid().ToString();
        }

        #region IUser Interface
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<SystemUser, long> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
        public new string UserName
        {
            get { return this.EmailAddress; }
            set { this.EmailAddress = value; }
        }
        public new string Email
        {
            get { return this.EmailAddress; }
            set { this.EmailAddress = value; }
        }
        #endregion

        public static SystemUser Get(long id, IQueryContext context)
        {
            if (id <= 0 || context == null) return null;
            var user = context.Users.AsNoTracking().Where(d => d.Id == id).FirstOrDefault();
            if (user != null)
            {
                SystemUser userDomain = CreateDomainFromEntity(null, user);
                return userDomain;
            }
            else
            {
                return null;
            }
        }
        public static SystemUser Get(long id, string password, IQueryContext context)
        {
            if (id <= 0 || context == null) return null;
            var user = context.Users.AsNoTracking().Where(d => d.Id == id).FirstOrDefault();
            if (user != null)
            {
                SystemUser userDomain = CreateDomainFromEntity(null, user);
                userDomain.Authenticate(password);
                return userDomain;
            }
            else
            {
                return null;
            }
        }
        public static SystemUser Get(string loginName, IQueryContext context)
        {
            if (string.IsNullOrWhiteSpace(loginName) || context == null)
            {
                return null;
            }
            var user = context.Users.AsNoTracking().Where(d => d.EmailAddress == loginName).FirstOrDefault();
            if (user != null)
            {
                SystemUser userDomain = CreateDomainFromEntity(null, user);
                return userDomain;
            }
            else
            {
                return null;
            }
        }
        public static SystemUser Get(string loginName, string password, IQueryContext context)
        {
            if (string.IsNullOrWhiteSpace(loginName) || context == null) return null;
            var user = context.Users.AsNoTracking().Where(d => d.EmailAddress == loginName).FirstOrDefault();
            if (user != null)
            {
                SystemUser userDomain = CreateDomainFromEntity(null, user);
                userDomain.Authenticate(password);
                return userDomain;
            }
            else
            {
                return null;
            }
        }
        internal static SystemUser CreateDomainFromEntity(SystemUser userDomain, User userEntity)
        {
            if (userEntity == null) return userDomain;
            if (userDomain == null)
            {
                userDomain = new SystemUser();
            }
            userDomain.Id = userEntity.Id;
            userDomain.FirstName = userEntity.FirstName;
            userDomain.LastName = userEntity.LastName;
            userDomain.EmailAddress = userEntity.EmailAddress;
            userDomain.PasswordHash = userEntity.Password;
            userDomain.Salt = userEntity.Salt;
            userDomain.IsAuthenticated = false;
            userDomain.Address1 = userEntity.Address1;
            userDomain.Address2 = userEntity.Address2;
            userDomain.City = userEntity.City;
            userDomain.Province = userEntity.Province;
            userDomain.PostalCode = userEntity.PostalCode;
            userDomain.MainPhone = userEntity.MainPhone;
            userDomain.CreatedDate = userEntity.CreatedDate;
            return userDomain;
        }
        internal User CreateEntityFromDomain(User userEntity)
        {
            if (userEntity == null)
            {
                userEntity = new User();
            }
            userEntity.FirstName = this.FirstName;
            userEntity.LastName = this.LastName;
            userEntity.EmailAddress = this.EmailAddress;
            userEntity.Password = this.PasswordHash;
            userEntity.Salt = this.Salt;
            userEntity.Address1 = this.Address1;
            userEntity.Address2 = this.Address2;
            userEntity.City = this.City;
            userEntity.Province = this.Province;
            userEntity.PostalCode = this.PostalCode;
            userEntity.MainPhone = this.MainPhone;
            userEntity.CreatedDate = this.CreatedDate;
            return userEntity;
        }
        public void Delete(ICommandContext context)
        {
            if (this.Id <= 0)
            {
                throw new ArgumentException("When deleting a SystemUser, the Id property cannot be less than or equal to 0.");
            }
            if (context == null)
            {
                throw new ArgumentException("When deleting a SystemUser, the context parameter cannot be null.");
            }
            var user = context.Users.Where(d => d.Id == this.Id).SingleOrDefault();
            if (user != null)
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }
        public SystemUser Save(ICommandContext context)
        {
            if (context == null)
            {
                throw new ArgumentException("When saving a SystemUser, the context parameter cannot be null.");
            }
            this.validate();
            bool creatingNew = this.Id <= 0;
            if (creatingNew)
            {
                this.setPasswordInfo();
                var user = this.CreateEntityFromDomain(null);
                user.CreatedDate = DateTime.UtcNow;
                context.Users.Add(user);
                context.SaveChanges();
                this.Id = user.Id;
                this.CreatedDate = user.CreatedDate;
            }
            else
            {
                var user = context.Users.Where(d => d.Id == this.Id).FirstOrDefault();
                if (user != null)
                {
                    if (!string.IsNullOrWhiteSpace(this.PasswordChangeTo))
                    {
                        this.setPasswordInfo();
                    }
                    user = this.CreateEntityFromDomain(user);
                    context.SaveChanges();
                    CreateDomainFromEntity(this, user);
                }
            }
            return this;
        }
        public bool Authenticate(string clearPassword)
        {
            this.IsAuthenticated = validatePassword(clearPassword, this.PasswordHash, this.Salt);
            return this.IsAuthenticated;
        }

        private bool validatePassword(string clearPassword, string hashPassword, string salt)
        {
            bool isValid = false;
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] clearPWDBytes = Convert.FromBase64String(clearPassword.Base64Encode());
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] clearPWDWithSalt = combinePasswordAndSalt(clearPWDBytes, saltBytes);
            string computedHash = Convert.ToBase64String(algorithm.ComputeHash(clearPWDWithSalt));
            if (computedHash == hashPassword)
            {
                isValid = true;
            }
            return isValid;
        }
        private byte[] combinePasswordAndSalt(byte[] clearPWDBytes, byte[] saltBytes)
        {
            byte[] clearPWDWithSalt = new byte[clearPWDBytes.Length + saltBytes.Length];
            for (int i = 0; i < clearPWDBytes.Length; i++)
            {
                clearPWDWithSalt[i] = clearPWDBytes[i];
            }
            for (int i = 0; i < saltBytes.Length; i++)
            {
                clearPWDWithSalt[clearPWDBytes.Length + i] = saltBytes[i];
            }
            return clearPWDWithSalt;
        }
        private Tuple<string, string> createPasswordHash(string clearPassword)
        {
            HashAlgorithm algorithm = new SHA256Managed();
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            byte[] saltBytes = new byte[SALT_BYTE_SIZE];
            csprng.GetBytes(saltBytes);
            byte[] clearPWDBytes = Convert.FromBase64String(clearPassword.Base64Encode());
            byte[] clearPWDWithSalt = combinePasswordAndSalt(clearPWDBytes, saltBytes);
            string computedHash = Convert.ToBase64String(algorithm.ComputeHash(clearPWDWithSalt));
            string computerSalt = Convert.ToBase64String(saltBytes);
            return new Tuple<string, string>(computedHash, computerSalt);
        }
        private void validate()
        {
            if (string.IsNullOrWhiteSpace(this.FirstName))
            {
                throw new ArgumentException("The FirstName must contain a valid value.");
            }
            if (string.IsNullOrWhiteSpace(this.LastName))
            {
                throw new ArgumentException("The LastName must contain a valid value.");
            }
            if (string.IsNullOrWhiteSpace(this.EmailAddress))
            {
                throw new ArgumentException("The EmailAddress must contain a valid value.");
            }
            if (this.Id <= 0) //creating a new system user
            {
                if (string.IsNullOrWhiteSpace(this.PasswordChangeTo))
                {
                    throw new ArgumentException("When creating a new SystemUser, the PasswordChangeTo property must contain a valid value.");
                }
            }
            try
            {
                MailAddress m = new MailAddress(this.EmailAddress);
            }
            catch (FormatException)
            {
                throw new ArgumentException("The EmailAddress must contain a valid value.");
            }
        }
        private void setPasswordInfo()
        {
            var tuple = createPasswordHash(this.PasswordChangeTo);
            this.PasswordHash = tuple.Item1;
            this.Salt = tuple.Item2;
            this.IsAuthenticated = true;
        }
    }
}
