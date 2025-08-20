using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Empowerments;
using ThoriumMod.Items;
using ThoriumMod.Projectiles.Bard;
using ThoriumMod.Sounds;

namespace BardCompat.Content;

[ExtendsFromMod("ThoriumMod")]
public class SandSlasher : BardItem {
	public override BardInstrumentType InstrumentType => BardInstrumentType.String;

	public override void SetStaticDefaults() {
		Empowerments.AddInfo<DamageReduction>(1);
		Empowerments.AddInfo<Defense>(1);
	}

	public override void SetBardDefaults() {
		Item.Size = new Vector2(62, 62);
		Item.value = Item.sellPrice(gold: 3);

		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.autoReuse = true;
		Item.useStyle = ItemUseStyleID.Guitar;

		Item.damage = 10;
		Item.knockBack = 4f;
		Item.noMelee = true;

		Item.UseSound = ThoriumSounds.String_Sound;
		Item.rare = ItemRarityID.LightRed; // ?

		Item.shoot = ModContent.ProjectileType<SandSlasher_Projectile>();
		Item.shootSpeed = 20f;

		InspirationCost = 1;
	}
}

[ExtendsFromMod("ThoriumMod")]
public class SandSlasher_Projectile : BardProjectile {
	public override BardInstrumentType InstrumentType => BardInstrumentType.String;

	public override void SetBardDefaults() {
		Main.projFrames[Type] = 3;

		Projectile.Size = new Vector2(50, 50);
		Projectile.timeLeft = 600; // 10 seconds
		Projectile.penetrate = 5; // ?

		Projectile.friendly = true;

	}

	public override void AI() {
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
	}

	public override bool OnTileCollide(Vector2 oldVelocity) {
		if (Main.myPlayer == Projectile.owner){
			Projectile tornado = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileID.SandnadoFriendly, Projectile.damage, Projectile.knockBack);
			tornado.timeLeft = 120;
			tornado.netUpdate = true;
		}

		if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
			Projectile.velocity.X = -oldVelocity.X;
		}

		if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
			Projectile.velocity.Y = -oldVelocity.Y;
		}

		return false;
	}
}