using System;
using System.Collections.Generic;
using System.IO;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using UnityEngine;


namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        private readonly TextAsset _lvls;
        private readonly TextAsset _dictionary;
        private const char IndexesSeparator = ';';
        private int _previousLvlIndex;
        public ProviderFillwordLevel()
        {
            _lvls = Resources.Load<TextAsset>("Fillwords/pack_0");
            _dictionary = Resources.Load<TextAsset>("Fillwords/words_list");
        }
        public GridFillWords LoadModel(int index)
        {
            var lvlData = LoadData(index);
            return lvlData;
        }
        private GridFillWords LoadData(int index)
        {
            using StreamReader lvlDataReader = new StreamReader(new MemoryStream(_lvls.bytes));
            using StreamReader dictionaryReader = new StreamReader(new MemoryStream(_dictionary.bytes));
            var line = ReadToLine(lvlDataReader, index - 1);
            if (line.Length == 0) return null;
            var prevWordIndex = 0;
            var totalLetterIndexesCount = 0;
            var sumOfIndexes = 0;
            var loadedData = new Dictionary<string, List<int>>();
            var i = 0;
            while (i < line.Length)
            {
                var wordIndex = GetWordLineIndex(line,ref i);
                if (wordIndex < 0) return null;
                
                var lettersIndexes = GetLettersIndexes(line, ref i);
                totalLetterIndexesCount += lettersIndexes.Count;
                sumOfIndexes += GetSumOfIndexes(lettersIndexes);
                
                var word = ReadToLine(dictionaryReader, wordIndex - prevWordIndex);
                prevWordIndex = wordIndex + 1;
                if (word.Length != lettersIndexes.Count) return null;
                
                loadedData.Add(word,lettersIndexes);
                i++;
            }
            return CreateFillWordsData(loadedData,totalLetterIndexesCount,sumOfIndexes);
        }
        
        private string ReadToLine(StreamReader reader, int lineIndex)
        {
            for (int i = 0; i < lineIndex; i++)
            {
                reader.ReadLine();
            }
            return reader.ReadLine();
        }
        
        private int GetWordLineIndex(string line,ref int index)
        {
            var lineOfWord = 0;
            while (index < line.Length && !char.IsWhiteSpace(line[index]))
            {
                if (!char.IsDigit(line[index])) return -1;
                lineOfWord *= 10;
                lineOfWord += (int)char.GetNumericValue(line[index]);
                index++;
            }
            index++;
            return lineOfWord;
        }
        
        private List<int> GetLettersIndexes(string line, ref int index)
        {
            var letterIndex = 0;
            var lettersIndexes = new List<int>();
            while (index < line.Length && !char.IsWhiteSpace(line[index]))
            {
                if (line[index] == IndexesSeparator)
                {
                    lettersIndexes.Add(letterIndex);
                    letterIndex = 0;
                    index++;
                    continue;
                }
                if(!char.IsDigit(line[index])) return null;
                letterIndex *= 10;
                letterIndex += (int)char.GetNumericValue(line[index]);
                index++;
            }
            index++;
            lettersIndexes.Add(letterIndex);
            return lettersIndexes;
        }
        
        private int GetSumOfIndexes(List<int> indexes)
        {
            var sumOfIndexes = 0;
            foreach (int index in indexes)
                sumOfIndexes += index;
            return sumOfIndexes;
        }
        
        private GridFillWords CreateFillWordsData(Dictionary<string, List<int>> lvlData, int letterIndexesCount,int sumOfIndexes)
        {
            var grid = SetSquareGrid(letterIndexesCount, sumOfIndexes);
            if (grid == null) return null;
            foreach (var data in lvlData)
            { 
                for (int i = 0; i < data.Value.Count; i++)
                {
                    var curY = data.Value[i] / grid.Size.x;
                    var curX = data.Value[i] - (curY * grid.Size.x);
                    grid.Set(curY,curX,new CharGridModel(data.Key[i]));
                }
            }
            return grid;
        }
        
        private GridFillWords SetSquareGrid(int lettersCount, int sumOfIndexes)
        {
            if (!CanFormAGrid(lettersCount, sumOfIndexes)) return null;
            var sideSize = Math.Sqrt(lettersCount);
            return sideSize % 1 != 0 ? null : new GridFillWords(new Vector2Int((int)sideSize, (int)sideSize));
        }
        
        private bool CanFormAGrid(int lettersCount, int sumOfIndexes) 
        {
            // Arithmetic progression helps check for duplicate indexes and indexes larger than possible
            var progressionOfIdxsSum = ((float)lettersCount / 2) * (lettersCount-1);
            return Math.Abs(progressionOfIdxsSum - sumOfIndexes) < 1;
        }
    }
}