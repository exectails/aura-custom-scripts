//--- Aura Script -----------------------------------------------------------
// Gangnam Concert
//--- Description -----------------------------------------------------------
// Some NPCs playing Gangnam Style on command.
// https://www.youtube.com/watch?v=U9zywnGnYx8
//--- Notes -----------------------------------------------------------------
// This was written for Aura Legacy and is severely outdated!
//---------------------------------------------------------------------------

using System;
using System.Threading;
using Aura.Data;
using Aura.Shared.Const;
using Aura.Shared.Network;
using Aura.Shared.Util;
using Aura.World.Events;
using Aura.World.Network;
using Aura.World.Scripting;
using Aura.World.World;

public class GangnamScript_Base : NPCScript
{
	DateTime start = DateTime.Now;
	int state = -1;

	protected override void Subscribe()
	{
		EventManager.PlayerEvents.PlayerTalks += OnPlayerTalks;
		EventManager.TimeEvents.RealTimeSecondTick += OnRealTimeSecondTick;
	}

	public virtual void OnPlayerTalks(MabiCreature creature, string message)
	{
		if (creature.Region == NPC.Region && message.Contains("start"))
		{
			Start();
			start = DateTime.Now;
			state = 0;
		}
	}

	public void OnRealTimeSecondTick(MabiTime time)
	{
		if (state == 0 && time.DateTime - start >= TimeSpan.FromSeconds(16))
		{
			StartHard();
			state = 1;
		}
		else if (state != -1 && time.DateTime - start >= TimeSpan.FromSeconds(100))
		{
			StopMusic();
			state = -1;
		}
	}

	public virtual void Start() { }
	public virtual void StartHard() { }
	public virtual void StopMusic()
	{
		WorldManager.Instance.Broadcast(new MabiPacket(Op.Effect, NPC.Id).PutInt(Effect.StopMusic), SendTargets.Range, NPC);

		Send.UseMotion(NPC, 53, 31, false, true);
	}
}

public class GangnamScript_Psy : GangnamScript_Base
{
	public override void OnLoad()
	{
		SetName("_<mini>NPC</mini> Psy");
		SetRace(10002);
		SetFace(skin: 18, eye: 5, eyeColor: 54, lip: 2);
		SetLocation(52, 46302, 43066, 222);

		EquipItem(Pocket.Hair, 4026, 0x000000, 0, 0);
		EquipItem(Pocket.Face, 4909, 18, 0, 0);
		EquipItem(Pocket.Head, 18974, 0x000000, 0x000000, 0x000000);
		EquipItem(Pocket.Armor, 15990, 0x707D9F, 0x000000, 0xFFFFFF);
		EquipItem(Pocket.Shoe, 210226, 0xFFFFFF, 0x000000, 0x000000);
		EquipItem(Pocket.RightHand1, 41125);
	}

	public override void Start()
	{
		var mml = "MML@l32o2e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&et132r1r1r1r4v15b8b8>d8c+c16.c<b8.&br1.>d8c+c16.c<b8.&br1l8.bbb8>c+c+c+8d+64e32.l16rdr<brb8r8brbrbr>c+64d32.rc-rc+64d32.r<brbrbr8.br>c+64d32.r<brbr>c+64d32.rc-rc+64d32.r<brbr>c+64d32.rc-rc+64d32.r<brbrbr8.br>c+64d32.r<b8.rbr>c+64d32.r<brbrbr>c+64d32.rc-rc+64d32.rc-rc+64d32.r<brbrbr>c+64l32.dr8.c+64dr8.c+64dl16rc-rc+64d32.r<brl8.bbb8>c+c+c+8d+64e32.l16rdr<b4r8brbrbr>c+64d32.r<brbrbrbrbrbrbr>d+64e32.rdr<b4r8brbrbr>c+64d32.rc-rc+64d32.r<brbrbrbrbr>d+64e32.rdr<b4r8brbrbrbrbrbr>c+64d32.r<brbrbrbr>d+64e32.rdr<b8&b32r.>drc-rdrd+rl8<b.b.b>c+4<bb>c+64d.&d32.rc-4f+f+f+f+4re4reed+4rl32c-f+b8.<brbrl16brbrb4l8rb32>f+32b.r<bb>c+64d.&d32.rc-4f+f+f+f+4re4reef+4rl32f+>c+f+8&f+r<f+rf+rl16f+rf+rf+4l8rf+32>c+32f+.r<f+f+f+f+f+f+f+f+f+f+l4f+f+f+f+l32f+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+fed+dc+c<ba+ag+gf+8r2.b8b8>d8c+c16.c<b8.&bb8.r1r4r16>d8c+c16.c<b8.&bl16br4rbr8.br8.br4rb8b8>d8c+32c.l32c<b8.&bb4r1r4>d8c+c16.c<b8.&bl16br4rbr8.br8.brl8b.b.b>dl32c+c16.cc-8.rc+d1&d8.&dl16<brbrl32>c+d8&dr16c+d8&dl16r<br4rbr8.br8.br4rbrb8>d8l32c+cr16c<b8.&b>c+d1&d8.&dl16<brbrl32>c+d8.&dc+d8.&dl16<br4rbr8.br8.brl8.>eee8f+f+f+8r4b1;";

		WorldManager.Instance.Broadcast(
			PacketCreator.PlayEffect(NPC, InstrumentType.MaleVoice1, PlayingQuality.VeryGood, MabiZip.Compress(mml)),
		SendTargets.Range, NPC);
	}

