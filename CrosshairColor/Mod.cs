using Centrifuge.GTTOD.Internal;
using Reactor.API.Attributes;
using Reactor.API.Configuration;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace CrosshairColor
{
    [ModEntryPoint(ModID, AwakeAfterInitialize = true)]
    public class Mod : MonoBehaviour
    {
        public const string ModID = "com.github.ciastex/CrosshairColor";

        private const string CrosshairColorRedSettingsKey = "CrosshairColorR";
        private const string CrosshairColorGreenSettingsKey = "CrosshairColorG";
        private const string CrosshairColorBlueSettingsKey = "CrosshairColorB";

        internal Settings Settings { get; private set; }
        internal Log Log { get; private set; }

        private Color ReticleColor { get; set; }

        public void Initialize(IManager manager)
        {
            InitSettings();
            Terminal.InitFinished += Terminal_InitFinished;
            Log = new Log("test");
        }

        private void Terminal_InitFinished(object sender, System.EventArgs e)
        {
            CommandTerminal.Terminal.Shell.AddCommand("reticle_rgb", (args) =>
            {
                int.TryParse(args[0].String, out int r);
                int.TryParse(args[1].String, out int g);
                int.TryParse(args[2].String, out int b);

                if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
                {
                    CommandTerminal.Terminal.Log("Invalid range. RGB must be in range of 0-255.");
                    return;
                }

                Settings[CrosshairColorRedSettingsKey] = r;
                Settings[CrosshairColorGreenSettingsKey] = g;
                Settings[CrosshairColorBlueSettingsKey] = b;
                Settings.Save();

                RebuildReticleColor();
            }, 3, 3, "Set your reticle color using RGB values.");
        }

        public void Update()
        {
            var topRight = GameObject.Find("/Game/Player/HUD/HUD Elements/Crosshair/Reticle/Crosshair/TopRight");
            var topLeft = GameObject.Find("/Game/Player/HUD/HUD Elements/Crosshair/Reticle/Crosshair/TopLeft");
            var bottomLeft = GameObject.Find("/Game/Player/HUD/HUD Elements/Crosshair/Reticle/Crosshair/BottomLeft");
            var bottomRight = GameObject.Find("/Game/Player/HUD/HUD Elements/Crosshair/Reticle/Crosshair/BottomRight");

            SetReticlePartColors(topRight, topLeft, bottomLeft, bottomRight);
        }

        private void SetReticlePartColors(params GameObject[] reticleParts)
        {
            foreach (var reticlePart in reticleParts)
            {
                if (reticlePart)
                {
                    var image = reticlePart.GetComponent<Image>();

                    if (image)
                    {
                        image.color = ReticleColor;
                    }
                }
            }
        }

        private void InitSettings()
        {
            Settings = new Settings("color_settings");

            Settings.GetOrCreate(CrosshairColorRedSettingsKey, 0);
            Settings.GetOrCreate(CrosshairColorGreenSettingsKey, 255);
            Settings.GetOrCreate(CrosshairColorBlueSettingsKey, 0);

            RebuildReticleColor();

            if (Settings.Dirty)
                Settings.Save();
        }

        private void RebuildReticleColor()
        {
            ReticleColor = new Color(
                Settings.GetItem<int>(CrosshairColorRedSettingsKey) / (float)255,
                Settings.GetItem<int>(CrosshairColorGreenSettingsKey) / (float)255,
                Settings.GetItem<int>(CrosshairColorBlueSettingsKey) / (float)255
            );
        }
    }
}
