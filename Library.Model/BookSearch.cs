using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Model
{
    public class BookSearch
    {
        /// <summary>
        /// 圖書名稱
        /// </summary>
        [DisplayName("圖書名稱")]
        public string BookName { get; set; }

        /// <summary>
        /// 圖書類別
        /// </summary>
        [DisplayName("圖書類別")]
        public string ClassId { get; set; }

        /// <summary>
        /// 借閱人
        /// </summary>
        [DisplayName("借閱人")]
        public string Keeper { get; set; }

        /// <summary>
        /// 圖書借閱狀態
        /// </summary>
        [DisplayName("圖書借閱狀態")]
        public string Status { get; set; }
    }
}
