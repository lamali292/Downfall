using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Gremlins.GremlinsCode.Cards.Rare;

[Pool(typeof(GremlinsCardPool))]
public class Duplicate : GremlinsCardModel
{
    public Duplicate() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
        WithCards(2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = (await DownfallCardCmd.SelectFromHand(ctx, DownfallCardSelectorPrefs.ApplySelectionPrompt, this,
            e => e.Type == CardType.Attack && !e.IsEcho)).FirstOrDefault();
        if (card == null) return;
        var copies = Enumerable.Range(0, DynamicVars.Cards.IntValue)
            .Select(_ =>
            {
                var echo = card.CreateEcho();
                echo.EnergyCost.AddThisCombat(-1);
                return echo;
            })
            .ToList();
        await CardPileCmd.Add(copies, PileType.Hand);
    }
}