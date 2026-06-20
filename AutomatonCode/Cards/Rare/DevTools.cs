using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Displays;
using Automaton.AutomatonCode.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class DevTools : AutomatonCardModel
{
    public DevTools() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithTip(AutomatonTip.Encode);
        WithCostUpgradeBy(-1);
        WithCalculatedVar("Dev", 0, Calc);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    private static decimal Calc(CardModel card, Creature? arg2)
    {
        return card.Owner.GetEncode().Count;
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var count = ((CalculatedVar)DynamicVars["Dev"]).Calculate(null);
        var cards = Owner.GetEncode().ToList();
        foreach (var card in cards)
            await CardCmd.Exhaust(ctx, card);
        await PlayerCmd.GainEnergy(count, Owner);
        await CardPileCmd.Draw(ctx, count, Owner);
        AutomatonDisplay.Refresh(Owner);
    }
}