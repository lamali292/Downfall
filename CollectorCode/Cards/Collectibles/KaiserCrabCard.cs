using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
namespace Collector.CollectorCode.Cards.Collectibles;

public class KaiserCrabCard : Collectible<KaiserCrabBoss>
{
    public KaiserCrabCard() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f)
    {
        WithDamage(16, 7);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await Cmd.CustomScaledWait(0.1f, 0.3f);
        CardModel? card;
        if (IsUpgraded)
        {
            var prefs = new CardSelectorPrefs(DownfallCardSelectorPrefs.PlaySelectionPrompt, 1);
            card = (await CardSelectCmd.FromHand(ctx, Owner, prefs, 
                e => e.Type == CardType.Attack, this)).FirstOrDefault();
        }
        else
        {        
            card = RunState!.Rng.CombatCardSelection.NextItem(Owner.Hand.Where(e => e.Type == CardType.Attack));
        }
    
        if (card != null) await CardCmd.AutoPlay(ctx, card, null);
    }
}