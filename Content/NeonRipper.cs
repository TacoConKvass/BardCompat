using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Projectiles.Healer;
using ThoriumMod.Projectiles.Scythe;

namespace BardCompat.Content;

[ExtendsFromMod("ThoriumMod")]
public class NeonRipper : ScytheItem {
	public override void SetDefaults() {
		SetDefaultsToScythe();
		Item.shoot = ModContent.ProjectileType<NeonRipperProjectile>();
		scytheSoulCharge = 5;
		Item.damage = 11;
		Item.rare = ItemRarityID.Blue; // ?
	}
}

[ExtendsFromMod("ThoriumMod")]
public class NeonRipperProjectile : ScythePro {
	public override void SafeSetDefaults() {
		dustType = DustID.Shadowflame;
		dustCount = 4;
		scytheCount = 1;
		Projectile.Size = new Vector2(186, 200);
	}

	public override void SafeAI() {
		Projectile.ai[0]++;
		if (Projectile.ai[0] > 5) {
			Projectile.ai[0] = 0;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 8, ModContent.ProjectileType<Nanodroid>(), Projectile.damage, 0);
		}
	}
}

[ExtendsFromMod("ThoriumMod")]
public class Nanodroid : HomingPro {
	public override bool HomingOnPlayer { get; } = false;

	public override bool Slowdown => false;

	public override float Speed => 8f;

	public override void SetDefaults() {
		Projectile.Size = new Vector2(16, 10);
		Projectile.timeLeft = 1000;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = 6;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 30; // Can only attack 60 / value times a second
		Projectile.ignoreWater = true;
	}

	public override void SafeAI() {
		if (Projectile.ai[0] != -1) Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center;
	}

	public override void OnSpawn(IEntitySource source) {
		Projectile.ai[0] = -1;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
		Projectile.ai[0] = target.whoAmI;
	}

	public override bool OnTileCollide(Vector2 oldVelocity) {
		if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
			Projectile.velocity.X = -oldVelocity.X;
		}

		if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
			Projectile.velocity.Y = -oldVelocity.Y;
		}
		return false;
	}
}