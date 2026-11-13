using System.Collections.Generic;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Avarice : SlimeBossCardModel
{
    public Avarice() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithBlock(7, 2);
        WithTip<Slimed>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = await DownfallCardSelectionCmd.SelectFromHand(ctx,
            CardSelectorPrefs.TransformSelectionPrompt, Owner.Hand.Count, this, c => c != this, true);
        var transformations = new List<CardTransformation>();
        foreach (var card in cards)
        {
            var slimed = CombatState?.CreateCard<Slimed>(Owner);
            if (slimed == null) continue;
            transformations.Add(new CardTransformation(card, slimed));
        }
        if (transformations.Count == 0) return;

        await CardCmd.Transform(transformations, null);
        for (var i = 0; i < transformations.Count; i++)
            await CommonActions.CardBlock(this, cardPlay);
    }
}
