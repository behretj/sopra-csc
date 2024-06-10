using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Renderer;
using SopraProjekt.Sound;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SopraProjekt.Entities.Functional
{
    [Serializable]
    internal sealed class BrewStand : Colliding
    {
        private const string OwnAssetName = "Images/Entities/Braustand";
        private const string UniqueIdentifier = "brewing_stand";
        private const int TextureSize = 100;

        [NonSerialized]
        private static readonly Color sMiniMapColor = Color.Thistle;

        private static readonly int[] sHealthPotionPrice = { 3, 1, 1 };
        private static readonly int[] sOxygenPotionCost = { 0, 3, 2 };
        private static readonly string[] sHerbsColors = { "blau", "lila", "pink" };

        [NonSerialized]
        public Camera mCamera;
        [NonSerialized]
        public Dictionary<string, Texture2D> mTextures;

        [NonSerialized]
        private StringBuilder mStringBuilder;

        [NonSerialized]
        private SpriteFont mFont;



        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="position"></param>
        /// <param name="camera"></param>
        /// <param name="textures">textures of the speech bubble</param>
        internal BrewStand(Point position, Camera camera, Dictionary<string, Texture2D> textures) :
            base(OwnAssetName, UniqueIdentifier, position, new Point(TextureSize), sMiniMapColor)
        {
            mCamera = camera;
            mTextures = textures;
            mStringBuilder = new StringBuilder();
            mFont = Content.LoadSpriteFont("Design/Button");
        }

        /// <summary>
        /// Use the brewing stand
        /// </summary>
        /// <param name="hero">hero which uses the brewing stand</param>
        /// <param name="gameTime">game time</param>
        internal void Use(Hero hero, GameTime gameTime)
        {

            CreateMassage(hero);
            var oxygen = CheckOxygenPotionIngredient(hero);
            var health = CheckHealthPotionIngredient(hero);


            EntityRenderer.mNpcMessages.Push(new Message(
                    mPosition,
                    new[]
                    {
                        mStringBuilder.ToString()
                    },
                    gameTime.TotalGameTime.TotalSeconds,
                    new Tuple<bool, bool>(health, oxygen),
                    hero,
                    this,
                    mCamera,
                    mTextures,
                    mFont
                    )
            );
        }


        /// <summary>
        /// Create the massage for the brew stand speech bubble
        /// </summary>
        /// <param name="hero"></param>
        /// <returns>string with text</returns>
        public string CreateMassage(Hero hero)
        {
            mStringBuilder.Clear();
            mStringBuilder.Append("Benoetigte Zutaten:\nHeiltrank : ");
            for (var i = 0; i < 3; i++)
            {
                if (sHealthPotionPrice[i] > 0)
                {
                    mStringBuilder.AppendFormat("({0}/{1}) {2}, ",
                        hero.mNumberHerbs[i],
                        sHealthPotionPrice[i],
                        sHerbsColors[i]);
                }
            }
            mStringBuilder.Length -= 2;

            mStringBuilder.Append("\nSauerstofftrank : ");
            for (var i = 0; i < 3; i++)
            {
                if (sOxygenPotionCost[i] > 0)
                {
                    mStringBuilder.AppendFormat("({0}/{1}) {2}, ",
                        hero.mNumberHerbs[i],
                        sOxygenPotionCost[i],
                        sHerbsColors[i]);
                }
            }
            mStringBuilder.Length -= 2;

            return mStringBuilder.ToString();
        }

        /// <summary>
        /// Brew a health potion
        /// </summary>
        /// <param name="hero">hero to brew for</param>
        /// <returns>true if health potion could be brewed, false otherwise</returns>
        public bool BrewHealthPotion(Hero hero)
        {
            if (!CheckHealthPotionIngredient(hero))
            {
                return false;
            }

            for (var i = 0; i < 3; i++)
            {
                hero.mNumberHerbs[i] -= sHealthPotionPrice[i];
            }

            SoundManager.Default.PlaySoundEffect("SoundEffects/bubbling_short", mPosition, mCamera);
            hero.mNumberHealthPotions += 1;

            return true;
        }

        /// <summary>
        /// Brew a oxygen potion
        /// </summary>
        /// <param name="hero">hero to brew for</param>
        /// <returns>true if oxygen potion could be brewed, false otherwise</returns>
        public bool BrewOxygenPotion(Hero hero)
        {
            if (!CheckOxygenPotionIngredient(hero))
            {
                return false;
            }

            for (var i = 0; i < 3; i++)
            {
                hero.mNumberHerbs[i] -= sOxygenPotionCost[i];
            }
            SoundManager.Default.PlaySoundEffect("SoundEffects/bubbling_short", mPosition, mCamera);

            hero.mNumberOxygenPotions += 1;

            return true;
        }

        /// <summary>
        /// Check if the hero has enough herbs for an oxygen potion
        /// </summary>
        /// <param name="hero"></param>
        /// <returns></returns>
        public bool CheckOxygenPotionIngredient(Hero hero)
        {
            for (var i = 0; i < 3; i++)
            {
                if (hero.mNumberHerbs[i] < sOxygenPotionCost[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if the hero has enough herbs for a health potion
        /// </summary>
        /// <param name="hero"></param>
        /// <returns></returns>
        public bool CheckHealthPotionIngredient(Hero hero)
        {
            for (var i = 0; i < 3; i++)
            {
                if (hero.mNumberHerbs[i] < sHealthPotionPrice[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks out Storage after deserialization
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        private void CheckOutStorage(StreamingContext context)
        {
            mStringBuilder = new StringBuilder();
            mFont = Content.LoadSpriteFont("Design/Button");
        }
    }
}
