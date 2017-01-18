using SamlHelperLibrary;
using SamlHelperLibrary.Configuration;
using SamlHelperLibrary.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Automated_Opportunity_Analysis_v1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Session.Clear();
            
            HttpContext.Current.Response.Redirect(@"https://loginbeta.advisory.com/cas/logout");
        }
    }
}