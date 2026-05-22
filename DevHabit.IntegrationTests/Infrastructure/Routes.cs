using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevHabit.IntegrationTests.Infrastructure;

public static class Routes
{
    public static class Auth
    {
        public const string Register = "auth/register";
        public const string Login = "auth/login";
    }

    public static class Habits
    {
        public const string Create = "habits";
        public const string GetAll = "habits";
        public static string GetById(string id) => $"habits/{id}";
    }

    public static class GitHub
    {
        public const string StoreAccessToken = "github/personal-access-token";
        public const string GetProfile = "github/profile";
        public const string GetEvents = "github/events";
    }
}
