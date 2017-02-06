//--- Aura Script -----------------------------------------------------------
// Smith of Awesome
//--- Description -----------------------------------------------------------
// Enchant effects experiment.
// https://www.youtube.com/watch?v=vc1UMd2_79Q
//---------------------------------------------------------------------------

using Aura.Mabi.Const;
using Aura.Mabi.Structs;

public class SmithOfAwesomeScript : NpcScript
{
	public override void Load()
	{
		SetRace(10002);
		SetName("Smith of Awesome");
		SetBody(height: 0f, weight: 1.4f, upper: 2f, lower: 1.4f);
		SetFace(skinColor: 25, eyeType: 9, eyeColor: 38, mouthType: 2);
		SetLocation(1, 13282, 39588, 132);

		EquipItem(Pocket.Face, 4910, 0x00F99D8B, 0x00F9E0EC, 0x009A1561);
		EquipItem(Pocket.Hair, 4024, 0x000000, 0x000000, 0x000000);
		EquipItem(Pocket.Armor, 15039, 0x009B5033, 0x009A835F, 0x00321007);
		EquipItem(Pocket.Glove, 16510, 0x00EABE7D, 0x00808080, 0x0057685E);
		EquipItem(Pocket.Shoe, 17504, 0x002B1C09, 0x00857756, 0x00321007);
		EquipItem(Pocket.RightHand1, 40024, 0x000000, 0x004F3C26, 0x00FAB052);
	}

	protected override async Task Talk()
	{
		Msg("A... a customer? Uhm, okay, what... what can I do for you?<br/>Sorry, I'm new to this.", Button("Upgrade item", "@upgrade"), Button("End conversation", "@end"));

		if (await Select() == "@upgrade")
		{
			Msg("<selectitem stringid='*/equip/*' />");
			var select = await Select();

			if (select != "@cancel")
			{
				var entityId = Convert.ToInt64(select.Substring("@select:".Length));
				var item = Player.Inventory.GetItem(entityId);
				if (item != null)
				{
					Msg("This one...? Of course, uhm, which upgrade would you like?");
					Msg(List("Select Upgrade", Button("500 STR", "@str"), Button("5 INT", "@int"), Button("5 DEX", "@dex"), Button("5 Will", "@will"), Button("5 Luck", "@luck")));

					UpgradeStat stat;
					short value = 5;
					switch (await Select())
					{
						default:
						case "@str": stat = UpgradeStat.STR; value = 500; break;
						case "@int": stat = UpgradeStat.Intelligence; break;
						case "@dex": stat = UpgradeStat.Dexterity; break;
						case "@will": stat = UpgradeStat.Will; break;
						case "@luck": stat = UpgradeStat.Luck; break;
					}

					var effect = new UpgradeEffect(UpgradeType.ItemAttribute);
					effect.SetStatEffect(stat, value, UpgradeValueType.Value);

					if (stat == UpgradeStat.STR)
						Msg("Heh, that's my favorite upgrade as well.<br/>Let's see.", Button("Continue"));
					else
						Msg("Eh...? But... nevermind, you surely know what you're doing!<br/>Let's see.", Button("Continue"));

					await Select();

					for (int i = 0; i < 3; ++i)
					{
						Task.Delay(i * 500).ContinueWith(_ => Send.PlaySound(Player, "data/sound/blacksmith_click_best.wav"));
						Task.Delay(i * 500 + 250).ContinueWith(_ => Send.PlaySound(Player, "data/sound/blacksmith_click_normal.wav"));
						Task.Delay(i * 500 + 350).ContinueWith(_ => Send.PlaySound(Player, "data/sound/blacksmith_click_normal.wav"));
					}
					Task.Delay(1750).ContinueWith(_ => Send.PlaySound(Player, "data/sound/emotion_fail.wav"));
					Msg("...<autopass duration='500'/><p/>... ...<autopass duration='500'/><p/>... ... ...<autopass duration='750'/><p/>Uh-oh...<br/>Uhm, could you wait a second?", Button("..."));
					await Select();
					for (int i = 0; i < 10; ++i)
						Task.Delay(i * 250).ContinueWith(_ => Send.PlaySound(Player, "data/sound/blacksmith_click_best.wav"));
					Task.Delay(3000).ContinueWith(_ => Send.PlaySound(Player, "data/sound/emotion_success.wav"));
					Msg("...<autopass duration='3000'/><p/>There it is, good as new, and with your upgrade!<br/>Have a good day!");

					item.AddUpgradeEffect(effect);
					Send.ItemUpdate(Player, item);

					End();
				}
			}
		}

		Close(Hide.None, "(phew) See you.");
	}
}
