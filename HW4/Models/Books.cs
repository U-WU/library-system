using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HW4.Models
{
    public class Books
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

        /// <summary>
        /// 圖書借閱狀態
        /// </summary>
        [DisplayName("圖書借閱狀態")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BOOK_STATUS { get; set; }

        /// <summary>
        /// 借閱人
        /// </summary>
        [DisplayName("借閱人")]
        public string BOOK_KEEPER { get; set; }

/*        /// <summary>
        /// 圖書建檔日期
        /// </summary>
        [DisplayName("圖書建檔日期")]
        public string CREATE_DATE { get; set; }

        /// <summary>
        /// 圖書建檔者
        /// </summary>
        [DisplayName("圖書建檔者")]
        public string CREATE_USER { get; set; }

        /// <summary>
        /// 圖書修改日期
        /// </summary>
        [DisplayName("圖書修改日期")]
        public string MODIFY_DATE { get; set; }

        /// <summary>
        /// 圖書修改者
        /// </summary>
        [DisplayName("圖書修改者")]
        public string MODIFY_USER { get; set; }
        */
    }
}