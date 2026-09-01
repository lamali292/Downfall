using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class SpaghettiCode : AutomatonCardModel
{
    public SpaghettiCode() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithTip(AutomatonTip.Encode);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    { 
        FunctionCard? functionCard = null;
        while (functionCard == null)
        {
            var choices = AutomatonCmd.GetEncodableCards(Owner, 3).ToList();
            var selected = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
            if (selected == null) break;
            functionCard = await AutomatonCmd.EncodeCard(selected, ctx);
        }
        if (IsUpgraded) functionCard?.SetToFreeThisTurn();
    }
}