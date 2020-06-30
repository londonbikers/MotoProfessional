using System;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;

namespace App_Code
{
	/// <summary>
	/// Handles call-back messages from Google Checkout, alerting us of new orders, 
	/// changings in charge states, order-item states, etc.
	/// </summary>
	public class DownloadHandler : IHttpHandler, IReadOnlySessionState
	{
		#region members
		private HttpContext _context;
		private IMember _member;
		private IOrder _order;
		private int _orderId;
		private int _orderItemId;
		#endregion

        #region public methods
        /// <summary>
		/// Called by ASP.NET to process the http request. This is the equiv main function.
		/// </summary>
		public void ProcessRequest(HttpContext context)
		{
			_context = context;
			HandleRequest();
		}

		/// <summary>
		/// Determines is subsequent requests can reuse this handler.
		/// </summary>
		public bool IsReusable
		{
			get { return false; }
		}
		#endregion

		#region private methods
		private void HandleRequest()
		{
			// WORKFLOW
			// *****************************************************
			//-- validate the parameters.
			//-- is user logged in? (handler not covered by app ACL)
			//-- can we retreive the order?
			//-- does user have right to download from this order.
			//-- is order complete?
			//-- retrieve DigitalGood.
			//-- render to client.
			// *****************************************************

			// get params
			if (!ProcessParametersAndValidate())
			{
				RenderTextMessage("Invalid parameters.");
				return;
			}

			// we require a logged-in user.
			_member = Helpers.GetCurrentUser();
			if (_member == null)
			{
				RenderTextMessage("You must be signed-in to download anything.");
				return;
			}

			// attempt to retrieve the order.
			_order = Controller.Instance.CommerceController.GetOrder(_orderId);
			if (_order == null)
			{
				RenderTextMessage("Invalid order id.");
				return;
			}

			// does the user have the right to access the download?
			if (!CanUserAccessDownload())
			{
				RenderTextMessage("This is not one of you or your organisation's orders.");
				return;
			}

			// is the order complete?
			if (_order.ChargeStatus != ChargeStatus.Complete)
			{
				RenderTextMessage("The order is not complete yet.");
				return;
			}

			// render the good.
			var dg = RetrieveDigitalGood();
			if (dg == null)
				return;

			RenderDigitalGood(dg);
		}

		private bool ProcessParametersAndValidate()
		{
			var isValid = true;
			if (string.IsNullOrEmpty(_context.Request.QueryString["o"]) || !Common.IsNumeric(_context.Request.QueryString["o"]))
				isValid = false;

			_orderId = Convert.ToInt32(_context.Request.QueryString["o"]);

			if (!string.IsNullOrEmpty(_context.Request.QueryString["oi"]) && Common.IsNumeric(_context.Request.QueryString["oi"]))
				_orderItemId = Convert.ToInt32(_context.Request.QueryString["oi"]);

			return isValid;
		}

		private void RenderTextMessage(string message)
		{
			_context.Response.ContentType = "text/html";
			_context.Response.Write(message);
		}

		/// <summary>
		/// Determines whether or not the user has the right to access the download. This means whether
		/// or not they bought it or the organisation they're associated with did.
		/// </summary>
		private bool CanUserAccessDownload()
		{
			// staff?
			if (Roles.IsUserInRole("Administrators"))
				return true;

			if ((Guid)_order.Customer.MembershipUser.ProviderUserKey == (Guid)_member.MembershipUser.ProviderUserKey)
				return true;

		    // is this one of the member's organisation orders?
		    return _member.Company != null && _member.Company.Employees.Exists(ce => (Guid)ce.Member.MembershipUser.ProviderUserKey == (Guid)_order.Customer.MembershipUser.ProviderUserKey);
		}

		private IDigitalGood RetrieveDigitalGood()
		{
			IDigitalGood dg;
			if (_orderItemId > 0)
			{
				var oi = _order.Items.Find(qoi => qoi.Id == _orderItemId);
				if (oi == null)
				{
					RenderTextMessage("No such order item.");
					return null;
				}
			    if (oi.DigitalGood == null)
			    {
			        RenderTextMessage("No digital good exists for this item. Create one first!");
			        return null;
			    }

			    dg = oi.DigitalGood;
			}
			else
			{
				if (_order.MasterDigitalGood == null)
				{
					RenderTextMessage("No digital good for this order exists. Create one first!");
					return null;
				}

				dg = _order.MasterDigitalGood;
			}

			return dg;
		}

		private void RenderDigitalGood(IDigitalGood digitalGood)
		{
			switch (digitalGood.Type)
			{
			    case DigitalGoodType.ZipArchive:
			        _context.Response.ContentType = "application/zip";
			        break;
			    case DigitalGoodType.Photo:
			        _context.Response.ContentType = "image/jpeg";
			        break;
			}

			// record this download.
			var log = digitalGood.NewLog();
			log.CustomerUid = (Guid)_member.MembershipUser.ProviderUserKey;

			if (_context.Request.UrlReferrer != null)
				log.HttpReferrer = _context.Request.UrlReferrer.AbsoluteUri;

			log.ClientName = _context.Request.UserAgent;
            log.IpAddress = _context.Request.UserHostName;

			digitalGood.AddLog(log);

			_context.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0};", digitalGood.Filename));
			_context.Response.WriteFile(digitalGood.FullStorePath, false);
		}
		#endregion
	}
}