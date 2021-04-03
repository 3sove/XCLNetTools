/*
һ��������Ϣ��
��ԴЭ�飺https://github.com/xucongli1989/XCLNetTools/blob/master/LICENSE
��Ŀ��ַ��https://github.com/xucongli1989/XCLNetTools
Create By: XCL @ 2012

 */

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XCLNetTools.FileHandler
{
    /// <summary>
    /// �ļ�Ŀ¼������
    /// </summary>
    public static class FileDirectory
    {
        #region Ŀ¼����

        /// <summary>
        /// ���Ŀ¼�Ƿ�Ϊ��Ŀ¼����û���ļ��У�Ҳû���ļ���
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
        /// �ж�Ŀ¼�Ƿ����
        /// </summary>
        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// ����Ŀ¼
        /// </summary>
        public static bool MakeDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ɾ��Ŀ¼��ɾ�����µ���Ŀ¼�����ļ�
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
        /// ���ָ��Ŀ¼
        /// </summary>
        public static bool ClearDirectory(string rootPath)
        {
            //ɾ����Ŀ¼
            string[] subPaths = System.IO.Directory.GetDirectories(rootPath);
            foreach (string path in subPaths)
            {
                DelTree(path);
            }
            //ɾ���ļ�
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
        /// ��ȡָ��Ŀ¼�µ������ļ����ļ�����Ϣ
        /// </summary>
        /// <param name="dirPath">Ҫ��ȡ��Ϣ��Ŀ¼·��</param>
        /// <param name="rootPath">��·�������ø�ֵ�󣬷��ص���Ϣʵ���н���������ڸø�·�������·����Ϣ��</param>
        /// <param name="webRootPath">web��·�����������ɸ��ļ����ļ��е�web·�������磺http://www.a.com/web/</param>
        /// <returns>�ļ���Ϣlist</returns>
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
            //�ļ���
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

            //�ļ�
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
        /// ����ָ���ļ���·���ĸ��ļ��е�ַ���磺"C:\a\b\c\d\" ---> "C:\a\b\c"
        /// </summary>
        public static string GetDirParentPath(string dirPath)
        {
            return Path.GetDirectoryName(Path.GetDirectoryName(dirPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar));
        }

        #endregion Ŀ¼����

        #region �ļ�����

        /// <summary>
        /// ����һ���ļ�
        /// </summary>
        public static bool CreateTextFile(string path)
        {
            FileInfo info = new FileInfo(path);
            if (info.Exists)
            {
                return true;
            }
            using (var fs = info.Create())
            {
                if (null != fs)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ���ļ���׷������
        /// </summary>
        /// <param name="filePathName">�ļ���</param>
        /// <param name="writeWord">׷������</param>
        public static void AppendText(string filePathName, string writeWord)
        {
            AppendText(filePathName, writeWord, System.Text.Encoding.Default);
        }

        /// <summary>
        /// ���ļ���׷������
        /// </summary>
        /// <param name="filePathName">�ļ���</param>
        /// <param name="writeWord">׷�ӵ�����</param>
        /// <param name="encode">����</param>
        public static void AppendText(string filePathName, string writeWord, System.Text.Encoding encode)
        {
            //�����ļ�
            CreateTextFile(filePathName);
            //�õ�ԭ���ļ�������
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
        /// ��ȡ�ı��ļ�������ݣ��Զ�ʶ����룬���ԭ������ascii����Ĭ����utf8��ȡ��
        /// </summary>
        /// <param name="filePathName">·��</param>
        /// <returns>�ļ�����</returns>
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
        /// ���ı��ļ���д�����ݣ������ļ��е��������ݣ����Զ�ʶ����룬���ԭ������ascii����Ĭ����utf8д�룩
        /// </summary>
        public static void WriteFileData(string filePathName, string content)
        {
            var encode = XCLNetTools.FileHandler.ComFile.GetFileEncoding(filePathName);
            if (encode == System.Text.Encoding.ASCII)
            {
                encode = System.Text.Encoding.UTF8;
            }
            System.IO.File.WriteAllText(filePathName, content, encode);
        }

        /// <summary>
        /// ɾ���ļ�
        /// </summary>
        /// <param name="absoluteFilePath">�ļ����Ե�ַ</param>
        /// <returns>true:ɾ���ļ��ɹ�,false:ɾ���ļ�ʧ��</returns>
        public static bool FileDelete(string absoluteFilePath)
        {
            try
            {
                FileInfo objFile = new FileInfo(absoluteFilePath);
                if (objFile.Exists)//�������
                {
                    //ɾ���ļ�.
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

        #endregion �ļ�����

        #region ����

        /// <summary>
        /// ��ȡ��ǰ����ϵͳ���������·�����磺C:\Users\XCL\Desktop
        /// </summary>
        public static string GetDesktopPath()
        {
            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        }

        #endregion ����
    }
}