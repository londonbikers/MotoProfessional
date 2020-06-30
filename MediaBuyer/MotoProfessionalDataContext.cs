using System;
using System.Configuration;

namespace MotoProfessional
{
    partial class MotoProfessionalDataContext
    {
        /// <summary>
        /// Constructor with dynamic connectionstring
        /// </summary>
		public MotoProfessionalDataContext()
			: base(ConfigurationManager.ConnectionStrings["MotoProfessionalDB"].ConnectionString, mappingSource)
        {
        }
    }
}