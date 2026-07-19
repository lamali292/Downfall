using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using Automaton.AutomatonCode.Vfx;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class SentientForm : AutomatonCardModel
{
    public SentientForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SentientFormPower>(ctx, this);
        NSequenceDisplay.Refresh(Owner, true);
    }
}