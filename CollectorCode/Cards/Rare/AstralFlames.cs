using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class AstralFlames : CollectorCardModel
{
    public AstralFlames() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(6, 3);
        WithVar("Increase", 2, 1);
        WithEnergyTip();
        WithTip(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var card = (await DownfallCardCmd.MulitPileSelect(ctx, Owner, prefs, null, PileType.Hand, PileType.Discard, PileType.Draw)).FirstOrDefault();
        var block = DynamicVars.Block.IntValue;
        if (card != null)
        {
            block += card.EnergyCost.GetResolved() * DynamicVars["Increase"].IntValue;
            await CardCmdCompatibility.Exhaust(ctx, card);
        }
        await CreatureCmd.GainBlock(Owner.Creature, block, DynamicVars.Block.Props, cardPlay);
    }

}