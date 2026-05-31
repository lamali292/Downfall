using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class FancyFootwork : ChampCardModel
{
    public FancyFootwork() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithPower<FancyFootworkPower>(10, 5, false);
        WithTip(ChampTip.Stance);
        WithTip(ChampTip.Finisher);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.EnterDifferentStance(ctx, Owner);
        await CommonActions.ApplySelf<FancyFootworkPower>(ctx, this);
    }
}