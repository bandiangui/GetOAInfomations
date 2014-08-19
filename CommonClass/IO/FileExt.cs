using System;
using System.IO;
using System.Net;
using GetOAInfomations;

namespace CommonClass.IO
{
    public static class FileExt
    {
        public static readonly string OaUser = "OaAdmin";
        public static readonly string OaDom = "NLIS";
        public static readonly string OaPwd = "qazwsx";
        private const string TempMark = "≮temp≯";                       // 防止文件并发的临时文件标识

        /// <summary>
        /// 文件从存储到字节
        /// </summary>
        /// <param name="filePath">存放路径...可以是磁盘路径;部分存储路径(storePath不能为空);完整的存储路径(storePath为空)</param>
        /// <param name="storePath">存储名称</param>
        /// <returns></returns>
        public static byte[] GetFileToBytes(string filePath, string storePath = null)
        {
            if (IsDiscPath(filePath))
            {
                return GetFileToBytes(filePath);
            }
            if (!IsStorePath(filePath) && string.IsNullOrEmpty(storePath))
            {
                return null;
            }

            if (!Helper.VirtualLogOn()) return null;

            filePath = string.Format("{0}{1}", storePath, CheckPath(filePath));
            var bt = GetFileToBytes(filePath);
            Helper.VirtualLogOff();
            return bt;
        }

