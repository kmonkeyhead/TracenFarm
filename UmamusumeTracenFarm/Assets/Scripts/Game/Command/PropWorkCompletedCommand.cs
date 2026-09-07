using Game.Content.InGame.Farms;
using VitalRouter;

namespace Game.Command
{
    public record PropWorkCompletedCommand(PropType PropType, int PropId) : ICommand;
}