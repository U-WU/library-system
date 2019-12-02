using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Library.Service
{
    public class CodeService : ICodeService
    {
        private Library.Dao.ICodeDao codeDao { get; set; }

        public List<SelectListItem> GetBook(string str_sql_status)
        {
            return codeDao.GetBook(str_sql_status);
        }
    }
}
