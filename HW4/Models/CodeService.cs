using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HW4.Models
{
    public class CodeService
    {
        /// <summary>
        /// 取得DB連線字串
        /// </summary>
        /// <returns></returns>
        private string GetConnecttonString()
        {
            return
                System.Configuration.ConfigurationManager.ConnectionStrings["SQLconnStr"].ConnectionString.ToString();
        }

        public List<SelectListItem> GetBook(string str_sql_status)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(this.GetConnecttonString()))
            {
                conn.Open();
                string sql = "";
                if(str_sql_status.Equals("書籍類別"))//代碼，何謂列舉型別
                {
                    sql = @"SELECT BOOK_CLASS_ID as value, BOOK_CLASS_NAME as text FROM dbo.BOOK_CLASS as class ;";
                }
                else if(str_sql_status.Equals("借閱狀態"))
                {
                    sql = @"SELECT c.CODE_ID as value, c.CODE_NAME as text FROM dbo.BOOK_CODE as c  Where c.CODE_TYPE = 'BOOK_STATUS';";
                }
                else
                {
                    sql = @"SELECT m.USER_ID as value, (m.USER_ENAME+'('+m.USER_CNAME+')') as text FROM dbo.MEMBER_M as m";
                }
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);

                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapCodeData(dt);
        }

        /// <summary>
        /// Maping 代碼資料
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<SelectListItem> MapCodeData(DataTable dt)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Text = row["text"].ToString(),
                    Value = row["value"].ToString()
                });
            }
            return result;
        }
    }
}