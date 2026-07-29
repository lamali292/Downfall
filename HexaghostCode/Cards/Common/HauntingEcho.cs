using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Ghostflames;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class HauntingEcho : HexaghostCardModel
{
    public HauntingEcho() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7, 3);
    }
    
    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            var a = HexaghostCmd.GetCurrentFlame(Owner);
            if (a.IsIgnited) return true;
            switch (a)
            {
                case SearingGhostflame when a.IgnitionRequirement - a.IgnitionProgress <= 1:
                case InfernoGhostflame when a.IgnitionRequirement - a.IgnitionProgress <= EnergyCost.GetResolved():
                    return true;
                default:
                    return false;
            }
        }
    }

    protected override Artist Artist => Artist.Get<Inmo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (!HexaghostCmd.IsIgnited(Owner)) return;
        await HexaghostCmd.Ignite(ctx, Owner);
    }
}