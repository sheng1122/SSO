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
            public const string SessionToken = "SessionToken";
            public const string TokenExpiredDate = "TokenExpiredDate";
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

        public static int? UserId
        {
            get
            {
                int? userId = session.Get<int?>(Key.UserId);

                return userId;
            }

            private set
            {
                session.Set(Key.UserId, value);
            }
        }

        public static Guid? SessionToken
        {
            get
            {
                Guid? sessionToken = session.Get<Guid?>(Key.SessionToken);

                return sessionToken;
            }

            private set
            {
                session.Set(Key.SessionToken, value);
            }
        }

        public static DateTime? TokenExpiredDate
        {
            get
            {
                DateTime? tokenExpiredDate = session.Get<DateTime?>(Key.TokenExpiredDate);

                return tokenExpiredDate;
            }

            private set
            {
                session.Set(Key.TokenExpiredDate, value);
            }
        }

        public static async Task Set(int userId, string email, string userName, Guid sessionToken, DateTime tokenExpiredDate)
        {
            Email = email;
            UserName = userName;
            UserId = userId;
            SessionToken = sessionToken;
            TokenExpiredDate = tokenExpiredDate;
        }

        public static void Clear()
        {
            Email = null;
            UserName = null;
            UserId = null;
            SessionToken = null;
            TokenExpiredDate = null;
        }
    }
}
