﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace ExampleMod.Items
{
	public class ExampleInstancedGlobalItem : GlobalItem
	{
		public string originalOwner;
		public byte awesome;

		private static string saveOriginalOwner;

		public ExampleInstancedGlobalItem()
		{
			originalOwner = "";
			awesome = 0;
		}

		public override bool InstancePerEntity
		{
			get
			{
				return true;
			}
		}

		public override GlobalItem Clone(Item item, Item itemClone)
		{
			ExampleInstancedGlobalItem myClone = (ExampleInstancedGlobalItem)base.Clone(item, itemClone);
			myClone.originalOwner = originalOwner;
			myClone.awesome = awesome;
			return myClone;
		}

		public override bool NewPreReforge(Item item)
		{
			saveOriginalOwner = originalOwner;
			return base.NewPreReforge(item);
		}

		public override void PostReforge(Item item)
		{
			originalOwner = saveOriginalOwner;
		}

		public override int ChoosePrefix(Item item, UnifiedRandom rand)
		{
			if (rand.Next(30) == 0)
			{
				return mod.PrefixType(rand.Next(2) == 0 ? "Awesome" : "ReallyAwesome");
			}
			return -1;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (!item.social && item.prefix > 0)
			{
				int awesomeBonus = awesome - Main.cpItem.GetGlobalItem<ExampleInstancedGlobalItem>().awesome;
				if (awesomeBonus > 0)
				{
					TooltipLine line = new TooltipLine(mod, "PrefixAwesome", "+" + awesomeBonus + " awesomeness");
					line.isModifier = true;
					tooltips.Add(line);
				}
			}
			if (originalOwner.Length > 0)
			{
				TooltipLine line = new TooltipLine(mod, "CraftedBy", "Crafted by: " + originalOwner);
				line.overrideColor = Color.LimeGreen;
				tooltips.Add(line);

				/*foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "ItemName")
					{
						line2.text = originalOwner + "'s " + line2.text;
					}
				}*/
			}
		}

		public override void Load(Item item, TagCompound tag)
		{
			originalOwner = tag.GetString("originalOwner");
		}

		public override bool NeedsSaving(Item item)
		{
			return originalOwner.Length > 0;
		}

		public override TagCompound Save(Item item)
		{
			return new TagCompound {
				{"originalOwner", originalOwner}
			};
		}

		public override void OnCraft(Item item, Recipe recipe)
		{
			if (item.maxStack == 1)
				originalOwner = Main.LocalPlayer.name;
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			writer.Write(originalOwner);
			writer.Write(awesome);
		}

		public override void NetReceive(Item item, BinaryReader reader)
		{
			originalOwner = reader.ReadString();
			awesome = reader.ReadByte();
		}
	}
}