using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Champ.ChampCode.Cards.Ancient;

[Pool(typeof(ChampCardPool))]
public class LastStand : ChampCardModel
{
    public LastStand() : base(1, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithPower<StrengthPower>(6);
        WithTip<WeakPower>();
        WithTip<VulnerablePower>();
        WithTip<FrailPower>();
    }

    private IEnumerable<LocString> Banter =>
    [
        new("cards", Id.Entry + ".banter.1"),
        new("cards", Id.Entry + ".banter.2")
    ];

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        SfxCmd.Play("event:/sfx/characters/champ-champ/charge");
        // calling runstate rng? hm. idc. it's prettier because its mp synced in comparison to Rng.Chaotic
        var banter = RunState?.Rng.Niche.NextItem(Banter);
        if (banter != null) TalkCmd.Play(banter, Owner.Creature, VfxColor.DarkGray);
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
        await PowerCmd.Remove<WeakPower>(Owner.Creature);
        await PowerCmd.Remove<VulnerablePower>(Owner.Creature);
        await PowerCmd.Remove<FrailPower>(Owner.Creature);
    }
}