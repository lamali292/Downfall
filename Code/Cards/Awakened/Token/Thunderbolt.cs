using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Events;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class Thunderbolt : AwakenedCardModel, ISpell, IOnAwaken
{
    public Thunderbolt() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithDamage(12, 6);
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
    }

    public Task OnAwaken(PlayerChoiceContext ctx, Player player)
    {
        CardCmd.Upgrade(this, CardPreviewStyle.None);
        return Task.CompletedTask;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitFx("vfx/vfx_attack_lightning")
            .Execute(ctx);
    }
}