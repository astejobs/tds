//using Microsoft.SqlServer.Management.Smo;
//using System.Collections.Specialized;
//using Microsoft.SqlServer.Management.Sdk.Sfc;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace tds.Controllers
//{
//    public class BackUpDataController : Controller
//    {
//        // GET: BackUpData
//        public void Index()
//        {
//            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
//            SqlConnection sqlconn = new SqlConnection(connStr);
//            SqlCommand sqlcmd = new SqlCommand();
//            SqlDataAdapter da = new SqlDataAdapter();
//            DataTable dt = new DataTable();
//            // Backup destibation
//            string backupDestination = "C:\\SQLBackUpFolder";
//            // check if backup folder exist, otherwise create it.
//            if (!System.IO.Directory.Exists(backupDestination))
//            {
//                System.IO.Directory.CreateDirectory("C:\\SQLBackUpFolder");
//            }
//            try
//            {
//                sqlconn.Open();
//                sqlcmd = new SqlCommand("backup database TDSDB to disk='" + backupDestination + "\\" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".Bak'", sqlconn);
//                sqlcmd.ExecuteNonQuery();
//                //Close connection
//                sqlconn.Close();
//                Response.Write("Backup database successfully");
//            }
//            catch (Exception ex)
//            {
//                Response.Write(ex.Message);
//            }
//        }
//        public void data()
//        {
//            Server myServer = new Server("DefaultConnection");

//            Scripter scripter = new Scripter(myServer);

//            Database TDSDB = myServer.Databases["TDSDB"];

//            Urn[] DatabaseURNs = new Urn[] { TDSDB.Urn };
//            StringCollection scriptCollection = scripter.Script(DatabaseURNs);

//            foreach (string script in scriptCollection)

//            {
//                Response.Write(script);

//                Response.Write("<br/>");
//            }
//        }
//    }
//}