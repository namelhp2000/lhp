using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RoadFlow.Utility
{
    /// <summary>
    /// 文件操作帮助类
    /// </summary>
    public class FileHelper
    {
        #region 读操作

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="path">文件目录</param>
        /// <returns></returns>
        public static bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// 获取当前程序根目录
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDir()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        #endregion

        #region 写操作

        /// <summary>
        /// 输出字符串到文件
        /// 注：使用系统默认编码;若文件不存在则创建新的,若存在则覆盖
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        public static void WriteTxt(string content, string path)
        {
            WriteTxt(content, path, null, null);
        }

        /// <summary>
        /// 输出字符串到文件
        /// 注：使用自定义编码;若文件不存在则创建新的,若存在则覆盖
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码</param>
        public static void WriteTxt(string content, string path, Encoding encoding)
        {
            WriteTxt(content, path, encoding, null);
        }

        /// <summary>
        /// 输出字符串到文件
        /// 注：使用自定义模式,使用默认编码
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        /// <param name="fileModel">输出方法</param>
        public static void WriteTxt(string content, string path, FileMode fileModel)
        {
            WriteTxt(content, path, null, fileModel);
        }

        /// <summary>
        /// 输出字符串到文件
        /// 注：使用自定义编码以及写入模式
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="fileModel">写入模式</param>
        public static void WriteTxt(string content, string path, Encoding encoding, FileMode fileModel)
        {
            WriteTxt(content, path, encoding, (FileMode?)fileModel);
        }

        /// <summary>
        /// 输出日志到指定文件
        /// </summary>
        /// <param name="msg">日志消息</param>
        /// <param name="path">日志文件位置（默认为D:\测试\a.log）</param>
        public static void WriteLog(string msg, string path = @"Log.txt")
        {
            string content = $"{DateTime.Now.ToCstTime().ToString("yyyy-MM-dd HH:mm:ss")}:{msg}";

            WriteTxt(content, $"{GetCurrentDir()}{content}");
        }


        /// <summary>
        /// 输出字符串到文件
        /// 注：使用自定义编码以及写入模式
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="fileModel">写入模式</param>
        private static void WriteTxt(string content, string path, Encoding encoding, FileMode? fileModel)
        {
            CheckDirectory(path);

            if (encoding == null)
                encoding = Encoding.Default;
            if (fileModel == null)
                fileModel = FileMode.Create;

            using (FileStream fileStream = new FileStream(path, fileModel.Value))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, encoding))
                {
                    streamWriter.Write(content);
                    streamWriter.Flush();
                }
            }
        }


        /// <summary>
        /// 文件另存为
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string SaveFile(string filepath, string content, Encoding encoding, FileMode? fileModel)
        {
            CheckDirectory(filepath);
            if (encoding == null)
                encoding = Encoding.Default;
            if (fileModel == null)
                fileModel = FileMode.Create;
            try
            {
                StreamWriter writer1 = new StreamWriter(filepath, false, Encoding.UTF8);
                writer1.Write(content);
                writer1.Close();
                return "success";
            }
            catch (Exception exception1)
            {
                return exception1.Message;
            }
        }




        /// <summary>
        /// 检验目录，若目录已存在则不变
        /// </summary>
        /// <param name="path">目录位置</param>
        public static void CheckDirectory(string path)
        {
            if (path.Contains("\\"))
                Directory.CreateDirectory(GetPathDirectory(path));
        }



        /// <summary>
        /// 获取文件位置中的目录位置（不包括文件名）
        /// </summary>
        /// <param name="path">文件位置</param>
        /// <returns></returns>
        public static string GetPathDirectory(string path)
        {
            if (!path.Contains("\\"))
                return GetCurrentDir();

            string pathDirectory = string.Empty;
            string pattern = @"^(.*\\).*?$";
            Match match = Regex.Match(path, pattern);

            return match.Groups[1].ToString();
        }

        #endregion


        #region  文件操作
        /// <summary>
        /// 流转换成字符串
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="bufferSize">缓冲区大小</param>
        /// <param name="isCloseStream">读取完成是否释放流，默认为true</param>
        public static string ToString(Stream stream, Encoding encoding = null, int bufferSize = 1024 * 2, bool isCloseStream = true)
        {
            if (stream == null)
                return string.Empty;
            if (encoding == null)
                encoding = Encoding.UTF8;
            if (stream.CanRead == false)
                return string.Empty;
            using (var reader = new StreamReader(stream, encoding, true, bufferSize, !isCloseStream))
            {
                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);
                var result = reader.ReadToEnd();
                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);
                return result;
            }
        }

        /// <summary>
        /// 流转换成字符串
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="bufferSize">缓冲区大小</param>
        /// <param name="isCloseStream">读取完成是否释放流，默认为true</param>
        public static async Task<string> ToStringAsync(Stream stream, Encoding encoding = null, int bufferSize = 1024 * 2, bool isCloseStream = true)
        {
            if (stream == null)
                return string.Empty;
            if (encoding == null)
                encoding = Encoding.UTF8;
            if (stream.CanRead == false)
                return string.Empty;
            using (var reader = new StreamReader(stream, encoding, true, bufferSize, !isCloseStream))
            {
                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);
                var result = await reader.ReadToEndAsync();
                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);
                return result;
            }
        }

        /// <summary>
        /// 复制流并转换成字符串
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="encoding">字符编码</param>
        public static async Task<string> CopyToStringAsync(Stream stream, Encoding encoding = null)
        {
            if (stream == null)
                return string.Empty;
            if (encoding == null)
                encoding = Encoding.UTF8;
            if (stream.CanRead == false)
                return string.Empty;
            using (var memoryStream = new MemoryStream())
            {
                using (var reader = new StreamReader(memoryStream, encoding))
                {
                    if (stream.CanSeek)
                        stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(memoryStream);
                    if (memoryStream.CanSeek)
                        memoryStream.Seek(0, SeekOrigin.Begin);
                    var result = await reader.ReadToEndAsync();
                    if (stream.CanSeek)
                        stream.Seek(0, SeekOrigin.Begin);
                    return result;
                }
            }
        }
        #endregion


        #region  判断文件或者文件夹是否可以保存

        /// <summary>
        /// 检查文件可以保存 通过是否添加内容判断是否可以保持
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool CheckFileCanSave(string file)
        {
            string path = GetMapPath(file);
            try
            {
                string str2 = "";
                StreamWriter writer1 = new StreamWriter(path, true, Encoding.UTF8);
                writer1.Write(str2);
                writer1.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查文件夹可以保存 通过添加文本检测是否保存
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool CheckFolderCanSave(string dir)
        {
            string mapPath = GetMapPath(dir);
            try
            {
                using (new FileStream(mapPath + @"\check.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                }
                System.IO.File.Delete(mapPath + @"\check.txt");
                return true;
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// 设置好文件夹路径
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        public static string GetMapPath(string strPath)
        {
            strPath = strPath.Replace("/", @"\");
            strPath = strPath.Replace("~", "");

            if (strPath.StartsWith(@"\"))
            {
                char[] trimChars = new char[] { '\\' };
                strPath = strPath.TrimStart(trimChars);
            }
            return Path.Combine(Tools.GetContentRootPath(), strPath);
        }


        #endregion



        /// <summary>
        /// 流转换为字节流
        /// </summary>
        /// <param name="stream">流</param>
        public static async Task<byte[]> ToBytesAsync(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// 流转换为字节流
        /// </summary>
        /// <param name="stream">流</param>
        public static byte[] ToBytes(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// 字符串转换成字节数组
        /// </summary>
        /// <param name="data">数据,默认字符编码utf-8</param>        
        public static byte[] ToBytes(string data)
        {
            return ToBytes(data, Encoding.UTF8);
        }

        /// <summary>
        /// 字符串转换成字节数组
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="encoding">字符编码</param>
        public static byte[] ToBytes(string data, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(data))
                return new byte[] { };
            return encoding.GetBytes(data);
        }

        /// <summary>
        /// 将文件读取到字节流中
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static byte[] Read(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return null;
            var fileInfo = new FileInfo(filePath);
            using (var reader = new BinaryReader(fileInfo.Open(FileMode.Open)))
            {
                return reader.ReadBytes((int)fileInfo.Length);
            }
        }





        #region  判断两个文件是否相同

        /// <summary>
        /// 读入到字节数组中比较(while循环比较字节数组)
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        private static bool CompareByByteArry(string file1, string file2)
        {
            const int BYTES_TO_READ = 1024 * 10;
            using (FileStream fs1 = File.Open(file1, FileMode.Open))
            using (FileStream fs2 = File.Open(file2, FileMode.Open))
            {
                byte[] one = new byte[BYTES_TO_READ];
                byte[] two = new byte[BYTES_TO_READ];
                while (true)
                {
                    int len1 = fs1.Read(one, 0, BYTES_TO_READ);
                    int len2 = fs2.Read(two, 0, BYTES_TO_READ);
                    int index = 0;
                    while (index < len1 && index < len2)
                    {
                        if (one[index] != two[index]) return false;
                        index++;
                    }
                    if (len1 == 0 || len2 == 0) break;
                }
            }
            return true;
        }



        /// <summary>
        /// 读入到字节数组中比较(ReadOnlySpan)  速度最快对比两个文件是否相同
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        private static bool CompareByReadOnlySpan(string file1, string file2)
        {
            const int BYTES_TO_READ = 1024 * 10;
            using (FileStream fs1 = File.Open(file1, FileMode.Open))
            using (FileStream fs2 = File.Open(file2, FileMode.Open))
            {
                byte[] one = new byte[BYTES_TO_READ];
                byte[] two = new byte[BYTES_TO_READ];
                while (true)
                {
                    int len1 = fs1.Read(one, 0, BYTES_TO_READ);
                    int len2 = fs2.Read(two, 0, BYTES_TO_READ);
                    // 字节数组可直接转换为ReadOnlySpan
                    if (!((ReadOnlySpan<byte>)one).SequenceEqual((ReadOnlySpan<byte>)two)) return false;
                    if (len1 == 0 || len2 == 0) break;  // 两个文件都读取到了末尾,退出while循环
                }
            }
            return true;
        }


        #endregion


    }
}
