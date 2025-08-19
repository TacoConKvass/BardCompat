using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Empowerments;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Sounds;
using ThoriumMod.Projectiles.Bard;
using CalamityMod.Projectiles.Boss;
using Terraria.DataStructures;
using CalamityMod.NPCs.Providence;
using System;
using CalamityMod.Items.Potions.Alcohol;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using System.IO;

namespace BardCompat.Content;

[ExtendsFromMod("ThoriumMod", "CalamityMod")]
public class Infrariff : BardItem {
	public override BardInstrumentType InstrumentType => BardInstrumentType.String;

	public override void SetStaticDefaults() {
		Empowerments.AddInfo<DamageReduction>(1);
		Empowerments.AddInfo<Defense>(1);
	}

	public override void SetBardDefaults() {
		Item.Size = new Vector2(82, 94);
		Item.value = Item.sellPrice(gold: 3);

		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.reuseDelay = 30;
		Item.autoReuse = true;
		Item.useStyle = ItemUseStyleID.Guitar;

		Item.damage = 10;
		Item.knockBack = 4f;
		Item.noMelee = true;

		Item.UseSound = ThoriumSounds.String_Sound;
		Item.rare = ItemRarityID.LightRed; // ?

		Item.shoot = ModContent.ProjectileType<InfrariffProjectile>();
		Item.shootSpeed = 1f;

		InspirationCost = 1;
	}

	public override bool BardShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
		float angle = (player.Center - Main.screenPosition).AngleTo(Main.MouseScreen);

		Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, ai0: angle - MathHelper.ToRadians(45f), ai1: 1);
		Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, ai0: angle + MathHelper.ToRadians(45f), ai1: -1);

		return false;
	}
}

[ExtendsFromMod("ThoriumMod")]
public class InfrariffProjectile : BardProjectile {
	public override BardInstrumentType InstrumentType => BardInstrumentType.String;

	public override string Texture => "CalamityMod/Projectiles/Boss/ProvidenceHolyRay";

	public override void SetBardDefaults() {
		Projectile.Size = new Vector2(48, 48);
		Projectile.timeLeft = 50;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
	}

	// Code below adapted/lifted from Calamity

	public override void SendExtraAI(BinaryWriter writer) {
		writer.Write(Projectile.localAI[1]);
	}

	public override void ReceiveExtraAI(BinaryReader reader) {
		Projectile.localAI[1] = reader.ReadSingle();
	}


	public override void AI() {
		if (Projectile.timeLeft > 5) Projectile.ai[0] += MathHelper.ToRadians(1 * Projectile.ai[1]);
		else if (Projectile.timeLeft == 1) {
			for (int i = 0; i < 7; i++) {
				Vector2 position = Projectile.Center + Projectile.velocity * (i + 1) * 6 * 16;
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, Vector2.Zero, ProjectileID.Flames, Projectile.damage, Projectile.knockBack, ai0: 5, ai1: 5, ai2: 5);
			}
		}
		Projectile.Center = Main.player[Projectile.owner].Center + Vector2.UnitX.RotatedBy(Projectile.ai[0]) * 8;
		Projectile.velocity = Projectile.Center.DirectionFrom(Main.player[Projectile.owner].Center);
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

		float[] array = new float[3];
		Collision.LaserScan(Projectile.Center, Projectile.velocity, Projectile.scale, 800f, array);
		float num4 = 0f;
		for (int i = 0; i < array.Length; i++) {
			num4 += array[i];
		}

		num4 /= 3f;

		Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num4, 0.5f);
	}

	Texture2D texture2D => TextureAssets.Projectile[Type].Value;
	Texture2D texture2D2 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ProvidenceHolyRayMid", AssetRequestMode.ImmediateLoad).Value;
	Texture2D texture2D3 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ProvidenceHolyRayEnd", AssetRequestMode.ImmediateLoad).Value;

	public override bool PreDraw(ref Color lightColor) {
		if (Projectile.velocity == Vector2.Zero) {
			return false;
		}

		float num2 = Projectile.localAI[1];
		Color color = Projectile.ai[1] > 0 ? new Color(255, 69, 0, 255) : new Color(0, 0, 255, 255);
		Vector2 position = Projectile.Center - Main.screenPosition;
		num2 -= (texture2D.Height / 2 + texture2D3.Height) * Projectile.scale;
		Vector2 center = Projectile.Center;
		center += Projectile.velocity * Projectile.scale * texture2D.Height / 2f;
		if (num2 > 0f) {
			float num3 = 0f;
			Rectangle value = new Rectangle(0, 36 * (Projectile.timeLeft / 3 % 4), texture2D2.Width, 36);
			while (num3 + 1f < num2) {
				Main.spriteBatch.Draw(texture2D2, center - Main.screenPosition, value, color, Projectile.rotation, new Vector2(value.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0f);
				num3 += value.Height * Projectile.scale;
				center += Projectile.velocity * value.Height * Projectile.scale;
				value.Y += 36;
				if (value.Y + value.Height > texture2D2.Height) {
					value.Y = 0;
				}
			}
		}

		Vector2 position2 = center - Main.screenPosition;
		Main.spriteBatch.Draw(texture2D3, position2, null, color, Projectile.rotation + MathHelper.Pi, texture2D3.Frame().Center() + (Vector2.UnitY * 24), Projectile.scale, SpriteEffects.None, 0f);
		return false;
	}
}
