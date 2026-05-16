using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Relics;

[Pool(typeof(HexaghostRelicPool))]
public class SoulStone() : HexaghostRelicModel(RelicRarity.Rare)
{
    private int _exhausted;
    private bool _isActivating;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;
    public override int DisplayAmount => _isActivating ? 4 : _exhausted;

    public override Task BeforeCombatStart()
    {
        _exhausted = 0;
        _isActivating = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card.Owner != Owner) return;
        _exhausted++;
        Status = _exhausted >= 3 ? RelicStatus.Active : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
        if (_exhausted < 4) return;
        _exhausted = 0;
        _ = DoActivateVisuals();
        await HexaghostCmd.Ignite(ctx, card.Owner);
    }

    private async Task DoActivateVisuals()
    {
        _isActivating = true;
        Flash();
        Status = RelicStatus.Normal;
        InvokeDisplayAmountChanged();
        await Cmd.Wait(1f);
        _isActivating = false;
        InvokeDisplayAmountChanged();
    }
}