	public override void StartHard()
	{
		Send.UseMotion(NPC, 88, 2, true);
	}
}

public class GangnamScript_Hyuna : GangnamScript_Base
{
	public override void OnLoad()
	{
		SetName("_<mini>NPC</mini> Hyuna");
		SetRace(10001);
		SetFace(skin: 17, eye: 139, eyeColor: 76, lip: 48);
		SetLocation(52, 46408, 43179, 225);

		EquipItem(Pocket.Hair, 3048, 268435492, 0, 0);
		EquipItem(Pocket.Face, 3907, 17, 0, 0);
		EquipItem(Pocket.Armor, 15065, 0xFFFFFFFF, 0xFFFFFFFF, 0xFF000000);
		EquipItem(Pocket.Shoe, 17263, 0xFFFFFF, 0xFFFFFF, 0xFFFFFF);
		EquipItem(Pocket.RightHand1, 41125);
	}

	public override void Start()
	{
		var mml = "MML@l32o2e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&e&et132r1r1r1r4v15b8b8>d8c+c16.c<b8.&br1.>d8c+c16.c<b8.&br1l8.bbb8>c+c+c+8d+64e32.l16rdr<brb8r8brbrbr>c+64d32.rc-rc+64d32.r<brbrbr8.br>c+64d32.r<brbr>c+64d32.rc-rc+64d32.r<brbr>c+64d32.rc-rc+64d32.r<brbrbr8.br>c+64d32.r<b8.rbr>c+64d32.r<brbrbr>c+64d32.rc-rc+64d32.rc-rc+64d32.r<brbrbr>c+64l32.dr8.c+64dr8.c+64dl16rc-rc+64d32.r<brl8.bbb8>c+c+c+8d+64e32.l16rdr<b4r8brbrbr>c+64d32.r<brbrbrbrbrbrbr>d+64e32.rdr<b4r8brbrbr>c+64d32.rc-rc+64d32.r<brbrbrbrbr>d+64e32.rdr<b4r8brbrbrbrbrbr>c+64d32.r<brbrbrbr>d+64e32.rdr<b8&b32r.>drc-rdrd+rl8<b.b.b>c+4<bb>c+64d.&d32.rc-4f+f+f+f+4re4reed+4rl32c-f+b8.<brbrl16brbrb4l8rb32>f+32b.r<bb>c+64d.&d32.rc-4f+f+f+f+4re4reef+4rl32f+>c+f+8&f+r<f+rf+rl16f+rf+rf+4l8rf+32>c+32f+.r<f+f+f+f+f+f+f+f+f+f+l4f+f+f+f+l32f+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+fed+dc+c<ba+ag+gf+8r2.b8b8>d8c+c16.c<b8.&bb8.r1r4r16>d8c+c16.c<b8.&bl16br4rbr8.br8.br4rb8b8>d8c+32c.l32c<b8.&bb4r1r4>d8c+c16.c<b8.&bl16br4rbr8.br8.brl8b.b.b>dl32c+c16.cc-8.rc+d1&d8.&dl16<brbrl32>c+d8&dr16c+d8&dl16r<br4rbr8.br8.br4rbrb8>d8l32c+cr16c<b8.&b>c+d1&d8.&dl16<brbrl32>c+d8.&dc+d8.&dl16<br4rbr8.br8.brl8.>eee8f+f+f+8r4b1;";

		WorldManager.Instance.Broadcast(
			PacketCreator.PlayEffect(NPC, InstrumentType.FemaleVoice1, PlayingQuality.VeryGood, MabiZip.Compress(mml)),
		SendTargets.Range, NPC);
	}

