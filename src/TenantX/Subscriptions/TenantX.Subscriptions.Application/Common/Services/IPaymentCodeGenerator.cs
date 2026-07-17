using TenantX.Common.Domain.Constants;

namespace TenantX.Subscriptions.Application.Common.Services;

public interface IPaymentCodeGenerator
{
   /// <summary>
    /// Generates a numeric code for wallet payments.
    /// Default length is pulled from domain validation constants.
    /// </summary>
    string GenerateCode(int length = ValidationConstants.Subscriptions.PaymentCodeLength);
}
