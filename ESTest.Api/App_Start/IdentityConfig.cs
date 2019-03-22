using ESTest.DAL;
using ESTest.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESTest.Api
{
    public class SystemUserStore : IUserStore<SystemUser, long>, IUserPasswordStore<SystemUser, long>, IUserSecurityStampStore<SystemUser, long>
    {
        private ICommandContext _commandContext = null;
        private IQueryContext _queryContext = null;
        public SystemUserStore(ICommandContext commandContext, IQueryContext queryContext)
        {
            _commandContext = commandContext;
            _queryContext = queryContext;
        }
        public Task CreateAsync(SystemUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordChangeTo = "Password1234";
            return Task.FromResult(user.Save(_commandContext));
        }
        public Task DeleteAsync(SystemUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.Delete(_commandContext);
            return null;
        }
        public Task<SystemUser> FindByIdAsync(long id)
        {
            var user = SystemUser.Get(id, _queryContext);
            return Task.FromResult(user);
        }
        public Task<SystemUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("userName");
            }
            var user = SystemUser.Get(userName, _queryContext);
            return Task.FromResult(user);
        }
        public Task UpdateAsync(SystemUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user = user.Save(_commandContext);
            return Task.FromResult(user);
        }

        public void Dispose() { }

        public Task SetPasswordHashAsync(SystemUser user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;
            return null;
        }

        public Task<string> GetPasswordHashAsync(SystemUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(SystemUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            bool result = !string.IsNullOrWhiteSpace(user.PasswordHash);
            return Task.FromResult(result);
        }

        public Task SetSecurityStampAsync(SystemUser user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.SecurityStamp = stamp;
            return null;
        }

        public Task<string> GetSecurityStampAsync(SystemUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            string securityStamp = user.SecurityStamp;
            return Task.FromResult(securityStamp);
        }
    }
    public class ApplicationUserManager : UserManager<SystemUser, long>
    {
        public ApplicationUserManager(IUserStore<SystemUser, long> store) : base(store) { }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            IQueryContext queryContext = new Context();
            ICommandContext commandContext = new Context();
            var manager = new ApplicationUserManager(new SystemUserStore(commandContext, queryContext));
            manager.UserStoreContext = commandContext;
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<SystemUser, long>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<SystemUser, long>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        internal ICommandContext UserStoreContext { get; set; }
        public override Task<IdentityResult> CreateAsync(SystemUser user, string password)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentException("The SystemUser parameter user was null when attempting to create a new Identity.");
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new ArgumentException("The string parameter password was null or empty when attempting to create a new Identity.");
                }
                user.PasswordChangeTo = password;
                user = user.Save(this.UserStoreContext);
            }
            catch (System.Exception ex)
            {
                IEnumerable<string> errors = new List<string>();
                (errors as List<string>).Add(ex.Message);
                return Task.FromResult(new IdentityResult(errors));
            }
            var ir = IdentityResult.Success;
            return Task.FromResult(ir);
        }

        public override Task<IdentityResult> AddPasswordAsync(long userId, string password)
        {
            try
            {
                if (userId <= 0)
                {
                    throw new ArgumentException("The int parameter userId was invalid when attempting to create a new password.");
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new ArgumentException("The string parameter password was invalid when attempting to create a new password.");
                }
                var user = SystemUser.Get(userId, this.UserStoreContext as IQueryContext);
                if (user == null)
                {
                    throw new KeyNotFoundException("The user could not be found with the given userId parameter when attempting to create a new password.");
                }
                user.PasswordChangeTo = password;
                user.Save(this.UserStoreContext);
            }
            catch (Exception ex)
            {
                IEnumerable<string> errors = new List<string>();
                (errors as List<string>).Add(ex.Message);
                return Task.FromResult(new IdentityResult(errors));
            }
            var ir = IdentityResult.Success;
            return Task.FromResult(ir);
        }

        public override Task<IdentityResult> ChangePasswordAsync(long userId, string currentPassword, string newPassword)
        {
            try
            {
                if (userId <= 0)
                {
                    throw new ArgumentException("The int parameter userId was invalid when attempting to create a new password.");
                }
                if (string.IsNullOrWhiteSpace(currentPassword))
                {
                    throw new ArgumentException("The string parameter currentPassword was invalid when attempting to create a new password.");
                }
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    throw new ArgumentException("The string parameter newPassword was invalid when attempting to create a new password.");
                }
                var user = SystemUser.Get(userId, currentPassword, this.UserStoreContext as IQueryContext);
                if (user == null)
                {
                    throw new KeyNotFoundException("The user could not be found with the given userId parameter when attempting to create a new password.");
                }
                if (!user.IsAuthenticated)
                {
                    throw new KeyNotFoundException("The user could not be authenticated with the given currentPassword parameter when attempting to create a new password.");
                }

                user.PasswordChangeTo = newPassword;
                user.Save(this.UserStoreContext);
            }
            catch (Exception ex)
            {
                IEnumerable<string> errors = new List<string>();
                (errors as List<string>).Add(ex.Message);
                return Task.FromResult(new IdentityResult(errors));
            }
            var ir = IdentityResult.Success;
            return Task.FromResult(ir);
        }

        public override Task<SystemUser> FindAsync(string userName, string password)
        {
            var user = SystemUser.Get(userName, password, this.UserStoreContext as IQueryContext);
            if (user != null && !user.IsAuthenticated)
            {
                user = null;
            }
            return Task.FromResult(user);
        }
    }
}
