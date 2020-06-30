using MotoProfessional.Controllers;

namespace MotoProfessional
{
	public class Controller
	{
		#region members
		private static readonly Controller _controller = new Controller();
        private readonly PhotoController _photoController;
		private readonly LicenseController _licenseController;
		private readonly CompanyController _companyController;
        private readonly PartnerController _partnerController;
        private readonly CommerceController _commerceController;
        private readonly PeripheralController _peripheralController;
        private readonly MemberController _memberController;
		private readonly BusinessRuleController _businessRuleController;
        private readonly DigitalGoodController _digitalGoodController;
		private readonly SystemController _systemController;
        private readonly Logging.Logger _logger;
		#endregion

		#region accessors
		public static Controller Instance { get { return _controller; } }
        public PhotoController PhotoController { get { return _photoController; } }
		public LicenseController LicenseController { get { return _licenseController; } }
        public CompanyController CompanyController { get { return _companyController; } }
        public CommerceController CommerceController { get { return _commerceController; } }
        public PartnerController PartnerController { get { return _partnerController; } }
        public PeripheralController PeripheralController { get { return _peripheralController; } }
        public MemberController MemberController { get { return _memberController; } }
		public BusinessRuleController BusinessRuleController { get { return _businessRuleController; } }
        public DigitalGoodController DigitalGoodController { get { return _digitalGoodController; } }
		public SystemController SystemController { get { return _systemController; } }
        /// <summary>
        /// Provides access to the application-logging functionality.
        /// </summary>
        public Logging.Logger Logger { get { return _logger; } }
		#endregion

		#region constructors
		protected Controller()
		{
            _logger = new Logging.Logger();
            Logger.LogInfo("MotoProfessional.Controller Initialising...");

            _photoController = new PhotoController();
			_licenseController = new LicenseController();
            _partnerController = new PartnerController();
            _commerceController = new CommerceController();
            _companyController = new CompanyController();
            _peripheralController = new PeripheralController();
            _memberController = new MemberController();
			_businessRuleController = new BusinessRuleController();
            _digitalGoodController = new DigitalGoodController();
			_systemController = new SystemController();
		}
		#endregion
    }
}