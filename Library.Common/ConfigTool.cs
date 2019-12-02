using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Common
{
    public static class ConfigTool
    {
        /// <summary>
        /// 取得DB連線字串
        /// </summary>
        /// <returns></returns>
        public static string GetDataBaseConnectionString(string connName)
        {
            return
                System.Configuration.ConfigurationManager.
                    ConnectionStrings[connName].ConnectionString.ToString();
        }

        public static string GetAppsetting(string Key)
        {
            string AppSetting = string.Empty;
            AppSetting = System.Configuration.ConfigurationManager.AppSettings[Key];
            return AppSetting;
        }
    }
}
