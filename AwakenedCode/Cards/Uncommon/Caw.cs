using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Events;
using Awakened.AwakenedCode.Interfaces;
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
public class Caw : AwakenedCardModel, IChantable, IOnChant
{
    private static readonly LocString CawCawDialogue = new("monsters", "DAMP_CULTIST.moves.INCANTATION.banter");

    public Caw() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(3, 1);
        WithVar("Caw", 3, 1);
    }

    protected override Artist Artist => Artist.Get<Occultpyromancer>();
    
    public bool HasChanted { get; set; } = false;

    public async Task PlayChantEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    public Task OnCardChanted(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay, bool firstTime)
    {
        if (card is Caw && card.Owner == Owner) DynamicVars.Damage.UpgradeValueBy(card.DynamicVars["Caw"].BaseValue);

        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions
            .CardAttack(this, cardPlay, sfx: "event:/sfx/enemy/enemy_attacks/cultists/cultists_buff_damp").Execute(ctx);
        TalkCmd.Play(CawCawDialogue, Owner.Creature, VfxColor.Blue);
    }
}