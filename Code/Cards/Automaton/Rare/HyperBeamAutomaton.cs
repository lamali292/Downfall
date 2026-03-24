using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Cards.Automaton.Rare;

[Pool(typeof(AutomatonCardPool))]
public class HyperBeamAutomaton() : AutomatonCardModel(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(25M, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Void>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        var hyperbeam = this;
        await DamageCmd.Attack(hyperbeam.DynamicVars.Damage.BaseValue)
            .FromCard(hyperbeam)
            .TargetingAllOpponents(hyperbeam.CombatState)
            .WithAttackerAnim("Cast", 0.5f)
            .BeforeDamage(BeforeDamageAction)
            .Execute(ctx);

        List<CardModel> burns =
        [
            CombatState.CreateCard<Void>(Owner),
            CombatState.CreateCard<Void>(Owner),
            CombatState.CreateCard<Void>(Owner),
            CombatState.CreateCard<Void>(Owner),
            CombatState.CreateCard<Void>(Owner)
        ];
        var result = await CardPileCmd.AddGeneratedCardsToCombat(burns, PileType.Draw, true, CardPilePosition.Top);
        CardCmd.PreviewCardPileAdd(result, 0.2f, CardPreviewStyle.MessyLayout);
    }

    private async Task BeforeDamageAction()
    {
        var enemies = CombatState?.Enemies.Where(e => e.IsAlive).ToList();

        if (enemies != null)
        {
            var vfx = NHyperbeamVfx.Create(Owner.Creature, enemies.Last());
            if (vfx != null) NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
        }

        await Cmd.Wait(0.5f);
        if (enemies != null)
            foreach (var impact in enemies
                         .Select(enemy => NHyperbeamImpactVfx.Create(Owner.Creature, enemy))
                         .OfType<NHyperbeamImpactVfx>())
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(impact);
    }
}