using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextClassificator.DataParser
{
    public class ParserFileInfo
    {
        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Тип файла
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Количество предложений
        /// </summary>
        public double SentenceCount { get; set; } = 0;
        /// <summary>
        /// Среднее кол-во слов в предложении
        /// </summary>
        public double SentenceWordCount => WordCount / SentenceCount;
        /// <summary>
        /// Кол-во иностранных слов
        /// </summary>
        public double ForeignWordsCount { get; set; } = 0;
        /// <summary>
        /// Среднее Кол-во иностранных слов
        /// </summary>
        public double ForeignWordsCountAverage => ForeignWordsCount / WordCount;
        /// <summary>
        /// Среднее Кол-во слов в параграфе
        /// </summary>
        public double ParagraphWordsCount { get; set; } = 0;
        /// <summary>
        /// Кол-во параграфов
        /// </summary>
        public double ParagraphCount { get; set; } = 0;
        /// <summary>
        /// Кол-во новых линий
        /// </summary>
        public double LineBreaksCount { get; set; } = 0;
        /// <summary>
        /// Отношение новых линий к кол-ву параграфов
        /// </summary>
        public double LineBreaksToParagraphCount => ParagraphCount/  LineBreaksCount;
        /// <summary>
        /// Кол-во запятых
        /// </summary>
        public double CommasCount { get; set; } = 0;
        /// <summary>
        /// Кол-во Цитат
        /// </summary>
        public double QuotesCount { get; set; } = 0;
        /// <summary>
        /// Отношение кол-ва цитат к кол-ву предложений.
        /// </summary>
        public double QuotesText => QuotesCount / SentenceCount;
        /// <summary>
        /// Кол-во прямых речей
        /// </summary>
        public double DirectSpeechCount { get; set; } = 0;
        /// <summary>
        /// Отношение кол-ва прямых речей к кол-ву предложений.
        /// </summary>
        public double DirectSpeech => DirectSpeechCount / SentenceCount;
        /// <summary>
        /// Кол-во слов
        /// </summary>
        public double WordCount { get; set; } = 0;
        public double AverageWordLength { get; set; } = 0;
        public double CorrelationFirstCriteria { get; set; } = 0;
        public double CorrelationSecondCriteria { get; set; } = 0;
        public List<CheckFile> checkData { get; set; } = new List<CheckFile>();
        public string FinalCheck { get; set; }
        public string CheckResult { get; set; }
        public bool NeedCheck { get; set; } = false;
        public List<CriteriaData> CriteriaFirst { get; set; } = new List<CriteriaData>();
        public List<CriteriaData> CriteriaSecond { get; set; } = new List<CriteriaData>();
    }

    public class CriteriaData
    {
        public int Words;
        public double SenteceWord;
        public double WordsDeviation;
        public double WordsDeviationSquare => WordsDeviation * WordsDeviation;
        public double SenteceWordDeviation;
        public double SenteceWordDeviationSquare => SenteceWordDeviation * SenteceWordDeviation;
        public double SubtractionAverage => WordsDeviation * SenteceWordDeviation;
    }
    public class CheckFile
    {
        public double ScienceClass { get; set; } = 0;
        public double FunClass { get; set; } = 0;
        public double PoemClass { get; set; } = 0;
    }
}
