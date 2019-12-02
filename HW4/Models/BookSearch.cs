using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HW4.Models
{
    public class BookSearch
    {
        /// <summary>
        /// 圖書名稱
        /// </summary>
        [DisplayName("圖書名稱")]
        public string Book_name { get; set; }

        /// <summary>
        /// 圖書類別
        /// </summary>
        [DisplayName("圖書類別")]
        public string Book_class_id { get; set; }

        /// <summary>
        /// 借閱人
        /// </summary>
        [DisplayName("借閱人")]
        public string BOOK_KEEPER { get; set; }

        /// <summary>
        /// 圖書借閱狀態
        /// </summary>
        [DisplayName("圖書借閱狀態")]
        public string BOOK_STATUS { get; set; }
    }
}