using Library.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dao
{
    public class BookDao : IBookDao
    {
        ///類別要是名詞
        /// <summary>
        /// 取得DB連線字串
        /// </summary>
        /// <returns></returns>
        private string GetConnecttonString()
        {
            return Library.Common.ConfigTool.GetDataBaseConnectionString("SQLconnStr");
        }

        ///<summary>
        /// 取得圖書查詢的資料
        /// </summary>
        ///// //(data.BOOK_BOUGHT_DATE <= GETDATE()) AND要坐在新增修改的部分
        public List<Library.Model.Book> GetBookByCondition(Library.Model.BookSearch bookSearch)
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
                cmd.Parameters.Add(new SqlParameter("@BookName", bookSearch.BookName == null ? string.Empty : bookSearch.BookName));
                cmd.Parameters.Add(new SqlParameter("@clasName", bookSearch.ClassId == null ? string.Empty : bookSearch.ClassId));
                cmd.Parameters.Add(new SqlParameter("@userName", bookSearch.Keeper == null ? string.Empty : bookSearch.Keeper));//重複部分可以用副程式做掉
                cmd.Parameters.Add(new SqlParameter("@codeName", bookSearch.Status == null ? string.Empty : bookSearch.Status));//可以在SQL下ISNULL直接轉成空字串做掉
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
        public int InsertBook(Library.Model.InserBookModel books)
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
                    cmd.Parameters.Add(new SqlParameter("@BookName", books.BookName == null ? string.Empty : books.BookName));
                    cmd.Parameters.Add(new SqlParameter("@BookAuthor", books.Author == null ? string.Empty : books.Author));
                    cmd.Parameters.Add(new SqlParameter("@BookPublisher", books.Publisher == null ? string.Empty : books.Publisher));
                    cmd.Parameters.Add(new SqlParameter("@BookNote", books.Note == null ? string.Empty : books.Note));
                    cmd.Parameters.Add(new SqlParameter("@BookBoughtDate", books.BoughtDate == null ? Convert.ToDateTime("1911/01/01") : books.BoughtDate));
                    cmd.Parameters.Add(new SqlParameter("@BookClass", books.ClassId == null ? string.Empty : books.ClassId));
                    cmd.Parameters.Add(new SqlParameter("@BookStatus", 'A'));//可以把A設為痊癒變數去取
                    BookId = Convert.ToInt32(cmd.ExecuteScalar());
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
        public List<Library.Model.Book> UpdateBookById(string bookId)//應該可以直接回傳一本書 public List<Models.Books> UpdateBookById(int id)
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

        public string UpdateCondition(Library.Model.Book book)
        {
            if ((book.Status.Equals("B") || book.Status.Equals("C")) && book.Keeper.Equals(""))//放在SERVICE內
            {
                return "NEEDKEEPER";
            }
            else if ((book.Status.Equals("A") || book.Status.Equals("U")) && book.Keeper != null)
            {
                return "NONKEEPER";
            }
            else
            {
                return "SUCCESS";
            }
        }

        /// <summary>
        /// 修改書籍
        /// </summary> not yet
        /// <param name="book"></param>
        /// <returns>員工編號</returns> not yet
        public void UpateBook(Library.Model.Book books)
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
                    cmd.Parameters.Add(new SqlParameter("@BookId", books.BookId == null ? (Object)DBNull.Value : books.BookId));
                    cmd.Parameters.Add(new SqlParameter("@BookName", books.BookName == null ? string.Empty : books.BookName));
                    cmd.Parameters.Add(new SqlParameter("@BookAuthor", books.Author == null ? string.Empty : books.Author));
                    cmd.Parameters.Add(new SqlParameter("@BookPublisher", books.Publisher == null ? string.Empty : books.Publisher));
                    cmd.Parameters.Add(new SqlParameter("@BookNote", books.Note == null ? string.Empty : books.Note));
                    cmd.Parameters.Add(new SqlParameter("@BookBoughtDate", books.BoughtDate));
                    cmd.Parameters.Add(new SqlParameter("@BookClass", books.ClassId == null ? string.Empty : books.ClassId));
                    cmd.Parameters.Add(new SqlParameter("@BookStatus", books.Status == null ? string.Empty : books.Status));
                    cmd.Parameters.Add(new SqlParameter("@BookKeeper", books.Keeper == null ? string.Empty : books.Keeper));
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

        public bool DeleteBookCondition(Library.Model.Book books)
        {
            if (books.Status.Equals("B") || books.Status.Equals("C"))
            {
                return false;
            }
            else
                return true;
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
                throw new Exception("My Error message", ex);
            }
        }

        /// <summary>
        /// Map資料進List
        /// </summary>
        /// <param name="bookData"></param>
        /// <returns></returns>

        private List<Library.Model.Book> MapBookDataToList(DataTable bookData)
        {
            List<Library.Model.Book> result = new List<Library.Model.Book>();
            foreach (DataRow row in bookData.Rows)
            {
                result.Add(new Book()
                {
                    BookId = (int)row["id"],
                    ClassId = row["bookClass"].ToString(),
                    BookName = row["bookName"].ToString(),
                    BoughtDate = Convert.ToDateTime(row["boughtday"]),
                    Status = row["Borrowstatus"].ToString(),
                    Keeper = row["Borrow_men"].ToString(),
                    Author = row["author"].ToString(),
                    Note = row["note"].ToString(),
                    Publisher = row["publisher"].ToString()
                });
            }
            return result;
        }
    }
}
