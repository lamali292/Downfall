using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Common;

[Pool(typeof(AwakenedCardPool))]
public class Gather : AwakenedCardModel, IChantable
{
    public Gather() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(3);
        WithTip(DownfallKeywords.Chant);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }

    public async Task OnChant(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var selected = await CommonActions.SelectSingleCard(this, SelectionScreenPrompt, ctx, PileType.Discard);
        if (selected == null) return;
        await CardPileCmd.Add(selected, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}