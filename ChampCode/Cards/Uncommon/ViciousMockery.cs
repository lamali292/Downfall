using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class ViciousMockery : ChampCardModel
{
    public ViciousMockery() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPower<VigorPower>(5, 1);
        WithPower<WeakPower>(1, 1);
        WithTip(ChampKeyword.TriggerSkillBonus);
    }

    private IEnumerable<LocString> Banter =>
    [
        new("cards", Id.Entry + ".banter.1"),
        new("cards", Id.Entry + ".banter.2"),
        new("cards", Id.Entry + ".banter.3"),
        new("cards", Id.Entry + ".banter.4")
    ];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (TestMode.IsOff) SfxCmd.Play("event:/sfx/characters/champ-champ/mockery");
        // calling runstate rng? hm. idc. it's prettier because its mp synced in comparison to Rng.Chaotic
        var banter = RunState?.Rng.Niche.NextItem(Banter);
        if (banter != null) TalkCmd.Play(banter, Owner.Creature, VfxColor.DarkGray);
        await CommonActions.ApplySelf<VigorPower>(ctx, this);
        await CommonActions.Apply<WeakPower>(ctx, cardPlay.Target!, this);
        await Owner.ChampStance.SkillBonus(ctx);
    }
}