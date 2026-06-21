using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class WildStrike : AutomatonCardModel
{
    public WildStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(13, 5);
        this.WithTip<Wound>();
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
        await DownfallCardCmd.GiveCard<Wound>(Owner, PileType.Draw);
    }
}