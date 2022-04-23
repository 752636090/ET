using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    [FriendClass(typeof(Item))]
    public static class ItemHelper
    {
        public static Color ItemQualityColor(this Item item)
        {
            ItemQuality quality = (ItemQuality)item.Quality;
            switch (quality)
            {
                case ItemQuality.General:
                    return Color.white;
                case ItemQuality.Good:
                    return Color.green;
                case ItemQuality.Excellent:
                    return Color.blue;
                case ItemQuality.Epic:
                    return Color.magenta;
                case ItemQuality.Legend:
                    return new Color(225.0f / 255, 128.0f / 255, 0.0f);
            }
            return Color.black;
        }
    }
}
