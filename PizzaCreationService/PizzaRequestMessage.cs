﻿namespace PizzaCreationService
{
    public class PizzaRequestedMessage
    {
        public string OrderId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string[] Toppings { get; set; }
        public string Coupon { get; set; }
    }
}