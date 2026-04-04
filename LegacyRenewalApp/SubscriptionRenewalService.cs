using System;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        
        private IBillingService _billingService;
        private ISubscriptionPlanRepository _planRepository;
        private ICustomerRepository _customerRepository;
        private IInputManager _inputManager;
        private IDiscountService _discountService;
        private IFeeService _feeService;
        private ITaxService _taxService;
        
        public SubscriptionRenewalService() 
            : this(new LegacyBillingServiceAdapter(), 
                new SubscriptionPlanRepository(), 
                new CustomerRepository(),
                new  InputManager(),
                new DiscountService(),
                new FeeService(),
                new TaxService())
        {
        }

        public SubscriptionRenewalService(IBillingService billingService, ISubscriptionPlanRepository planRepository, 
            ICustomerRepository customerRepository,  IInputManager inputManager, IDiscountService discountService,  IFeeService feeService,
            ITaxService taxService)
        {
            _billingService = billingService;
            _planRepository = planRepository;
            _customerRepository = customerRepository;
            _inputManager = inputManager;
            _discountService =  discountService;
            _feeService = feeService;
            _taxService = taxService;
        }

        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            
            _inputManager.CheckInput(customerId, planCode, seatCount, paymentMethod);

            string normalizedPlanCode = planCode.Trim().ToUpperInvariant();
            string normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();
            
            var customer = _customerRepository.GetById(customerId);
            var plan = _planRepository.GetByCode(normalizedPlanCode);

            if (!customer.IsActive)
            {
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");
            }

            decimal baseAmount = (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;

            var discountResult = _discountService.CalculateDiscount(customer, plan, baseAmount, seatCount, useLoyaltyPoints);
            decimal discountAmount = discountResult.discountAmount;
            string notes = discountResult.notes;

            decimal subtotalAfterDiscount = baseAmount - discountAmount;
            if (subtotalAfterDiscount < 300m)
            {
                subtotalAfterDiscount = 300m;
                notes += "minimum discounted subtotal applied; ";
            }

            var supportFeeResult = _feeService.CalculateSupportFee(includePremiumSupport, normalizedPlanCode);
            decimal supportFee = supportFeeResult.supportFee;
            notes += supportFeeResult.notes;

            var paymentFeeResult = _feeService.CalculatePaymentFee(normalizedPaymentMethod,subtotalAfterDiscount,supportFee);
            decimal paymentFee = paymentFeeResult.paymentFee;
            notes += paymentFeeResult.notes;

            decimal taxRate = _taxService.CalculateTax(customer);
            
            decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
            decimal taxAmount = taxBase * taxRate;
            decimal finalAmount = taxBase + taxAmount;

            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                notes += "minimum invoice amount applied; ";
            }

            var invoice = new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{normalizedPlanCode}",
                CustomerName = customer.FullName,
                PlanCode = normalizedPlanCode,
                PaymentMethod = normalizedPaymentMethod,
                SeatCount = seatCount,
                BaseAmount = Math.Round(baseAmount, 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(supportFee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };

            _billingService.SaveInvoice(invoice);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                string subject = "Subscription renewal invoice";
                string body =
                    $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
                    $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

                _billingService.SendEmail(customer.Email, subject, body);
            }

            return invoice;
        }
    }
}
