using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using SamlHelperLibrary;
using SamlHelperLibrary.Service;
using SamlHelperLibrary.Configuration;

using System.Text;

using System.Web.Script.Services;
using System.Web.Services;
using System.Web;

namespace DAReportsAutomation
{
    public partial class Home : System.Web.UI.Page
    {
        public static string Username = "Rajesh";
        public static string Email;
        public readonly static string StgConn = ConfigurationManager.ConnectionStrings["ProductionConnectionString"].ToString();
        public static string DomainControllerName { get; private set; }

        public static string ComputerName { get; private set; }

        public static string DomainName { get; private set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            var cas = new CasAuthenticationService(SamlHelperConfiguration.Config, UserSessionHandler.Get());            
            if(!cas.IsSAMLResponse(new HttpContextWrapper(this.Context)))
            {
                    cas.RedirectUserToCasLogin(
                        new Guid("5B95F3B2-C265-4E1A-91AB-60FC449E96EB"),
                    new Guid("85346158-DB2E-49CE-80AC-0E868527DF2B"),
                    new Guid("37B473AE-B5A5-4839-91D5-80676A86B4B9"),                   
                   null);                
            } else
            {
                var sessionInfo = cas.GetSessionFromSaml(new HttpContextWrapper(this.Context));                
            }
            //throw new Exception("Unable to authenticate");
        }

        public static string DomainPath
        {
            get
            {
                bool bFirst = true;
                StringBuilder sbReturn = new StringBuilder(200);
                string[] strlstDc = DomainName.Split('.');
                foreach (string strDc in strlstDc)
                {
                    if (bFirst)
                    {
                        sbReturn.Append("DC=");
                        bFirst = false;
                    }
                    else
                        sbReturn.Append(",DC=");

                    sbReturn.Append(strDc);
                }
                return sbReturn.ToString();
            }
        }

        public static string RootPath
        {
            get
            {
                return string.Format("LDAP://{0}/{1}", DomainName, DomainPath);
            }
        }

        public static string GetProperty(SearchResult searchResult, string PropertyName)
        {
            if (searchResult.Properties.Contains(PropertyName))
            {
                return searchResult.Properties[PropertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetDBList()
        {
            try
            {
                Domain domain = null;
                DomainController domainController = null;
                try
                {
                    domain = Domain.GetCurrentDomain();
                    DomainName = domain.Name;
                    domainController = domain.PdcRoleOwner;
                    DomainControllerName = domainController.Name.Split('.')[0];
                    ComputerName = Environment.MachineName;
                }
                finally
                {
                    if (domain != null)
                        domain.Dispose();
                    if (domainController != null)
                        domainController.Dispose();
                }

                DirectoryEntry entry = new DirectoryEntry(RootPath);
                Console.WriteLine(RootPath);
                DirectorySearcher dSearch = new DirectorySearcher(entry);
                string[] Name = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\');
                dSearch.Filter = string.Format("(&(objectCategory=person)(objectClass=user)(SAMAccountname={0}))", Name[1]);

                foreach (SearchResult sResultSet in dSearch.FindAll())
                {

                    // Login Name


                    // email address
                    Email = GetProperty(sResultSet, "mail");
                    // Challenge Question

                }

                Email = "sathyakr@advisory.com";


                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(StgConn))
                {
                    string completeLogQuery = "select Name from CCCPRDDBCLS01.master.sys.databases where name like '%PRD%' ";

                    using (SqlCommand cmd = new SqlCommand(completeLogQuery, con))
                    {
                        con.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                        Dictionary<string, object> row;
                        foreach (DataRow dr in dt.Rows)
                        {
                            row = new Dictionary<string, object>();
                            foreach (DataColumn col in dt.Columns)
                            {
                                row.Add(col.ColumnName, dr[col]);
                            }
                            rows.Add(row);
                        }

                        return serializer.Serialize(rows);
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message + " >> << " + ex.StackTrace + " >> << " + ex.ToString();
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetMaxDischargeDate(string databasename)
        {
 
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(StgConn))
            {
                string completeLogQuery = "select convert(nvarchar(max),MAX(b.dischargedate)) as MaxDischargeDate from CCCPRDDBCLS01." + databasename + ".dbo.Discharges a JOIN CCCPRDDBCLS01." + databasename + ".dbo.DischargeDates b on a.Dischargedate= b.DischargeDatekey";

                using (SqlCommand cmd = new SqlCommand(completeLogQuery, con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }

                    return serializer.Serialize(rows);
                }
            }


        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetHosList(string databasename)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(StgConn))
            {
                string completeLogQuery = "select HospitalName from CCCPRDDBCLS01." + databasename + ".dbo.hospitals";

                using (SqlCommand cmd = new SqlCommand(completeLogQuery, con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }

                    return serializer.Serialize(rows);
                }
            }
        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string Register(string databaseName, string hospital, string startdate, string enddate)
        {
            try
            {
                
                DataTable dataTable = new DataTable();

                using (SqlConnection conn = new SqlConnection(StgConn))
                {
                    conn.Open();

                    string InputForPPTGen = "insert into InputForReportReadmissions ( [DatabaseName], [Hospital], [Startdate],[Enddate] , [UserName], [status],[email])values('" + databaseName + "'" + "," + "'" + hospital + "'" + "," + "'" + startdate + "'" + "," + "'" + enddate + "'" + "," + "'" + Username + "'" + "," + "'" + "-1" + "'" + "," + "'" + Email + "'" + ") select SCOPE_IDENTITY() As FileToBeSearched;";
                    SqlCommand cmd = new SqlCommand(InputForPPTGen, conn);

                    Int32 PPTId = Convert.ToInt32(cmd.ExecuteScalar());

                    conn.Close();

                    int StatusOfFIle = -1;

                    while (StatusOfFIle != 1 && StatusOfFIle != 2)
                    {
                        string FileSearchQuery = "select status from InputForReportReadmissions where InputUserID= " + PPTId;

                        SqlConnection connection = new SqlConnection(StgConn);

                        connection.Open();

                        SqlCommand command = new SqlCommand(FileSearchQuery, connection);

                        StatusOfFIle = (Int32)command.ExecuteScalar();


                        connection.Close();
                    }

                    if (StatusOfFIle == 1)
                    {
                        return "Success";
                    }
                    else
                    {
                        return "Failure";
                    }
                }
            }
            catch (Exception e)
            {
                return "Test run fail";
            }
        }
    }
}