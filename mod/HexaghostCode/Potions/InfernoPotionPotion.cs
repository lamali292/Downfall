using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Potions;

[Pool(typeof(HexaghostPotionPool))]
public class InfernoPotionPotion : HexaghostPotionModel
{
    public InfernoPotionPotion() : base(PotionRarity.Rare, PotionUsage.CombatOnly, TargetType.Self)
    {
        WithTip<SoulBurnPower>();
        WithPower<InfernoPotionPower>(1, false);
    }

    protected override Artist Artist => Artist.Get<Chimedragon>();

    protected override Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        return MyCommonActions.ApplySelf<InfernoPotionPower>(choiceContext, this);
    }
}