using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWatchDog
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // 要監控的應用程序的名稱
            string filePath = @"./applicationPath.txt";

            // 檢查檔案是否存在，如果不存在，則創建檔案和路徑
            if (!File.Exists(filePath))
            {
                // 確保目錄存在
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                // 創建並寫入預設路徑，這裡需要替換為實際的預設路徑
                File.WriteAllText(filePath, @"D:\DispatchSystem.exe");
            }


            string applicationName = File.ReadAllText(filePath).Trim(); // 使用 Trim() 移除可能存在的換行符或空格

            // string applicationName = @"D:\00_專案文件\01_1_AGV 派車系統\F18AB Parts AGV\CODE\dispatchsystem\DispatchSystem\bin\Debug\RunDispatchSystem.bat";
            // 檢查間隔時間（毫秒）
            int checkInterval = 5000;

            while (true)
            {
                // 嘗試找到該程序的進程
                Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(applicationName));

                if (processes.Length == 0)
                {
                    // 如果沒有找到，則啟動該應用程序
                    try
                    {
                        Process.Start(applicationName);
                        Console.WriteLine($"{applicationName} has been started.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error starting {applicationName}: {ex.Message}");
                    }
                }
                else
                {
                    foreach (var process in processes)
                    {
                        if (!process.Responding)
                        {
                            // 如果進程沒有回應，則結束該進程並重啟
                            try
                            {
                                process.Kill();
                                process.WaitForExit(); // 等待進程結束
                                Process.Start(applicationName);
                                Console.WriteLine($"{applicationName} was not responding and has been restarted.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error restarting {applicationName}: {ex.Message}");
                            }
                        }
                    }
                }

                // 等待一段時間後再次檢查
                Thread.Sleep(checkInterval);
            }
        }
    }
}
