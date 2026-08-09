using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class Endure : ChampCardModel, IIgnoreDexterityCard
{
    public Endure() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithCalculatedBlock(6, BlockCalc, BlockProps.card, 2);
        this.WithTip<StrengthPower>();
        WithTips(e => e.IsUpgraded ? [] : [HoverTipFactory.FromPower<DexterityPower>()]);
        this.WithEnterDefensive();
    }
    
    protected override Artist Artist => Artist.Get<Opal>();

    private static decimal BlockCalc(CardModel card, Creature? creature)
    {
        return card.Owner.Creature.GetPowerAmount<StrengthPower>();
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.EnterDefensiveStance(ctx, Owner);
        await CommonActions.CardBlock(this, cardPlay);
    }

    public bool ShouldIgnoreDexterity => !IsUpgraded;
}