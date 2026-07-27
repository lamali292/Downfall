using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Potions;

[Pool(typeof(HexaghostPotionPool))]
public class CombustiveFluidPotion : HexaghostPotionModel
{
    public CombustiveFluidPotion() : base(PotionRarity.Uncommon, PotionUsage.CombatOnly, TargetType.Self)
    {
        WithVar("Ignite", 3);
    }
    
    protected override Artist Artist => Artist.Get<Chimedragon>();


    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        var player = target?.Player;
        if (player == null) return;
        var a = DynamicVars["Ignite"].IntValue;
        for (var i = 0; i < a; i++)
        {
            await HexaghostCmd.Ignite(ctx, player);
        }
    }
}