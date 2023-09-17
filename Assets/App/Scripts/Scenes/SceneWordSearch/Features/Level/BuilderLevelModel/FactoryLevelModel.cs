using System.Collections.Generic;
using System.Linq;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{
    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();
            model.LevelNumber = levelNumber;
            model.Words = value.words;
            model.InputChars = BuildListChars(value.words);
            return model;
        }

        private List<char> BuildListChars(List<string> words)
        {
            var checkingCounter = new Dictionary<char, int>();
            var inputChars = new List<char>();
            foreach (string word in words)
            {
                SetLettersDictionary(checkingCounter, word, inputChars);
            }
            return inputChars;
        }
        private void SetLettersDictionary(Dictionary<char,int> checkingDictionary,string word, List<char> inputChars) 
        {
            var wordCounter = new Dictionary<char, int>();
            for (int i = 0; i < word.Length; i++)
            {
                if (!wordCounter.TryAdd(word[i],1))
                    wordCounter[word[i]]++;
                if (!checkingDictionary.ContainsKey(word[i]))
                {
                    checkingDictionary.Add(word[i],1);
                    inputChars.Add(word[i]);
                    continue;
                }
                if(checkingDictionary[word[i]] >= wordCounter[word[i]]) continue;
                checkingDictionary[word[i]]++;
                inputChars.Add(word[i]);
            }
        }
    }
}