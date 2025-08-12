using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Empowerments;
using ThoriumMod.Items;
using ThoriumMod.Projectiles.Bard;

namespace BardCompat.Content;

[ExtendsFromMod("ThoriumMod", "CalamityMod")]
public class PlasmaOcarina : BardItem {
	public override BardInstrumentType InstrumentType => BardInstrumentType.Wind;

	public override void SetStaticDefaults() {
		Empowerments.AddInfo<Defense>(4);
		Empowerments.AddInfo<Damage>(4);
		Empowerments.AddInfo<DamageReduction>(4);
		Empowerments.AddInfo<CriticalStrikeChance>(3);
		ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
	}

	public override void SetBardDefaults() {
		Item.Size = new Vector2(44, 48);
		Item.value = Item.sellPrice(gold: 3);

		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.autoReuse = true;
		Item.useStyle = ItemUseStyleID.HoldUp;

		Item.damage = 10;
		Item.knockBack = 4f;
		Item.noMelee = true;

		Item.UseSound = SoundID.Item42;
		Item.rare = ItemRarityID.LightRed;

		Item.shoot = ProjectileID.PurificationPowder;
		Item.shootSpeed = 10f;

		InspirationCost = 0;
	}

	public override bool AltFunctionUse(Player player) => true;

	public override bool? BardUseItem(Player player) => null;

	public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) { }

	public override bool BardShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
		return false;
	}
}