	public override void StartHard()
	{
		Send.UseMotion(NPC, 88, 2, true);
	}
}

public class GangnamScript_Piano : GangnamScript_Base
{
	public override void OnLoad()
	{
		SetName("_<mini>NPC</mini> Piano Player");
		SetRace(10001);
		SetFace(skin: 18, eye: 0, eyeColor: 49, lip: 48);
		SetLocation(52, 46224, 43756, 206);

		EquipItem(Pocket.Hair, 9200, 0x000000, 0, 0);
		EquipItem(Pocket.Face, 3900, 0, 0, 0);
		EquipItem(Pocket.Armor, 15001, 0xFFFFFF, 0xFFFFFF, 0xFFFFFF);
		EquipItem(Pocket.Shoe, 220056, 0xFFFFFF, 0xFFFFFF, 0x000000);

		// Spawn piano
		var pos = NPC.GetPosition();
		var prop = new MabiProp(44311, NPC.Region, pos.X, pos.Y, MabiMath.DirToRad(NPC.Direction));
		prop.State = "stand";
		prop.ExtraData = string.Format("<xml OWNER='{0}' SITCHAR='{0}'/>", NPC.Id);
		SpawnProp(prop);

		// Sit
		NPC.Temp.SittingProp = prop;
		NPC.State |= CreatureStates.SitDown;
	}

	public override void Start()
	{
		var mml = "MML@a1t132v14l8<c-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d>e.e.ef+.f+.f+<<c-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d>e.e.ef+.f+.f+<<c-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>dl8.>eee8f+f+f+8v12<ggc-g4rc+<aaa>a8a8bbbbr4bbl8b>aabbl8.<ggddd4eee8l16ra8re8a8f+8r<f+8r>c+8rf+8r<f+8r8>c+8rc+8r<f+8rf+8rl8>c+f+n42f+f+f+n42f+f+f+c+f+f+c+f+c+f+<f+l32f+rf+r>f+rf+rc+rc+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+fed+dc+c<ba+ag+gl8f+r1.v14c-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d>e.e.ef+.f+.f+<<c-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>dr1r4b1;";

		WorldManager.Instance.Broadcast(
			PacketCreator.PlayEffect(NPC, InstrumentType.Piano, PlayingQuality.VeryGood, MabiZip.Compress(mml)),
		SendTargets.Range, NPC);

		Send.UseMotion(NPC, 88, 5, true);
	}

	public override void StartHard()
	{
		Send.UseMotion(NPC, 88, 6, true);
	}
}

public class GangnamScript_Ukulele : GangnamScript_Base
{
	public override void OnLoad()
	{
		SetName("_<mini>NPC</mini> Ukulele Player");
		SetRace(10001);
		SetFace(skin: 18, eye: 0, eyeColor: 49, lip: 48);
		SetLocation(52, 45821, 43318, 241);

		EquipItem(Pocket.Hair, 9200, 0x000000, 0, 0);
		EquipItem(Pocket.Face, 3900, 0, 0, 0);
		EquipItem(Pocket.Armor, 15001, 0xFFFFFF, 0xFFFFFF, 0xFFFFFF);
		EquipItem(Pocket.Shoe, 220056, 0xFFFFFF, 0xFFFFFF, 0x000000);
		EquipItem(Pocket.RightHand1, 40018);
	}

	public override void Start()
	{
		var mml = "MML@a1t132v15l8o1brbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbr>ererf+rf+r<brbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbr>ererf+rf+r<brbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbr>ererf+rf+rgrgrgrgrarararar<brbrbrbrbrbrbrbr>grgrgrgrararararf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+r1.r4.<brbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbr>ererf+rf+r<brbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbrbro4b.b.b>c+.c+.c+r4o2b1;";

		WorldManager.Instance.Broadcast(
			PacketCreator.PlayEffect(NPC, InstrumentType.Ukulele, PlayingQuality.VeryGood, MabiZip.Compress(mml)),
		SendTargets.Range, NPC);
	}

