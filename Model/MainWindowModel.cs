using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TextClassificator.DataParser;

namespace TextClassificator.Model
{
    class MainWindowModel
    {
        private ObservableCollection<ParserFileInfo> _info;
        public ObservableCollection<ParserFileInfo> Infos
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
            }
        }
        public MainWindowModel()
        {
            Infos = new ObservableCollection<ParserFileInfo>();
        }

        public void LoadFile(string Type, bool directory = false)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выберите имя";
            string path = string.Empty;


            if (Type != "Проверка")
            {
                foreach (var item in _info)
                {
                    if (Type == item.Type)
                    {
                        _info.Remove(item);
                        break;
                    }
                }
            }
            else
            {
                int count = 0;
                foreach (var item in _info)
                {
                    switch (item.Type)
                    {
                        case "Эталон Научной":
                            count++;
                            break;
                        case "Эталон художественного":
                            count++;
                            break;
                        case "Эталон стихотворения":
                            count++;
                            break;
                    }
                }
                if (count != 3)
                {
                    MessageBox.Show("Загрузите все 3 эталона");
                    return;
                }
            }

            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;
            }

            string[] files = { path };
            if (directory)
                files = Directory.GetFiles(Path.GetDirectoryName(path));

            if (string.IsNullOrEmpty(path))
                return;

            foreach (var file in files)
            {
                var fileInfo = new ParserFileInfo();
                fileInfo.Type = Type;
                fileInfo.FileName = file.Split('\\').Last();
                if(Type == "Проверка")
                    fileInfo.NeedCheck = true;

                LoadFile(file, fileInfo);

                fileInfo.CorrelationFirstCriteria = Math.Round(FirstCriteriaWork(fileInfo.CriteriaFirst), 4);
                if (double.IsNaN(fileInfo.CorrelationFirstCriteria))
                {
                    fileInfo.CorrelationFirstCriteria = 0;
                }
                fileInfo.CorrelationSecondCriteria = Math.Round(FirstCriteriaWork(fileInfo.CriteriaSecond), 4);
                if (double.IsNaN(fileInfo.CorrelationSecondCriteria))
                {
                    fileInfo.CorrelationSecondCriteria = 0;
                }

                Infos.Add(fileInfo);
            }
        }

        private static void LoadFile(string path, ParserFileInfo fileInfo)
        {
            var text = File.ReadAllText(path, Encoding.UTF8);
            for (int i = 0; i < 10; i++)
            {
                text = text.Replace("\r\n\r\n\r\n", "\r\n\r\n");
            }

            var pattern = "\r\n\r\n";
            var paragraphSplit = System.Text.RegularExpressions.Regex.Split(text, pattern);
            var paragraphWords = 0;

            fileInfo.WordCount = text.Split(' ').Where(q => q.Length > 2).ToArray().Length;
            fileInfo.AverageWordLength = fileInfo.WordCount / text.Length;

            Dictionary<string, int> wordsLanguages = new Dictionary<string, int>();
            foreach (var word in text.Split(' ').Where(q => q.Length > 2).ToArray())
            {
                var language = LanguagetText(word);
                if (wordsLanguages.ContainsKey(language))
                {
                    wordsLanguages[language]++;
                }
                else
                {
                    wordsLanguages.Add(language, 1);
                }
            }
            var mainLanguage = wordsLanguages.First(q => q.Value == wordsLanguages.Max(l => l.Value));
            fileInfo.ForeignWordsCount = wordsLanguages.Where(q => q.Key != mainLanguage.Key).Sum(q => q.Value);

            foreach (var paragparh in paragraphSplit)
            {
                var line = paragparh.Split(' ');
                paragraphWords = line.Where(q=>q.Length>2).ToArray().Length;

                var sentece = paragparh.Split(new char[] { '.', '!', '?' });
                var sentenceWords = paragraphWords / (double)sentece.Length;

                fileInfo.CriteriaFirst.Add(new CriteriaData()
                {
                    Words = paragraphWords,
                    SenteceWord = sentenceWords,
                });

                var newLineCount = System.Text.RegularExpressions.Regex.Split(paragparh, "\r\n");
                foreach (var newLine in newLineCount)
                {
                    var words = newLine.Split(' ');
                    var LineSenteces = newLine.Split(new char[] { '.', '!', '?' });
                    fileInfo.CriteriaSecond.Add(new CriteriaData()
                    {
                        Words = words.Where(q => q.Length > 2).ToArray().Length,
                        SenteceWord = LineSenteces.Length,
                    });
                }
            }

            fileInfo.ParagraphWordsCount = fileInfo.WordCount / paragraphSplit.Length;
            for (int i = 0; i < text.Length; i++)
            {
                var letter = text[i];
                if (letter == '.' || letter == '!' || letter == '?')
                {
                    fileInfo.SentenceCount++;
                }
                if (letter == '\n')
                {
                    if (i + 2 < text.Length && text[i + 2] == '\n')
                    {
                        fileInfo.ParagraphCount++;
                        while (i + 2 < text.Length && text[i + 2] == '\n')
                            i += 2;
                    }
                    else
                        fileInfo.LineBreaksCount++;
                }
                if (letter == ',')
                {
                    fileInfo.CommasCount++;
                }
                if (letter == '–' && text[i - 2] == '\n')
                {
                    fileInfo.DirectSpeechCount++;
                }
                if (letter == '\"' || letter == '«')
                {
                    fileInfo.QuotesCount++;
                }
            }
        }
        public void Clean()
        {
            var files = new List<ParserFileInfo>();

            foreach (var file in _info)
            {
                files.Add(file);
            }
            files = files.Where(q => q.Type != "Проверка").ToList();
            var cleanedInfo = new ObservableCollection<ParserFileInfo>();

            foreach (var file in files)
            {
                cleanedInfo.Add(file);
            }
            _info = cleanedInfo;
        }
        public void CheckFile()
        {
            var checkFiles = new List<ParserFileInfo>();

            foreach (var file in _info)
            {
                checkFiles.Add(file);
            }

            var scienceFile = checkFiles.Find(q => q.Type == "Эталон Научной");
            var funFile = checkFiles.Find(q => q.Type == "Эталон художественного");
            var poemFile = checkFiles.Find(q => q.Type == "Эталон стихотворения");
            var needCheckFiles = checkFiles.Where(q => q.NeedCheck);

            foreach (var file in needCheckFiles)
            {
                CheckOnCriteria(scienceFile.QuotesText, funFile.QuotesText, poemFile.QuotesText, file.QuotesText, file);
                CheckOnCriteria(scienceFile.DirectSpeech, funFile.DirectSpeech, poemFile.DirectSpeech, file.DirectSpeech, file);
                CheckOnCriteria(scienceFile.CommasCount, funFile.CommasCount, poemFile.CommasCount, file.CommasCount, file);
                CheckOnCriteria(scienceFile.LineBreaksToParagraphCount, funFile.LineBreaksToParagraphCount, poemFile.LineBreaksToParagraphCount, file.LineBreaksToParagraphCount, file);
                CheckOnCriteria(scienceFile.ForeignWordsCountAverage, funFile.ForeignWordsCountAverage, poemFile.ForeignWordsCountAverage, file.ForeignWordsCountAverage, file);
                CheckOnCriteria(scienceFile.SentenceWordCount, funFile.SentenceWordCount, poemFile.SentenceWordCount, file.SentenceWordCount, file);
                CheckOnCriteria(scienceFile.AverageWordLength, funFile.AverageWordLength, poemFile.AverageWordLength, file.AverageWordLength, file);
                CheckOnCriteria(scienceFile.CorrelationFirstCriteria, funFile.CorrelationFirstCriteria, poemFile.CorrelationFirstCriteria, file.CorrelationFirstCriteria, file);
                CheckOnCriteria(scienceFile.CorrelationSecondCriteria, funFile.CorrelationSecondCriteria, poemFile.CorrelationSecondCriteria, file.CorrelationSecondCriteria, file);

                var finalCheck = new CheckFile();
                finalCheck.ScienceClass = Math.Round(file.checkData.Sum(q => q.ScienceClass) / file.checkData.Count, 2);
                finalCheck.FunClass = Math.Round(file.checkData.Sum(q => q.FunClass) / file.checkData.Count, 2);
                finalCheck.PoemClass = Math.Round(file.checkData.Sum(q => q.PoemClass) / file.checkData.Count, 2);
                file.FinalCheck = "Науч-" + finalCheck.ScienceClass + "%; Худ-" + finalCheck.FunClass + "%; Стих-" + finalCheck.PoemClass + "%";

                if (finalCheck.PoemClass > finalCheck.FunClass && finalCheck.PoemClass > finalCheck.ScienceClass)
                {
                    file.CheckResult = "Стихотворный класс";
                }
                if (finalCheck.FunClass > finalCheck.PoemClass && finalCheck.FunClass > finalCheck.ScienceClass)
                {
                    file.CheckResult = "Художественный класс";
                }
                if (finalCheck.ScienceClass > finalCheck.FunClass && finalCheck.ScienceClass > finalCheck.PoemClass)
                {
                    file.CheckResult = "Научный класс";
                }
                file.NeedCheck = false;
            }

            _info.GroupBy(q => q.CheckResult);
        }

        private static void CheckOnCriteria(double scienceFile, double funFile, double poemFile, double file, ParserFileInfo fileClass)
        {
            var sumCriteria = Math.Abs(scienceFile - file) +
                                              Math.Abs(funFile - file) +
                                              Math.Abs(poemFile - file);
            CheckFile data = new CheckFile();
            if (sumCriteria == 0)
                sumCriteria = 1;

            data.ScienceClass = (sumCriteria - Math.Abs(scienceFile - file)) / sumCriteria * 100;
            data.FunClass = (sumCriteria - Math.Abs(funFile - file)) / sumCriteria * 100;
            data.PoemClass = (sumCriteria - Math.Abs(poemFile - file)) / sumCriteria * 100;
            fileClass.checkData.Add(data);
        }

        private static double FirstCriteriaWork(List<CriteriaData> fileInfo)
        {
            var sumSentenceWords = fileInfo.Sum(q => q.SenteceWord);
            var sumWords = fileInfo.Sum(q => q.Words);
            var averageSumSentenceWords = sumSentenceWords / fileInfo.Count;
            var averageSumWords = sumWords / (double)fileInfo.Count;

            foreach (var criteriaData in fileInfo)
            {
                criteriaData.WordsDeviation = averageSumWords - criteriaData.Words;
                criteriaData.SenteceWordDeviation = averageSumSentenceWords - criteriaData.SenteceWord;
            }

            var sumSquares = fileInfo.Sum(q => q.WordsDeviationSquare);
            var sumSquaresS = fileInfo.Sum(q => q.SenteceWordDeviationSquare);
            var sumSubstraction = fileInfo.Sum(q => q.SubtractionAverage);

            return sumSubstraction / Math.Sqrt(sumSquaresS * sumSquares);
        }

        static public string LanguagetText(string text)
        {
            bool rus = false, eng = false;

            text = text.ToLower();

            byte[] Ch = System.Text.Encoding.Default.GetBytes(text);
            foreach (byte ch in Ch)
            {
                if ((ch >= 97) && (ch <= 122)) eng = true;
                if ((ch >= 224) && (ch <= 255)) rus = true;
            }

            if (eng & !rus) return "eng";
            if (rus & !eng) return "rus";
            if (eng & rus) return "mix"; //смашанный состав
            return "uni"; // универсальный состав (например если будет только 12345)
        }
    }
}
