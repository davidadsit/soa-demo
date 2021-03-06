﻿using System;
using System.Collections.Generic;
using log4net;
using RabbitWrapper;

namespace PizzaCreationService
{
    public class RabbitQueueListener
    {
        static ILog logger = LogManager.GetLogger("RabbitQueueListener");
        static readonly ExchangeName PizzaRequestExchangeName = new ExchangeName("pizzarequested", 1);
        static readonly ExchangeName CouponIssuedExchangeName = new ExchangeName("couponissued", 1);

        readonly IRabbitMessageConsumer messageConsumer;
        readonly IQueueFactory queueFactory;
        readonly IRabbitMessagePublisher rabbitMessagePublisher;
        IDisposable pizzaRequestedMessageDisposable;
        static readonly HashSet<string> issuedCoupons = new HashSet<string>();

        public RabbitQueueListener(IRabbitMessageConsumer messageConsumer,
            IQueueFactory queueFactory,
            IRabbitMessagePublisher rabbitMessagePublisher)
        {
            this.messageConsumer = messageConsumer;
            this.queueFactory = queueFactory;
            this.rabbitMessagePublisher = rabbitMessagePublisher;
        }

        public void Start()
        {
            logger.Info("Waiting to bake pizzas");
            logger.Info("---------------------------------------------");
            logger.Info("");
            pizzaRequestedMessageDisposable = messageConsumer.Subscribe<PizzaRequestedMessage>(queueFactory.Create(PizzaRequestExchangeName, "kitchen", "chef1"), MandrillHandler);
        }

        public void Stop()
        {
            pizzaRequestedMessageDisposable.Dispose();
        }

        public void MandrillHandler(PizzaRequestedMessage message)
        {
            if (string.IsNullOrEmpty(message.Name))
            {
                logger.Error("Pizza requested with no name" + Environment.NewLine);
                return;
            }

            if (string.IsNullOrEmpty(message.Address))
            {
                logger.Error("Pizza requested with no address" + Environment.NewLine);
                return;                
            }

            if (message.Toppings.Length <= 0)
            {
                logger.Error("Pizza requested with no toppings" + Environment.NewLine);
                return;                                
            }

            bool hasValidCouponCode = issuedCoupons.Contains(BuildCouponHash(message.Address, message.Coupon));

            logger.InfoFormat("Pizza Baked!{0}For: {1}{0}Living at: {2}{0}With toppings: {3}{0}Cost: {4}{0}",
                Environment.NewLine,
                message.Name,
                message.Address,
                string.Join(", ", message.Toppings),
                hasValidCouponCode ? "FREE!!!!" : "$" + (10 + message.Toppings.Length));

            if (hasValidCouponCode)
            {
                issuedCoupons.Remove(BuildCouponHash(message.Address, message.Coupon));
            }
            else
            {
                string couponCode = Guid.NewGuid().ToString("D");
                string couponHash = BuildCouponHash(message.Address, couponCode);
                issuedCoupons.Add(couponHash);
                rabbitMessagePublisher.Publish(CouponIssuedExchangeName, new CouponIssuedMessage {CorrelationId = message.OrderId, Coupon = couponCode});
            }
        }

        static string BuildCouponHash(string address, string couponCode)
        {
            return address + couponCode;
        }
    }
}