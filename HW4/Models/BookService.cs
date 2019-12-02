using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace HW4.Models
{
    public class BookService
    {
        ///類別要是名詞
        /// <summary>
        /// 取得DB連線字串
        /// </summary>
        /// <returns></returns>
        private string GetConnecttonString()
        {
            return
                System.Configuration.ConfigurationManager.ConnectionStrings["SQLconnStr"].ConnectionString.ToString();
        }

        ///<summary>
        /// 取得圖書查詢的資料
        /// </summary>
        ///// //(data.BOOK_BOUGHT_DATE <= GETDATE()) AND要坐在新增修改的部分
        public List<Models.Books> GetBookByCondition(Models.BookSearch bookSearch)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT class.BOOK_CLASS_NAME as bookClass, data.BOOK_NAME as bookName, data.BOOK_BOUGHT_DATE as boughtday,
		                           code.CODE_NAME as Borrowstatus, m.USER_ENAME+'('+m.USER_CNAME+')' as Borrow_men, data.BOOK_ID as id,
								   data.BOOK_AUTHOR as author,data.BOOK_PUBLISHER as publisher, data.BOOK_NOTE as note
                           FROM dbo.BOOK_DATA as data 
                           INNER JOIN dbo.BOOK_CLASS as class 
	                           ON (data.BOOK_CLASS_ID = class.BOOK_CLASS_ID) 
                           INNER JOIN dbo.BOOK_CODE as code 
	                           ON ((code.CODE_TYPE = 'BOOK_STATUS')AND(data.BOOK_STATUS = code.CODE_ID)) 
                           LEFT JOIN dbo.MEMBER_M as m 
	                           ON (data.BOOK_KEEPER = m.USER_ID) 
                           Where ((data.BOOK_NAME) LIKE ('%' + @BookName + '%') COLLATE Chinese_Taiwan_Stroke_CS_AI) AND 
	                             (class.BOOK_CLASS_ID = @clasName OR @clasName='') AND 
	                             (data.BOOK_KEEPER = @userName OR @userName='') AND 
                                 (data.BOOK_BOUGHT_DATE <= GETDATE()) AND
	                             (code.CODE_ID = @codeName OR @codeName='')
                           ORDER BY data.BOOK_BOUGHT_DATE DESC;";

            using (SqlConnection conn = new SqlConnection(this.GetConnecttonString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookName", bookSearch.Book_name == null ? string.Empty : bookSearch.Book_name));
                cmd.Parameters.Add(new SqlParameter("@clasName", bookSearch.Book_class_id == null ? string.Empty : bookSearch.Book_class_id));
                cmd.Parameters.Add(new SqlParameter("@userName", bookSearch.BOOK_KEEPER == null ? string.Empty : bookSearch.BOOK_KEEPER));//重複部分可以用副程式做掉
                cmd.Parameters.Add(new SqlParameter("@codeName", bookSearch.BOOK_STATUS == null ? string.Empty : bookSearch.BOOK_STATUS));//可以在SQL下ISNULL直接轉成空字串做掉
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapBookDataToList(dt);
        }

        /// <summary>
        /// 新增書籍
        /// </summary> not yet
        /// <param name="book"></param>
        public int InsertBook(Models.InserBookModel books)
        {
            string sql = @" INSERT INTO dbo.BOOK_DATA
						 (
							 BOOK_NAME, BOOK_AUTHOR, BOOK_PUBLISHER, BOOK_NOTE, BOOK_BOUGHT_DATE, BOOK_CLASS_ID, BOOK_STATUS
						 )
						VALUES
						(
							 @BookName,@BookAuthor, @BookPublisher, @BookNote, @BookBoughtDate,@BookClass,@BookStatus
						)
						Select SCOPE_IDENTITY()";
            int BookId;
            using (SqlConnection conn = new SqlConnection(this.GetConnecttonString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlTransaction transaction = conn.BeginTransaction();
                cmd.Transaction = transaction;

                try
                {
                    cmd.Parameters.Add(new SqlParameter("@BookName", books.Book_name == null ? string.Empty : books.Book_name));
                    cmd.Parameters.Add(new SqlParameter("@BookAuthor", books.BOOK_AUTHOR == null ? string.Empty : books.BOOK_AUTHOR));
                    cmd.Parameters.Add(new SqlParameter("@BookPublisher", books.BOOK_PUBLISHER == null ? string.Empty : books.BOOK_PUBLISHER));
                    cmd.Parameters.Add(new SqlParameter("@BookNote", books.BOOK_NOTE == null ? string.Empty : books.BOOK_NOTE));
                    cmd.Parameters.Add(new SqlParameter("@BookBoughtDate", books.BOOK_BOUGHT_DATE == null ? Convert.ToDateTime("1911/01/01") : books.BOOK_BOUGHT_DATE));
                    cmd.Parameters.Add(new SqlParameter("@BookClass", books.Book_class_id == null ? string.Empty : books.Book_class_id));
                    cmd.Parameters.Add(new SqlParameter("@BookStatus", 'A'));//可以把A設為痊癒變數去取
                    // cmd.Parameters.Add(new SqlParameter("@ManagerId", employee.ManagerId == null ? (Object)DBNull.Value : employee.ManagerId));
                    BookId = Convert.ToInt32(cmd.ExecuteScalar());
                    //cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
            return BookId;
        }

        ///<summary>
        /// 取得修改前的圖書資料的資料，這邊要直接坐船值傳給電腦看得
        /// </summary>
        public List<Models.Books> UpdateBookById(string bookId)//應該可以直接回傳一本書 public List<Models.Books> UpdateBookById(int id)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT class.BOOK_CLASS_ID as bookClass, data.BOOK_NAME as bookName, data.BOOK_BOUGHT_DATE as boughtday,
		                           code.CODE_ID as Borrowstatus, m.USER_ID as Borrow_men, data.BOOK_ID as id,
								   data.BOOK_AUTHOR as author,data.BOOK_PUBLISHER as publisher, data.BOOK_NOTE as note
                           FROM dbo.BOOK_DATA as data 
                           INNER JOIN dbo.BOOK_CLASS as class 
	                           ON (data.BOOK_CLASS_ID = class.BOOK_CLASS_ID) 
                           INNER JOIN dbo.BOOK_CODE as code 
	                           ON ((code.CODE_TYPE = 'BOOK_STATUS')AND(data.BOOK_STATUS = code.CODE_ID)) 
                           LEFT JOIN dbo.MEMBER_M as m 
	                           ON (data.BOOK_KEEPER = m.USER_ID) 
                           Where data.BOOK_ID = @id
                           ORDER BY data.BOOK_BOUGHT_DATE DESC;";

            using (SqlConnection conn = new SqlConnection(this.GetConnecttonString()))//新增完要直接跳修改，也要清空
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@id", bookId == null ? (Object)DBNull.Value : bookId));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapBookDataToList(dt);
        }

        /// <summary>
        /// 修改書籍
        /// </summary> not yet
        /// <param name="book"></param>
        /// <returns>員工編號</returns> not yet
        public void UpateBook(Models.Books books)
        {
            string sql = @" Update dbo.BOOK_DATA Set BOOK_NAME=@BookName, BOOK_AUTHOR=@BookAuthor, BOOK_PUBLISHER=@BookPublisher,
						 BOOK_NOTE=@BookNote, BOOK_BOUGHT_DATE=@BookBoughtDate, BOOK_STATUS=@BookStatus,BOOK_KEEPER=@BookKeeper,
                         BOOK_CLASS_ID=@BookClass
                         Where Book_id=@BookId;";//要分大小寫
            using (SqlConnection conn = new SqlConnection(this.GetConnecttonString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlTransaction transaction = conn.BeginTransaction();
                cmd.Transaction = transaction;

                try
                {
                    cmd.Parameters.Add(new SqlParameter("@BookId", books.Book_id == null ? (Object)DBNull.Value : books.Book_id));
                    cmd.Parameters.Add(new SqlParameter("@BookName", books.Book_name == null ? string.Empty : books.Book_name));
                    cmd.Parameters.Add(new SqlParameter("@BookAuthor", books.BOOK_AUTHOR == null ? string.Empty : books.BOOK_AUTHOR));
                    cmd.Parameters.Add(new SqlParameter("@BookPublisher", books.BOOK_PUBLISHER == null ? string.Empty : books.BOOK_PUBLISHER));
                    cmd.Parameters.Add(new SqlParameter("@BookNote", books.BOOK_NOTE == null ? string.Empty : books.BOOK_NOTE));
                    cmd.Parameters.Add(new SqlParameter("@BookBoughtDate", books.BOOK_BOUGHT_DATE));
                    cmd.Parameters.Add(new SqlParameter("@BookClass", books.Book_class_id == null ? string.Empty : books.Book_class_id));
                    cmd.Parameters.Add(new SqlParameter("@BookStatus", books.BOOK_STATUS == null ? string.Empty : books.BOOK_STATUS));
                    cmd.Parameters.Add(new SqlParameter("@BookKeeper", books.BOOK_KEEPER == null ? string.Empty : books.BOOK_KEEPER));
                    //BookId = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 刪除書籍
        /// </summary> not yet
        public void DeleteBook(string bookId)
        {
            try
            {
                string sql = "Delete FROM dbo.BOOK_DATA Where BOOK_ID=@BookId";
                using (SqlConnection conn = new SqlConnection(this.GetConnecttonString()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add(new SqlParameter("@BookId", bookId == null ? (Object)DBNull.Value : bookId));
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Map資料進List
        /// </summary>
        /// <param name="bookData"></param>
        /// <returns></returns>

        private List<Models.Books> MapBookDataToList(DataTable bookData)
        {
            List<Models.Books> result = new List<Books>();
            foreach (DataRow row in bookData.Rows)
            {
                result.Add(new Books()
                {
                    Book_id = (int)row["id"],
                    Book_class_id = row["bookClass"].ToString(),
                    Book_name = row["bookName"].ToString(),
                    BOOK_BOUGHT_DATE = Convert.ToDateTime(row["boughtday"]),
                    BOOK_STATUS = row["Borrowstatus"].ToString(),
                    BOOK_KEEPER = row["Borrow_men"].ToString(),
                    BOOK_AUTHOR = row["author"].ToString(),
                    BOOK_NOTE = row["note"].ToString(),
                    BOOK_PUBLISHER = row["publisher"].ToString()
                });
            }
            return result;
        }
    }
}