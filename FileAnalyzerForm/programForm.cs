using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace FileAnalyzerForm
{
    public partial class programForm : Form
    {
        private Methods _fileExtensions;

        public programForm()
        {
            InitializeComponent();
            this.Text = "FileAnalyzer";

            //this.Size = new Size(800, 800);
            //this.FormBorderStyle = FormBorderStyle.None;
           // this.Paint += new PaintEventHandler(this.programForm_Paint);

           // CreateHexagonShape();
        }
        

        public void programForm_Load(object sender, EventArgs e)
        {
            cmbFileType.Items.AddRange(new string[] { "PDF", "DOCX", "TXT" });
            dataGridView1.Columns.Add("Metric", "Metric");
            dataGridView1.Columns.Add("Count", "Count");
            dataGridView1.BackgroundColor = Color.Gray;
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Dosya türüne göre tercih numarasını belirle
                _fileExtensions = new Methods(dataGridView1); // Burada da kullanabilirsin
            if (cmbFileType.SelectedItem is null)
            {
                MessageBox.Show("Geçerli bir dosya yolu seçiniz.");
                return;
            }
            string selectedFileType = cmbFileType.SelectedItem.ToString();
                int prefer = 0;

                if (selectedFileType == "TXT")
                {
                    prefer = 1;
                }
                else if (selectedFileType == "PDF")
                {
                    prefer = 2;
                }
                else if (selectedFileType == "DOCX")
                {
                    prefer = 3;
                }

                // Dosya yolunu almak için filePrefer metodunu çağır
                string filePath = _fileExtensions.FilePrefer(prefer);

                // Dosya yolu boş değilse, yükleme işlemini başlat
                if (!string.IsNullOrEmpty(filePath))
                {
                    // Dosyayı yüklemek için asenkron fonksiyonu çağır
                    await LoadFileAsync(filePath);
                    _fileExtensions.TotalWordSay(filePath, prefer);
                    _fileExtensions.WordSay(filePath, prefer);
                }
                else
                {
                    MessageBox.Show("Geçerli bir dosya yolu bulunamadı.");
                }
        }
            catch (Exception ex)
            {
                // Hata mesajı göster
                MessageBox.Show($"Hata: {ex.Message}");
            }
}


        private async Task LoadFileAsync(string filePath)
        {
            // ProgressBar'ı sıfırla
            progressBar1.Value = 0;
            labelYuzde.Text = "0%";
            //lblPercentage.Text = "0%";
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                long fileSize = fileInfo.Length;
                long readBytes = 0;

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[4096]; // 4 KB buffer
                    int bytesRead = 0;

                    // Asenkron dosya okuma işlemi
                    while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        readBytes += bytesRead;
                        int progress = (int)((readBytes * 100) / fileSize);

                        // ProgressBar'ı ve Label'ı güncellemek için Invoke kullanarak UI thread'ine yönlendir
                        this.Invoke(new Action(() =>
                        {
                            progressBar1.Value = progress;
                            labelYuzde.Text = $"{progress}%";
                        }));

                        await Task.Delay(50); // UI'nin güncellenmesini beklemek için
                    }
                }

                // Yükleme tamamlandığında UI'yı güncelle
                this.Invoke(new Action(() =>
                {
                    labelYuzde.Text = "Tamamlandı";
                }));
            }
            catch (Exception ex)
            {
                // Hata mesajı göster
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show($"Dosya yüklenirken bir hata oluştu: {ex.Message}");
                }));
            }
        }

        private void programForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
