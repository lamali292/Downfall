using Automaton.AutomatonCode.Cards.Basic;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class RecursiveStrike : AutomatonCardModel
{
    public RecursiveStrike() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithTip(AutomatonTip.Encode);
        WithTags(CardTag.Strike);
        WithUpgradingCardTip<StrikeAutomaton>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, 2)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
        await AutomatonCmd.EncodeCard<StrikeAutomaton>(Owner, ctx, Upgrade);
        await AutomatonCmd.EncodeCard<StrikeAutomaton>(Owner, ctx, Upgrade);
    }

    private void Upgrade(CardModel card)
    {
        if (IsUpgraded) CardCmd.Upgrade(card);
    }
}