using UnityEngine;
using UnityEngine.UI;
namespace PFACombat
{
 public static class AmmoDisplay
 {
     public static void Refresh(Text label, int current, int capacity)
     {
         label.text = current + "/" + capacity;
         label.color = current == 0 ? new Color(1, 0.25f, 0.2f) : Color.cyan;
     }
 }
}
