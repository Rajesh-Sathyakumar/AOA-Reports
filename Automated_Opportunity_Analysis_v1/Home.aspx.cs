using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using SamlHelperLibrary;
using SamlHelperLibrary.Service;
using SamlHelperLibrary.Configuration;
using xlNS = Microsoft.Office.Interop.Excel;

using System.Web.Script.Services;
using System.Web.Services;
using System.Web;
using SamlHelperLibrary.Models;

namespace DAReportsAutomation
{
    public partial class Home : System.Web.UI.Page
    {
        public readonly static string ProdConn = ConfigurationManager.ConnectionStrings["ProductionConnectionString"].ToString();
        public readonly static string StgConn = ConfigurationManager.ConnectionStrings["StagingConnectionString"].ToString();


        protected void Page_Init(object sender, EventArgs e)
        {
            //HttpContext.Current.Session.Add("UserSessionInfo", new UserSessionInfo()
            //{
            //    userEmail = "sathyakr@advisory.com",
            //    userName = "Rajesh"
            //});
            //return;

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
               
                if (sessionInfo != null)
                {
                    HttpContext.Current.Session.Add("UserSessionInfo", sessionInfo);
                    HttpContext.Current.Session.Timeout = 20;
                }

            }
        }


        public static void Excelpaste(xlNS.Sheets sheet1, string tabname, DataSet dset, int rowstartposition, int ColumnStartposition)
        {
            Microsoft.Office.Interop.Excel.Range range;
            try
            {
                System.Data.DataTable dtable = dset.Tables[0];
                ADODB.Recordset rs;
                xlNS.Worksheet targetSheet = null;
                rs = ConvertToRecordset(dtable);

                targetSheet = (Microsoft.Office.Interop.Excel.Worksheet)sheet1.get_Item(tabname);
                range = (xlNS.Range)targetSheet.Cells[rowstartposition, ColumnStartposition];
                range.CopyFromRecordset(rs, dtable.Rows.Count, dtable.Columns.Count);
                //Console.WriteLine(tabname+ "ran");  

                try
                {
                    if (targetSheet != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(targetSheet);
                    targetSheet = null;
                }
                catch (Exception)
                {
                    targetSheet = null;
                }
                finally
                {
                    GC.Collect();
                }


            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex);
            }
            finally
            {
                range = null;
            }

        }

        public static ADODB.Recordset ConvertToRecordset(DataTable inTable)
        {
            ADODB.Recordset result = new ADODB.Recordset();
            result.CursorLocation = ADODB.CursorLocationEnum.adUseClient;

            ADODB.Fields resultFields = result.Fields;
            System.Data.DataColumnCollection inColumns = inTable.Columns;

            foreach (DataColumn inColumn in inColumns)
            {

                resultFields.Append(inColumn.ColumnName
                    , TranslateType(inColumn.DataType)
                    , inColumn.MaxLength
                    , inColumn.AllowDBNull ? ADODB.FieldAttributeEnum.adFldIsNullable :
                                             ADODB.FieldAttributeEnum.adFldUnspecified
                    , null);
            }

            result.Open(System.Reflection.Missing.Value
                    , System.Reflection.Missing.Value
                    , ADODB.CursorTypeEnum.adOpenStatic
                    , ADODB.LockTypeEnum.adLockOptimistic, 0);

            foreach (DataRow dr in inTable.Rows)
            {
                result.AddNew(System.Reflection.Missing.Value,
                              System.Reflection.Missing.Value);

                for (int columnIndex = 0; columnIndex < inColumns.Count; columnIndex++)
                {
                    resultFields[columnIndex].Value = dr[columnIndex];
                }
            }

            return result;
        }

