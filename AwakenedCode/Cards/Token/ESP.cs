using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Events;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Awakened.AwakenedCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class ESP : AwakenedCardModel, ISpell, IOnAwaken, ICustomTypePlaque
{
    public ESP() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithCards(1, 1);
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
        WithTags(AwakenedTag.Spell);
    }
    
    public LocString GetTypePlaqueName => new("gameplay_ui", "AWAKENED-SPELL");

    public Task OnAwaken(PlayerChoiceContext ctx, Player player)
    {
        if (player != Owner) return Task.CompletedTask;
        CardCmd.Upgrade(this, CardPreviewStyle.None);
        return Task.CompletedTask;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, cardPlay.Card.Owner);
    }
}