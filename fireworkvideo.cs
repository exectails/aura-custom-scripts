//--- Aura Script -----------------------------------------------------------
// Firework Video
//--- Description -----------------------------------------------------------
// Happy Holidays video 2015.
// https://www.youtube.com/watch?v=WT68msZj6js
//---------------------------------------------------------------------------

using Aura.Channel.Skills.Life;
using Aura.Channel.World.Weather;

public class FireworkVideoScript : GeneralScript
{
	public override void Load()
	{
		AddCommand(99, 99, "fireworkvideo", "", HandleFireworkVideo);
	}

	private CommandResult HandleFireworkVideo(ChannelClient client, Creature sender, Creature target, string message, IList<string> args)
	{
		var rnd = RandomProvider.Get();
		var region = ChannelServer.Instance.World.GetRegion(52);
		var radius = 2000;
		var center = new Position(35400, 34400);
		var amount = 100;
		var pos1 = new Position(38995, 38290);
		var pos2 = new Position(38370, 37683);

		sender.Jump(pos1.X, pos1.Y);

		Task.Delay(800).ContinueWith(_ =>
		{
			Send.UseMotion(sender, 11, 3);
		});

		// 1, rain
		Task.Delay(5000).ContinueWith(_ =>
		{
			ChannelServer.Instance.Weather.SetProviderAndUpdate(target.RegionId, new WeatherProviderConstant(52, 1.95f));

			// "Hm?"
			Task.Delay(1000).ContinueWith(__ =>
			{
				Send.Chat(sender, "Hm? Snow...?");
			});
		});

		// 2, equip+music
		Task.Delay(10000).ContinueWith(_ =>
		{
			// Equip
			var item = new Item(13158);
			item.Info.Pocket = Pocket.Armor;
			Send.EquipmentMoved(sender, item.Info.Pocket);
			Send.EquipmentChanged(sender, item);

			item = new Item(18122);
			item.Info.Pocket = Pocket.Head;
			Send.EquipmentMoved(sender, item.Info.Pocket);
			Send.EquipmentChanged(sender, item);

			item = new Item(17368);
			item.Info.Pocket = Pocket.Shoe;
			Send.EquipmentMoved(sender, item.Info.Pocket);
			Send.EquipmentChanged(sender, item);

			Send.SetBgm(sender, "17_One_Fine_Day.mp3", BgmRepeat.Once);

			// "Wuah!"
			Task.Delay(500).ContinueWith(__ =>
			{
				Send.UseMotion(sender, 53, 7);
				Send.Chat(sender, "Wuah! What is going on here!?");
			});
		});

		// 3, queue firework
		Task.Delay(12000).ContinueWith(_ =>
		{
			Send.ForceWalkTo(sender, pos2.GetRelative(pos1, -50));

			// "..."
			Task.Delay(2000).ContinueWith(__ =>
			{
				Send.Chat(sender, "Huh.");
			});

			// Go
			Task.Delay(3000).ContinueWith(__ =>
			{
				Send.ForceWalkTo(sender, pos2);
			});

			// Site
			Task.Delay(6000).ContinueWith(__ =>
			{
				Send.SitDown(sender);
			});

			// Firework
			for (int i = 0; i < amount; ++i)
			{
				var delay = 500 + (rnd.Next(500, 1500) * i);

				// 0=normal
				// 1=heart
				// 2=coke
				// 3=nao
				// 4=devcat
				// 5=icespear
				// 6=cake
				// 7=stars1
				// 8=stars2
				// 9=stars3
				// 10=dove
				// 11=cake

				var type = 2;
				var rndd = rnd.NextDouble();
				if (rndd < 0.12) type = 0;
				else if (rndd < 0.24) type = 5;
				else if (rndd < 0.36) type = 7;

				var prop = new Prop(208, 52, center.X + rnd.Next(-radius, radius), center.Y + rnd.Next(-radius, radius), 0);
				prop.DisappearTime = DateTime.Now.AddSeconds(20 + delay);
				region.AddProp(prop);

				Task.Delay(delay).ContinueWith(__ =>
				{
					prop.Xml.SetAttributeValue("height", rnd.Between(750, 2500));
					prop.Xml.SetAttributeValue("message", "");
					prop.Xml.SetAttributeValue("type", type);
					prop.Xml.SetAttributeValue("seed", rnd.Next());
					Send.PropUpdate(prop);
				});
			}
		});

		return CommandResult.Okay;
	}
}
