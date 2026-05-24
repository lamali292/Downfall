using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class FindAndReplace : AutomatonCardModel
{
    public FindAndReplace() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var choices = Owner.GetStash().Concat(Owner.GetDraw()).Concat(Owner.GetDiscard()).ToList();
        var selected =
            (await DownfallCardCmd.SelectFromCards(ctx, choices, DownfallCardSelectorPrefs.ToHandSelectionPrompt, this))
            .FirstOrDefault();
        var sourcePile = selected?.Pile;
        if (sourcePile == null || selected == null) return;
        var index = sourcePile._cards.IndexOf(selected);
        await CardPileCmd.Add(selected, PileType.Hand);
        var error = CombatState.CreateCard<Error>(Owner);
        await DownfallCardCmd.AddWithIndex(error, sourcePile, index);
    }
}