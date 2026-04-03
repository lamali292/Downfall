using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Events;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Caw : AwakenedCardModel, IChantable, IOnChant
{
    private static readonly LocString CawCawDialogue = new("monsters", "DAMP_CULTIST.moves.INCANTATION.banter");

    public Caw() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(3);
        WithVar("Caw", 3);
        WithTip(DownfallKeyword.Chant);
    }

    public async Task PlayChantEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    public Task OnCardChanted(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card is Caw && card.Owner == Owner) DynamicVars.Damage.UpgradeValueBy(card.DynamicVars["Caw"].BaseValue);

        return Task.CompletedTask;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions
            .CardAttack(this, cardPlay, sfx: "event:/sfx/enemy/enemy_attacks/cultists/cultists_buff_damp").Execute(ctx);
        TalkCmd.Play(CawCawDialogue, Owner.Creature, VfxColor.Blue);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
        DynamicVars["Caw"].UpgradeValueBy(1);
    }
}