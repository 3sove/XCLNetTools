/*
一：基本信息：
开源协议：https://github.com/xucongli1989/XCLNetTools/blob/master/LICENSE
项目地址：https://github.com/xucongli1989/XCLNetTools
Create By: XCL @ 2012

 */

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XCLNetTools.FileHandler
{
    /// <summary>
    /// 文件目录操作类
    /// </summary>
    public static class FileDirectory
    {
        #region 目录操作

        /// <summary>
        /// 检测目录是否为空目录（既没有文件夹，也没有文件）
        /// </summary>
        public static bool IsEmpty(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return true;
            }
            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);
            return (null == files || files.Length == 0) && (null == dirs || dirs.Length == 0);
        }

        /// <summary>
        /// 判断目录是否存在
        /// </summary>
        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// 建立目录
        /// </summary>
        public static bool MakeDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }
            try
            {
                Directory.CreateDirectory(path);
            }
            catch
            {
                //
            }
            return Directory.Exists(path);
        }

        /// <summary>
        /// 给文件路径创建目录
        /// </summary>
        public static void MakeDirectoryForFile(string path)
        {
            XCLNetTools.FileHandler.FileDirectory.MakeDirectory(XCLNetTools.FileHandler.ComFile.GetFileFolderPath(path));
        }

        /// <summary>
        /// 删除目录并删除其下的子目录及其文件
        /// </summary>
        public static bool DelTree(string path)
        {
            if (DirectoryExists(path))
            {
                Directory.Delete(path, true);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 清空指定目录
        /// </summary>
        public static bool ClearDirectory(string rootPath)
        {
            //删除子目录
            string[] subPaths = System.IO.Directory.GetDirectories(rootPath);
            foreach (string path in subPaths)
            {
                DelTree(path);
            }
            //删除文件
            string[] files = XCLNetTools.FileHandler.ComFile.GetFolderFiles(rootPath);
            if (null != files && files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    XCLNetTools.FileHandler.ComFile.DeleteFile(files[i]);
                }
            }
            return true;
        }

        /// <summary>
        /// 获取指定目录下的所有文件及文件夹信息
        /// </summary>
        /// <param name="dirPath">要获取信息的目录路径</param>
        /// <param name="rootPath">根路径（设置该值后，返回的信息实体中将包含相对于该根路径的相对路径信息）</param>
        /// <param name="webRootPath">web根路径（用于生成该文件或文件夹的web路径），如：http://www.a.com/web/</param>
        /// <returns>文件信息list</returns>
        public static List<XCLNetTools.Entity.FileInfoEntity> GetFileList(string dirPath, string rootPath = "", string webRootPath = "")
        {
            List<XCLNetTools.Entity.FileInfoEntity> result = new List<Entity.FileInfoEntity>();
            if (!string.IsNullOrEmpty(dirPath))
            {
                dirPath = XCLNetTools.FileHandler.ComFile.MapPath(dirPath);
            }
            if (!string.IsNullOrEmpty(rootPath))
            {
                rootPath = XCLNetTools.FileHandler.ComFile.MapPath(rootPath);
            }

            if (string.IsNullOrEmpty(dirPath) || FileDirectory.IsEmpty(dirPath))
            {
                return result;
            }
            int idx = 1;

            XCLNetTools.Entity.FileInfoEntity tempFileInfoEntity = null;
            //文件夹
            var directories = System.IO.Directory.EnumerateDirectories(dirPath);
            if (null != directories && directories.Any())
            {
                directories.ToList().ForEach(k =>
                {
                    var dir = new System.IO.DirectoryInfo(k);
                    if (dir.Exists)
                    {
                        tempFileInfoEntity = new Entity.FileInfoEntity();
                        tempFileInfoEntity.ID = idx++;
                        tempFileInfoEntity.Name = dir.Name;
                        tempFileInfoEntity.IsFolder = true;
                        tempFileInfoEntity.Path = k;
                        tempFileInfoEntity.RootPath = rootPath;
                        tempFileInfoEntity.RelativePath = ComFile.GetUrlRelativePath(rootPath, k);
                        tempFileInfoEntity.WebPath = webRootPath.TrimEnd('/') + "/" + tempFileInfoEntity.RelativePath;
                        tempFileInfoEntity.ModifyTime = dir.LastWriteTime;
                        tempFileInfoEntity.CreateTime = dir.CreationTime;
                        result.Add(tempFileInfoEntity);
                    }
                });
            }

            //文件
            string[] files = XCLNetTools.FileHandler.ComFile.GetFolderFiles(dirPath);
            if (null != files && files.Length > 0)
            {
                files.ToList().ForEach(k =>
                {
                    var file = new System.IO.FileInfo(k);
                    if (file.Exists)
                    {
                        tempFileInfoEntity = new Entity.FileInfoEntity();
                        tempFileInfoEntity.ID = idx++;
                        tempFileInfoEntity.Name = file.Name;
                        tempFileInfoEntity.IsFolder = false;
                        tempFileInfoEntity.Path = k;
                        tempFileInfoEntity.RootPath = rootPath;
                        tempFileInfoEntity.RelativePath = ComFile.GetUrlRelativePath(rootPath, k);
                        tempFileInfoEntity.WebPath = webRootPath.TrimEnd('/') + "/" + tempFileInfoEntity.RelativePath;
                        tempFileInfoEntity.ModifyTime = file.LastWriteTime;
                        tempFileInfoEntity.CreateTime = file.CreationTime;
                        tempFileInfoEntity.Size = file.Length;
                        tempFileInfoEntity.ExtName = (file.Extension ?? "").Trim('.');
                        result.Add(tempFileInfoEntity);
                    }
                });
            }

            return result;
        }

        /// <summary>
        /// 返回指定文件夹路径的父文件夹地址，如："C:\a\b\c\d\" ---> "C:\a\b\c"
        /// </summary>
        public static string GetDirParentPath(string dirPath)
        {
            if (string.IsNullOrWhiteSpace(dirPath))
            {
                return string.Empty;
            }
            return Path.GetDirectoryName(Path.GetDirectoryName(dirPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar));
        }

        #endregion 目录操作

        #region 文件操作

        /// <summary>
        /// 建立一个文件
        /// </summary>
        public static bool CreateTextFile(string path)
        {
            var info = new FileInfo(path);
            if (info.Exists)
            {
                return true;
            }
            try
            {
                MakeDirectoryForFile(path);
                //如果磁盘不存在，using 语句会异常
                using (var fs = info.Create())
                {
                    //
                }
            }
            catch
            {
                //
            }
            return System.IO.File.Exists(path);
        }

        /// <summary>
        /// 在文件里追加内容
        /// </summary>
        /// <param name="filePathName">文件名</param>
        /// <param name="writeWord">追加内容</param>
        public static void AppendText(string filePathName, string writeWord)
        {
            AppendText(filePathName, writeWord, System.Text.Encoding.Default);
        }

        /// <summary>
        /// 在文件里追加内容
        /// </summary>
        /// <param name="filePathName">文件名</param>
        /// <param name="writeWord">追加的内容</param>
        /// <param name="encode">编码</param>
        public static void AppendText(string filePathName, string writeWord, System.Text.Encoding encode)
        {
            //建立文件
            CreateTextFile(filePathName);
            //得到原来文件的内容
            using (FileStream fileRead = new FileStream(filePathName, FileMode.Open, FileAccess.ReadWrite))
            using (StreamReader fileReadWord = new StreamReader(fileRead, encode))
            using (StreamWriter fileWrite = new StreamWriter(fileRead, encode))
            {
                string oldString = fileReadWord.ReadToEnd().ToString();
                oldString = oldString + writeWord;
                fileWrite.Write(oldString);
            }
        }

        /// <summary>
        /// 读取文本文件里的内容（自动识别编码，如果原编码是ascii，则默认以utf8读取）
        /// </summary>
        /// <param name="filePathName">路径</param>
        /// <returns>文件内容</returns>
        public static string ReadFileData(string filePathName)
        {
            if (!System.IO.File.Exists(filePathName))
            {
                return "";
            }
            var encode = XCLNetTools.FileHandler.ComFile.GetFileEncoding(filePathName);
            if (encode == System.Text.Encoding.ASCII)
            {
                encode = System.Text.Encoding.UTF8;
            }
            return System.IO.File.ReadAllText(filePathName, encode) ?? "";
        }

        /// <summary>
        /// 向路径中写入内容（覆盖路径中的所有内容，如果路径不存在，则自动创建该路径。如果编码是 ascii 且包含 Unicode 字符，则默认以 utf8 写入）
        /// </summary>
        public static void WriteFileData(string filePathName, string content, System.Text.Encoding encode)
        {
            if (encode == System.Text.Encoding.ASCII && XCLNetTools.Encode.Unicode.HasUnicode(content))
            {
                encode = System.Text.Encoding.UTF8;
            }
            System.IO.File.WriteAllText(filePathName, content, encode);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="absoluteFilePath">文件绝对地址</param>
        /// <returns>true:删除文件成功,false:删除文件失败</returns>
        public static bool FileDelete(string absoluteFilePath)
        {
            try
            {
                FileInfo objFile = new FileInfo(absoluteFilePath);
                if (objFile.Exists)//如果存在
                {
                    //删除文件.
                    objFile.Delete();
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        #endregion 文件操作

        #region 其它

        /// <summary>
        /// 获取当前操作系统桌面的物理路径，如：C:\Users\XCL\Desktop
        /// </summary>
        public static string GetDesktopPath()
        {
            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        }

        #endregion 其它
    }
}