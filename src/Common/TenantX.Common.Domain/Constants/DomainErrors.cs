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
        public const string InvalidAmount = "Money.InvalidAmount";

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
    public static class Feature
    {
        public const string NotFound = "Feature.NotFound";
        public const string AlreadyExists = "Feature.AlreadyExists";
        public const string InvalidKey = "Feature.InvalidKey";
        public const string TypeMismatch = "Feature.TypeMismatch";
        public const string Inactive = "Feature.Inactive";

    }
    public static class Plan
    {
        public const string PlanNotFound = "Plan.NotFound";
        public const string Archived = "Plan.Archived";
        public const string DuplicateFeature = "Plan.DuplicateFeature";
        public const string TenantMismatch = "Plan.TenantMismatch";
    }
    public static class Entitlement
    {
        public const string ValueRequired = "Entitlement.ValueRequired";
        public const string FeatureIdRequired = "Entitlement.FeatureIdRequired";
        public const string InvalidValue = "Entitlement.InvalidValue";
    }
}