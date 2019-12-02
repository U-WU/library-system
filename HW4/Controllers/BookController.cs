using Library.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HW4.Controllers
{
    public class BookController : Controller
    {
        private ICodeService CodeService { get; set; }
        private IBookService BookService { get; set; }

        [HttpGet()]
        public JsonResult CallDropDownList(string param)
        {//可以用GET做，在DDL內的URL = url+"?param="+代碼，controller可以去取得jason裡的值來節省程式碼
            try
            {
                var JsonSrc = this.CodeService.GetBook(param);
                return this.Json(JsonSrc, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                Library.Common.Logger.Write(Library.Common.Logger.LogCategoryEnum.Error, ex.ToString());
                return this.Json(false);
            }
        }

        public static bool ValidateDateTime(DateTime datetime)
        {
            if (datetime.CompareTo(DateTime.Now) > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Book
        public ActionResult BooksSearch()
        {
            return View();
        }

        [HttpPost()]
        public JsonResult BooksSearch(Library.Model.BookSearch srch, string Book_name)
        {
            try
            {
                var JsonSrc = BookService.GetBookByCondition(srch);
                return this.Json(JsonSrc);
            }

            catch (Exception ex)
            {
                Library.Common.Logger.Write(Library.Common.Logger.LogCategoryEnum.Error,ex.ToString());
                return this.Json(false);
            }
        }

        /// <summary>
        /// 新增圖書畫面
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult InsertBook()
        {
            return View(new Library.Model.InserBookModel());
        }

        /// <summary>
        /// 新增圖書
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost()]
        public JsonResult InsertBook(Library.Model.InserBookModel book)
        {
            try
            {
                if (ValidateDateTime(book.BoughtDate))
                {
                    var JsonSrc = BookService.InsertBook(book);
                    return this.Json(JsonSrc);
                }
                else
                {
                    return this.Json("OverNow");
                }
            }
            catch (Exception ex)
            {
                Library.Common.Logger.Write(Library.Common.Logger.LogCategoryEnum.Error, ex.ToString());
                return this.Json(false);
            }
        }
        //新增成功後，若不想留資料，return View(index);  新增修改的日期不能>今天

        /// <summary>
        /// 刪除圖書
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost()]
        public JsonResult DeleteBook(string bookId)
        {
            try
            {
                var book = BookService.UpdateBookById(bookId);
                if (BookService.DeleteBookCondition(book[0]))//這算是明確的商業邏輯，要放在service裡去做更改
                {
                    BookService.DeleteBook(bookId);
                }
                return this.Json(BookService.DeleteBookCondition(book[0]));
            }
            catch (Exception ex)
            {
                Library.Common.Logger.Write(Library.Common.Logger.LogCategoryEnum.Error, ex.ToString());
                return this.Json(false);
            }
        }

        /// <summary>
        /// 修改圖書畫面
        /// </summary>
        [HttpGet()]
        public ActionResult UpdateBook(int id)
        {
            return View();
        }

        [HttpPost()]
        public JsonResult GetBookDetail(string bookId)
        {
            try
            {
                var updataModel = BookService.UpdateBookById(bookId).FirstOrDefault<Library.Model.Book>();  //datarow to model
                return this.Json(updataModel);
            }

            catch (Exception ex)
            {
                Library.Common.Logger.Write(Library.Common.Logger.LogCategoryEnum.Error, ex.ToString());
                return this.Json(false);
            }
        }

        [HttpPost()]
        public JsonResult UpdateBook(Library.Model.Book books)
        {
            try
            {
                if (ValidateDateTime(books.BoughtDate))
                {
                    return this.Json(BookService.UpdateCondition(books));
                }
                else
                {
                    return this.Json("OverNow");
                }
            }
            catch (Exception ex)
            {
                Library.Common.Logger.Write(Library.Common.Logger.LogCategoryEnum.Error, ex.ToString());
                return this.Json(false);
            }
            ///要去判斷是否有人已經借出書本，在送出之前
        }
    }
}