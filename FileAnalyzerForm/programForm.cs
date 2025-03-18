using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

//using System.Linq;
//using System.Net.PeerToPeer.Collaboration;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileAnalyzerForm
{
    public partial class programForm : Form
    {
        private programForm pF;
        private Methods m;

        public programForm()
        {
            InitializeComponent();
            this.Text = "Altıgen Şekli Form";
            this.Size = new Size(800, 800);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Paint += new PaintEventHandler(this.programForm_Paint);

            CreateHexagonShape();
        }
        private void programForm_Paint(object sender, PaintEventArgs e)
        {
            // Kenarlık için renk ve kalınlık belirleyin
            Pen borderPen = new Pen(Color.Blue, 5); // Mavi renkte 5 piksel kalınlığında kenarlık

            // Kenarlık çizimlerini yapıyoruz
            int kenarlikUzunlugu = 30; // Kenarlığın uzunluğunu kısaltmak için burada 30px belirledik

            // Sol kenar (kısa)
            e.Graphics.DrawLine(borderPen, 0, 0, 0, kenarlikUzunlugu); // Sol üstten aşağıya doğru çiz
                                                                       // Sağ kenar (kısa)
            e.Graphics.DrawLine(borderPen, this.Width - 1, 0, this.Width - 1, kenarlikUzunlugu); // Sağ üstten aşağıya doğru çiz
                                                                                                 // Alt kenar (kısa)
            e.Graphics.DrawLine(borderPen, 0, this.Height - 1, this.Width - 1, this.Height - 1); // Alt kenar boyunca çiz

            // Üst kenar (tam uzunlukta)
            e.Graphics.DrawLine(borderPen, 0, 0, this.Width - 1, 0); // Üst kenar boyunca çiz

            // Diğer kenarları çizmek için
            // Sol kenar
            e.Graphics.DrawLine(borderPen, 0, kenarlikUzunlugu, 0, this.Height - 1);
            // Sağ kenar
            e.Graphics.DrawLine(borderPen, this.Width - 1, kenarlikUzunlugu, this.Width - 1, this.Height - 1);
        }
        private void CreateHexagonShape()
        {
            // GraphicsPath oluşturun
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            // Altıgeni tanımlayın (örnek bir altıgen şekli)
            path.AddPolygon(new Point[]
            {
            new Point(295, 46),   // Üst sol
            new Point(335, 135),  // Üst orta
            new Point(750, 135),  // Üst sağ
            new Point(750, 720),  // sağ
            new Point(41, 720),  // Sol alt
            new Point(41, 46)   // Sol üst
            });

            // Form'un şekli olarak GraphicsPath'i ayarlayın
            this.Region = new Region(path);
        }

        private void cmbFileType_SelectedIndexChanged(object sender, EventArgs e)
        {


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
                // Dosya türüne göre tercih numarasını belirle
                m = new Methods(dataGridView1); // Burada da kullanabilirsin
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
                string filePath = m.filePrefer(prefer);

                // Dosya yolu boş değilse, yükleme işlemini başlat
                if (!string.IsNullOrEmpty(filePath))
                {
                    // Dosyayı yüklemek için asenkron fonksiyonu çağır
                    await LoadFileAsync(filePath);
                    m.totalWordSay(filePath, prefer);
                    m.wordSay(filePath, prefer);
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
    }
}
