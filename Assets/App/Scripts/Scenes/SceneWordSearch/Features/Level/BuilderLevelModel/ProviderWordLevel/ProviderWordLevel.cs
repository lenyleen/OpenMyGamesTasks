using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using UnityEngine;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        private string _dataPath;
        public ProviderWordLevel()
        {
            _dataPath ="WordSearch/Levels/";
        }
        public LevelInfo LoadLevelData(int levelIndex)
        {
            var file = Resources.Load<TextAsset>(_dataPath + $"{levelIndex}");
            if (file is null) return null;
            return JsonUtility.FromJson<LevelInfo>(file.text); 
        }
    }
}  