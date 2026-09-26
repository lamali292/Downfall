using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Multiplayer;

[Pool(typeof(SlimeBossCardPool))]
public class Unison : SlimeBossCardModel
{
    public Unison() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer)
    {
        WithCostUpgradeBy(-1);
        WithKeyword(CardKeyword.Exhaust);
    }
    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var target = cardPlay.Target?.Player;
        if (target == null || CombatState == null) return;

        var mySlimes = SlimeBossCmd.GetSlimes(Owner).Select(s => s.GetType()).Distinct()
            .Select(t => SlimeBossModelDb.AllSlimes.First(s => s.GetType() == t))
            .ToList();
        if (mySlimes.Count == 0) return;

        var slimeCards = mySlimes
            .Select(SlimeBossModelDb.GetCardForSlime)
            .Select(c => CombatState.CreateCard(c, Owner))
            .ToList();

        var chosen = await CardSelectCmd.FromChooseACardScreen(ctx, slimeCards, Owner);
        if (chosen is not ISlimeCard slimeCard) return;
        await SlimeBossCmd.Split(ctx, target, slimeCard.SlimeModel);
    }
}
