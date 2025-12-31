using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DuckRace.Scripts.Global;
using Godot;

namespace DuckRace.Scripts.DataModels
{
    public class DuckRaceInfo
    {
        public string Name { get; set; }
        public string Id { get; set; }
        //This property is never manually set but instead always executes a method with the color name property.
        public Color Color => GetColor(ColorName);

        public string ColorName { get; set; }

        public bool HasPartyHat { get; set; } 

        /// <summary>
        /// You Can just give the Ducks Html Color codes and ignore this
        /// </summary>
        /// <param name="colorName"></param>
        /// <returns></returns>
        private Color GetColor(string colorName)
        {
            if (string.IsNullOrEmpty(colorName))
            {
                return Colors.Black;
            }
            if (colorName.ToLowerInvariant() == "random")
            {
                return GlobalDataManager.Instance.GetRandomColor();
            }
            if (colorName.ToLowerInvariant() == "name" && !string.IsNullOrEmpty(Name))
            {
                return GlobalDataManager.Instance.GetRandomColor(Name.ToLowerInvariant());
            }
            if (colorName.ToLowerInvariant() == "id" && !string.IsNullOrEmpty(Id))
            {
                return GlobalDataManager.Instance.GetRandomColor(Id.ToLowerInvariant());
            }

            return new Color(colorName);
        }
    }
}
