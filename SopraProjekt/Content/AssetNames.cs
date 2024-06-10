using System.Collections.Generic;

namespace SopraProjekt.Content
{
    /// <summary>
    /// Class to store asset names from used textures and acoustic effects
    /// </summary>
    internal static class AssetNames
    {
        /// <summary>
        /// List of asset names for the sound effects
        /// </summary>
        public static List<string> AcousticEffectsTextureNames { get; } = new List<string>
        {
            "SoundEffects/backgroundMusic",
            "SoundEffects/soundEffectLaserShoot",
            "SoundEffects/soundEffectWalking",
            "SoundEffects/soundEffectOxygen",
            "SoundEffects/shieldSound",
            "SoundEffects/crushingSound",
            "SoundEffects/bubbling_short",
            "SoundEffects/power_increase",
            "SoundEffects/Laufgeraeusch",
            "SoundEffects/Kamikazemonster",
            "SoundEffects/Kampf",
            "SoundEffects/soundFight",
            "SoundEffects/Sieg",
            "SoundEffects/Sterben",
            "SoundEffects/Boost",
            "SoundEffects/Nahkampfmonster",
            "SoundEffects/ice_crumble"
        };
    }
}