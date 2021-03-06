﻿using System.Linq;
using System.Net;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.RazorStore.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
{
	public class BasketpreviewController : RenderMvcController
    {
        [HttpGet]
        public override ActionResult Index(ContentModel m)
		{
			PurchaseOrderViewModel model = MapOrder();
			return View("/Views/Preview.cshtml", model);
		}

		private PurchaseOrderViewModel MapOrder()
		{
			PurchaseOrder basket = TransactionLibrary.GetBasket(false).PurchaseOrder;

			var basketModel = new PurchaseOrderViewModel();

            basketModel.BillingAddress = TransactionLibrary.GetBillingInformation();
		    basketModel.ShipmentAddress = TransactionLibrary.GetShippingInformation();

            foreach (var orderLine in basket.OrderLines)
			{
				var orderLineModel = new OrderlineViewModel();
				orderLineModel.ProductName = orderLine.ProductName;
				orderLineModel.Sku = orderLine.Sku;
				orderLineModel.VariantSku = orderLine.VariantSku;
				orderLineModel.Total = 
                    new Money(orderLine.Total.GetValueOrDefault(), orderLine.PurchaseOrder.BillingCurrency).ToString();
                orderLineModel.Tax = 
                    new Money(orderLine.VAT, basket.BillingCurrency).ToString();
                orderLineModel.Price = 
                    new Money(orderLine.Price, basket.BillingCurrency).ToString();
                orderLineModel.PriceWithDiscount = 
                    new Money(orderLine.Price - orderLine.Discount, basket.BillingCurrency).ToString();
				orderLineModel.Quantity = orderLine.Quantity;
			    orderLineModel.Discount = orderLine.Discount;

				basketModel.OrderLines.Add(orderLineModel);
			}

			basketModel.DiscountTotal = new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketModel.DiscountAmount = basket.DiscountTotal.GetValueOrDefault();
            basketModel.SubTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
			basketModel.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
			basketModel.TaxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
			basketModel.ShippingTotal = new Money(basket.ShippingTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
			basketModel.PaymentTotal = new Money(basket.PaymentTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
		    

		    var shipment = basket.Shipments.FirstOrDefault();
		    if (shipment != null)
		    {
                basketModel.ShipmentName = shipment.ShipmentName;
		        basketModel.ShipmentAmount = basket.ShippingTotal.GetValueOrDefault();
		    }

		    var payment = basket.Payments.FirstOrDefault();
		    if (payment != null)
		    {
		        basketModel.PaymentName = payment.PaymentMethodName;
		        basketModel.PaymentAmount = basket.PaymentTotal.GetValueOrDefault();
		    }

            ViewBag.RowSpan = 4;
            if (basket.DiscountTotal > 0)
            {
                ViewBag.RowSpan++;
            }
            if (basket.ShippingTotal > 0)
            {
                ViewBag.RowSpan++;
            }
            if (basket.PaymentTotal > 0)
            {
                ViewBag.RowSpan++;
            }

            return basketModel;
		}
		
		[HttpPost]
		public ActionResult Index()
		{
			TransactionLibrary.RequestPayments();
			
			var payment = TransactionLibrary.GetBasket().PurchaseOrder.Payments.First();
			
			if (payment.PaymentMethod.PaymentMethodServiceName == null)
			{
				var parent = PublishedRequest.PublishedContent.AncestorOrSelf("basket");
				var confirmation = parent.Children(x => x.Name == "Confirmation").FirstOrDefault();
				return Redirect(confirmation.Url);
			}
			
			string paymentUrl = TransactionLibrary.GetPaymentPageUrl(payment);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            return Redirect(paymentUrl);
        }
    }
}