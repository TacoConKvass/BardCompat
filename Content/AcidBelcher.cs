using Microsoft.Xna.Framework;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Empowerments;
using ThoriumMod.Items;
using ThoriumMod.Sounds;
using Terraria.DataStructures;

namespace BardCompat.Content;

[ExtendsFromMod("ThoriumMod", "CalamityMod")]
public class AcidBelcher : BardItem {
	public override BardInstrumentType InstrumentType => BardInstrumentType.Brass;

	public override void SetStaticDefaults() {
		Empowerments.AddInfo<CriticalStrikeChance>(1);
		Empowerments.AddInfo<FlatDamage>(1);
	}

	public override void SetBardDefaults() {
		Item.Size = new Vector2(44, 48);
		Item.value = Item.sellPrice(gold: 3);

		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.autoReuse = true;
		Item.useStyle = ItemUseStyleID.Shoot;

		Item.damage = 10;
		Item.knockBack = 4f;
		Item.noMelee = true;

		Item.UseSound = ThoriumSounds.Trombone_Sound;
		Item.rare = ItemRarityID.LightRed;

		Item.shoot = ModContent.ProjectileType<AcidBarrelDrop>();
		Item.shootSpeed = 8f;

		InspirationCost = 1;
	}

	public override bool BardShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
		for (int i = 0; i < 3; i++) {
			Projectile projectile = Projectile.NewProjectileDirect(source, position + velocity * 8, velocity.RotatedBy(Main.rand.NextFloat(-.3f, .3f)), type, damage, knockback, player.whoAmI);
			projectile.extraUpdates = 1;
			projectile.timeLeft = 300;
		}
		return false;
	}

	public override Vector2? HoldoutOffset() {
		return new Vector2(-8, 0);
	}
}

