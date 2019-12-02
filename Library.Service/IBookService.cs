using System.Collections.Generic;
using Library.Model;

namespace Library.Service
{
    public interface IBookService
    {
        void DeleteBook(string bookId);
        List<Book> GetBookByCondition(BookSearch bookSearch);
        int InsertBook(InserBookModel books);
        void UpateBook(Book books);
        List<Book> UpdateBookById(string bookId);
        bool DeleteBookCondition(Library.Model.Book books);
        string UpdateCondition(Library.Model.Book book);
    }
}