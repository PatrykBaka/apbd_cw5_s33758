namespace LegacyRenewalApp;

public interface IDiscountService
{

    public (decimal discountAmount, string notes) CalculateDiscount(Customer customer, 
        SubscriptionPlan plan, 
        decimal baseAmount, 
        int seatCount, 
        bool useLoyaltyPoints);

}