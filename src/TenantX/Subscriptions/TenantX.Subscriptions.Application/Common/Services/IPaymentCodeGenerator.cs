namespace TenantX.Subscriptions.Application.Common.Services;

public interface IPaymentCodeGenerator
{
  string GenerateCode(int length = 6);
}
