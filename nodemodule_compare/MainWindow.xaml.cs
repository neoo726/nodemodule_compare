using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Win32; 
namespace nodemodule_compare
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string oldFolderPath = string.Empty;
        private string newFolderPath = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
        }
        private async Task AppendTextAsync(string text)
        {
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
            var  result = dialog.ShowDialog(); // 显示对话框并获取结果

            if (result ==  System.Windows.Forms.DialogResult.OK) // 如果用户点击了确定
            {
                string selectedFolderPath = dialog.SelectedPath; // 获取选择的目录路径
                                                                 // 假设有一个全局变量或属性用于存储路径
                oldFolderPath = selectedFolderPath; // 保存选择的目录路径
                this.OldPathTxtBox.Text = oldFolderPath;
                await AppendTextAsync($"旧版本目录已选择：{selectedFolderPath}");
                // 这里可以继续添加其他逻辑，如验证目录是否存在、比较操作等
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
            var result = dialog.ShowDialog(); // 显示对话框并获取结果

            if (result == System.Windows.Forms.DialogResult.OK) // 如果用户点击了确定
            {
                string selectedFolderPath = dialog.SelectedPath; // 获取选择的目录路径
                                                                 // 假设有一个全局变量或属性用于存储路径
                newFolderPath = selectedFolderPath; // 保存选择的目录路径
                this.NewPathTxtBox.Text = newFolderPath;
                await AppendTextAsync($"新版本目录已选择：{selectedFolderPath}");
                // 这里可以继续添加其他逻辑，如验证目录是否存在、比较操作等
            }
            else
            {
                await AppendTextAsync("目录选择取消。");
            }
        }
        //开始对比
        private async void StartCompare_Click(object sender, RoutedEventArgs e)
        {
            await AppendTextAsync("开始对比...");
            // 这里添加您的对比逻辑
            await Task.Delay(1000); // 模拟耗时操作
            await AppendTextAsync("对比完成。");
        }
        //提取文件
        private async void ExtractFiles_Click(object sender, RoutedEventArgs e)
        {
            await AppendTextAsync("开始提取文件...");
            // 这里添加您的对比逻辑
            await Task.Delay(1000); // 模拟耗时操作
            await AppendTextAsync("提取完成。");
        }
    }
}
