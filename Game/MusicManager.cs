using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace Splatoon2D
{
    public static class MusicManager
    {
        public static Song current, normal, fight;

        public static void LoadContent(ContentManager Content)
        {
            normal = Content.Load<Song>("song_normal");
            fight = Content.Load<Song>("song_fight");

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.8f;
        }

        public static void Update(World world, Player player)
        {
            bool seen = false;
            foreach(PhysicalObject o in world.Stuff)
            {
                if(o is Frogtarian f)
                {
                    if (f.see_player) seen = true;
                }
                if (o is BigFrogarian bf)
                {
                    if (bf.player_seen) seen = true;
                }
            }

            if (seen)
            {
                if (current != fight)
                {
                    MediaPlayer.Play(fight, MediaPlayer.PlayPosition);
                    current = fight;
                }
            }
            else
            {
                if (current != normal)
                {
                    MediaPlayer.Play(normal, MediaPlayer.PlayPosition);
                    current = normal;
                }
            }

            if(player.is_on_ink && player.CurrentForm == Player.PlayerForm.squid)
            {
                if (MediaPlayer.Volume > 0.2f) MediaPlayer.Volume -= 0.01f;
            }
            else
            {   
                if (MediaPlayer.Volume < 0.7f) MediaPlayer.Volume += 0.01f;
            }
        }
    }
}
