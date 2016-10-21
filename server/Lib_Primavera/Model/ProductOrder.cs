using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstREST.Lib_Primavera.Model
{
    public class ProductOrder
    {
        public ProductOrder(Product paramProduct, int paramQuantity)
        {
            Product = paramProduct;
            Quantity = paramQuantity;
        }

        public Product Product
        {
            get;
            set;
        }

        public int Quantity
        {
            get
            {
                return Quantity;
            }
            set
            {
                if (value < 0)
                {
                    Quantity = 0;
                }
                else
                {
                    Quantity = value;
                }
            }
        }
    }
}