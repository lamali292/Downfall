using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Altar : AwakenedCardModel
{
    public Altar() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(5);
        WithTip(CardKeyword.Exhaust);
        WithTip(DownfallKeyword.Conjure);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay); 
        var card = await CommonActions.SelectSingleCard(this, SelectionScreenPrompt, ctx, PileType.Hand);
        if (card != null) await CardCmd.Exhaust(ctx, card);
        await AwakenedCmd.Conjure(Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
    
    
}