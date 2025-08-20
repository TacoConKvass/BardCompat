using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Items;
using ThoriumMod.Projectiles;
using System;
using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod;

namespace BardCompat.Content;

[ExtendsFromMod("ThoriumMod", "CalamityMod")]
public class DeepseaTrident : ThoriumItem {
	public override void SetStaticDefaults() {
		ItemID.Sets.SkipsInitialUseSound[Item.type] = true; 
		ItemID.Sets.Spears[Item.type] = true; 
	}

	public override void SetDefaults() {
		Item.Size = new Vector2(78, 78);

		Item.rare = ItemRarityID.Pink; 
		Item.value = Item.sellPrice(silver: 10);

		Item.damage = 10;
		Item.knockBack = 6.5f;
		Item.noUseGraphic = true;
		Item.noMelee = true;

		Item.useStyle = ItemUseStyleID.Shoot; 
		Item.useAnimation = 25; 
		Item.useTime = 25; 
		Item.UseSound = SoundID.Item71; 
		Item.autoReuse = true; 
		
		Item.shootSpeed = 3.7f; 
		Item.shoot = ModContent.ProjectileType<DeepseaTrident_Projectile>();
		isHealer = true;
	}

	public override bool CanUseItem(Player player) {
		return player.ownedProjectileCounts[Item.shoot] < 1;
	}

	public override bool? UseItem(Player player) {
		if (!Main.dedServ && Item.UseSound.HasValue) {
			SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
		}

		return null;
	}
}

[ExtendsFromMod("ThoriumMod", "CalamityMod")]
public class DeepseaTrident_Projectile : ThoriumProjectile {
	public float RangeMax = 120;
	public float RangeMin = 40;

	public override string Texture => "BardCompat/Content/DeepseaTrident"; // replave with your actual path

	public override void SetDefaults() {
		Projectile.CloneDefaults(ProjectileID.Spear);
		RangeMax = 110;
	}

	public override bool PreAI() {
		Player player = Main.player[Projectile.owner]; 
		int duration = player.itemAnimationMax; 
		player.heldProj = Projectile.whoAmI; 

		if (Projectile.timeLeft > duration) {
			Projectile.timeLeft = duration;
		}

		Projectile.velocity = Vector2.Normalize(Projectile.velocity); 

		float halfDuration = duration * 0.5f;
		float progress = Projectile.timeLeft < halfDuration ? Projectile.timeLeft / halfDuration : (duration - Projectile.timeLeft) / halfDuration;

		Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * RangeMin, Projectile.velocity * RangeMax, progress);
		Projectile.rotation = Projectile.velocity.ToRotation();

		if (Projectile.timeLeft == Math.Floor(halfDuration) && Main.myPlayer == Projectile.owner) {
			IEntitySource soource = Projectile.GetSource_FromAI();
			Vector2 position = Projectile.Center - (Projectile.velocity * 4);
			int damage = Projectile.damage;
			float knockback = Projectile.knockBack;
			float spread = MathHelper.ToRadians(30);

			Projectile.NewProjectile(soource, position, Projectile.velocity * 3, ModContent.ProjectileType<ArcherfishShot>(), damage, knockback);
			Projectile bubble1 = Projectile.NewProjectileDirect(soource, position, Projectile.velocity.RotatedBy(spread) * 3, ModContent.ProjectileType<TyphonsGreedBubble>(), damage, knockback, ai1: Main.rand.NextFloat() + 0.5f);
			Projectile bubble2 = Projectile.NewProjectileDirect(soource, position, Projectile.velocity.RotatedBy(-spread) * 3, ModContent.ProjectileType<TyphonsGreedBubble>(), damage, knockback, ai1: Main.rand.NextFloat() + 0.5f);
		}

		return false;
	}

	public override bool PreDraw(ref Color lightColor) {
		Texture2D texture = TextureAssets.Projectile[Type].Value;
		Main.EntitySpriteDraw(texture, (Projectile.Center - (Projectile.velocity * 32)) - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.ToRadians(45), texture.Size() / 2, Projectile.scale, SpriteEffects.None);
		return false;
	}
}