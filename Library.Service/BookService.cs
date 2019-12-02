using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Service
{
    public class BookService : IBookService
    {
        private Library.Dao.IBookDao bookDao { get; set; }

        public List<Library.Model.Book> GetBookByCondition(Library.Model.BookSearch bookSearch)
        {
            return bookDao.GetBookByCondition(bookSearch);
        }

        public int InsertBook(Library.Model.InserBookModel books)
        {
            return bookDao.InsertBook(books);
        }

        public List<Library.Model.Book> UpdateBookById(string bookId)//應該可以直接回傳一本書 public List<Models.Books> UpdateBookById(int id)
        {
            return bookDao.UpdateBookById(bookId);
        }

        public void UpateBook(Library.Model.Book books)
        {
            bookDao.UpateBook(books);
        }

        public void DeleteBook(string bookId)
        {
            bookDao.DeleteBook(bookId);
        }

        public bool DeleteBookCondition(Library.Model.Book books)
        {
            return bookDao.DeleteBookCondition(books);
        }

        public string UpdateCondition(Library.Model.Book book)
        {
            return bookDao.UpdateCondition(book);
        }
    }
}
