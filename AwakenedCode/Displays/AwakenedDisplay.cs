using Awakened.AwakenedCode.Vfx;
using Downfall.DownfallCode.Core;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Awakened.AwakenedCode.Displays;

public class AwakenedDisplay
{
   
    private static readonly PlayerField<NAwakenMeter> AwakenMeterDisplays = new(() => null);
    
   
    public static void RefreshAwakenMeter(Player player, int value)
    {
        if (!LocalContext.IsMe(player)) return;;
        Callable.From(() =>
        {
            var existing = AwakenMeterDisplays.Get(player);
            if (existing != null && (!GodotObject.IsInstanceValid(existing) || existing.IsExiting))
            {
                AwakenMeterDisplays.Set(player, null);
                existing = null;
            }

            if (existing == null)
            {
                var display = NAwakenMeter.Create(player);
                if (display == null) return;
                display.SetProgress(value);
                AwakenMeterDisplays.Set(player, display);
            }
            else
            {
                existing.Refresh(value);
            }
        }).CallDeferred();
    }
    
    
    /*
   private static readonly PlayerField<NSpellbookDisplay> AwakenSpellDisplays = new(() => null);
   public static void RefreshSpellDisplays(Player player)
   {
       if (!LocalContext.IsMe(player)) return;;
       Callable.From(() =>
       {
           var existing = AwakenSpellDisplays.Get(player);
           if (existing != null && (!GodotObject.IsInstanceValid(existing) || existing.IsExiting))
           {
               AwakenSpellDisplays.Set(player, null);
               existing = null;
           }

           if (existing == null)
           {
               var display = NSpellbookDisplay.Create(player);
               if (display == null) return;
               AwakenSpellDisplays.Set(player, display);
           }
           else
           {
               existing.Refresh();
           }
       }).CallDeferred();
   }
   */
}