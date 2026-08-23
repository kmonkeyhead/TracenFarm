using Game.Content.InGame.Props;
using VitalRouter;

namespace Game.Command
{
    public record PropWorkCompletedCommand(PropType PropType, int PropId) : ICommand;
}