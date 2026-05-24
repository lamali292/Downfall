using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Mutator : AutomatonCardModel
{
    public Mutator() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<StrengthPower>(2);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithTip(StaticHoverTip.Transform);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1);
        var selected = (await CardSelectCmd.FromHand(ctx, Owner, prefs, card => card.Type == CardType.Status, this))
            .FirstOrDefault();
        if (selected == null) return;
        await CardCmd.Transform(selected, CreateClone());
    }
}