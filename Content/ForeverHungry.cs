using ThoriumMod;
using ThoriumMod.Empowerments;
using ThoriumMod.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Sounds;
using ThoriumMod.Projectiles.Bard;
using System;
using Terraria.GameContent.ItemDropRules;

namespace BardCompat.Content;

[ExtendsFromMod("ThoriumMod")]
public class ForeverHungry : BardItem {
	public override BardInstrumentType InstrumentType => BardInstrumentType.String;

	public override void SetStaticDefaults() {
		Empowerments.AddInfo<DamageReduction>(3);
		Empowerments.AddInfo<Defense>(3);
	}

	public override void SetBardDefaults() {
		Item.Size = new Vector2(44, 48);
		Item.value = Item.sellPrice(gold: 3);
		
		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.autoReuse = true;
		Item.useStyle = ItemUseStyleID.Guitar;

		Item.damage = 10;
		Item.knockBack = 4f;
		Item.noMelee = true;

		Item.UseSound = ThoriumSounds.String_Sound;
		Item.rare = ItemRarityID.LightRed;

		Item.shoot = ModContent.ProjectileType<BouncingHungry>();
		Item.shootSpeed = 20f;

		InspirationCost = 1;
	}
}

[ExtendsFromMod("ThoriumMod")]
public class BouncingHungry : BardProjectile {
	public override string Texture => $"Terraria/Images/NPC_{NPCID.TheHungry}";

	public override BardInstrumentType InstrumentType => BardInstrumentType.Wind;

	public float BounceDampenX = .8f;
	public float BounceDampenY = .8f;
	public int TileBounces = 2;

	public override void SetBardDefaults() {
		Main.projFrames[Type] = 3;

		Projectile.Size = new Vector2(32, 32);
		Projectile.timeLeft = 1000;
		Projectile.penetrate = 5;

		Projectile.friendly = true;
	}

	public override void AI() {
		Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI;
		Projectile.velocity.Y += .8f;
	}

	public override bool OnTileCollide(Vector2 oldVelocity) {
		TileBounces--;
		for (int i = 0; i < 5; i++) {
			Dust.NewDust(Projectile.Center, 10, 10, DustID.BloodWater, Scale: 1.5f);
			Dust.NewDust(Projectile.Center, 10, 10, DustID.Blood, Scale: 1.3f);
		}

		if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
			Projectile.velocity.X = -oldVelocity.X * BounceDampenX;
		}
		
		if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
			Projectile.velocity.Y = -oldVelocity.Y * BounceDampenY;
		}
		
		return TileBounces < 0;
	}
}

[ExtendsFromMod("ThoriumMod")]
public class WoFDrop : GlobalNPC {
	public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.WallofFlesh;
	
	public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ForeverHungry>(), chanceDenominator: 1));
	}
}