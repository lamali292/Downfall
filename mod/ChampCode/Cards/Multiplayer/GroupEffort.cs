using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Interfaces;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Multiplayer;

[Pool(typeof(ChampCardPool))]
public class GroupEffort : ChampCardModel
{
    public GroupEffort() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(9, 3);
        WithFinisher();
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override Artist Artist => Artist.Get<Fulgur>();
    protected override bool IsBetaArt => true;

    public override FinisherDescriptor Finisher => new(AffectsAllPlayers: true);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await ChampCmd.PlayFinisher(ctx, cardPlay, Finisher);
    }
}