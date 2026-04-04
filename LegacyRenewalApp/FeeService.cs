using System;

namespace LegacyRenewalApp;

public class FeeService : IFeeService
{

    public (decimal supportFee, string notes) CalculateSupportFee(bool  includePremiumSupport, string normalizedPlanCode)
    {
        
        decimal supportFee = 0;
        string notes =  string.Empty;
        
        if (includePremiumSupport)
        {
            if (normalizedPlanCode == "START")
            {
                supportFee = 250m;
            }
            else if (normalizedPlanCode == "PRO")
            {
                supportFee = 400m;
            }
            else if (normalizedPlanCode == "ENTERPRISE")
            {
                supportFee = 700m;
            }

            notes += "premium support included; ";
        }
        
        return (supportFee, notes);
    }

    public (decimal paymentFee, string notes) CalculatePaymentFee(string normalizedPaymentMethod,decimal subtotalAfterDiscount,
        decimal supportFee)
    {
        
        decimal paymentFee = 0;
        string notes = string.Empty;
        
        if (normalizedPaymentMethod == "CARD")
        {
            paymentFee = (subtotalAfterDiscount + supportFee) * 0.02m;
            notes += "card payment fee; ";
        }
        else if (normalizedPaymentMethod == "BANK_TRANSFER")
        {
            paymentFee = (subtotalAfterDiscount + supportFee) * 0.01m;
            notes += "bank transfer fee; ";
        }
        else if (normalizedPaymentMethod == "PAYPAL")
        {
            paymentFee = (subtotalAfterDiscount + supportFee) * 0.035m;
            notes += "paypal fee; ";
        }
        else if (normalizedPaymentMethod == "INVOICE")
        {
            paymentFee = 0m;
            notes += "invoice payment; ";
        }
        else
        {
            throw new ArgumentException("Unsupported payment method");
        }
        
        return (paymentFee, notes);
    }
    
}