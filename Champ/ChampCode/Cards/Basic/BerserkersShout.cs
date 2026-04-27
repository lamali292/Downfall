using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Basic;

[Pool(typeof(ChampCardPool))]
public class BerserkersShout : ChampCardModel
{
    public BerserkersShout() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithPower<VigorPower>(2, 2);
        WithIcon<VigorPower>();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<VigorPower>(ctx, this);
        await ChampCmd.EnterBerserkerStance(ctx, Owner);
    }
}