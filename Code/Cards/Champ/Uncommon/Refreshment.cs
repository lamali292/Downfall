using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class Refreshment : ChampCardModel
{
    public Refreshment() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithEnergy(2, 1);
        WithCards(3, 1);
    }


    protected override bool ShouldGlowGoldInternal =>
        Owner.ShouldBerserkerComboTrigger() || Owner.ShouldDefensiveComboTrigger();

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Owner.ShouldBerserkerComboTrigger())
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
            await CardCmd.Exhaust(ctx, this);
        }

        if (Owner.ShouldDefensiveComboTrigger()) await CommonActions.Draw(this, ctx);
    }
}