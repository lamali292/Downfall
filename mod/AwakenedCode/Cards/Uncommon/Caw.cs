using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Caw : AwakenedCardModel
{
    private static readonly LocString CawCawDialogue = new("monsters", "DAMP_CULTIST.moves.INCANTATION.banter");

    public Caw() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(4, 1);
        WithVar("Caw", 4, 1);
        WithKeyword(AwakenedKeyword.Chant);
    }

    protected override Artist Artist => Artist.Get<Occultpyromancer>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions
            .CardAttack(this, cardPlay, sfx: "event:/sfx/enemy/enemy_attacks/cultists/cultists_buff_damp").Execute(ctx);
        TalkCmd.Play(CawCawDialogue, Owner.Creature, VfxColor.Blue);
        await ChantCmd.Chant(cardPlay, () =>
        {
            if (Owner.PlayerCombatState == null) return Task.CompletedTask;
            var caws = Owner.PlayerCombatState.AllCards.OfType<Caw>();
            foreach (var caw in caws)
            {
                caw.DynamicVars.Damage.UpgradeValueBy(DynamicVars["Caw"].BaseValue);
            }
            return Task.CompletedTask;
        });
    }
}