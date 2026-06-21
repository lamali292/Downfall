using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Break : AutomatonCardModel
{
    public Break() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(20, 5);
        this.WithTip<Error>();
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
        await DownfallCardCmd.GiveCard<Error>(Owner, PileType.Hand);
        await DownfallCardCmd.GiveCard<Error>(Owner, PileType.Draw);
        await DownfallCardCmd.GiveCard<Error>(Owner, PileType.Discard);
        await StashCmd.Stash<Error>(Owner);
    }
}