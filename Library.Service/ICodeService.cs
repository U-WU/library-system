using System.Collections.Generic;
using System.Web.Mvc;

namespace Library.Service
{
    public interface ICodeService
    {
        List<SelectListItem> GetBook(string str_sql_status);
    }
}