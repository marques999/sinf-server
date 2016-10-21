namespace FirstREST.Lib_Primavera.Model
{
    public class Quote
    {
        public Address BillingAddress
        {
            get
            {
                return BillingAddress;
            }
            set
            {
                if (value != null)
                {
                    BillingAddress = value;
                }
            }
        }

        public Address ShippingAddress
        {
            get
            {
                return ShippingAddress;
            }
            set
            {
                if (value != null)
                {
                    ShippingAddress = value;
                }
            }
        }
    }
}