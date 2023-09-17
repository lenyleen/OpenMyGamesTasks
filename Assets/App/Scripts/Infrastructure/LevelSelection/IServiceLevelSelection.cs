using System;

namespace App.Scripts.Infrastructure.LevelSelection
{
    public interface IServiceLevelSelection
    {
        int CurrentLevelIndex { get; }
        public int TotalLevelsCount { get; }
        event Action OnSelectedLevelChanged;
        void UpdateSelectedLevel(int levelIndex);
    }
}