using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TerrariaTenebrous.Content.Items.Materials.Starlight;

namespace TerrariaTenebrous.Content.Items.Armor.StarSet
{
	[AutoloadEquip(EquipType.Body)]
	public class StarlightPajamas : ModItem
	{
		public override void SetDefaults() {
			Item.width = 24;
			Item.height = 24;
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.Blue;
			Item.defense = 4;
		}

		public override void UpdateEquip(Player player) {
			player.maxMinions += 1;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<StarlightBar>(), 22)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

	[AutoloadEquip(EquipType.Head)]
	public class StarlightTopHat : ModItem
	{
		public static readonly int SetBonusMinionIncrease = 1;

		public static LocalizedText SetBonusText { get; private set; }

		public override void SetStaticDefaults() {
			SetBonusText = this.GetLocalization("SetBonus")
				.WithFormatArgs(SetBonusMinionIncrease);
		}

		public override void SetDefaults() {
			Item.width = 26;
			Item.height = 20;
			Item.value = Item.sellPrice(gold: 1, silver: 50);
			Item.rare = ItemRarityID.Blue;
			Item.defense = 3;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<StarlightPajamas>()
				&& legs.type == ModContent.ItemType<StarlightSlides>();
		}

		public override void UpdateEquip(Player player) {
			player.maxMinions += 1;
			player.statManaMax2 += 40;
		}

		public override void UpdateArmorSet(Player player) {
			player.maxMinions += SetBonusMinionIncrease;

			player.GetModPlayer<TenebrousPlayer>().noManaCostForStarSet = true;

			player.setBonus = SetBonusText.Value;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<StarlightBar>(), 17)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

	[AutoloadEquip(EquipType.Legs)]
	public class StarlightSlides : ModItem
	{
		public override void SetDefaults() {
			Item.width = 30;
			Item.height = 12;
			Item.value = Item.sellPrice(gold: 1, silver: 50);
			Item.rare = ItemRarityID.Blue;
			Item.defense = 3;
		}

		public override void UpdateEquip(Player player) {
			player.moveSpeed += 0.1f;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<StarlightBar>(), 17)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}