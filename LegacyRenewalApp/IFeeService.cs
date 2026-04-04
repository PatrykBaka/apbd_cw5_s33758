namespace LegacyRenewalApp;

public interface IFeeService
{

    public (decimal supportFee, string notes) CalculateSupportFee(bool includePremiumSupport, string normalizedPlanCode);

    public (decimal paymentFee, string notes) CalculatePaymentFee(string normalizedPaymentMethod,
        decimal subtotalAfterDiscount, decimal supportFee);

}