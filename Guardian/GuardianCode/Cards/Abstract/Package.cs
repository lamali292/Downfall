using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Guardian.GuardianCode.Cards.Common;
using Guardian.GuardianCode.Cards.Rare;
using Guardian.GuardianCode.Cards.Token;
using Guardian.GuardianCode.Cards.Uncommon;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Guardian.GuardianCode.Cards.Abstract;

#pragma warning disable STS001
[Pool(typeof(TokenCardPool))]
public class PackageAncients() : Package<Overload, AncientPower, AncientConstruct>(2);

[Pool(typeof(TokenCardPool))]
public class PackageBronze() : Package<GigaBeam, OrbSupport, ResilientPlate>(1);

[Pool(typeof(TokenCardPool))]
public class PackageDefect() : Package<Reroute, Preprogram, TimeCapacitor>(1);

[Pool(typeof(TokenCardPool))]
public class PackageOrbwalker() : Package<Orbwalk, WalkerClaw, Incinerate>(0);

[Pool(typeof(TokenCardPool))]
public class PackageSentry() : Package<SentryBlast, SentryWave, SentryWave>(0);

[Pool(typeof(TokenCardPool))]
public class PackageShapes() : Package<TimeBomb, SpikerProtocol, RepulsorGuardian>(0);

[Pool(typeof(TokenCardPool))]
public class PackageSpheric() : Package<SphericShield, FloatingOrbs, Harden>(2);
#pragma warning restore STS001

public abstract class Package<T1, T2, T3> : GuardianCardModel, IPackageCard
    where T1 : CardModel
    where T2 : CardModel
    where T3 : CardModel
{
    protected Package(int cost) : base(cost, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithUpgradedCardTip<T1>();
        WithUpgradedCardTip<T2>();
        WithUpgradedCardTip<T3>();
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        var card1 = ModelDb.Card<T1>().ToMutable();
        var card2 = ModelDb.Card<T2>().ToMutable();
        var card3 = ModelDb.Card<T3>().ToMutable();
        if (IsUpgraded)
        {
            card1.UpgradeInternal();
            card2.UpgradeInternal();
            card3.UpgradeInternal();
        }

        description.Add("card1", card1.Title);
        description.Add("card2", card2.Title);
        description.Add("card3", card3.Title);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await DownfallCardCmd.GiveCard<T1>(Owner, PileType.Hand, upgraded: IsUpgraded);
        await DownfallCardCmd.GiveCard<T2>(Owner, PileType.Hand, upgraded: IsUpgraded);
        await DownfallCardCmd.GiveCard<T3>(Owner, PileType.Hand, upgraded: IsUpgraded);
    }
}

internal interface IPackageCard;

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
public static class FunctionCardTitlePatch
{
    private static bool Prefix(CardModel __instance, ref LocString __result)
    {
        if (__instance is not IPackageCard) return true;
        __result = new LocString("cards", "GUARDIAN-PACKAGE.description");
        return false;
    }
}