        private static byte[] GetFileToBytes(string fileFullPath)
        {
            try
            {
                if (!IsFileExist(fileFullPath)) return null;
                DealConcurrentReadFile(ref fileFullPath);

                FileStream fs = new FileStream(fileFullPath, FileMode.Open);
                byte[] bt = new byte[fs.Length];
                fs.Read(bt, 0, bt.Length);
                fs.Dispose();

                DisposeConcurrent(fileFullPath);
                return bt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 文件从存储到项目服务器
        /// </summary>
        /// <param name="sourcePath">存放路径...可以是部分存储路径(storePath不能为空);完整的存储路径(storePath为空)</param>
        /// <param name="destFileName">最终全路径</param>
        /// <param name="storePath">存储名称</param>
        public static void GetFileStoreToServer(string sourcePath, string destFileName, string storePath = null)
        {
            if (!IsStorePath(sourcePath) && string.IsNullOrEmpty(storePath)) return;
            sourcePath = string.Format("{0}{1}", storePath, CheckPath(sourcePath));
            try
            {
                if (!Helper.VirtualLogOn(OaUser, OaDom, OaPwd)) return;

                if (IsFileExist(sourcePath))
                {
                    File.Copy(sourcePath, destFileName, true);
                }
                Helper.VirtualLogOff();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 文件从项目服务器到存储
        /// </summary>
        /// <param name="serverFullPath">项目服务器全路径</param>
        /// <param name="storeFullPath">存储全路径</param>
        public static void GetFileServerToStore(string serverFullPath, string storeFullPath)
        {
            try
            {
                if (!Helper.VirtualLogOn(OaUser, OaDom, OaPwd)) return;

                if (IsFileExist(storeFullPath))
                {
                    File.Copy(serverFullPath, storeFullPath, true);
                }
                Helper.VirtualLogOff();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 文件从Url地址到项目服务器
        /// </summary>
        /// <param name="url">Url访问地址</param>
        /// <param name="destFileName">项目服务器全路径</param>
        public static void GetFileUrlToServer(string url, string destFileName)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile(url, destFileName);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 文件从Url地址到文件流
        /// </summary>
        /// <param name="url">Url访问地址</param>
        /// <returns></returns>
        public static Stream GetFileUrlToStream(string url)
        {
            try
            {
                WebClient webClient = new WebClient();
                var fileBytes = webClient.DownloadData(url);
                MemoryStream ms = new MemoryStream(fileBytes);
                return ms;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 文件从存储到文件流
        /// </summary>
        /// <param name="savePath">存放路径</param>
        /// <param name="storePath">存储名称</param>
        /// <returns></returns>
        public static Stream GetFileStoreToStream(string savePath, string storePath = null)
        {
            if (!IsStorePath(savePath) && string.IsNullOrEmpty(storePath)) return null;
            string sourcePath = string.Format("{0}{1}", storePath, CheckPath(savePath));
            try
            {
                if (!Helper.VirtualLogOn(OaUser, OaDom, OaPwd)) return null;
                if (!IsFileExist(sourcePath)) return null;

                DealConcurrentReadFile(ref sourcePath);

                FileStream fs = new FileStream(sourcePath, FileMode.Open);
                Helper.VirtualLogOff();
                return fs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void DelFileFormStore(string fullFileName)
        {
            try
            {
                if (!Helper.VirtualLogOn(OaUser, OaDom, OaPwd)) return;
                if (!IsFileExist(fullFileName)) return;
                File.Delete(fullFileName);
                Helper.VirtualLogOff();
            }
            catch (Exception)
            {
                return;
            }
        }

        #region 工具
        public static void CheckDirectoryExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static bool IsFileExist(string path)
        {
            return File.Exists(path);
        }

        private static bool IsStorePath(string path)
        {
            // 以"\\"开头的是存储路径
            return path.StartsWith("\\\\");
        }

        private static bool IsDiscPath(string path)
        {
            //第二三个字符是":\"的是磁盘路径
            return path.Substring(0, 3).EndsWith(":\\");
        }

        private static string CheckPath(string path)
        {
            // 规范化存储地址: xxx\xxx\
            string newPath = path.StartsWith(@"\") ? path.Substring(1) : path;
            return newPath;
        }

        #endregion

        #region 并发问题解决

        public static void DealConcurrentReadFile(ref string fileFullPath)
        {
            // FileStream Open文件会引起并发问题,故此,先创建一个GUID的临时文件...
            string filePath = DealConcurrentCreateTempPath(fileFullPath);
            File.Copy(fileFullPath, filePath);
            fileFullPath = filePath;
        }

        public static string DealConcurrentCreateFile(string fileFullPath)
        {
            return DealConcurrentCreateTempPath(fileFullPath);
        }

        private static string DealConcurrentCreateTempPath(string fileFullPath)
        {
            string filePath = fileFullPath.Substring(0, fileFullPath.LastIndexOf("\\") + 1);
            string fileExt = fileFullPath.Substring(fileFullPath.LastIndexOf("."));
            string newFileFullPath = string.Format("{0}{1}{2}{3}", filePath, Guid.NewGuid().ToString(), TempMark, fileExt);

            return newFileFullPath;
        }


        public static void DisposeConcurrent(string fileFullPath)
        {
            //删除解除并发的临时文件
            if (fileFullPath.Contains(TempMark))
            {
                File.Delete(fileFullPath);
            }
        }

        public static void DisposeConcurrent(string tempFileFullPath, string fileFullPath)
        {

            string filePath = fileFullPath.Substring(0, fileFullPath.LastIndexOf("\\") + 1);

            if (!Helper.VirtualLogOn(FileExt.OaUser, FileExt.OaDom, FileExt.OaPwd)) return;

            CheckDirectoryExist(filePath);
            if (Helper.IsFileUsing(fileFullPath))
            {
                File.Copy(tempFileFullPath, fileFullPath, true);
            }
            Helper.VirtualLogOff();
            if (tempFileFullPath.Contains(TempMark))
            {
                File.Delete(tempFileFullPath);
            }

        }

        public static void Dispose(this Stream st, bool delTempFile)
        {
            st.Dispose();
            try
            {
                FileStream fs = st as FileStream;
                if (fs == null || !IsFileExist(fs.Name)) return;

                if (fs.Name.Contains(TempMark))
                {
                    File.Delete(fs.Name);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        #endregion
    }
}