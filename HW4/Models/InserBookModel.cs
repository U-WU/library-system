using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HW4.Models
{
    public class InserBookModel
    {
        /// <summary>
        /// 書籍編號
        /// </summary>
        ///[MaxLength(4)]
        [DisplayName("書籍編號")]
        public int Book_id { get; set; }

        /// <summary>
        /// 圖書名稱
        /// </summary>
        [DisplayName("圖書名稱")]
        [Required(ErrorMessage = "此欄位必填")]
        public string Book_name { get; set; }

        /// <summary>
        /// 圖書類別
        /// </summary>
        [DisplayName("圖書類別")]
        [Required(ErrorMessage = "此欄位必填")]
        public string Book_class_id { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [DisplayName("作者")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BOOK_AUTHOR { get; set; }

        /// <summary>
        /// 購書日期
        /// </summary>
        [DisplayName("購書日期")]
        [Required(ErrorMessage = "此欄位必填")]
        public DateTime BOOK_BOUGHT_DATE { get; set; }

        /// <summary>
        /// 出版商
        /// </summary>
        [DisplayName("出版商")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BOOK_PUBLISHER { get; set; }

        /// <summary>
        /// 內容簡介
        /// </summary>
        [DisplayName("內容簡介")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BOOK_NOTE { get; set; }
    }
}