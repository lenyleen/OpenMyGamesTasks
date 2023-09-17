using System;
using System.Threading.Tasks;
using App.Scripts.Infrastructure.GameCore.States.SetupState;
using App.Scripts.Infrastructure.LevelSelection;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels.View.ViewGridLetters;
using App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel;

namespace App.Scripts.Scenes.SceneFillwords.States.Setup
{
    public class HandlerSetupFillwords : IHandlerSetupLevel
    {
        private readonly ContainerGrid _containerGrid;
        private readonly IProviderFillwordLevel _providerFillwordLevel;
        private readonly IServiceLevelSelection _serviceLevelSelection;
        private readonly ViewGridLetters _viewGridLetters;
        private int _previousLoadedLvl;

        public HandlerSetupFillwords(IProviderFillwordLevel providerFillwordLevel,
            IServiceLevelSelection serviceLevelSelection,
            ViewGridLetters viewGridLetters, ContainerGrid containerGrid)
        {
            _providerFillwordLevel = providerFillwordLevel;
            _serviceLevelSelection = serviceLevelSelection;
            _viewGridLetters = viewGridLetters;
            _containerGrid = containerGrid;
        }
        public Task Process()
        {
            var model = _providerFillwordLevel.LoadModel(_serviceLevelSelection.CurrentLevelIndex);
            var startLvlIndex = _serviceLevelSelection.CurrentLevelIndex;
            while (model == null)
            {
                var delta = _previousLoadedLvl - _serviceLevelSelection.CurrentLevelIndex;
                var direction = delta == -1 || delta > 1 ? 1 : -1;
                _previousLoadedLvl = _serviceLevelSelection.CurrentLevelIndex;
                _serviceLevelSelection.UpdateSelectedLevel(_serviceLevelSelection.CurrentLevelIndex + direction);
                if (_serviceLevelSelection.CurrentLevelIndex == startLvlIndex) throw new Exception();
                model = _providerFillwordLevel.LoadModel(_serviceLevelSelection.CurrentLevelIndex);
            }
            _previousLoadedLvl = _serviceLevelSelection.CurrentLevelIndex;
            _viewGridLetters.UpdateItems(model);
            _containerGrid.SetupGrid(model, _serviceLevelSelection.CurrentLevelIndex);
            return Task.CompletedTask;
        }
    }
}