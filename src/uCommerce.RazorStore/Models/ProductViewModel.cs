﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCommerce.Api;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.Models;

namespace UCommerce.RazorStore.Models
{

    public class ProductPageViewModel: ContentModel
    {
        public ProductPageViewModel() : base(Current.UmbracoContext.PublishedRequest.PublishedContent)
        {
            
        }

        public ProductViewModel ProductViewModel { get; set; }

        public bool AddedToBasket { get; set; }

        public bool ItemAlreadyExists { get; set; } 
    }

	public class ProductViewModel : ContentModel
	{
		public ProductViewModel() : base(Current.UmbracoContext.PublishedRequest.PublishedContent)
        {
			Variants = new List<ProductViewModel>();
            Properties = new List<ProductPropertiesViewModel>();
            Reviews = new List<ProductReviewViewModel>();

		}
        public bool IsVariant { get; set; }

        public bool IsProductFamily { get; set; }

        public string Name { get; set; }

		public string Url { get; set; }

		public string LongDescription { get; set; }

		public IList<ProductViewModel> Variants { get; set; }

		public string Sku { get; set; }

		public string VariantSku { get; set; }

		public PriceCalculation PriceCalculation { get; set; }

        public string ThumbnailImageUrl { get; set; }

        //new 

        public IList<ProductPropertiesViewModel> Properties { get; set; }

        public IList<ProductReviewViewModel> Reviews { get; set; }

        public string TaxCalculation { get; set; }

        public bool IsOrderingAllowed { get; set; }

    }

}
