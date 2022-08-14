namespace Notes.Domain.Contracts;

public static class ApiRoutes
{
    public const string Root = "api";
    public const string Version = "v1";
    public const string Base = $"{Root}/{Version}";

    public static class Notes
    {
        public const string GetAll = $"{Base}/notes";
        public const string Get = $"{Base}/notes";
        public const string Create = $"{Base}/notes";
        public const string Update = $"{Base}/notes/";
        public const string Delete = $"{Base}/notes";
    }
}