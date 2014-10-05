// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeroExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The hero extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Core
{
    using System.Windows.Media;

    using HearthCap.Data;

    using Color = System.Drawing.Color;

    /// <summary>
    /// The hero extensions.
    /// </summary>
    public static class HeroExtensions
    {
        /// <summary>
        /// The get brush.
        /// </summary>
        /// <param name="hero">
        /// The hero.
        /// </param>
        /// <returns>
        /// The <see cref="Brush"/>.
        /// </returns>
        public static Brush GetBrush(this Hero hero)
        {
            return GetBrush(hero != null ? hero.Key : null);
        }

        /// <summary>
        /// The get color.
        /// </summary>
        /// <param name="hero">
        /// The hero.
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public static Color GetColor(this Hero hero)
        {
            return GetColor(hero != null ? hero.Key : null);
        }

        /// <summary>
        /// The get color.
        /// </summary>
        /// <param name="heroKey">
        /// The hero key.
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public static Color GetColor(string heroKey)
        {
            // http://www.wowwiki.com/Class_colors
            Color color;
            if (string.IsNullOrEmpty(heroKey))
            {
                color = Color.Black;
            }
            else
            {
                switch (heroKey)
                {
                    case "deathknight":
                        color = Color.FromArgb(196, 30, 59);
                        break;
                    case "druid":
                        color = Color.FromArgb(255, 125, 10);
                        break;
                    case "hunter":
                        color = Color.FromArgb(171, 212, 115);
                        break;
                    case "mage":
                        color = Color.FromArgb(105, 204, 240);
                        break;
                    case "monk":
                        color = Color.FromArgb(0, 255, 150);
                        break;
                    case "paladin":
                        color = Color.FromArgb(245, 140, 186);
                        break;
                    case "priest":
                        color = Color.Silver;
                        break;
                    case "rogue":
                        color = Color.FromArgb(255, 245, 105);
                        break;
                    case "shaman":
                        color = Color.FromArgb(0, 112, 222);
                        break;
                    case "warlock":
                        color = Color.FromArgb(148, 130, 201);
                        break;
                    case "warrior":
                        color = Color.FromArgb(199, 156, 110);
                        break;
                    default:
                        color = Color.Black;
                        break;
                }
            }

            return color;
        }

        /// <summary>
        /// The get brush.
        /// </summary>
        /// <param name="heroKey">
        /// The hero key.
        /// </param>
        /// <returns>
        /// The <see cref="Brush"/>.
        /// </returns>
        public static Brush GetBrush(string heroKey)
        {
            var color = GetColor(heroKey);
            var brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
            brush.Freeze();
            return brush;
        }
    }
}