using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Displays;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class SentientForm : AutomatonCardModel
{
    public SentientForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<SentientFormPower>( 1, false);
        WithTip(StaticHoverTip.ReplayStatic);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);

    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SentientFormPower>(ctx, this);
        AutomatonDisplay.Refresh(Owner, true);
    }
}