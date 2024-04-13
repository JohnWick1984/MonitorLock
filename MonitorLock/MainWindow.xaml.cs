using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MonitorLock
{
    public partial class MainWindow : Window
    {
        private Timer autosaveTimer;

        public MainWindow()
        {
            InitializeComponent();

            
            autosaveTimer = new Timer(30000); 
            autosaveTimer.Elapsed += AutosaveTimer_Elapsed;
            autosaveTimer.AutoReset = true;
            autosaveTimer.Start();
        }

        private void AutosaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
           
            Dispatcher.Invoke(() =>
            {
                
                string textToSave = textBox.Text;
                File.WriteAllText("autosave.txt", textToSave);
            });
        }


        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
           
            textBox.Text = string.Empty;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"; 

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                string fileContent = File.ReadAllText(fileName);
                textBox.Text = fileContent;
            }
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"; 

            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                string textToSave = textBox.Text;
                File.WriteAllText(fileName, textToSave);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
           
            string searchText = searchTextBox.Text;

            
            int partSize = textBox.Text.Length / 3;
            string[] parts = new string[3];
            for (int i = 0; i < 3; i++)
            {
                int startIndex = i * partSize;
                int length = i == 2 ? textBox.Text.Length - startIndex : partSize;
                parts[i] = textBox.Text.Substring(startIndex, length);
            }

           
            int[] results = new int[3];
            Thread[] threads = new Thread[3];
            for (int i = 0; i < 3; i++)
            {
                int index = i;
                threads[i] = new Thread(() =>
                {
                    int count = CountOccurrences(parts[index], searchText);
                    lock (results)
                    {
                        results[index] = count;
                    }
                });
                threads[i].Start();
            }

            
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

           
            int totalCount = 0;
            foreach (int count in results)
            {
                totalCount += count;
            }

           
            MessageBox.Show($"Total occurrences: {totalCount}");
        }

        private int CountOccurrences(string text, string searchText)
        {
            int count = 0;
            int index = 0;
            while ((index = text.IndexOf(searchText, index)) != -1)
            {
                count++;
                index += searchText.Length;
            }
            return count;
        }
    }
}
