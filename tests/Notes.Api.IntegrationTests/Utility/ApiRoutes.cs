namespace Notes.Api.IntegrationTests.Utility;

public static class ApiRoutes
{
    private const string Base = "api/v1";
    public static class Notes
    {
        private const string Note = $"{Base}/notes";
        public const string Get = $"{Base}/notes";
        public const string GetById = $"{Base}/notes/<id>";
        public const string Post = $"{Base}/notes";
        public const string Put = $"{Base}/notes";
        public const string Delete = $"{Base}/notes";

        public static class User
        {
            public const string Get = $"{Note}/user";
            public const string GetById = $"{Note}/user/<id>";
            public const string Update = $"{Note}/user";
            public const string Delete = $"{Note}/user";
        }
    }
    
    public static class Identity
    {
        public const string Register = $"{Base}/identity/register";
        public const string Login = $"{Base}/identity/login";
        public const string RefreshToken = $"{Base}/identity/refreshToken";
    }
}