using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Ghostflames;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class NaughtySpirit : HexaghostCardModel, IModifyCardPlayResultLocation
{
    public NaughtySpirit() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPower<SoulBurnPower>(3, 2);
        WithTip(HexaghostKeyword.Retract);
    }

    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            var a = HexaghostCmd.GetCurrentFlame(Owner);
            if (a.IsIgnited) return true;
            switch (a)
            {
                case CrushingGhostflame when a.IgnitionRequirement - a.IgnitionProgress <= 1:
                case InfernoGhostflame when a.IgnitionRequirement - a.IgnitionProgress <= EnergyCost.GetResolved():
                    return true;
                default:
                    return false;
            }
        }
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
        if (!HexaghostCmd.IsIgnited(Owner)) return;
        await CardPileCmd.Add(this, PileType.Hand);
        await HexaghostCmd.Retract(ctx, Owner, this);
    }
}