        //To handle the datatype during conversion of dataset into recordset
        static ADODB.DataTypeEnum TranslateType(Type columnType)
        {
            switch (columnType.UnderlyingSystemType.ToString())
            {
                case "System.Boolean":
                    return ADODB.DataTypeEnum.adBoolean;

                case "System.Byte":
                    return ADODB.DataTypeEnum.adUnsignedTinyInt;

                case "System.Char":
                    return ADODB.DataTypeEnum.adChar;

                case "System.DateTime":
                    return ADODB.DataTypeEnum.adDate;

                case "System.Decimal":
                    return ADODB.DataTypeEnum.adCurrency;

                case "System.Double":
                    return ADODB.DataTypeEnum.adDouble;

                case "System.Int16":
                    return ADODB.DataTypeEnum.adSmallInt;

                case "System.Int32":
                    return ADODB.DataTypeEnum.adInteger;

                case "System.Int64":
                    return ADODB.DataTypeEnum.adBigInt;

                case "System.SByte":
                    return ADODB.DataTypeEnum.adTinyInt;

                case "System.Single":
                    return ADODB.DataTypeEnum.adSingle;

                case "System.UInt16":
                    return ADODB.DataTypeEnum.adUnsignedSmallInt;

                case "System.UInt32":
                    return ADODB.DataTypeEnum.adUnsignedInt;

                case "System.UInt64":
                    return ADODB.DataTypeEnum.adUnsignedBigInt;

                case "System.String":
                default:
                    return ADODB.DataTypeEnum.adVarChar;
            }
        }




        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetUsername()
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            return serializer.Serialize(((UserSessionInfo)HttpContext.Current.Session["UserSessionInfo"]).firstName);
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetMemberList()
        {
            try
            {

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(ProdConn))
                {
                    string completeLogQuery = "SELECT ProjectName from MemberList (nolock) ORDER BY ProjectName";

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
            var databaseName = HttpContext.Current.Session["DatabaseName"];

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(StgConn))
            {
                string completeLogQuery = "select convert(nvarchar(max),MAX(b.dischargedate)) as MaxDischargeDate from " + databaseName + ".dbo.Discharges (NoLock) a JOIN " + databaseName + ".dbo.DischargeDates (NoLock) b on a.Dischargedate= b.DischargeDatekey";

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
                    string completeLogQuery = "select HospitalKey, HospitalName from " + HttpContext.Current.Session["DatabaseName"] + ".dbo.Hospitals (NoLock)";

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


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetPayerList()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(StgConn))
                {
                    string completeLogQuery =
                        @"select distinct PayorClassKey as PK, payorsummarydescription +'-'+payorclassdescription as PD
                                        from " + HttpContext.Current.Session["DatabaseName"] + @".dbo.payorclasses (NOLOCK)
                                        JOIN "+ HttpContext.Current.Session["DatabaseName"]+ @".DBO.DISCHARGES (NOLOCK) ON DISCHARGES.PAYORCLASS = PAYORCLASSES.PAYORCLASSKEY
                                        WHERE DISCHARGINGSERVICE = 0
                                        order by PD"; 

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
            catch (Exception e)
            {
                return e.Message + " <<   " + e.StackTrace;
            }

        }

        private static void getDatabaseName(string ProjectName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ProdConn))
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
                            HttpContext.Current.Session.Add("DatabaseName", dr[col].ToString());
                        }
                    }
                }
            }
        }



        [WebMethod]
        public static string Register(string hospital, string startdate, string enddate, string aprdrgCheck,string payers)
        {
            try
            {
                var sessionInfo = ((UserSessionInfo) HttpContext.Current.Session["UserSessionInfo"]);

                DataTable dataTable = new DataTable();

                using (SqlConnection conn = new SqlConnection(ProdConn))
                {
                    conn.Open();

                    string InputForPPTGen = @"Insert into InputForReportReadmissions ( 
                            [DatabaseName], [Hospital], [Startdate],[Enddate] , [UserName], [status],[email], [aprdrgw/excludes],[PayerKeys])values('" 
                            + HttpContext.Current.Session["DatabaseName"] + "'" + "," + 
                            "'" + hospital + "'" + "," + "'" + startdate + "'" + "," + "'" + enddate + "'" + "," 
                            + "'" + sessionInfo.userName + "'" + "," + "'" + "-1" + "'" + "," + "'" + sessionInfo.userEmail + "'" +","
                            +"'" + aprdrgCheck + "'" + ","
                            + "'" + payers + "'" +") select SCOPE_IDENTITY() As FileToBeSearched;";
                    SqlCommand cmd = new SqlCommand(InputForPPTGen, conn);

                    Int32 PPTId = Convert.ToInt32(cmd.ExecuteScalar());

                    conn.Close();

                    int StatusOfFIle = -1;

                    while (StatusOfFIle != 1 && StatusOfFIle != 2)
                    {
                        string FileSearchQuery = "select status from InputForReportReadmissions (NoLock) WHERE InputUserID= " + PPTId;

                        SqlConnection connection = new SqlConnection(ProdConn);

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


        protected void Button1_Click(object sender, EventArgs e)
        {

            //xlNS.Application excelApplication = null;
            //xlNS.Workbook excelWorkBook = null;
            //excelApplication = new xlNS.Application();

            //DateTime date = new DateTime();
            //string Time = DateTime.Now.ToString("ddMMMyyyy.hh.m.s tt");

            //string UsageLogsTemplatePath = ConfigurationManager.AppSettings["UsageLogsTemplatePath"];
            //string UsageLogPath = ConfigurationManager.AppSettings["UsageLogPath"];
            //string filename = "AOA_UsageLogs";

            //string currentWorkbookPath = UsageLogPath + filename + "_" + Time + ".xlsx";

            //File.Copy(UsageLogsTemplatePath, currentWorkbookPath);

            ////Below scripts adds an instance instead of opening the template
            //excelWorkBook = excelApplication.Workbooks.Open(currentWorkbookPath);
            //excelApplication.Visible = false;
            //excelApplication.DisplayAlerts = false;



            //using (SqlConnection con = new SqlConnection(ProdConn))
            //{
            //    string completeLogQuery = "EXEC AOA_UsageLogs";

            //    using (SqlCommand cmd = new SqlCommand(completeLogQuery, con))
            //    {
            //        con.Open();
            //        SqlDataAdapter da = new SqlDataAdapter(cmd);
            //        DataSet ds = new DataSet();
            //        da.Fill(ds);

            //        DataSet dsTemp = new DataSet();
            //        dsTemp.Tables.Add(ds.Tables[0].Copy());

            //        Excelpaste(excelWorkBook.Worksheets, "Raw Data Logs", dsTemp, 2, 1);
            //        dsTemp.Clear();
            //        dsTemp.Tables.Add(ds.Tables[1].Copy());
            //        Excelpaste(excelWorkBook.Worksheets, "Usage Data Stats", dsTemp, 2, 1);
            //        dsTemp.Clear();
            //        dsTemp.Tables.Add(ds.Tables[2].Copy());
            //        Excelpaste(excelWorkBook.Worksheets, "Usage Data Stats", dsTemp, 2, 5);
            //        dsTemp.Dispose();
            //        con.Close();

                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.Buffer = true;
                    HttpContext.Current.Response.Charset = "";
                    HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                    HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=AOA_UsageLogs.xlsx");
                    using (MemoryStream myMemoryStream = new MemoryStream())
                    {
                        //excelWorkBook.SaveAs(myMemoryStream);
                        //HttpContext.Current.Response.TransmitFile();
                        myMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);

                    }

                    //excelWorkBook.Save();
                    //excelWorkBook.Close();
                    //excelApplication.Quit();
                    HttpContext.Current.Response.TransmitFile("AOA_UsageLogs.xlsx");
                }
            //}

            //try
            //{
            //    if (excelWorkBook != null)
            //        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkBook);
            //    excelWorkBook = null;
            //}
            //catch (Exception)
            //{
            //    excelWorkBook = null;
            //}
            //finally
            //{
            //    GC.Collect();
            //}
            //Console.WriteLine("Clearing Excel");
            //try
            //{
            //    if (excelApplication != null)
            //        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
            //    excelApplication = null;
            //}
            //catch (Exception)
            //{
            //    excelApplication = null;
            //}
            //finally
            //{
            //    GC.Collect();
            //}

            //HttpContext.Current.Response.Flush();
            //HttpContext.Current.Response.End();


       // }
    }
}