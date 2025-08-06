using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Projectiles.Healer;

namespace BardCompat.Content;

[ExtendsFromMod("ThoriumMod")]
public class BottleOfSouls : ThoriumItem
{
	public override void SetDefaults() {
		Item.Size = new Vector2(18, 36);
		Item.value = Item.sellPrice(0, 0, 50);

		Item.DamageType = ThoriumDamageBase<HealerTool>.Instance;
		Item.noMelee = true;
		Item.mana = 25;

		Item.useTime = 2;
		Item.useAnimation = 10;
		Item.useStyle = ItemUseStyleID.HoldUp;
		Item.autoReuse = true;

		Item.rare = ItemRarityID.Blue; // ?
		Item.UseSound = SoundID.Item24; // ?

		Item.shoot = ProjectileID.PurificationPowder;
		Item.shootSpeed = 0f;

		isHealer = true;
	}

	public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
		type = Main.rand.NextBool() ? ModContent.ProjectileType<DamagingSoul>() : ModContent.ProjectileType<HealingSoul>();
		velocity = new Vector2(0, -10).RotatedBy(Main.rand.NextFloat(-.3f, .3f));
		damage = 1;
	}
}

[ExtendsFromMod("ThoriumMod")]
class DamagingSoul : HomingPro {
	/// <summary>
	/// The debuff duration in ticks
	/// </summary>
	public int ShadowflameDuration = 60;

	public override bool HomingOnPlayer { get; } = false;

	public override float Speed => 7.5f;

	public override void SetDefaults() {
		Projectile.Size = new Vector2(24, 30);
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 1000;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
		Projectile.tileCollide = false;
	}

	public override void SafeAI() {
		Projectile.rotation = Projectile.velocity.ToRotation() - rotationOffset;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
		target.AddBuff(BuffID.ShadowFlame, ShadowflameDuration);
	}
}

[ExtendsFromMod("ThoriumMod")]
class HealingSoul : HomingPro {
	public override bool HomingOnPlayer { get; } = true;

	public override float Speed => 7.5f;

	public override void SetDefaults() {
		Projectile.Size = new Vector2(24, 30);
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 1000;
		Projectile.hostile = true;
		Projectile.tileCollide = false;
	}

	public override void SafeAI() {
		Projectile.rotation = Projectile.velocity.ToRotation() - rotationOffset;
	}

	public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) {
		modifiers.Cancel();
		if (target.whoAmI == Projectile.owner && Projectile.timeLeft > 1000 - 60) return;
		target.Heal(3);
		Projectile.Kill();
	}

	public override bool? CanHitNPC(NPC target) => false;
}