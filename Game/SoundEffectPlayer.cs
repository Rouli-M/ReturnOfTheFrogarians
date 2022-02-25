using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Splatoon2D
{
    public static class SoundEffectPlayer
    {
        private static List<SoundEffect> FramePlaylist = new List<SoundEffect>(), SoundPlayedThisFrame;
        private static List<float> FramePlaylistVolume = new List<float>();
        private static List<float> FramePlaylistPitch = new List<float>();
        public static void Play(SoundEffect se, float Volume = 1f, float Pitch = 0f)
        {
            //se.Play(Volume * Save.Instance.SEVolume, Pitch, 0.5f);
            FramePlaylist.Add(se);
            FramePlaylistVolume.Add(Volume);
            FramePlaylistPitch.Add(Pitch);
        }

        public static void PlayInstance(SoundEffectInstance instance)
            // usefull when the caller wants to interrupt the sound
        {
            instance.Play();
        }

        public static void Update()
        {
            SoundPlayedThisFrame = new List<SoundEffect>();
            int i = 0;
            foreach (SoundEffect se in FramePlaylist)
            {
                if (!SoundPlayedThisFrame.Contains(se))
                {
                    se.Play(FramePlaylistVolume[i], FramePlaylistPitch[i], 0.5f);
                    SoundPlayedThisFrame.Add(se);
                }
                i++;
            }
            FramePlaylist = new List<SoundEffect>() { };
            FramePlaylistVolume = new List<float>() { };
            FramePlaylistPitch = new List<float>() { };
        }

        public static void RandomPlay(float Volume = 1f, float Pitch = 1f, params SoundEffect[] soundEffects)
        {
            int i = 0;

            int random = 1;
            while (random != 0)
            {
                random = Player.r.Next(0, soundEffects.Count());
                if (random == 0) soundEffects[i].Play();
            }
        }
    }
}