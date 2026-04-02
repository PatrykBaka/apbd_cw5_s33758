namespace LegacyRenewalApp;

public interface IInputManager
{
    
    void CheckInput(int customerId, string planCode, int seatCount, string paymentMethod);

}