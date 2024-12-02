using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Audio;

namespace SpatialSound;

internal class ModEntry : Mod {

    ICue sound = null!;
    bool playing = false;
    Vector2 pos = new();

    public override void Entry(IModHelper helper) {
        helper.Events.Player.Warped += OnWarped;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        //helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
    }

    //private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e) {
    //    Game1.playSound("portalRadio", out sound);
    //    sound.Pause();
    //}

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e) {
        if (!Context.IsWorldReady) return;
        // adjust volume on all spatial sounds based on distance
        if (playing && e.IsMultipleOf(10)) {
            Vector2 farmerPosition = Game1.player.getStandingPosition();
            float distance = Vector2.Distance(pos, farmerPosition);
            if (distance <= 1536f) {
                float volume = Math.Max(0f, 1f - 1f / 1536f * distance);
                sound.Volume = volume * Math.Min(Game1.ambientPlayerVolume, Game1.options.ambientVolumeLevel);
                if (sound.IsPaused) sound.Resume();
            } else {
                sound.Volume = -1f;
            }
        }
    }

    private void OnWarped(object? sender, WarpedEventArgs e) {
        // clear spatial sounds
        // check Stardew decompile for any other reset actions
        // check map properties for spatial sounds to add
        if (e.NewLocation == Game1.getLocationFromName("BusStop")) {
            pos = new Vector2(24, 24) * 64f;
            playing = true;
        } else {
            sound?.Pause();
            playing = false;
        }
    }

}