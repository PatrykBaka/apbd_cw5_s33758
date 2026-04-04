namespace LegacyRenewalApp;

public class TaxService : ITaxService
{

    public decimal CalculateTax(Customer customer)
    {

        decimal taxRate = 0;
        
        if (customer.Country == "Poland")
        {
            taxRate = 0.23m;
        }
        else if (customer.Country == "Germany")
        {
            taxRate = 0.19m;
        }
        else if (customer.Country == "Czech Republic")
        {
            taxRate = 0.21m;
        }
        else if (customer.Country == "Norway")
        {
            taxRate = 0.25m;
        }
        else
        {
            taxRate = 0.20m;
        }

        return taxRate;
    }
    
}