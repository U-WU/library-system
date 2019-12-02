using System.Collections.Generic;
using System.Web.Mvc;

namespace Library.Dao
{
    public interface ICodeDao
    {
        List<SelectListItem> GetBook(string str_sql_status);
    }
}