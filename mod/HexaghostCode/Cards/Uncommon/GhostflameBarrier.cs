using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using Godot;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class GhostflameBarrier : HexaghostCardModel
{
    public GhostflameBarrier() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(12, 4);
        WithTip<SoulBurnPower>();
        WithPower<GhostflameBarrierPower>(5, 2, false);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(Owner.Creature);
        if (creatureNode != null)
        {
            var child = NFireBurningVfx.Create(creatureNode.GetBottomOfHitbox(), 0.75f, false,
                Color.FromHtml("#8bff57"));
            var instance = NCombatRoom.Instance;
            instance?.CombatVfxContainer.AddChildSafely(child);
        }

        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<GhostflameBarrierPower>(ctx, this);
    }
}