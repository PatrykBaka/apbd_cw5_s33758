using System;

namespace LegacyRenewalApp;

public class DiscountService : IDiscountService
{

    public (decimal discountAmount, string notes) CalculateDiscount(Customer customer, 
        SubscriptionPlan plan, 
        decimal baseAmount, 
        int seatCount, 
        bool useLoyaltyPoints)
    {
        decimal totalDiscount = 0;
        string allNotes =  "";
        
        var segment =  CalculateDiscountSegment(customer, plan, baseAmount);
        var years = CalculateDiscountYears(customer, baseAmount);
        var seat = CalculateDiscountSeat(seatCount, baseAmount);
        var loyality = CalculateDiscountLoyality(customer, baseAmount, useLoyaltyPoints);

        totalDiscount = segment.discountAmount + years.discountAmount + seat.discountAmount + loyality.discountAmount;
        
        allNotes += segment.notes + years.notes +  seat.notes + loyality.notes;
        
        return (totalDiscount, allNotes);

    }
    

    public (decimal discountAmount, string notes) CalculateDiscountSegment(Customer customer,SubscriptionPlan plan, decimal baseAmount)
    {
        
        decimal discountAmount = 0;
        string notes = String.Empty;
        
        if (customer.Segment == "Silver")
        {
            discountAmount += baseAmount * 0.05m;
            notes += "silver discount; ";
        }
        else if (customer.Segment == "Gold")
        {
            discountAmount += baseAmount * 0.10m;
            notes += "gold discount; ";
        }
        else if (customer.Segment == "Platinum")
        {
            discountAmount += baseAmount * 0.15m;
            notes += "platinum discount; ";
        }
        else if (customer.Segment == "Education" && plan.IsEducationEligible)
        {
            discountAmount += baseAmount * 0.20m;
            notes += "education discount; ";
        }
        
        return (discountAmount, notes);
    }

    public (decimal discountAmount, string notes) CalculateDiscountYears(Customer customer, decimal baseAmount)
    {

        decimal discountAmount = 0;
        string notes = String.Empty;
        
        if (customer.YearsWithCompany >= 5)
        {
            discountAmount += baseAmount * 0.07m;
            notes += "long-term loyalty discount; ";
        }
        else if (customer.YearsWithCompany >= 2)
        {
            discountAmount += baseAmount * 0.03m;
            notes += "basic loyalty discount; ";
        }
        
        return (discountAmount, notes);
    }

    public (decimal discountAmount, string notes) CalculateDiscountSeat(int seatCount, decimal  baseAmount)
    {
        
        decimal discountAmount = 0;
        string notes = String.Empty;
        
        if (seatCount >= 50)
        {
            discountAmount += baseAmount * 0.12m;
            notes += "large team discount; ";
        }
        else if (seatCount >= 20)
        {
            discountAmount += baseAmount * 0.08m;
            notes += "medium team discount; ";
        }
        else if (seatCount >= 10)
        {
            discountAmount += baseAmount * 0.04m;
            notes += "small team discount; ";
        }
        
        return  (discountAmount, notes);
    }

    public (decimal discountAmount, string notes) CalculateDiscountLoyality(Customer customer, decimal baseAmount,bool useLoyaltyPoints)
    {
        
        decimal discountAmount = 0;
        string notes = String.Empty;
        
        if (useLoyaltyPoints && customer.LoyaltyPoints > 0)
        {
            int pointsToUse = customer.LoyaltyPoints > 200 ? 200 : customer.LoyaltyPoints;
            discountAmount += pointsToUse;
            notes += $"loyalty points used: {pointsToUse}; ";
        }
        
        return (discountAmount, notes);
    }
    
}