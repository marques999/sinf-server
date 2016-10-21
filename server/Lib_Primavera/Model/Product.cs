using System.Collections.Generic;

namespace FirstREST.Lib_Primavera.Model
{
    public class Product
    {
        public string ID
        {
            get;
            set;
        }

        public string Nome
        {
            get;
            set;
        }

        public double Cost
        {
            get
            {
                return Cost;
            }
            set
            {
                if (value < 0.0)
                {
                    Cost = 0.0;
                }
                else
                {
                    Cost = value;
                }
            }
        }

        public double Price
        {
            get
            {
                return Price;
            }
            set
            {
                if (value < 0.0)
                {
                    Price = 0.0;
                }
                else
                {
                    Price = value;
                }
            }
        }

        public double Tax
        {
            get
            {
                return Tax;
            }
            set
            {
                if (value < 0.0)
                {
                    Tax = 0.0;
                }
                else if (value > 1.0)
                {
                    Tax = 1.0;
                }
                else
                {
                    Tax = value;
                }
            }
        }

        public double Discount 
        {
            get
            {
                return Discount;
            }
            set
            {
                if (value < 0.0)
                {
                    Discount = 0.0;
                }
                else if (value > 1.0)
                {
                    Discount = 1.0;
                }
                else
                {
                    Discount = value;
                }
            }
        }

        public IEnumerable<Warehouse> Warehouses
        {
            get;
            set;
        }
    }
}