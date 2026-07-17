namespace TenantX.Common.Domain.Constants;

public static class DomainErrors
{
    public static class Currency
    {       
        public const string CodeRequired = "Currency.CodeRequired";
        public const string NameRequired = "Currency.NameRequired";
        public const string SymbolRequired = "Currency.SymbolRequired";
        public const string InvalidDecimalPlaces = "Currency.InvalidDecimalPlaces";
        public const string Unsupported = "Currency.Unsupported";
    }
    public static class Common
    {
        public const string Required = "string.ValueRequired";
        public const string TooLong = "string.ValueTooLong";
        public const string TooShort = "string.ValueTooShort";
        public const string NullValue = "string.NullValue";
    }
    public static class Context
    {
        public const string UserRequired = "Context.UserRequired";
    }
    public static class Email
    {
        public const string InvalidFormat = "Email.InvalidFormat";
        public const string TooLong = "Email.TooLong";
        public const string Required = "Email.Required";
    }
     public static class Tenant
    {
        public const string NotFound = "Tenant.NotFound";
        public const string AlreadyExists = "Tenant.AlreadyExists";
        public const string Inactive = "Tenant.Inactive";
        public const string ApiKeyRequired = "Tenant.ApiKeyRequired";
    }
   
}