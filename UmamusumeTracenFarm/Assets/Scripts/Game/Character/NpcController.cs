using System;
using System.Linq;
using Game.Content.InGame;
using Game.Content.InGame.Farms;
using Game.Content.InGame.Store;
using Game.Content.InGame.UseCase;
using Game.Service.Farm;
using Game.Service.Input;
using UnityEngine;
using VContainer;

namespace Game.Character
{
    public class NpcController : MonoBehaviour
    {
        [SerializeField] private Actor _actor;

        //TODO : 액터 가져오기, 맵 오브젝트 가져오기 함수를 제공하는 인터페이스로 주입 받아야 한다
        private ActorStore _actorStore;
        private InGameMap _inGameMap;
        private FarmStore _farmStore;
        private FarmWorkUseCase _farmWorkUseCase;
        private FarmService _farmService;

        private AiStatusType _aiStatusType;

        private enum AiStatusType
        {
            Idle,
            MoveToPlayer,
            MoveToFarm,
            WorkToFarm
        }

        [Inject]
        public void Construct(ActorStore actorStore, InGameMap inGameMap, FarmStore farmStore, FarmWorkUseCase farmWorkUseCase, FarmService farmService)
        {
            _actorStore = actorStore;
            _inGameMap = inGameMap;
            _farmStore = farmStore;
            _farmWorkUseCase = farmWorkUseCase;
            _farmService = farmService;
        }

        private void MoveToPlayer()
        {
            var myActor = _actorStore.MyActor;
            MoveToPosition(myActor.Position);
        }

        private void MoveToPosition(Vector3 position)
        {
            _actor.SetAimDirection(position);
            _actor.SetMoveFlag(true);
        }

        private void WorkToFarm()
        {
            //1. 작업 가능한 farm을 먼저 찾는다
            var farm = _farmStore.Entries.FirstOrDefault();
            if (farm == null)
            {
                _actor.SetMoveFlag(false);
                return;
            }

            bool available = _farmService.CheckStorageSpace(farm.Payload.Id);
            if (!available)
            {
                _aiStatusType = AiStatusType.MoveToPlayer;
                MoveToPlayer();

                return;
            }

            Vector3 actorPosition = _actor.Position;
            actorPosition.y = 0f;

            if (_aiStatusType == AiStatusType.WorkToFarm)
            {
                return;
            }
            
            if (farm.CanInteract(actorPosition))
            {
                _actor.SetMoveFlag(false);
                _farmWorkUseCase.StartInteracting(farm.Payload.Id, _actor.Id);
                _aiStatusType = AiStatusType.WorkToFarm;
                return;
            }

            _aiStatusType = AiStatusType.MoveToFarm;
            MoveToPosition(farm.Position);
        }

        private void Update()
        {
            _actor.SetWalkFlag(true);
            WorkToFarm();
        }
    }
}