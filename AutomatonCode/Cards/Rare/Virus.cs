using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Virus : AutomatonCardModel
{
    public Virus() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(4, 2);
        WithKeywords(CardKeyword.Exhaust);
        WithUpgradingCardTip<MinorBeam>();
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
        var hand = Owner.GetHand();
        var size = hand.Count;
        await CardCmd.Discard(ctx, hand);
        await DownfallCardCmd.GiveCards<MinorBeam>(Owner, PileType.Hand, size, upgraded: IsUpgraded);
    }
}