using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using SopraProjekt.Content;
using SopraProjekt.Helper;
using SopraProjekt.Renderer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SopraProjekt.Sound
{
    /// <summary>
    /// Class to handle sound effects and music
    /// </summary>
    internal sealed class SoundManager
    {
        private static readonly Dictionary<string, SoundEffect> sAcousticEffects = new Dictionary<string, SoundEffect>();
        private static readonly List<string> sAcousticEffectAssetNames = AssetNames.AcousticEffectsTextureNames;

        private static readonly Dictionary<int, SoundEffectInstance> sLoopingSoundEffectInstances = new Dictionary<int, SoundEffectInstance>();

        private static SoundEffectInstance PlayingMusic { get; set; }

        internal static SoundManager Default { get; } = new SoundManager();

        private float mVolumeMaster;
        private float mVolumeEffects;
        private float mVolumeMusic;

        public static bool mTechDemo;

        private bool mSoundEffectOn;
        private SoundEffectInstance mCurrentEffectInstance;
        internal float VolumeMaster
        {
            get => mVolumeMaster;
            set
            {
                mVolumeMaster = Math.Clamp(value, 0, 1);
                ApplyVolume();
            }
        }

        internal float VolumeEffect
        {
            get => mVolumeEffects;
            set
            {
                mVolumeEffects = Math.Clamp(value, 0, 1);
                ApplyVolume();
            }
        }

        internal float VolumeMusic
        {
            get => mVolumeMusic;
            set
            {
                mVolumeMusic = Math.Clamp(value, 0, 1);
                ApplyVolume();
            }
        }

        private SoundManager()
        {
            mVolumeMaster = Settings1.Default.masterVolume;
            mVolumeEffects = Settings1.Default.effectsVolume;
            mVolumeMusic = Settings1.Default.musicVolume;
        }

        internal static bool IsPlayingMusic => PlayingMusic != null;

        /// <summary>
        /// Load all sound effects
        /// </summary>
        /// <param name="contentManager"></param>
        internal static void LoadContent(ContentManager contentManager)
        {
            var contentLoader = new ContentLoader(contentManager);
            foreach (var assetName in sAcousticEffectAssetNames)
            {
                try
                {
                    sAcousticEffects.Add(assetName, contentLoader.LoadSoundEffect(assetName));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Apply the volume values to the sound effects
        /// </summary>
        private void ApplyVolume()
        {
            var volumeEffect = VolumeEffect * VolumeMaster;

            foreach (var acousticEffect in sLoopingSoundEffectInstances)
            {
                acousticEffect.Value.Volume = volumeEffect;
            }


            if (PlayingMusic == null)
            {
                return;
            }
            PlayingMusic.Volume = VolumeMusic * VolumeMaster;
        }

        /// <summary>
        /// Play a sound effect
        /// </summary>
        /// <param name="assetName">name of the sound effect</param>
        /// <param name="source"></param>
        /// <param name="camera">position of the camera</param>
        /// <param name="volume">volume of this specific acoustic effect, default 1</param>
        /// <param name="pan">set if one of the speakers should be louder than the other</param>
        /// <param name="looped">if looped ist true, the sound is looped until it is ended by teh End() function, default: false</param>
        internal void PlaySoundEffect(string assetName, Point source, Camera camera, float volume = 1, float pan = 0, bool looped = false)
        {
            if (!mTechDemo)
            {
                //stops background music when game starts
                //PlayingMusic.Stop();
                var soundEffect = sAcousticEffects[assetName];
                mCurrentEffectInstance = soundEffect.CreateInstance();
                mCurrentEffectInstance.Volume = volume * VolumeEffect * VolumeMaster;
                if (camera != null)
                {
                    mCurrentEffectInstance.Volume *= Math.Clamp(-1 / 10f * Distance(source, IsoHelper.IsometricToTwoD(camera.Position).ToPoint()) + 4, 0, 1);
                }

                mCurrentEffectInstance.IsLooped = looped;
                mCurrentEffectInstance.Pan = pan;

                var keyValuePair = new KeyValuePair<int, SoundEffectInstance>(sLoopingSoundEffectInstances.Count + 1, mCurrentEffectInstance);

                if (looped && mSoundEffectOn)
                {
                    sLoopingSoundEffectInstances.TryAdd(keyValuePair.Key, keyValuePair.Value);
                }

                mSoundEffectOn = true;
                mCurrentEffectInstance.Play();
            }
            // return keyValuePair;
        }

        private float Distance(Point source, Point camera)
        {
            return (float)Math.Sqrt(Math.Pow(source.X - camera.X, 2) + Math.Pow(source.Y - camera.Y, 2));
        }

        /*
        /// <summary>
        /// Ends a looped sound effect
        /// </summary>
        /// <param name="id">id of the sound effect loop, if id is not given or -1, all sound effects are stopped.</param>
        internal void EndEffect(int id = -1)
        {
            if (id == -1)
            {
                foreach (var effect in sLoopingSoundEffectInstances)
                {
                    effect.Value.Stop();
                }
                return;
            }

            sLoopingSoundEffectInstances[id].Stop();
        }
        */

        internal void PlayMusic(string assetName, bool looped = true)
        {
            PlayingMusic = sAcousticEffects[assetName].CreateInstance();
            PlayingMusic.Volume = VolumeMusic * VolumeMaster;
            PlayingMusic.IsLooped = looped;
            PlayingMusic.Play();
        }

        internal void StopSoundEffects()
        {
            mSoundEffectOn = false;
            if (mCurrentEffectInstance != null)
            {
                //mCurrentEffectInstance.Stop(true);
            }
        }
    }
}
