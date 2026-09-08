using DataType;
using Game.Content.InGame.Farms;
using VitalRouter;

namespace Game.Command
{
    public record FarmWorkCompletedCommand(PropType PropType, FarmId FarmId) : ICommand;
}