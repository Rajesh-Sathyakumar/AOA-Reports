using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SamlHelperLibrary;
using SamlHelperLibrary.Service;
using SamlHelperLibrary.Configuration;


using System.Web.Script.Services;
using System.Web.Services;
using System.Web;

namespace DAReportsAutomation
{
    public partial class Home : System.Web.UI.Page
    {
        public static string Email;

        public static string Username { get; set; }


        public static string DatabaseName;
        

        public readonly static string StgConn = ConfigurationManager.ConnectionStrings["ProductionConnectionString"].ToString();


        protected void Page_Init(object sender, EventArgs e)
        {
            var cas = new CasAuthenticationService(SamlHelperConfiguration.Config, UserSessionHandler.Get());

            if (!cas.IsSAMLResponse(new HttpContextWrapper(this.Context)))
            {
                cas.RedirectUserToCasLogin(
                    new Guid("5B95F3B2-C265-4E1A-91AB-60FC449E96EB"),
                new Guid("85346158-DB2E-49CE-80AC-0E868527DF2B"),
                new Guid("37B473AE-B5A5-4839-91D5-80676A86B4B9"),
               null);
            }
            else
            {
                var sessionInfo = cas.GetSessionFromSaml(new HttpContextWrapper(this.Context));
                Email = sessionInfo.userEmail;
                Username = sessionInfo.firstName;

                if (sessionInfo != null)
                {
                    HttpContext.Current.Session.Add("UserSessionInfo", sessionInfo);
                    HttpContext.Current.Session.Timeout = 20;
                }

            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetUsername()
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            return serializer.Serialize(Username);
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetMemberList()
        {
            try
            {

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(StgConn))
                {
                    string completeLogQuery = "SELECT ProjectName from MemberList (nolock)";

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
        public static string GetMaxDischargeDate()
        {
 
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(StgConn))
            {
                string completeLogQuery = "select convert(nvarchar(max),MAX(b.dischargedate)) as MaxDischargeDate from CCCPRDDBCLS01." + DatabaseName + ".dbo.Discharges a JOIN CCCPRDDBCLS01." + DatabaseName + ".dbo.DischargeDates b on a.Dischargedate= b.DischargeDatekey";

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
        public static string GetHosList(string ProjectName)
        {
            try
            {
                

                getDatabaseName(ProjectName);
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(StgConn))
                {
                    string completeLogQuery = "select HospitalName from CCCPRDDBCLS01." + DatabaseName + ".dbo.hospitals";

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
            catch(Exception e)
            {
                return e.Message + " <<   " + e.StackTrace; 
            }
            
        }

        private static void getDatabaseName(string ProjectName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(StgConn))
            {
                string completeLogQuery = "SELECT ProductionDatabase from MemberList (nolock) where ProjectName= '" + ProjectName + "'";

                using (SqlCommand cmd = new SqlCommand(completeLogQuery, con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {

                        foreach (DataColumn col in dt.Columns)
                        {
                            DatabaseName = dr[col].ToString();
                        }
                    }
                }
            }
        }



        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string Register(string hospital, string startdate, string enddate)
        {
            try
            {

                DataTable dataTable = new DataTable();

                using (SqlConnection conn = new SqlConnection(StgConn))
                {
                    conn.Open();

                    string InputForPPTGen = "insert into InputForReportReadmissions ( [DatabaseName], [Hospital], [Startdate],[Enddate] , [UserName], [status],[email])values('" + DatabaseName + "'" + "," + "'" + hospital + "'" + "," + "'" + startdate + "'" + "," + "'" + enddate + "'" + "," + "'" + Username + "'" + "," + "'" + "-1" + "'" + "," + "'" + Email + "'" + ") select SCOPE_IDENTITY() As FileToBeSearched;";
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