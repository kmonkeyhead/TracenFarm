using Game.Content.InGame.Payload;
using Game.Content.InGame.Interaction;

namespace Game.Content.InGame.Farms
{
    public interface IFarm : IInteractable
    {
        PropType PropType { get; }

        void Initialize(IFarmWorkState farmWork);
    }


    public enum PropType
    {
        Farm,
        Carrot,
    }
}