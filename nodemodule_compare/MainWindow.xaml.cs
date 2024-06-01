using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using MessageBox = System.Windows.Forms.MessageBox;
using Path = System.IO.Path;

namespace nodemodule_compare
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string  packageJson = "package-lock.json";
        private string oldFolderPath = string.Empty;
        private string newFolderPath = string.Empty;
        private string targetFolderPath = string.Empty;
        private Dictionary<string, string> modulesOldDict = new Dictionary<string, string>();
        private Dictionary<string, string> modulesNewDict = new Dictionary<string, string>();
        private Dictionary<string, string> moduleUpgradeDict = new Dictionary<string, string>();
        
        private int totalCount = 0; //升级模块数量
        private bool isCompleteCompare = false;
        private int iIndex = 0; //升级模块数量索引
        public MainWindow()
        {
            InitializeComponent();
        }
        private async Task AppendTextAsync(string text)
        {
            //Console.WriteLine(text);
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.richTxtBox.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} : {text}" + "\r");
                // 滚动到RichTextBox的最底端
                this.richTxtBox.ScrollToEnd();
            });
        }
        //选择旧版本目录
        private async void SelectOldPathBtn_Click(object sender, RoutedEventArgs e)
        {
            await AppendTextAsync("正在选择旧版本目录...");

            // 初始化FolderBrowserDialog
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath =string.IsNullOrEmpty(oldFolderPath)?AppDomain.CurrentDomain.BaseDirectory: oldFolderPath;
            var  result = dialog.ShowDialog(); // 显示对话框并获取结果

            if (result ==  System.Windows.Forms.DialogResult.OK) // 如果用户点击了确定
            {
                string selectedFolderPath = dialog.SelectedPath; // 获取选择的目录路径
                                                                 // 假设有一个全局变量或属性用于存储路径
                oldFolderPath = selectedFolderPath; // 保存选择的目录路径
                this.OldPathTxtBox.Text = oldFolderPath;
                await AppendTextAsync($"旧版本目录已选择：{selectedFolderPath}");
                // 这里可以继续添加其他逻辑，如验证目录是否存在、比较操作等
                modulesOldDict=await ExtractModulesAndVersions(oldFolderPath);
                if (!string.IsNullOrEmpty(oldFolderPath) && !string.IsNullOrEmpty(newFolderPath))
                {
                    isCompleteCompare = false;
                    this.StartCompare.IsEnabled = true;
                }
                await AppendTextAsync($"旧版本目录解析完成.");
            }
            else
            {
                await AppendTextAsync("目录选择取消。");
            }
        }
        //选择新版本目录
        private async void SelectNewPathBtn_Click(object sender, RoutedEventArgs e)
        {
            await AppendTextAsync("正在选择新版本目录...");
            // 初始化FolderBrowserDialog
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = string.IsNullOrEmpty(newFolderPath) ? AppDomain.CurrentDomain.BaseDirectory : newFolderPath; ;
            var result = dialog.ShowDialog(); // 显示对话框并获取结果

            if (result == System.Windows.Forms.DialogResult.OK) // 如果用户点击了确定
            {
                string selectedFolderPath = dialog.SelectedPath; // 获取选择的目录路径
                                                                 // 假设有一个全局变量或属性用于存储路径
                newFolderPath = selectedFolderPath; // 保存选择的目录路径
                this.NewPathTxtBox.Text = newFolderPath;
                await AppendTextAsync($"新版本目录已选择：{selectedFolderPath}");
                // 这里可以继续添加其他逻辑，如验证目录是否存在、比较操作等
                modulesNewDict = await ExtractModulesAndVersions(newFolderPath );
                if (!string.IsNullOrEmpty(oldFolderPath) && !string.IsNullOrEmpty(newFolderPath))
                {
                    isCompleteCompare = false;
                    this.StartCompare.IsEnabled = true;
                }
                await AppendTextAsync($"新版本目录解析完成.");
            }
            else
            {
                await AppendTextAsync("目录选择取消。");
            }
        }
        //选择目标目录
        private async void SelectTargetPathBtn_Click(object sender, RoutedEventArgs e)
        {
            await AppendTextAsync("正在选择提取文件的目标位置...");
            // 初始化FolderBrowserDialog
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = string.IsNullOrEmpty(targetFolderPath) ? AppDomain.CurrentDomain.BaseDirectory : targetFolderPath; ;
            var result = dialog.ShowDialog(); // 显示对话框并获取结果

            if (result == System.Windows.Forms.DialogResult.OK) // 如果用户点击了确定
            {
                string selectedFolderPath = dialog.SelectedPath; // 获取选择的目录路径
                                                                 // 假设有一个全局变量或属性用于存储路径
                targetFolderPath = selectedFolderPath; // 保存选择的目录路径
                this.TargetPathTxtBox.Text = targetFolderPath;
                await AppendTextAsync($"提取文件的目标位置已选择：{targetFolderPath}");
                if (isCompleteCompare)
                {
                    this.ExtractFiles.IsEnabled = true;
                }
            }
            else
            {
                await AppendTextAsync("提取文件的目标位置选择取消。");
            }
        }
        //开始对比
        private async void StartCompare_Click(object sender, RoutedEventArgs e)
        {
            await AppendTextAsync("开始对比...");
            // 这里添加您的对比逻辑
            moduleUpgradeDict=await CompareModules(modulesOldDict, modulesNewDict);
            totalCount = moduleUpgradeDict.Count;
            await AppendTextAsync("对比完成。");
            isCompleteCompare = true;
            if (!string.IsNullOrEmpty(targetFolderPath))
            {
                this.ExtractFiles.IsEnabled = true;
            }
            iIndex = 0;
        }
        
        //提取文件
        private async void ExtractFiles_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(targetFolderPath))
            {
                MessageBox.Show("请选择提取目标位置！");
                return;
            }
            await AppendTextAsync("开始提取node_modules差异文件...");
           
            // 这里添加您的对比逻辑
            foreach (var item in moduleUpgradeDict.Keys)
            {
                await CreateUpgradeModules(item, item);
            }
            await AppendTextAsync($"提取node_modules差异文件完成,提取文件路径:{targetFolderPath}" + @"\" + "node_modules");
            if (this.IsCopyDistDirChkBox.IsChecked == true)
            {
                await AppendTextAsync("开始复制dist目录...");
                await CopyDirectory(Path.Combine(newFolderPath,"dist"), Path.Combine(targetFolderPath,"dist"));
                await AppendTextAsync($"复制dist目录完成,目录路径:{targetFolderPath}" + @"\" + "dist");
                await AppendTextAsync("开始复制package-lock.json文件...");
                ////备份现有package-lock.json
                //await AppendTextAsync("备份现有package-lock.json至bak文件夹...");
                //File.Copy(Path.Combine(targetFolderPath, "package-lock.json"), Path.Combine(targetFolderPath,$"package-lock-{DateTime.Now.ToString()}.json"), true);
                //拷贝最新的package-lock.json
                await AppendTextAsync("复制最新的package-lock.json...");
                File.Copy(Path.Combine(newFolderPath, "package-lock.json"), Path.Combine(targetFolderPath, "package-lock.json"), true);
                //拷贝最新的node_modules下面的.package-lock.json
                await AppendTextAsync("复制最新的node_modules/.package-lock.json...");
                File.Copy(Path.Combine(newFolderPath,"node_modules",".package-lock.json"), Path.Combine(targetFolderPath, "node_modules", ".package-lock.json"), true);
                await AppendTextAsync("复制package-lock.json完成");
            }         
        }
        //加载package-lock.json
        private  async Task<Dictionary<string, string>> ExtractModulesAndVersions(string path)
        {
            var packageJsonPath = path;
            var resultDict= new Dictionary<string, string>();
            //string jsonContent = File.ReadAllText(path);
            //JObject json = JObject.Parse(jsonContent);
            //JObject packages = json["packages"] as JObject;
            //if (packages != null)
            //{
            //    foreach (var package in packages.Properties())
            //    {
            //        if (string.IsNullOrEmpty(package.Name)) continue;
            //        ////Console.WriteLine(package.Name);
            //        var jObj = package.Value;
            //        string version = jObj["version"]?.ToString();
            //        resultDict.Add(package.Name, version);
            //        await AppendTextAsync($"Name:{package.Name},Version:{version}");

            //    }
            //}
            if (!File.Exists(path + "/package-lock.json"))
            {
                if(!File.Exists(path + "/node_modules/.package-lock.json"))
                {
                    MessageBox.Show("未找到package-lock.json文件！");
                    return null;
                }
                else
                {
                    packageJsonPath = path + "/node_modules/.package-lock.json";
                }
            }
            else
            {
                packageJsonPath = path + "/package-lock.json";
            }
            using (var stream = new FileStream(packageJsonPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string jsonContent = await reader.ReadToEndAsync();
                JObject json = JObject.Parse(jsonContent);
                JObject packages = json["packages"] as JObject;

                if (packages != null)
                {
                    foreach (var package in packages.Properties())
                    {
                        if (string.IsNullOrEmpty(package.Name)) continue;

                        var jObj = package.Value;
                        string version = jObj["version"]?.ToString();
                        resultDict.Add(package.Name, version);

                        // 假设AppendTextAsync是一个模拟的异步写入方法，需要在您的上下文中实现
                        await Task.Delay(1);
                        await AppendTextAsync($"Name:{package.Name},Version:{version}");
                    }
                }
            }
            return resultDict;
        }
        //对比版本，new old 交集中寻找更新版本的，差集中寻找增量的
        private async Task<Dictionary<string,string>>  CompareModules(Dictionary<string,string> oldModuesDict,Dictionary<string,string> newModulesDict)
        {
            var resultDict=new Dictionary<string,string>();
            //交集
            var inter=newModulesDict.Intersect(oldModuesDict);
            //差集
            var diff = newModulesDict.Except(newModulesDict);
            await AppendTextAsync("检查引用库版本是否升级...");
            foreach (var moduleName in newModulesDict.Keys.Intersect(oldModuesDict.Keys))
            {
                if (newModulesDict[moduleName] != oldModuesDict[moduleName])
                {
                    resultDict.Add(moduleName, newModulesDict[moduleName]);
                    await Task.Delay(1);
                    await AppendTextAsync($"upgrade module name:{moduleName},version:{oldModuesDict[moduleName]} ---> {newModulesDict[moduleName]}");
                }
            }
            await AppendTextAsync("检查引用库是否有新增...");
            foreach (var moduleName in newModulesDict.Keys.Except(newModulesDict.Keys))
            {
                resultDict.Add(moduleName, newModulesDict[moduleName]);
                await Task.Delay(1);
                await AppendTextAsync($"add new module name:{moduleName},version:{newModulesDict[moduleName]}");
            }
            return resultDict;
        }

        /// <summary>
        /// 创建升级文件存储的目录
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        private async Task CreateUpgradeModules(string destPath,string moduleName)
        {
            //await AppendTextAsync("创建目录...");
            //// 分割路径以创建目录
            //string[] directories = moduleName.Split('/');
            //var path = rootPath;

            //// 递归创建目录
            //foreach (string directory in directories)
            //{
            //    if (!string.IsNullOrWhiteSpace(directory))
            //    {
            //        path = path +directory+"\\";
            //        Directory.CreateDirectory(path);
            //        await AppendTextAsync($"create directory:{path}");

            //    }
            //}
            iIndex += 1;
            if (iIndex > totalCount)
            {
                iIndex = 1;
            }
            await Task.Delay(1);
            await AppendTextAsync($"复制要升级的模块{iIndex}/{totalCount}: {moduleName}...");
            // 递归拷贝目录
            var sourceDirPath =newFolderPath+"\\"+string.Join("\\", moduleName.Split('/'));
            var destDirPath = Path.Combine(targetFolderPath , destPath);
            await CopyDirectory(sourceDirPath, destDirPath);
        }
        private async Task CopyDirectory(string source, string destination)
        {
            // 获取源文件夹中的所有文件和子文件夹
            string[] files = Directory.GetFiles(source);
            string[] directories = Directory.GetDirectories(source);

            // 创建目标文件夹
            await Task.Delay(1);
            await AppendTextAsync($"create directory:{destination}.");
            Directory.CreateDirectory(destination);

            // 拷贝文件
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destinationPath = Path.Combine(destination, fileName);
                File.Copy(file, destinationPath,true);
            }

            // 递归拷贝子文件夹
            foreach (string directory in directories)
            {
                string directoryName = Path.GetFileName(directory);
                string destinationPath = Path.Combine(destination, directoryName);
                await CopyDirectory(directory, destinationPath);
            }
        }       
    }
}
