using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Actions;

public sealed class DrawCardsAction : GameAction
{
    private readonly uint _count;

    private readonly Player _player;

    public DrawCardsAction(Player player, uint count)
    {
        _player = player;
        _count = count;
    }

    public DrawCardsAction(Player player, uint count, bool fromNet)
    {
        _player = player;
        _count = count;
    }

    public override ulong OwnerId => _player.NetId;
    public override GameActionType ActionType => GameActionType.Combat;

    protected override async Task ExecuteAction()
    {
        PlayerChoiceContext context = new GameActionPlayerChoiceContext(this);
        await CardPileCmd.Draw(context, _count, _player);
    }

    public override INetAction ToNetAction()
    {
        return new NetDrawCardsAction { Count = _count };
    }
}