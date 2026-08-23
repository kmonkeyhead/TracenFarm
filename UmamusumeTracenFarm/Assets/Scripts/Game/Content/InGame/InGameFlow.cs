using System;
using Game.Character;
using Game.Content.InGame.Payload;
using Game.Content.InGame.UseCase;
using Game.Service.Gesture;
using R3;
using VContainer.Unity;

namespace Game.Content.InGame
{
    public class InGameFlow : IStartable, IDisposable
    {
        private readonly InteractionUseCase _interactionUseCase;
        private readonly FarmStore _farmStore;
        private readonly InGameMap _inGameMap;
        private readonly PropsProgressService _progressService;
        private readonly IDisposable _disposable;

        public InGameFlow(ClickGesture clickGesture, InteractionUseCase interactionUseCase, ActorStore actorStore, FarmStore farmStore, Actor userActor, InGameMap inGameMap, PropsProgressService progressService)
        {
            //userActor는 현재 임시다 - actor 생성기가 있어야 한다
            _interactionUseCase = interactionUseCase;
            _farmStore = farmStore;
            _inGameMap = inGameMap;
            _progressService = progressService;
            var builder = Disposable.CreateBuilder();
            builder.Add(clickGesture.HoldGesture.Subscribe(OnHoldGesture));
            _disposable = builder.Build();

            actorStore.Register(userActor);
            actorStore.SetMyActor(userActor.Id);
        }

        public void Start()
        {
            CreateMap();
            _progressService.StartProgress();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        private void OnHoldGesture(HoldGesturePayload payload)
        {
            _interactionUseCase.UpdateGesture(payload);
        }

        private void CreateMap()
        {
            //원래는 생성 후 맵에 넣어야 하지만 현재 생성 기능이 없다
            var prop = _inGameMap.FarmProp;
            var farmPayload = new PropState(1, 0, 0);
            _farmStore.Register(farmPayload, prop);
        
        }
    }
}