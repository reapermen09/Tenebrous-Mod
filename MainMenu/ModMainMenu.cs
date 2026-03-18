using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

public class MainMenuBackground : ModSurfaceBackgroundStyle
{
    public override void ModifyFarFades(float[] fades, float transitionSpeed) { }

    public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
    {
        Texture2D bg = ModContent.Request<Texture2D>(
            $"{nameof(TerrariaTenebrous)}/MainMenu/Background",
            AssetRequestMode.ImmediateLoad
        ).Value;

        spriteBatch.Draw(
            bg,
            new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
            Color.White
        );


        return true;
    }
}

public class ModMainMenu : ModMenu
{
    public override string DisplayName => "Tenebrous Mod";

    public override int Music =>
        MusicLoader.GetMusicSlot(Mod, "Assets/Music/TitleTheme");

    public override Asset<Texture2D> Logo =>
        ModContent.Request<Texture2D>(
            $"{nameof(TerrariaTenebrous)}/MainMenu/ModLogo"
        );

    public override ModSurfaceBackgroundStyle MenuBackgroundStyle =>
        ModContent.GetInstance<MainMenuBackground>();

    public override void Load()
    {
        Main.menuBGChangedDay = true;
    }
}
