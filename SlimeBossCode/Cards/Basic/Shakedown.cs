using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Cards.Ancient;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Basic;

[Pool(typeof(SlimeBossCardPool))]
public class Shakedown : SlimeBossCardModel, ITranscendenceCard
{
    public Shakedown() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithPower<ShakedownPotencyPower>(2, false);
        WithTip<PotencyPower>();
        WithSlimeTip<BruiserSlime>();
        WithCommand(1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ShakedownPotencyPower>(ctx, this);
        await SlimeBossCmd.Command<BruiserSlime>(ctx, this);
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<FullOnAssault>();
    }
}

public class ShakedownPotencyPower : CustomTemporaryPowerModelWrapper<Shakedown, PotencyPower>;
