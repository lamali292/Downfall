using Downfall.Code.Abstract;
using Downfall.Code.Events;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Awakened;

public class DemonGlyphPower: AwakenedPowerModel, IOnAwaken
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public async Task OnAwaken(PlayerChoiceContext ctx, Player player)
    {
        await PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
        await PowerCmd.Apply<DexterityPower>(Owner, Amount, Owner, null);
        await PowerCmd.Remove(this);
    }
}