using Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace SswareApi
{
    public partial class Command
    {
        public sealed class FileManagerClass : XObjectManagerClass
        {
            private FileHelper FileHelper;

            private string RootPath;

            public FileManagerClass(Command command) : base(command)
            {

            }

            #region Manage File

            public void SetFileServer(string rootPath, byte[] iv)
            {
                RootPath = rootPath;
                FileHelper = new FileHelper(rootPath, iv);
            }

            public void SetFtpServer(string rootPath, string id, string password, byte[] iv)
            {
                RootPath = rootPath;
                FileHelper = new FileHelper(rootPath, id, password, iv);
            }

            public XFile Upload(string path)
            {
                if (!File.Exists(path))
                {
                    throw new Exception("File doesn't exist");
                }

                byte[] fileByte = File.ReadAllBytes(path);
                return Upload(Path.GetFileNameWithoutExtension(path)
                    , Path.GetExtension(path), fileByte);
            }

            public XFile Upload(byte[] content, string name, string extension)
            {
                if (content.Length == 0)
                {
                    throw new Exception("Zero file size");
                }

                return Upload(name, extension, content);
            }

            private XFile Upload(string name, string extension, byte[] content)
            {
                Verification(UserType.Employee);

                string randomName;

                do
                {
                    string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZ1234567890@#$%&";
                    StringBuilder stringBuilder = new StringBuilder();
                    Random random = new Random();
                    for (int i = 0; i < 200; i++)
                    {
                        stringBuilder.Append(chars[random.Next(40)]);
                    }
                    randomName = stringBuilder.ToString() + ".SSW";
                } while (File.Exists(RootPath + randomName));

                DataSet ds = new DataSet();
                byte[] keyCode = FileHelper.Upload(@"Files\" + randomName, content, true);

                Command.DbHelper.AddInput("@operation", 1, SqlDbType.Int);
                Command.DbHelper.AddInput("@namez", name, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@extension", extension, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@directory", @"Files\", SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@randomName", randomName, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@keyCode", keyCode, SqlDbType.Binary);
                Command.DbHelper.AddInput("@registeredBy", Command.CurrentUser.Id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_FileManager", ds);

                return new XFile(GetObjectInfo((int)XObjectType.File
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            private DataRow GetDownloadInfo(int id)
            {
                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 2, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_FileManager", ds);

                return ds.Tables[0].Rows[0];
            }

            public byte[] Download(int id)
            {
                Verification(UserType.Employee, XObjectType.File, id);

                DataRow dataRow = GetDownloadInfo(id);
                return FileHelper.Download(
                    (string)dataRow["Directory"], null, (byte[])dataRow["keyCode"]);
            }

            public void Download(int id, string directory)
            {
                Verification(UserType.Employee, XObjectType.File, id);

                DataRow dataRow = GetDownloadInfo(id);
                string sourcePath = @"Files\" + dataRow["RandomName"] + ".SSW";
                string path = directory + (string)dataRow["Namez"] + (string)dataRow["Extension"];
                FileHelper.Download(sourcePath, path, (byte[])dataRow["keyCode"]);
            }

            public void EnableDisableFile(int id, bool enable)
            {
                Verification(UserType.Employee, XObjectType.File, id);

                EnableDisableObject(10, id, enable);
            }

            public void DeleteFile(int id)
            {
                Verification(UserType.ItAdministrator);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 2, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_FileManager", ds);

                DataRow dataRow = ds.Tables[0].Rows[0];
                File.Delete(RootPath + dataRow["Directory"] + dataRow["RandomName"] + ".SSW");

                DeleteObject(10, id);
            }

            #endregion

            #region Manage Folder

            public Folder CreateFolder(string name)
            {
                Verification(UserType.Employee);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 101, SqlDbType.Int);
                Command.DbHelper.AddInput("@name", name, SqlDbType.NVarChar);
                Command.DbHelper.StoredProcedure("p_FileManager", ds);

                return new Folder(GetObjectInfo((int)XObjectType.Folder
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            public void EnableDisableFolder(int id, bool enable)
            {
                Verification(UserType.Employee, XObjectType.Folder, id);

                EnableDisableObject(11, id, enable);
            }

            public void DeleteFolder(int id)
            {
                Verification(UserType.ItAdministrator);

                DeleteObject(11, id);
            }

            public void BindFolder(int parent, int child)
            {
                Verification(UserType.Employee, XObjectType.Folder, parent);
                Verification(UserType.Employee, XObjectType.Folder, child);

                Command.DbHelper.AddInput("@operation", 102, SqlDbType.Int);
                Command.DbHelper.AddInput("@parent", parent, SqlDbType.Int);
                Command.DbHelper.AddInput("@child", child, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_FileManager");
            }

            public void BindFile(int folder, int file)
            {
                Verification(UserType.Employee, XObjectType.Folder, folder);
                Verification(UserType.Employee, XObjectType.File, file);

                Command.DbHelper.AddInput("@operation", 103, SqlDbType.Int);
                Command.DbHelper.AddInput("@folder", folder, SqlDbType.Int);
                Command.DbHelper.AddInput("@files", file, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_FileManager");
            }

            public List<Folder> GetChildFolders(int id)
            {
                Verification(UserType.Employee, XObjectType.Folder, id);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 104, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_FileManager", ds);

                List<Folder> result = new List<Folder>();

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    result.Add(new Folder(ToDictionary(r)));
                }

                return result;
            }

            public List<XFile> GetMemberFiles(int id)
            {
                Verification(UserType.Employee, XObjectType.Folder, id);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 105, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_FileManager", ds);

                List<XFile> result = new List<XFile>();

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    result.Add(new XFile(ToDictionary(r)));
                }

                return result;
            }

            #endregion
        }
    }
}
