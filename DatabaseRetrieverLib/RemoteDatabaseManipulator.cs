using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;


namespace DatabaseRetrieverLib
{
    public class RemoteDatabaseManipulator
    {
        private string RemoteServer { get; set; }
        private int RemotePort { get; set; }

        private string DbName { get; set; }
        private string User { get; set; }
        private string Password { get; set; }
        

        public RemoteDatabaseManipulator()
        {     
        }

        private Server Connect()
        {
            ServerConnection serverConnection = new ServerConnection
            {
                ServerInstance = RemoteServer,
                DatabaseName = DbName,
                ConnectAsUserName = User,
                Password = Password
            };
            Server server = new Server(serverConnection);
            return null;
        }

        public Boolean PerformRemoteBackup(String backupDir)
        {
            if (!System.IO.Directory.Exists(backupDir))
            {
                System.IO.Directory.CreateDirectory(backupDir);
            }

            Server sqlServer = Connect();
            if (sqlServer != null)
            {
                DatabaseCollection dbc = sqlServer.Databases;
                if (dbc.Contains(DbName))
                {
                    Backup bkpDatabase = new Backup
                                             {
                                                 Action = BackupActionType.Database,
                                                 Database = DbName,
                                                 Incremental = false,
                                                 Initialize = true,
                                             };
                    BackupDeviceItem bkpDevice = new BackupDeviceItem(backupDir + "\\" + DbName + ".bak", DeviceType.File);
                    bkpDatabase.Devices.Add(bkpDevice);

                    // Perform the backup
                    bkpDatabase.SqlBackup(sqlServer);

                    //TODO retrieve backup file

                    if (System.IO.File.Exists(backupDir + "\\" + DbName + ".bak"))
                    {
                        return true;
                    }
                    //todo throw useful exception
                    return false;
                }
                //todo throw useful exception
                return false;
            }
            //todo throw useful exception
            return false;
        }

        private void RetrieveBackupFile(string backupFile)
        {
            TcpClient client = new TcpClient(RemoteServer,RemotePort);
            NetworkStream stream = client.GetStream();
            byte[] request = System.Text.Encoding.ASCII.GetBytes("i want "+backupFile);
            stream.Write(request, 0, request.Count());

            int i;
            Byte[] bytes = new Byte[256];
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {

            }
        }
    }
}
