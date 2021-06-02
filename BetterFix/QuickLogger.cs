using System.Text;
using BepInEx.Logging;

namespace BetterFix
{
    /// <summary>
    /// 自己的绿皮报错信息工具
    /// </summary>
    public static class QuickLogger
    {
        /// <summary>
        /// 用于反复使用的StringBuilder实例
        /// </summary>
        private static StringBuilder _stringBuilder = new StringBuilder();

        /// <summary>
        /// 用Main.gLogging，按指定信息等级输出复合格式字符串信息
        /// </summary>
        /// <param name="level">输出信息的等级</param>
        /// <param name="formatString">复合格式字符串（例如"Year{0} Month{1}"这种）</param>
        /// <param name="stringArgs">要设置字符串格式的参数数组</param>
        public static void Log(LogLevel level, string formatString, params object[] stringArgs)
        {
            _stringBuilder.Clear();                                    //用前清空（虽然感觉没必要，但以防万一吧）
            _stringBuilder.AppendFormat(formatString, stringArgs);     //调用StringBuilder处理复合格式字符串
            Main.Logger.Log(level, _stringBuilder.ToString());         //输出
            _stringBuilder.Clear();                                    //用后清空
        }
    }
}
