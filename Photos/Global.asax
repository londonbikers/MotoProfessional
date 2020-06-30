<%@ Application Language="C#" %>
<%@ Import Namespace="App_Code" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
    }
    
    void Application_End(object sender, EventArgs e) 
    {
    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs
		var ex = Server.GetLastError();
		var message = "Unhandled Site Exception - Url: " + Request.ServerVariables["HTTP_X_REWRITE_URL"];

		if (Request.UrlReferrer != null)
			message += string.Format("\nReferrer: '{0}'", Request.UrlReferrer.AbsoluteUri);

		if (User != null && User.Identity != null)
			message += string.Format("\nUser name: '{0}'\nUser.IsAuthenticated: {1}", User.Identity.Name, User.Identity.IsAuthenticated);
		
        Logger.LogError(message, ex);
		Server.Transfer("~/Whoops.aspx");
    }

    void Session_Start(object sender, EventArgs e) 
    {
    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
    }

    protected void Application_BeginRequest(Object sender, EventArgs e)
    {
        var url = Request.Url.AbsoluteUri.ToLower();

        // check to see if the visitor is entering the www. domain, if so, get them onto the base domain.
        // this is so that there's no trouble with cookies and sessions across domains.

        if (url.IndexOf("http://www.") > -1)
            Response.Redirect(url.Replace("http://www.", "http://").Replace("default.aspx", String.Empty));
    }

    /// <summary>
    /// Handles the ASPNET user being associated with the MotoProfessional Member object and then stored in Session.
    /// </summary>
    void Application_PreRequestHandlerExecute(object sender, EventArgs ea)
    {
        try
        {
            if (Session["Member"] == null)
            {
                var membershipUser = Membership.GetUser();
                if (membershipUser != null)
                {
                    var member = MotoProfessional.Controller.Instance.MemberController.GetMember(membershipUser);
                    if (member != null)
                    {
						// keep a track of their IP address in the Member object for auditing.
						member.IpAddress = Request.UserHostAddress;
                        Session["Member"] = member;
                    }
                    else
                    {
                        Logger.LogError("Null Member returned from controller on login.");
                    }
                }
            }
        }
        catch (HttpException)
        {
            // some contexts don't have session support, so this is an acceptable failure.
        }
    }

	void Application_PostMapRequestHandler(object sender, EventArgs e)
	{
		// sort out the re-written paths so forms post-back and ASPNET uses the right path for Themes and the like.
		var originalUrl = Request.ServerVariables["HTTP_X_REWRITE_URL"];

		// don't rewrite compact default queries, i.e. "search/?t=brands"; doing so would break the form element action param.
 		if (string.IsNullOrEmpty(originalUrl) || originalUrl.Contains("/?")) return;
		
 		// required to get <form> working!
 		if (originalUrl.EndsWith("/"))
 			originalUrl += "default.aspx";

 		Context.RewritePath(originalUrl, true);
	}
       
</script>