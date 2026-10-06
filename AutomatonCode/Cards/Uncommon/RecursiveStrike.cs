using Automaton.AutomatonCode.Cards.Token;
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
        WithCards(2);
        WithTip(AutomatonKeyword.Encode);
        WithTags(CardTag.Strike);
        WithUpgradingCardTip<CoreStrike>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, 2)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
            await AutomatonCmd.EncodeCard<CoreStrike>(Owner, ctx, Upgrade);
    }

    private void Upgrade(CardModel card)
    {
        if (IsUpgraded) CardCmd.Upgrade(card);
    }
}