	public override void StartHard()
	{
		Send.UseMotion(NPC, 88, 2, true);
	}
}

public class GangnamScript_Mandolin : GangnamScript_Base
{
	public override void OnLoad()
	{
		SetName("_<mini>NPC</mini> Mandolin Player");
		SetRace(10001);
		SetFace(skin: 18, eye: 0, eyeColor: 49, lip: 48);
		SetLocation(52, 45737, 43189, 241);

		EquipItem(Pocket.Hair, 9200, 0x000000, 0, 0);
		EquipItem(Pocket.Face, 3900, 0, 0, 0);
		EquipItem(Pocket.Armor, 15001, 0xFFFFFF, 0xFFFFFF, 0xFFFFFF);
		EquipItem(Pocket.Shoe, 220056, 0xFFFFFF, 0xFFFFFF, 0x000000);
		EquipItem(Pocket.RightHand1, 40017);
	}

	public override void Start()
	{
		var mml = "MML@a1t132v14l8<c-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d>e.e.ef+.f+.f+<<c-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d>e.e.ef+.f+.f+<<c-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>dl8.>eee8f+f+f+8v12<ggc-g4rc+<aaa>a8a8bbbbr4bbl8b>aabbl8.<ggddd4eee8l16ra8re8a8f+8r<f+8r>c+8rf+8r<f+8r8>c+8rc+8r<f+8rf+8rl8>c+f+n42f+f+f+n42f+f+f+c+f+f+c+f+c+f+<f+l32f+rf+r>f+rf+rc+rc+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+rf+fed+dc+c<ba+ag+gl8f+r1.v14c-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d>e.e.ef+.f+.f+<<c-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>d<c-.b16rbba+adc-.b16rb4bb>dr1r4b1;";

		WorldManager.Instance.Broadcast(
			PacketCreator.PlayEffect(NPC, InstrumentType.Mandolin, PlayingQuality.VeryGood, MabiZip.Compress(mml)),
		SendTargets.Range, NPC);
	}

	public override void StartHard()
	{
		Send.UseMotion(NPC, 88, 2, true);
	}
}

public class GangnamScript_Clip : GangnamScript_Base
{
	bool stop = false;

	public override void OnLoad()
	{
		SetName("_<mini>NPC</mini> Video Guy");
		SetRace(10001);
		SetFace(skin: 18, eye: 0, eyeColor: 49, lip: 48);
		SetLocation(52, 45748, 43750, 225);

		EquipItem(Pocket.Hair, 9200, 0x000000, 0, 0);
		EquipItem(Pocket.Face, 3900, 0, 0, 0);
		EquipItem(Pocket.Armor, 15001, 0xFFFFFF, 0xFFFFFF, 0xFFFFFF);
		EquipItem(Pocket.Shoe, 220056, 0xFFFFFF, 0xFFFFFF, 0x000000);
		EquipItem(Pocket.RightHand1, 40201);
	}

	public override void Start()
	{
		stop = false;

		Thread t = null;
		t = new Thread(() =>
		{
			for (int i = 15; i < 2000; ++i) // 2521
			{
				if (stop)
					i = 2175;
				if (i > 2175)
					break;

				var url = string.Format("http://localhost/aura/visualchat/img/chat_20131005_00{0}_Zerono.png", i.ToString().PadLeft(4, '0'));
				ushort width = 170;
				ushort height = 96;

				var p = new MabiPacket(Op.VisualChat, NPC.Id);
				p.PutString(NPC.Name);
				p.PutString(url);
				p.PutShorts(width, height);
				p.PutByte(0);
				WorldManager.Instance.Broadcast(p, SendTargets.Range, NPC);

				Thread.Sleep(100);

				// skip 1, 5fps
				Thread.Sleep(100);
				i++;
			}

			GC.KeepAlive(t);
		});
		t.Start();
	}

	public override void StartHard()
	{
		Send.UseMotion(NPC, 53, 62, true);
	}

	public override void StopMusic()
	{
		stop = true;
	}
}
