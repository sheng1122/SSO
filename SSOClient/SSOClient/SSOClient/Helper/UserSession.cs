using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace SSOClient.Helpers
{
    public class UserSession
    {
        private static IServiceProvider _services;

        private class Key
        {
            public const string UserName = "UserName";
            public const string Email = "Email";
            public const string UserId = "UserId";
        }

        public static IServiceProvider Service
        {
            get { return _services; }
            set
            {
                if (_services != null)
                {
                    throw new Exception("Can't set once a value has already been set.");
                }

                _services = value;
            }
        }

        private static ISession session
        {
            get
            {
                IHttpContextAccessor httpContext = Service.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;

                HttpContext context = httpContext?.HttpContext;

                return context.Session;
            }
        }

        public static string UserName
        {
            get
            {

                string name = session.Get<string>(Key.UserName);

                return name;
            }

            private set
            {
                session.Set(Key.UserName, value);
            }
        }

        public static string Email
        {
            get
            {

                string email = session.Get<string>(Key.Email);

                return email;
            }

            private set
            {
                session.Set(Key.Email, value);
            }
        }

        public static Guid? UserId
        {
            get
            {

                Guid? userId = session.Get<Guid?>(Key.UserId);

                return userId;
            }

            private set
            {
                session.Set(Key.UserId, value);
            }
        }

        public static async Task Set(Guid userId, string email, string userName)
        {
            Email = email;
            UserName = userName;
            UserId = userId;
        }

        public static void Clear()
        {
            Email = null;
            UserName = null;
            UserId = null;
        }
    }
}
