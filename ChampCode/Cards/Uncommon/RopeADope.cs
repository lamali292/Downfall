using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class RopeADope : ChampCardModel
{
    public RopeADope() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithFinisher();
        WithBlock(8, 2);
        WithEnergy(1, 1);
        this.WithPower<DrawCardsNextTurnPower>(2, false);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<EnergyNextTurnPower>(ctx, this, DynamicVars.Energy.BaseValue);
        await CommonActions.ApplySelf<DrawCardsNextTurnPower>(ctx, this);
    }
}