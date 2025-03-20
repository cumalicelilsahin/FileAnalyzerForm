using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using Aspose.Pdf.Text;
using Aspose.Words;

namespace FileAnalyzerForm
{
    public class Methods
    {
        private programForm _programForm;
        private loginForm _loginForm;
        private DataGridView gridView;

        public Methods(loginForm form)
        {
            _loginForm = form;
        }

        public Methods(programForm form)
        {
            _programForm = form;
        }
        public Methods(DataGridView view)
        {
            gridView = view;
        }


        public string Login(string username, string password, Form loginForm)
        {
            if (username == "CCS" && password == "123")
            {
                MessageBox.Show("Giriş başarılı, Hoşgeldiniz", "INFO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Formu kapatma işlemi
                loginForm.Invoke((MethodInvoker)delegate {
                    loginForm.Close(); // Giriş formunu kapat
                });

                return "Giriş başarılı";
            }
            else
            {
                MessageBox.Show("Hatalı k.adı veya şifre", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Hatalı k.adı veya şifre";
            }
        }


        public string FilePrefer(int prefer)
        {
            OpenFileDialog of = new OpenFileDialog();
            switch (prefer)
            {
                case 1:
                    of.Filter = "Text Files|*.txt";
                    break;
                case 2:
                    of.Filter = "Pdf Files|*.pdf";
                    break;
                case 3:
                    of.Filter = "Word Files|*.docx";
                    break;
                default:
                    MessageBox.Show("Hatalı Seçim");
                    return null;
            }

            if (of.ShowDialog() == DialogResult.OK)
            {
                return of.FileName;
            }

            MessageBox.Show("Dosya Seçilmedi");
            return null;
        }

        public int ConjunctionSay(string file, int prefer)
        {
            string[] conjunctions = { "ve", "veya", "ama", "fakat", "ancak", "çünkü", "ile", "zira" };
            int counter = 0;

            if (prefer == 1) // TXT Dosyası
            {
                foreach (var line in File.ReadAllLines(file))
                {
                    counter += conjunctions.Count(conj => line.IndexOf(conj, StringComparison.OrdinalIgnoreCase) >= 0);
                }
            }
            else if (prefer == 2) // PDF Dosyası
            {
                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(file);
                TextAbsorber textAbsorber = new TextAbsorber();

                foreach (var page in pdfDocument.Pages)
                {
                    page.Accept(textAbsorber);
                    string pageText = textAbsorber.Text;
                    counter += conjunctions.Sum(conj => CountOccurrences(pageText, conj));
                }
            }
            else if (prefer == 3) // Word Dosyası
            {
                Aspose.Words.Document wordDocument = new Aspose.Words.Document(file);
                foreach (Aspose.Words.Paragraph paragraph in wordDocument.GetChildNodes(NodeType.Paragraph, true))
                {
                    counter += conjunctions.Sum(conj => CountOccurrences(paragraph.Range.Text, conj));
                }
            }

            return counter;
        }

        public int CountOccurrences(string text, string word)
        {
            int count = 0, index = 0;
            while ((index = text.IndexOf(word, index, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                count++;
                index += word.Length;
            }
            return count;
        }

        public void TotalWordSay(string file, int prefer)
        {
            int conjunctionTotal = ConjunctionSay(file, prefer);
            int numberTotal = NumberSay(file, prefer);
            int punctuationTotal = PunctuationSay(file, prefer);
            int sentenceTotal = SentenceSay(file, prefer);
            HashSet<string> wordSet = new HashSet<string>();

            if (prefer == 1) // TXT Dosyası
            {
                foreach (var line in File.ReadAllLines(file))
                {
                    foreach (var word in line.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        wordSet.Add(word.ToLower());
                    }
                }
            }
            else if (prefer == 2) // PDF Dosyası
            {
                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(file);
                TextAbsorber textAbsorber = new TextAbsorber();

                foreach (var page in pdfDocument.Pages)
                {
                    page.Accept(textAbsorber);
                    foreach (var word in textAbsorber.Text.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        wordSet.Add(word.ToLower());
                    }
                }
            }
            else if (prefer == 3) // Word Dosyası
            {
                Aspose.Words.Document wordDocument = new Aspose.Words.Document(file);
                foreach (Aspose.Words.Paragraph paragraph in wordDocument.GetChildNodes(NodeType.Paragraph, true))
                {
                    foreach (var word in paragraph.Range.Text.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        wordSet.Add(word.ToLower());
                    }
                }
            }

            int totalWords = wordSet.Count;
            int uniqueWords = totalWords - conjunctionTotal;

            gridView.Rows.Add("Benzersiz Kelime",uniqueWords.ToString());
           gridView.Rows.Add("Cümle Sayisi", sentenceTotal.ToString());
           gridView.Rows.Add("Sayı Adeti",numberTotal.ToString());
           gridView.Rows.Add("noktalama işareti",punctuationTotal.ToString());
           gridView.Rows.Add("Bağlaç Adeti",conjunctionTotal.ToString());

        }



        public int NumberSay(string file, int prefer)
        {
            int counter = 0;
            if (prefer == 1) // TXT Dosyası
            {
                foreach (var line in File.ReadAllLines(file))
                {
                    counter += line.Split(' ').Count(word => word.All(char.IsDigit));
                }
            }
            else if (prefer == 2) // PDF Dosyası
            {
                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(file);
                TextAbsorber textAbsorber = new TextAbsorber();

                foreach (var page in pdfDocument.Pages)
                {
                    page.Accept(textAbsorber);
                    counter += textAbsorber.Text.Split(' ').Count(word => word.All(char.IsDigit));
                }
            }
            else if (prefer == 3) // Word Dosyası
            {
                Aspose.Words.Document wordDocument = new Aspose.Words.Document(file);
                foreach (Aspose.Words.Paragraph paragraph in wordDocument.GetChildNodes(NodeType.Paragraph, true))
                {
                    counter += paragraph.Range.Text.Split(' ').Count(word => word.All(char.IsDigit));
                }
            }
            return counter;
        }

        public int SentenceSay(string file, int prefer)
        {
            char[] sentenceEndings = { '.', '!', '?' };
            int counter = 0;

            if (prefer == 1) // TXT Dosyası
            {
                foreach (var line in File.ReadAllLines(file))
                {
                    counter += line.Count(c => sentenceEndings.Contains(c));
                }
            }
            else if (prefer == 2) // PDF Dosyası
            {
                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(file);
                TextAbsorber textAbsorber = new TextAbsorber();

                foreach (var page in pdfDocument.Pages)
                {
                    page.Accept(textAbsorber);
                    counter += textAbsorber.Text.Count(c => sentenceEndings.Contains(c));
                }
            }
            else if (prefer == 3) // Word Dosyası
            {
                Aspose.Words.Document wordDocument = new Aspose.Words.Document(file);
                foreach (Aspose.Words.Paragraph paragraph in wordDocument.GetChildNodes(NodeType.Paragraph, true))
                {
                    counter += paragraph.Range.Text.Count(c => sentenceEndings.Contains(c));
                }
            }
            return counter;
        }

        public int PunctuationSay(string file, int prefer)
        {
            char[] punctuationMarks = { '.', ',', ';', ':', '!', '?', '"', '(', ')', '[', ']', '{', '}', '-', '_' };
            int counter = 0;

            if (prefer == 1) // TXT Dosyası
            {
                foreach (var line in File.ReadAllLines(file))
                {
                    counter += line.Count(c => punctuationMarks.Contains(c));
                }
            }
            else if (prefer == 2) // PDF Dosyası
            {
                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(file);
                TextAbsorber textAbsorber = new TextAbsorber();

                foreach (var page in pdfDocument.Pages)
                {
                    page.Accept(textAbsorber);
                    counter += textAbsorber.Text.Count(c => punctuationMarks.Contains(c));
                }
            }
            else if (prefer == 3) // Word Dosyası
            {
                Aspose.Words.Document wordDocument = new Aspose.Words.Document(file);
                foreach (Aspose.Words.Paragraph paragraph in wordDocument.GetChildNodes(NodeType.Paragraph, true))
                {
                    counter += paragraph.Range.Text.Count(c => punctuationMarks.Contains(c));
                }
            }

            return counter;
        }

        public void WordSay(string file, int prefer)
        {
            char[] punctuation = new char[] { ' ', '.', ',', '?', '!', ':', ';', '-', '_', '(', ')', '[', ']', '{', '}', '<', '>', '/', '\\', '|', '*', '+', '=', '&', '%', '$', '#', '@', '^', '~', '`' };

            // Veriyi dosyadan okuma işlemi
            if (prefer == 1)
            {
                string text = File.ReadAllText(file);
                string clear = new string(text.Select(c => punctuation.Contains(c) && c != ' ' ? ' ' : c).ToArray());

                // Kelimeleri ayırma
                string[] word = clear.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, int> wordSay = new Dictionary<string, int>();

                // Kelimeleri sayma
                foreach (string words in word)
                {
                    if (words.All(c => Char.IsLetter(c)))
                    {
                        if (wordSay.ContainsKey(words))
                        {
                            wordSay[words]++;
                        }
                        else
                        {
                            wordSay.Add(words, 1);
                        }
                    }
                }

                // Kelimeleri sıralama (büyükten küçüğe)
                var sortedWordSay = wordSay.OrderByDescending(kv => kv.Value).ToList();

                // DataGridView'i güncelleme işlemi
                foreach (var item in sortedWordSay)
                {
                    if (item.Value > 2)
                    {
                        gridView.Rows.Add(item.Key, item.Value);
                    }
                }

                Console.WriteLine("defa kullanılmıştır.");
            }
            else if (prefer == 2) // PDF dosyası için
            {
                string text = string.Empty;
                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(file);
                TextAbsorber textAbsorber = new TextAbsorber();
                for (int i = 1; i <= pdfDocument.Pages.Count; i++)
                {
                    pdfDocument.Pages[i].Accept(textAbsorber);
                    text += textAbsorber.Text;
                }

                string clear = new string(text.Select(c => punctuation.Contains(c) && c != ' ' ? ' ' : c).ToArray());
                string[] words = clear.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<string, int> wordSay = new Dictionary<string, int>();
                foreach (string word in words)
                {
                    if (word.All(c => Char.IsLetter(c)))
                    {
                        if (wordSay.ContainsKey(word))
                        {
                            wordSay[word]++;
                        }
                        else
                        {
                            wordSay.Add(word, 1);
                        }
                    }
                }

                var sortedWordSay = wordSay.OrderByDescending(kv => kv.Value).ToList();

                // DataGridView'i güncelleyelim
                foreach (var item in sortedWordSay)
                {
                    if (item.Value > 2)
                    {
                        gridView.Rows.Add(item.Key, item.Value);
                    }
                }

                Console.WriteLine("defa kullanılmıştır.");
            }
            else if (prefer == 3) // Word dosyası için
            {
                string text = string.Empty;
                Aspose.Words.Document wordDocument = new Aspose.Words.Document(file);

                // Word dosyasındaki tüm metni paragraf paragraf okuyalım
                foreach (Aspose.Words.Paragraph paragraph in wordDocument.GetChildNodes(Aspose.Words.NodeType.Paragraph, true))
                {
                    text += paragraph.Range.Text;
                }

                string clear = new string(text.Select(c => punctuation.Contains(c) && c != ' ' ? ' ' : c).ToArray());
                string[] words = clear.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<string, int> wordSay = new Dictionary<string, int>();
                foreach (string word in words)
                {
                    if (wordSay.ContainsKey(word))
                    {
                        wordSay[word]++;
                    }
                    else
                    {
                        wordSay.Add(word, 1);
                    }
                }
                var sortedWordSay = wordSay.OrderByDescending(kv => kv.Value).ToList();

                foreach (var item in sortedWordSay)
                {
                    if (item.Value > 2)
                    {
                        gridView.Rows.Add(item.Key, item.Value);
                    }
                }
            }
        }

    }
}







//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.IO;
//using System.Windows.Forms;
//using Aspose.Pdf.Text;
//using Aspose.Words;

//namespace FileAnalyzerForm
//{
//    public Methods (programForm form)
//    {
//        private programForm _programForm;
//        _programForm = form;
//    }
//    {
//        public string login(string username, string password, Form currentForm)
//        {
//            if (username == "CCS" && password == "123")
//            {
//                MessageBox.Show("Giriş başarılı, Hoşgeldiniz", "INFO", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                currentForm.Hide();
//                programForm programForm = new programForm();
//                programForm.Show();
//                return "Giriş başarılı";
//            }
//            else
//            {
//                MessageBox.Show("Hatalı k.adı veya şifre", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                return "Hatalı k.adı veya şifre";
//            }
//        }
//        public string filePrefer(int prefer)
//        {
//            OpenFileDialog of = new OpenFileDialog();

//            switch (prefer)
//            {
//                case 1:
//                    of.Filter = "Text Files|*.txt";
//                    break;
//                case 2:
//                    of.Filter = "Pdf Files|*.pdf";
//                    break;
//                case 3:
//                    of.Filter = "Word Files|*.docx";
//                    break;
//                default:
//                    MessageBox.Show("Hatalı Seçim");
//                    return null;
//            }
//            if (of.ShowDialog() == DialogResult.OK)
//            {
//                return of.FileName;
//            }


//            MessageBox.Show("Dosya Seçilmedi");
//            return null;




//        }
//        public int conjunctionSay(string file, int prefer)
//        {
//            string[] conjunctions = { "ve", "veya", "ama", "fakat", "ancak", "çünkü", "ile", "zira" };
//            if (prefer == 1)
//            {

//                int counter = 0;
//                foreach (var item in File.ReadAllLines(file))
//                {
//                    foreach (var conjunction in conjunctions)
//                    {
//                        if (item.Contains(conjunction))
//                        {
//                            counter++;
//                        }
//                    }
//                }
//                return counter;
//            }

//            else if (prefer == 2)
//            {
//                int counter = 0;
//                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(file);
//                TextAbsorber textAbsorber = new TextAbsorber();
//                for (int i = 1; i <= pdfDocument.Pages.Count; i++)
//                {
//                    pdfDocument.Pages[i].Accept(textAbsorber);
//                    string pageText = textAbsorber.Text;
//                    foreach (var conjunction in conjunctions)
//                    {
//                        int index = 0;
//                        while ((index = pageText.IndexOf(conjunction, index, StringComparison.OrdinalIgnoreCase)) != -1)
//                        {
//                            counter++;
//                            index += conjunction.Length;  // Sonraki arama için indeksi güncelliyoruz
//                        }

//                    }
//                }
//                return counter;

//            }
//            else if (prefer == 3)
//            {
//                int counter = 0;
//                Aspose.Words.Document wordDocument = new Aspose.Words.Document(file); // Aspose.Words.Document kullanıyoruz
//                foreach (Aspose.Words.Paragraph paragraph in wordDocument.GetChildNodes(NodeType.Paragraph, true))
//                {
//                    string paragraphText = paragraph.Range.Text; // Paragrafları metne dönüştürüyoruz
//                    foreach (var conjunction in conjunctions)
//                    {
//                        int index = 0;
//                        while ((index = paragraphText.IndexOf(conjunction, index, StringComparison.OrdinalIgnoreCase)) != -1)
//                        {
//                            counter++;
//                            index += conjunction.Length;  // Sonraki arama için indeksi güncelliyoruz
//                        }
//                    }
//                }
//                return counter;
//            }
//            return 0;
//        }

//        public int numberSay(string file, int prefer)
//        {
//            string[] number = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
//            if (prefer == 1)
//            {
//                int counter = 0;
//                foreach (var item in File.ReadAllLines(file))
//                {
//                    string[] words = item.Split(' ');
//                    foreach (string s in words)
//                    {
//                        if (s.All(char.IsDigit))
//                        {
//                            counter++;
//                        }
//                    }
//                }
//                return counter;
//            }
//            else if (prefer == 2)
//            {
//                int counter = 0;
//                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(file);
//                TextAbsorber textAbsorber = new TextAbsorber();
//                for (int i = 1; i <= pdfDocument.Pages.Count; i++)
//                {
//                    pdfDocument.Pages[i].Accept(textAbsorber);
//                    string pageText = textAbsorber.Text;
//                    string[] words = pageText.Split(' ');
//                    foreach (var s in number)
//                    {
//                        if (s.All(char.IsDigit))
//                        {
//                            counter++;
//                        }
//                    }
//                }
//                return counter;
//            }
//            else if (prefer == 3) // Word dosyası için
//            {
//                int counter = 0;
//                Aspose.Words.Document wordDocument = new Aspose.Words.Document(file);

//                // Word dosyasındaki tüm metni paragraf paragraf okuyalım
//                foreach (Aspose.Words.Paragraph paragraph in wordDocument.GetChildNodes(Aspose.Words.NodeType.Paragraph, true))
//                {
//                    string paragraphText = paragraph.Range.Text;

//                    // Kelimeleri ayırırken boşluk ve diğer özel karakterleri dikkate alalım
//                    string[] words = paragraphText.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
//                    foreach (string s in words)
//                    {
//                        if (s.All(char.IsDigit))
//                        {
//                            counter++;
//                        }
//                    }
//                }

//                return counter;
//            }
//            return 0;
//        }

//        public void totalWordSay(string file, int prefer)
//        {
//            int consuctionTotal = conjunctionSay(file, prefer);
//            int numberTotal = numberSay(file, prefer);
//            int punctuationTotal = punctuationSay(file, prefer);
//            int sentenceTotal = sentenceSay(file, prefer);
//            int tWS = 0;
//            HashSet<string> wordSet = new HashSet<string>();

//            if (prefer == 1)
//            {
//                int totalWordSay = 0;
//                foreach (var item in File.ReadAllLines(file))
//                {
//                    string[] words = item.Split(new char[] { ' ', '.', ',', '?', '!', ':', ';', '-', '_', '(', ')', '[', ']', '{', '}', '<', '>', '/', '\\', '|', '*', '+', '=', '&', '%', '$', '#', '@', '^', '~', '`' }, StringSplitOptions.RemoveEmptyEntries);
//                    foreach (var word in words)
//                    {
//                        wordSet.Add(word.ToLower());
//                    }
//                }
//                totalWordSay = wordSet.Count;
//                tWS = totalWordSay;

//            }
//            else if (prefer == 2) // PDF dosyası için
//            {
//                int totalWordSay = 0;
//                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(file);
//                TextAbsorber textAbsorber = new TextAbsorber();

//                for (int i = 1; i <= pdfDocument.Pages.Count; i++)
//                {
//                    pdfDocument.Pages[i].Accept(textAbsorber);
//                    string pageText = textAbsorber.Text;
//                    string[] words = pageText.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
//                    foreach (var word in words)
//                    {
//                        wordSet.Add(word.ToLower());
//                    }
//                }
//                totalWordSay = wordSet.Count;
//                tWS = totalWordSay;
//            }
//            else if (prefer == 3) // Word dosyası için
//            {
//                int totalWordSay = 0;
//                Aspose.Words.Document wordDocument = new Aspose.Words.Document(file);

//                // Word dosyasındaki tüm metni paragraf paragraf okuyalım
//                foreach (Aspose.Words.Paragraph paragraph in wordDocument.GetChildNodes(Aspose.Words.NodeType.Paragraph, true))
//                {
//                    string paragraphText = paragraph.Range.Text;

//                    // Kelimeleri ayırırken boşluk ve diğer özel karakterleri dikkate alalım
//                    string[] words = paragraphText.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
//                    foreach (var word in words)
//                    {
//                        wordSet.Add(word.ToLower());
//                    }
//                }
//                totalWordSay = wordSet.Count;
//                tWS = totalWordSay;
//            }

//            // DataGridView'e sütun ekleme
//            form.dataGridView1.Columns.Add("Column1", "Veri Başlığı");

//            // Daha sonra satır ekleyebilirsiniz


//            form.dataGridView1.Rows.Add("Veri");
//            form.dataGridView1.Rows.Clear(); // Önceki verileri temizle
//            form.dataGridView1.Rows.Add("Unique Words", (tWS - consuctionTotal - numberTotal).ToString());

//            form.dataGridView1.Rows.Add("Sentences", sentenceTotal.ToString());
//            form.dataGridView1.Rows.Add("Numbers", numberTotal.ToString());
//            form.dataGridView1.Rows.Add("Punctuation Marks", punctuationTotal.ToString());
//            form.dataGridView1.Rows.Add("Conjunctions", consuctionTotal.ToString());

//        }

//        public int sentenceSay(string file, int prefer)
//        {

//            if (prefer == 1)
//            {
//                int counter = 0;
//                foreach (var item in File.ReadAllLines(file))
//                {
//                    string[] sentences = item.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
//                    counter += sentences.Length;
//                }
//                return counter;
//            }
//            else if (prefer == 2) // PDF dosyası için
//            {
//                int counter = 0;
//                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(file);
//                TextAbsorber textAbsorber = new TextAbsorber();
//                for (int i = 1; i <= pdfDocument.Pages.Count; i++)
//                {
//                    pdfDocument.Pages[i].Accept(textAbsorber);
//                    string pageText = textAbsorber.Text;
//                    // Cümleleri ayırmak için nokta, ünlem, soru işareti kullanıyoruz
//                    string[] sentences = pageText.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
//                    counter += sentences.Length;
//                }
//                return counter;
//            }
//            else if (prefer == 3) // Word dosyası için
//            {
//                int counter = 0;
//                Aspose.Words.Document wordDocument = new Aspose.Words.Document(file);

//                // Word dosyasındaki tüm metni paragraf paragraf okuyalım
//                foreach (Aspose.Words.Paragraph paragraph in wordDocument.GetChildNodes(Aspose.Words.NodeType.Paragraph, true))
//                {
//                    string paragraphText = paragraph.Range.Text;
//                    // Cümleleri ayırmak için nokta, ünlem, soru işareti kullanıyoruz
//                    string[] sentences = paragraphText.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
//                    counter += sentences.Length;
//                }

//                return counter;
//            }
//            else
//            {
//                return 0;
//            }
//        }
//        public int punctuationSay(string file, int prefer)
//        {
//            char[] punctuations = new char[] { '.', ',', '?', '!', ':', ';', '-', '_', '(', ')', '[', ']', '{', '}', '<', '>', '/', '\\', '|', '*', '+', '=', '&', '%', '$', '#', '@', '^', '~', '`' };
//            if (prefer == 1)
//            {
//                int counter = 0;
//                foreach (var item in File.ReadAllLines(file))
//                {
//                    foreach (var c in item)
//                    {
//                        if (punctuations.Contains(c))
//                        {
//                            counter++;
//                        }
//                    }
//                }
//                return counter;
//            }
//            else if (prefer == 2) // PDF dosyası için
//            {
//                int counter = 0;
//                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(file);
//                TextAbsorber textAbsorber = new TextAbsorber();
//                for (int i = 1; i <= pdfDocument.Pages.Count; i++)
//                {
//                    pdfDocument.Pages[i].Accept(textAbsorber);
//                    string pageText = textAbsorber.Text;
//                    foreach (var c in pageText)
//                    {
//                        if (punctuations.Contains(c))
//                        {
//                            counter++;
//                        }
//                    }
//                }
//                return counter;
//            }
//            else if (prefer == 3) // Word dosyası için
//            {
//                int counter = 0;
//                Aspose.Words.Document wordDocument = new Aspose.Words.Document(file);
//                // Word dosyasındaki tüm metni paragraf paragraf okuyalım
//                foreach (Aspose.Words.Paragraph paragraph in wordDocument.GetChildNodes(Aspose.Words.NodeType.Paragraph, true))
//                {
//                    string paragraphText = paragraph.Range.Text;
//                    foreach (var c in paragraphText)
//                    {
//                        if (punctuations.Contains(c))
//                        {
//                            counter++;
//                        }
//                    }
//                }
//                return counter;
//            }
//            else
//            {
//                return 0;
//            }
//        }

//        public void wordSay(string file, int prefer)
//        {
//            char[] punctuation = new char[] { ' ', '.', ',', '?', '!', ':', ';', '-', '_', '(', ')', '[', ']', '{', '}', '<', '>', '/', '\\', '|', '*', '+', '=', '&', '%', '$', '#', '@', '^', '~', '`' };

//            if (prefer == 1)
//            {
//                string text = File.ReadAllText(file);
//                string clear = new string(text.Select(c => punctuation.Contains(c) && c != ' ' ? ' ' : c).ToArray());

//                string[] word = clear.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
//                Dictionary<string, int> wordSay = new Dictionary<string, int>();
//                foreach (string words in word)
//                {
//                    if (words.All(c => Char.IsLetter(c)))
//                    {
//                        if (wordSay.ContainsKey(words))
//                        {
//                            wordSay[words]++;
//                        }
//                        else
//                        {
//                            wordSay.Add(words, 1);
//                        }
//                    }
//                }
//                var sortedWordSay = wordSay.OrderByDescending(kv => kv.Value).ToList();
//                foreach (var item in sortedWordSay)
//                {
//                    if (item.Value > 1)
//                    {
//                        Console.Write(" " + item.Key + "=" + item.Value + ", ");
//                    }
//                }
//                Console.WriteLine("defa kullanılmıştır.");
//            }
//            else if (prefer == 2) // PDF dosyası için
//            {
//                string text = string.Empty;
//                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(file);
//                TextAbsorber textAbsorber = new TextAbsorber();
//                for (int i = 1; i <= pdfDocument.Pages.Count; i++)
//                {
//                    pdfDocument.Pages[i].Accept(textAbsorber);
//                    text += textAbsorber.Text;
//                }

//                string clear = new string(text.Select(c => punctuation.Contains(c) && c != ' ' ? ' ' : c).ToArray());
//                string[] words = clear.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

//                Dictionary<string, int> wordSay = new Dictionary<string, int>();
//                foreach (string word in words)
//                {
//                    if (word.All(c => Char.IsLetter(c)))
//                    {
//                        if (wordSay.ContainsKey(word))
//                        {
//                            wordSay[word]++;
//                        }
//                        else
//                        {
//                            wordSay.Add(word, 1);
//                        }
//                    }
//                }
//                var sortedWordSay = wordSay.OrderByDescending(kv => kv.Value).ToList();
//                foreach (var item in sortedWordSay)
//                {
//                    if (item.Value > 1)
//                    {
//                        Console.Write(" " + item.Key + "=" + item.Value + ", ");
//                    }
//                }
//                Console.WriteLine("defa kullanılmıştır.");
//            }
//            else if (prefer == 3) // Word dosyası için
//            {
//                string text = string.Empty;
//                Aspose.Words.Document wordDocument = new Aspose.Words.Document(file);

//                // Word dosyasındaki tüm metni paragraf paragraf okuyalım
//                foreach (Aspose.Words.Paragraph paragraph in wordDocument.GetChildNodes(Aspose.Words.NodeType.Paragraph, true))
//                {
//                    text += paragraph.Range.Text;
//                }

//                string clear = new string(text.Select(c => punctuation.Contains(c) && c != ' ' ? ' ' : c).ToArray());
//                string[] words = clear.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

//                Dictionary<string, int> wordSay = new Dictionary<string, int>();
//                foreach (string word in words)
//                {
//                    if (wordSay.ContainsKey(word))
//                    {
//                        wordSay[word]++;
//                    }
//                    else
//                    {
//                        wordSay.Add(word, 1);
//                    }
//                }
//                var sortedWordSay = wordSay.OrderByDescending(kv => kv.Value).ToList();
//                foreach (var item in sortedWordSay)
//                {
//                    if (item.Value > 1)
//                    {
//                        Console.Write(" " + item.Key + "=" + item.Value + ", ");
//                    }
//                }
//                Console.WriteLine("defa kullanılmıştır.");
//            }
//        }
//}
//}
