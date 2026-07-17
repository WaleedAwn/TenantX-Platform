namespace TenantX.Common.Domain.Constants;

public static class ValidationConstants
{
  public static class Names
  {
    public const int MaxLength = 150;
  }

  public static class Description
  {
    public const int MaxLength = 500;
  }
  public static class Subscriptions
  {
    public const int PaymentCodeLength = 6;
    public const int DefaultPaymentCodeExpiryMinutes = 60;
    public const int DefaultGracePeriodInDays = 3;
  }
}
