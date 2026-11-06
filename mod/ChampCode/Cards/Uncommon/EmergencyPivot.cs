using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class EmergencyPivot : ChampCardModel
{
    public EmergencyPivot() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeywords(CardKeyword.Retain);
        WithBlock(10, 4);
        WithFinisher();
    }

    protected override Artist Artist => Artist.Get<Fulgur>();
    protected override bool IsBetaArt => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await ChampCmd.PlayFinisher(ctx, cardPlay, Finisher);
    }
}