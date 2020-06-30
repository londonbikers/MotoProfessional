namespace MotoProfessional
{
	/// <summary>
	/// Defines how the client has been authorised to pay (or not) for their downloads.
	/// </summary>
	public enum ClientPaymentTerms
	{
		/// <summary>
		/// The default payment terms are to pay for the order at the point of sale.
		/// </summary>
		PointOfSale = 0,
		/// <summary>
		/// Authorised clients will be able to complete their orders immediately without payment and then be able to settle their account later.
		/// </summary>
		Invoiced = 1,
		/// <summary>
		/// Select clients are not charged for use of the service. This will allow them to complete their orders without payment.
		/// </summary>
		NoCharge = 2
	}

	public enum PhotoSearchProperty
	{
		Tags = 0,
		Name = 1,
		Comment = 2
	}

    public enum DigitalGoodType
    {
        Photo = 0,
        ZipArchive = 1
    }

	public enum OrderTransactionType
	{
		GC_NewOrder = 0,
		GC_OrderStateChange = 1,
		GC_RefundAmount = 2,
		GC_ChargebackAmount = 3,
        GC_RiskInformation = 4,
        GC_ChargeAmount = 5,
		Generic = 6
	}

	/// <summary>
	/// At times the application needs to know how to construct client-application url's and this helps
	/// to differentiate between urls.
	/// </summary>
	public enum ClientUrlPage
	{
		OrderPage
	}

	public enum ProductType
	{
		PhotoProduct = 0
	}

    public enum PersonSex
    {
        Unknown = 0,
        Male = 1,
        Female = 2
    }

    public enum CompanyEmployeeStatus
    {
        Pending = 0,
        Confirmed = 1
    }

    public enum OrderItemStatus
    {
        Normal = 0,
        Refunded = 1
    }

    public enum ChargeStatus
    {
        Outstanding = 0,
        Complete = 1,
        Refunded = 2,
		PartialRefund = 3,
		ChargeBack = 4
    }

    public enum ChargeMethod
    {
        PointOfSale = 0,
        Invoiced = 1,
		NoCharge = 2
    }

	public enum PhotoOrientation
	{
		Landscape = 0,
		Portrait = 1,
		Square = 2,
        NotDefined = 3
	}

	/// <summary>
	/// Denotes what the state of a class being instantiated is.
	/// </summary>
	public enum ClassMode
	{
		New,
		Existing
	}

	public enum DomainObject
	{
		Unknown = 0,
		Photo = 1,
		Partner = 2,
		Company = 3,
		Basket = 4,
        BasketItem = 5,
        Collection = 6,
        License = 7,
        Order = 8,
        OrderItem = 9,
        OrderTransaction = 10,
        Country = 13,
		RateCard = 14,
		RateCardItem = 15,
        Profile = 16,
        DigitalGood = 17,
		Member = 18
	}

	public enum GeneralStatus
	{
		New = 0,
		Active = 1,
		Disabled = 2
	}

	public enum BasketStatus
	{
		Current = 0,
		Saved = 1
	}

    public enum HotPhotosMode
    {
        TopSellingPhotos,
        TopViewedPhotos,
        RandomFromLatestCollections
    }

    public enum SiteMapItemContentType
    {
        Collection,
        Photo,
        Tag
    }
}