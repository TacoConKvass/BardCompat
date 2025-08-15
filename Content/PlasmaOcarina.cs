using CalamityMod;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Empowerments;
using ThoriumMod.Items;

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
		Item.Size = new Vector2(38, 31);
		Item.value = Item.sellPrice(gold: 3);

		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.autoReuse = true;
		Item.useStyle = ItemUseStyleID.Shoot;

		Item.damage = 10;
		Item.knockBack = 4f;
		Item.noMelee = true;

		Item.UseSound = SoundID.Item42;
		Item.rare = ItemRarityID.LightRed;

		Item.shoot = ModContent.ProjectileType<PulseRifleShot>();
		Item.shootSpeed = 10f;

		InspirationCost = 0;
	}

	int channelValue = 0;
	int notUsedTimer = 0;

	public override bool AltFunctionUse(Player player) => true;

	public override void BardHoldItem(Player player) {
		if (notUsedTimer > 0) notUsedTimer--;
		if (channelValue > 0 && notUsedTimer <= 0) channelValue = 0;
	}

	public override bool? BardUseItem(Player player) {
		if (player.altFunctionUse == 2) {
			notUsedTimer = 10;
			channelValue++;
		}

		return player.altFunctionUse == 2 ? channelValue > 180 : null;
	}

	public override bool BardShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
		if (channelValue > 180) {
			Projectile projectile2 = Projectile.NewProjectileDirect(source, player.Center + (HoldoutOffset() ?? Vector2.Zero), Vector2.Zero, ModContent.ProjectileType<WavePounderBoom>(), damage, knockback);
			float j = 10;
			projectile2.ai[1] = Main.rand.NextFloat(320f, 870f) + j * 45f;
			projectile2.localAI[1] = Main.rand.NextFloat(0.08f, 0.25f);
			projectile2.Opacity = MathHelper.Lerp(0.18f, 0.6f, j / 7f) + Main.rand.NextFloat(-0.08f, 0.08f);
			projectile2.Calamity().stealthStrike = true;
			projectile2.DamageType = Item.DamageType;
			projectile2.netUpdate = true;
			channelValue = 0;
			return false;
		}

		if (player.altFunctionUse != 2) {
			Projectile shot1 = Projectile.NewProjectileDirect(source, position, velocity.RotatedBy(-.2f), type, damage, knockback);
			shot1.DamageType = Item.DamageType;
			shot1.netUpdate = true;
			Projectile shot2 = Projectile.NewProjectileDirect(source, position, velocity.RotatedBy(.2f), type, damage, knockback);
			shot2.DamageType = Item.DamageType;
			shot2.netUpdate = true;
		}
		return false;
	}
}