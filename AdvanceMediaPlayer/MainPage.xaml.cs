using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace AdvanceMediaPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async void FileChooseClick(object sender, RoutedEventArgs e)
        {
            await SetLocalMedia();
        }

        async private System.Threading.Tasks.Task SetLocalMedia()
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp3");

            var file = await openPicker.PickSingleFileAsync();

            // mediaPlayer is a MediaPlayerElement defined in XAML
            if (file != null)
            {
                player.Source = MediaSource.CreateFromStorageFile(file);
                player.MediaPlayer.Play();
            }
        }

        private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            String inputURL = queryURL.Text;
            if (inputURL.Length == 0)
                return;
            if (online.IsChecked == true)
            {
                errlog.Text = "开始在线播放";
                PlayOnline();
                return;
            }
            await StartDownload();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            errlog.Text = "";
        }

        private void PlayOnline()
        {
            player.Source = MediaSource.CreateFromUri(new Uri("http://www.neu.edu.cn/indexsource/neusong.mp3"));
            player.MediaPlayer.Play();
        }

        
        private async System.Threading.Tasks.Task StartDownload()
        {
            String inputURL = queryURL.Text;
            if (inputURL.Length == 0)
                return;

            try
            {
                errlog.Text = "文件开始下载，请耐心等待";
                Uri source = new Uri(inputURL);

                StorageFile destinationFile = await KnownFolders.MusicLibrary.CreateFileAsync(
                   RandomString(4) + ".mp3", CreationCollisionOption.GenerateUniqueName);
                BackgroundDownloader downloader = new BackgroundDownloader();
                DownloadOperation download = downloader.CreateDownload(source, destinationFile);

                await download.StartAsync();
                // Attach progress and completion handlers.
                errlog.Text = "文件下载完成，马上开始播放";
                player.Source = MediaSource.CreateFromStorageFile(destinationFile);
                player.MediaPlayer.Play();
            }
            catch (Exception ex)
            {
                errlog.Text = "文件下载失败，请检查输入网址是否错误";
            }
        }

        /*
        async private System.Threading.Tasks.Task StartDownload()
        {
            String inputURL = queryURL.Text;
            if (inputURL.Length == 0)
                return;

            errlog.Text = "文件开始下载，请耐心等待";
            StorageFile destinationFile = await KnownFolders.MusicLibrary.CreateFileAsync(
                   RandomString(4) + ".mp3", CreationCollisionOption.GenerateUniqueName);

            using (WebClient client = new WebClient())
            {
                client.DownloadFileAsync(new Uri(inputURL.Trim()), destinationFile.Path);
                client.DownloadProgressChanged += client_DownloadProgressChanged;
                client.DownloadFileCompleted += client_DownloadFileCompleted;
            }

            player.Source = MediaSource.CreateFromStorageFile(destinationFile);
            player.MediaPlayer.Play();
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        { 
            progressBar.Value = e.ProgressPercentage;
        }

        void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            progressBar.Value = 0;
            errlog.Text = "文件下载完成，马上开始播放";
        }*/

         /*
        private void HttpDownloadFile(string url, string path, bool overwrite, Action<string, long, long> callback = null)
        {
            errlog.Text = "文件开始下载，请耐心等待";
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //获取文件名
            string fileName = response.Headers["Content-Disposition"];//attachment;filename=FileName.txt
            if (string.IsNullOrEmpty(fileName))
                fileName = response.ResponseUri.Segments[response.ResponseUri.Segments.Length - 1];
            else
                fileName = fileName.Remove(0, fileName.IndexOf("filename=") + 9);

            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            using (Stream responseStream = response.GetResponseStream())
            {
                long totalLength = response.ContentLength;
                //创建本地文件写入流
                using (Stream stream = new FileStream(Path.Combine(path, fileName), overwrite ? FileMode.Create : FileMode.CreateNew))
                {
                    byte[] bArr = new byte[1024];
                    int size;
                    while ((size = responseStream.Read(bArr, 0, bArr.Length)) > 0)
                    {
                        stream.Write(bArr, 0, size);
                        callback?.Invoke(fileName, totalLength, stream.Length);
                    }
                }
            }

            errlog.Text = "文件下载完成，马上开始播放";
        }

        private void updateProgress(String fileName, long total, long length)
        {
            progressBar.Value = length / total;
        }

         */
    }
}
