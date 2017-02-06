//--- Aura Script -----------------------------------------------------------
// Mabimon
//--- Description -----------------------------------------------------------
// Pokemon inspired battles in Mabinogi, created for a video.
// 
// https://www.youtube.com/watch?v=LTO1MhncEJk&t=5s
//---------------------------------------------------------------------------

using Aura.Channel.Skills;
using Aura.Channel.Skills.Base;

[AiScript("trainer")]
public class TrainerAi : AiScript
{
	HashSet<long> _fought = new HashSet<long>();

	public TrainerAi()
	{
	}

	protected override IEnumerable Idle()
	{
		var challenger = Creature;

		if (challenger.Vars.Temp["inTrainerBattleWith"] != null)
			Return();

		var dir = MabiMath.ByteToRadian(challenger.Direction) - Math.PI; // opposite direction...?
		var pos = challenger.GetPosition();

		var players = challenger.Region.GetCreatures(a => a.IsPlayer && a.GetPosition().InCone(pos, dir, 1000, 45));
		if (players.Count != 1)
			Return();

		var opponent = players.First();
		if (_fought.Contains(opponent.EntityId))
			Return();

		_fought.Add(opponent.EntityId);

		TrainerBaseScript.StartBattle(challenger, opponent);
	}
}

public abstract class TrainerBaseScript : NpcScript
{
	const int BattleDistance = 1000;
	const int MonsterDistance = 400;
	const int FlashDelay = 3000;
	const int InitDelay = 3000;
	const int AttackDelay = 3000;
	const int LifeRatio = 3;

	static readonly SkillId[] Skills = new SkillId[] { SkillId.CombatMastery, SkillId.Smash, SkillId.Defense, SkillId.Counterattack, SkillId.Icebolt };

	protected List<NPC> _challengerMonsters = new List<NPC>();
	protected List<NPC> _playerMonsters = new List<NPC>();
	Position _challengerMonsterPos;
	Position _playerMonsterPos;

	protected virtual string Cheerleader { get { return null; } }

	protected override async Task Talk()
	{
		var challenger = NPC;
		var opponent = Player;

		if (challenger.Vars.Temp["inTrainerBattleWith"] == null)
			challenger.Vars.Temp["inTrainerBattleWith"] = Player.EntityId;
		else if (challenger.Vars.Temp["inTrainerBattleWith"] != Player.EntityId)
			End("(" + challenger.Name + " is currently in battle.)");

		PrepareTrainers(challenger, Player);
		AssembleChallengerTeam();
		AssemblePlayerTeam();

		Creature cheerleader = null;
		if (Cheerleader != null)
		{
			cheerleader = challenger.Region.GetCreature(Cheerleader) as NPC;
			if (cheerleader != null)
			{
				cheerleader.TurnTo(Player.GetPosition());
				PlaySong(cheerleader, "MML@t94l32>g+gf+fg+dd+eg+d+dd+g+dc+dg+c+cc+g+cc-cg+<ba+bn68a+g+a+l8.>a<aa8aa>a+8a<aa8aa>a+8>ccc8ccc+8ccc8cc<b8<<a>ed16e16l8gf+edf2a+2<a.>e.d16e16gf+edf2<f2l16>edefa+aa+>dc+4.<a+8a1<agaa+8aga+agaa+8aga+agaa+8aga+a>dc<a+agaa+gfgg+8gfg+gfgg+8gfg+gfgg+8gfg+fb+a+g+gfgg+g8.>d8.cdl8fedcd+1<f4d+fgfd+gg+4fgg+gfg+a4f+g+ag+f+aa+d+a+d+a+d+a+d+a+n51a+n51a+n51a+>d+l16n63a+>gn58d+n58gn58d+n58gn58d+n58gn58d+n58gn58d+n58gn58d+n58gn58d+n58gn58d+n58gn58d+n58gn58d+n58gn58d+n58gn58d+n58gn58d+n58gn58d+n58gn58d+n58g<a+f8n46fg+8d+8f8g+gf4f8n46fg+8d+8f8gd+f4f8n46fg+8d+8f8gd+g+4g+4>c4d+4<g8d+g8.g8gfd+l8g.g+gd+16g.gg.g.g+g+.g.fd+fgd+f2g+2g.d+.g>c+c<a+g+f2a+g+gf<a+.l16>d+d+4<a+8.>gg4<a+8.>a+a+4g8.>d+d+4<a+4a+4g8.a+8.g+gd+4d+4a+8.g8ga+8<a8.>e8.del8gf+edf2a+2<a.>e.d16e16gf+edf2f2,r8l32rdc+cc-c<ba+ag+aa+bgf+feed+dc+dc+c<bbb+al16>aef+gef+geaef+gef+geaef+gef+geaef+gef+aa+b+gaa+gaa+gb+gaa+>cc+cn46c<gaa+gaa+gb+gaa+gaa+g<eaeaef+gaeaeaef+gefa+fa+faa+>d<fa+fa+faa+>d<eaeaef+gaeaeaef+gafa+fa+faa+>d<fa+fa+faa+>d<eaeaeaeaeaeaedefeaeaeaeae>fed<ag+aa+dadadadadadadagadadadadad>dc<a+agaa+cgcgcgcgcgcgcgfgcgcgcgcgcb+a+g+gfd+d>g4f4e4d+4d+a+d+a+d+a+ab+d+a+d+a+d+a+ab+cgcgcgcgcb+a+g+gfd+fc+g+c+g+c+g+c+g+c+g+c+g+c+g+bg+dadadadadabag+f+g+al8a+d+a+d+a+d+a+l1d+&d+&d+&d+&d+&d+&d+&d+&d+2&d+8l16<a+g+gfd+a+d+a+d+a+n37a+d+a+>c+c<a+g+gfd+a+d+a+d+a+n37a+d+a+>cc+d+c+c<a+d+a+d+a+d+a+d+a+d+a+d+a+d+a+d+a+c+g+c+g+c+g+c+g+c+g+c+g+c+b+a+g+d+a+d+a+d+a+d+a+d+a+d+a+d+a+d+a+c+g+c+g+c+g+c+g+c+>c+c<a+gg+a+>cl4d+c+cn34a+g+gf>d+d+<a+a+ggl8d+.d+d+16a+l2<ab+a+>d<ab+a+>d,r1l2.a&a8a+8al8&afl8.o6ccc8ccc+8ccc8cc<b8<<a>el16del8gf+edf2a+2<a.>e.d16e16gf+edf2<f2>a.e.a>d.fl16edfefedc+n58c+<a+2&a+l4edefedgfdcdd+dcdd+dd+efl2ga+d+l8gfd+gg+2g+gfg+a2ag+f+al4gd+a+gl8gd+gd+gd+gd+f4gd+fg16d+16fn46f4gd+fg+16g16fn46f4gd+fg16d+16fn46f4gd+fl16g+gf8<a+8>f8n46fg+gfd+l32dd+dd+l16fn46c4f8n46fg+gfd+l32dd+dd+l16fgf4f8n46fg+gfd+l32dd+dd+l16fn46c2>d+2<g8.g8.g8gfd+l8.gg+8ggg8l16gfd+l8.gg+8>d+a+l16g+gl8g+gfd+f2g+2d+.a+.g+16g16g+gfd+c+2c+c<a+g+g.l16d+a+4a+8.g>d+4d+8.n58g4g8.d+a+4g8.a+8.g+gd+2d+8.a+8.g+gd+1&d+1&d+2<<a8.>e8.del8gf+edf2f2;");
			}
		}

		var rnd = RandomProvider.Get();
		var regionId = challenger.RegionId;
		var challengerMonster = GetNextMonster(_challengerMonsters);
		var playerMonster = GetNextMonster(_playerMonsters);

		Task.Delay(FlashDelay + InitDelay / 2 + InitDelay * 1).ContinueWith(_ => SpawnMonster(challengerMonster, regionId, _challengerMonsterPos, _playerMonsterPos));
		Task.Delay(FlashDelay + InitDelay / 2 + InitDelay * 2).ContinueWith(_ => SpawnMonster(playerMonster, regionId, _playerMonsterPos, _challengerMonsterPos));

		Msg("<title name='NONE'/>You are challenged by<br/>{0}!<autopass duration='{1}'/>", challenger.Name, FlashDelay + InitDelay);
		Msg("<title name='NONE'/>{0} sent<br/>out {1}!<autopass duration='{2}'/>", challenger.Name, challengerMonster.Name, InitDelay);
		Msg("<title name='NONE'/>Go! {0}!<autopass duration='{1}'/>", playerMonster.Name, InitDelay);

		var options = "";
		foreach (var skillId in Skills)
			options += string.Format("<button title='{0}' keyword='@{1}'/>", GetSkillName(skillId), skillId);
		//options += "<button title='Cancel' keyword='@cancel'/>";

		var canceled = false;
		var result = BattleResult.None;
		while (true)
		{
			Msg("<title name='NONE'/>What will {0} do?" + options, playerMonster.Name);
			var action = await Select();

			if (action == "@cancel")
			{
				canceled = true;
				break;
			}

			var playerSkillId = (SkillId)Enum.Parse(typeof(SkillId), action.Substring(1));
			var challengerSkillId = SkillId.None;
			while ((challengerSkillId = rnd.Rnd(Skills)) == playerSkillId) ;

			Msg("<title name='NONE'/>{0} used<br/>{1}!<autopass duration='{2}'/>", playerMonster.Name, GetSkillName(playerSkillId), AttackDelay);
			Msg("<title name='NONE'/>The foe's {0} used<br/>{1}!<autopass duration='{2}'/>", challengerMonster.Name, GetSkillName(challengerSkillId), AttackDelay);

			PrepareAction(challengerSkillId, challengerMonster, playerMonster);
			PrepareAction(playerSkillId, playerMonster, challengerMonster);
			DoAction(challengerMonster, challengerSkillId, playerMonster, playerSkillId);

			var cm = challengerMonster;
			var pm = playerMonster;
			Task.Delay(AttackDelay / 2 + AttackDelay).ContinueWith(_ =>
			{
				if (cm.Region != Region.Limbo)
				{
					if (!cm.IsDead && cm.GetPosition() != _challengerMonsterPos) cm.Move(_challengerMonsterPos, false);
					SharpMind(cm, pm, SkillId.None, SharpMindStatus.None);
					if (cm.Skills.ActiveSkill != null) cm.Skills.CancelActiveSkill();
				}

				if (pm.Region != Region.Limbo)
				{
					if (!pm.IsDead && pm.GetPosition() != _playerMonsterPos) pm.Move(_playerMonsterPos, false);
					SharpMind(pm, cm, SkillId.None, SharpMindStatus.None);
					if (pm.Skills.ActiveSkill != null) pm.Skills.CancelActiveSkill();
				}
			});

			if (challengerMonster.IsDead)
			{
				Msg("<title name='NONE'/>The foe's {0} fainted!<autopass duration='{1}'/>", challengerMonster.Name, AttackDelay);

				Task.Delay(AttackDelay / 2 + AttackDelay + AttackDelay).ContinueWith(_ => UnspawnMonster(cm));
				if ((challengerMonster = GetNextMonster(_challengerMonsters)) == null)
					result = BattleResult.Won;
				else
				{
					Msg("<title name='NONE'/>{0} sent<br/>out {1}!<autopass duration='{2}'/>", challenger.Name, challengerMonster.Name, InitDelay);
					Task.Delay(AttackDelay / 2 + AttackDelay + AttackDelay + InitDelay / 2).ContinueWith(_ => SpawnMonster(challengerMonster, regionId, _challengerMonsterPos, _playerMonsterPos));
				}
			}
			else if (playerMonster.IsDead)
			{
				Msg("<title name='NONE'/>{0} fainted!<autopass duration='{1}'/>", playerMonster.Name, AttackDelay);

				Task.Delay(AttackDelay / 2 + AttackDelay + AttackDelay).ContinueWith(_ => UnspawnMonster(pm));
				if ((playerMonster = GetNextMonster(_playerMonsters)) == null)
					result = BattleResult.Lost;
				else
				{
					Msg("<title name='NONE'/>Go! {0}!<autopass duration='{1}'/>", playerMonster.Name, InitDelay);
					Task.Delay(AttackDelay / 2 + AttackDelay + AttackDelay + InitDelay / 2).ContinueWith(_ => SpawnMonster(playerMonster, regionId, _playerMonsterPos, _challengerMonsterPos));
				}
			}

			if (result != BattleResult.None)
				break;
		}

		if (result == BattleResult.Won)
		{
			var gold = rnd.Next(100, 1000);

			if (cheerleader != null)
			{
				StopSong(cheerleader);
				PlaySong(cheerleader, "MML@<gl16g.gl16.gf+4dc+16<ba8&a32a>f+d16<ab8&b32b>ge16c-c+8&c+32c+af+16c+<a8&a32>aa4<a8&a32a>f+d16<ab8&b32b>ge16c-c+8&c+32c+af+16c+<a8&a32>aa4<a8&a32a>f+d16<ab8&b32b>ge16c-c8&c32caf+16cd8&d32d>d<a+16gdf+16a>d2<ag16d<a>d16f+a2ge16c+<a8&a32a>f+d16<ab8&b32b>ge16c-c+8&c+32c+af+16c+<a8&a32>aa4<a8&a32a>f+d16<ab8&b32b>ge16c-c+8&c+32c+af+16c+<a8&a32>aa4<a8&a32a>f+d16<ab8&b32b>ge16c-c8&c32caf+16cd8&d32d>d<a+16gdf+16a>d2<ag16d<a>d16f+a2ge16c+,t160l16>e.el16.eef+16ga2<f+8&f+32df+4g8&g32eg4c+e16f+ab16n61a8&a32>c+e4<f+8&f+32df+4g8&g32eg4c+e16f+ab16n61a8&a32>c+e4<f+8&f+32df+4g8&g32eg4a8&a32fa4a+8&a+32ga+4>dc+16<a>dc+16<a>dc+16<af+a16>dc+<b16an61b16an61b16aef+16gf+8&f+32df+4g8&g32eg4c+e16f+ab16n61a8&a32>c+e4<f+8&f+32df+4g8&g32eg4c+e16f+ab16n61a8&a32>c+e4<f+8&f+32df+4g8&g32eg4a8&a32fa4a+8&a+32ga+4>dc+16<a>dc+16<a>dc+16<af+a16>dc+<b16an61b16an61b16aef+16g,l16>a.al16.aab16>c+d2<d8&d32<a>d4e8&e32c-e4f+8&f+32ga4e8&e32f+g4d8&d32<a>d4e8&e32c-e4f+8&f+32ga4e8&e32f+g4d8&d32<a>d4e8&e32c-e4f8&f32cf4g8&g32dg4f+1e1d8&d32<a>d4e8&e32c-e4f+8&f+32ga4e8&e32f+g4d8&d32<a>d4e8&e32c-e4f+8&f+32ga4e8&e32f+g4d8&d32<a>d4e8&e32c-e4f8&f32cf4g8&g32dg4f+1e1;");
			}

			Msg("<title name='NONE'/>Player defeated<br/>{0}!<autopass duration='{1}'/>", challenger.Name, InitDelay);
			Msg("<title name='NONE'/>{0} got {1}g<br/>for winning!<autopass duration='{2}'/>", opponent.Name, gold, InitDelay);
			Task.Delay(InitDelay * 2).ContinueWith(_ => opponent.Inventory.Gold += gold);
		}
		else if (result == BattleResult.Lost)
		{
			Msg("<title name='NONE'/>Player was defeated<br/>by {0}!<autopass duration='{1}'/>", challenger.Name, InitDelay);
			Task.Delay(InitDelay).ContinueWith(_ => opponent.Inventory.Gold -= opponent.Inventory.Gold / 2);
		}

		if (result != BattleResult.Won && cheerleader != null)
			StopSong(cheerleader);

		if (result == BattleResult.Lost && cheerleader != null)
			Send.UseMotion(cheerleader, 53, 3);

		challenger.Vars.Temp["inTrainerBattleWith"] = null;
		opponent.Unlock(Locks.Move, true);

		var wait = (canceled ? 0 : AttackDelay / 2 + AttackDelay + AttackDelay + InitDelay);
		Task.Delay(wait).ContinueWith(_ =>
		{
			if (challengerMonster != null && !challengerMonster.IsDead) UnspawnMonster(challengerMonster);
			if (playerMonster != null && !playerMonster.IsDead) UnspawnMonster(playerMonster);
		});
		if (result == BattleResult.Won && cheerleader != null)
			Task.Delay(wait + InitDelay).ContinueWith(_ => StopSong(cheerleader));

		if (canceled)
			End("(Battle canceled.)");
		else
			End("(The battle is over.)");
	}

	public static void StartBattle(Creature challenger, Creature player)
	{
		challenger.Vars.Temp["inTrainerBattleWith"] = player.EntityId;

		player.StopMove();
		player.Lock(Locks.Move, true);

		Send.Chat(challenger, "I challenge you, " + player.Name + "!");

		Task.Delay(1000).ContinueWith(_ =>
		{
			Send.Effect(player, Effect.ScreenFlash, FlashDelay, 0);
			player.TalkToNpc(challenger.Name);
		});
	}

	private void PrepareTrainers(Creature challenger, Creature player)
	{
		var pos = challenger.GetPosition();
		var playerNewPos = pos.GetRelative(MabiMath.ByteToRadian(challenger.Direction), BattleDistance);

		_challengerMonsterPos = playerNewPos.GetRelative(pos, -MonsterDistance);
		_playerMonsterPos = pos.GetRelative(playerNewPos, -MonsterDistance);

		//challenger.Region.AddProp(new Prop(10, challenger.RegionId, _challengerMonsterPos.X, _challengerMonsterPos.Y, 0));
		//challenger.Region.AddProp(new Prop(10, challenger.RegionId, _playerMonsterPos.X, _playerMonsterPos.Y, 0));

		// Camera
		var packet = new Packet(Op.SetCamera, player.EntityId);
		packet.PutFloat(1000); // distance
		packet.PutFloat(0);
		packet.PutFloat(5); // pitch
		packet.PutFloat(MabiMath.DirectionToRadian(pos.X - playerNewPos.X, pos.Y - playerNewPos.Y) * (180 / Math.PI) + 25); // yaw
		packet.PutFloat(0);
		player.Client.Send(packet);

		// Move and turn
		player.Jump(playerNewPos);
		player.TurnTo(pos);
		challenger.TurnTo(playerNewPos);
	}

	private Creature GetNextMonster(List<NPC> monsters)
	{
		return monsters.FirstOrDefault(a => !a.IsDead);
	}

	protected abstract void AssembleChallengerTeam();

	protected virtual void AssemblePlayerTeam()
	{
		_playerMonsters.Add(new NPC(20002));
	}

	private void Msg(string format, params object[] args)
	{
		base.Msg(string.Format(format, args));
	}

	private void SpawnMonster(Creature challenger, int regionId, Position pos, Position turnPos)
	{
		var npc = (NPC)challenger;
		if (npc.AI != null)
			npc.AI.Detach();
		npc.Direction = MabiMath.DirectionToByte(turnPos.X - pos.X, turnPos.Y - pos.Y);
		npc.LifeMaxBase /= LifeRatio;
		npc.Life = npc.LifeMax;
		npc.State |= CreatureStates.Spawned;
		npc.State |= CreatureStates.GoodNpc;

		if (!npc.Skills.Has(SkillId.CombatMastery)) npc.Skills.Give(SkillId.CombatMastery, SkillRank.RF);
		if (!npc.Skills.Has(SkillId.Smash)) npc.Skills.Give(SkillId.Smash, SkillRank.RF);
		if (!npc.Skills.Has(SkillId.Defense)) npc.Skills.Give(SkillId.Defense, SkillRank.RF);
		if (!npc.Skills.Has(SkillId.Counterattack)) npc.Skills.Give(SkillId.Counterattack, SkillRank.RF);
		if (!npc.Skills.Has(SkillId.Icebolt)) npc.Skills.Give(SkillId.Icebolt, SkillRank.RF);

		challenger.Warp(regionId, pos);
		challenger.TurnTo(turnPos);
		Send.SpawnEffect(SpawnEffect.Monster, regionId, pos.X, pos.Y, challenger, challenger);
	}

	private void UnspawnMonster(Creature challenger)
	{
		if (challenger == null)
			return;

		var pos = challenger.GetPosition();
		Send.SpawnEffect(SpawnEffect.MonsterDespawn, challenger.RegionId, pos.X, pos.Y, challenger, challenger);

		challenger.Disappear();
	}

	private string GetSkillName(SkillId skillId)
	{
		switch (skillId)
		{
			case SkillId.CombatMastery: return "Combat Mastery";
			default: return skillId.ToString();
		}
	}

	private void PrepareAction(SkillId skillId, Creature creature, Creature target)
	{
		SharpMind(creature, target, skillId, SharpMindStatus.Loading);
		SharpMind(creature, target, skillId, SharpMindStatus.Loaded);
	}

	private void DoAction(Creature creature1, SkillId skillId1, Creature creature2, SkillId skillId2)
	{
		if (skillId1 == skillId2)
			return;

		var skill1 = creature1.Skills.Get(skillId1);
		var skill2 = creature2.Skills.Get(skillId2);

		if (skillId1 != SkillId.CombatMastery) creature1.Skills.ActiveSkill = skill1;
		if (skillId2 != SkillId.CombatMastery) creature2.Skills.ActiveSkill = skill2;

		if (skillId1 != SkillId.CombatMastery)
			PrepareSkill(creature1, skill1);
		if (skillId2 != SkillId.CombatMastery)
			PrepareSkill(creature2, skill2);

		if (skillId1 == SkillId.CombatMastery && skillId2 == SkillId.Smash) UseCombatSkill(creature1, skill1, creature2);
		else if (skillId2 == SkillId.CombatMastery && skillId1 == SkillId.Smash) UseCombatSkill(creature2, skill2, creature1);
		else if (skillId1 == SkillId.CombatMastery && skillId2 == SkillId.Icebolt) UseCombatSkill(creature1, skill1, creature2);
		else if (skillId2 == SkillId.CombatMastery && skillId1 == SkillId.Icebolt) UseCombatSkill(creature2, skill2, creature1);
		else if (skillId1 == SkillId.Icebolt && skillId2 == SkillId.Smash) UseCombatSkill(creature1, skill1, creature2);
		else if (skillId2 == SkillId.Icebolt && skillId1 == SkillId.Smash) UseCombatSkill(creature2, skill2, creature1);
		else
		{
			if (skillId1 == SkillId.CombatMastery || skillId1 == SkillId.Smash || skillId1 == SkillId.Icebolt)
				UseCombatSkill(creature1, skill1, creature2);
			if (skillId2 == SkillId.CombatMastery || skillId2 == SkillId.Smash || skillId2 == SkillId.Icebolt)
				UseCombatSkill(creature2, skill2, creature1);
		}

		skill1.State = SkillState.None;
		skill2.State = SkillState.None;
	}

	private void PrepareSkill(Creature creature, Skill skill)
	{
		var prepareHandler = ChannelServer.Instance.SkillManager.GetHandler<IPreparable>(skill.Info.Id);
		var readyHandler = (IReadyable)prepareHandler;

		prepareHandler.Prepare(creature, skill, null);
		readyHandler.Ready(creature, skill, null);
		skill.State = SkillState.Ready;
	}

	private void UseCombatSkill(Creature creature, Skill skill, Creature target)
	{
		var handler = ChannelServer.Instance.SkillManager.GetHandler<ICombatSkill>(skill.Info.Id);
		handler.Use(creature, skill, target.EntityId);
	}

	private void SharpMind(Creature user, Creature target, SkillId skillId, SharpMindStatus state)
	{
		var packet = new Packet(Op.SharpMind, target.EntityId);
		packet.PutLong(user.EntityId);
		packet.PutByte(1);
		packet.PutByte(1);
		packet.PutUShort((ushort)skillId);
		packet.PutInt((int)state);

		user.Region.Broadcast(packet, user);
	}

	private void PlaySong(Creature creature, string mml)
	{
		var type = (creature.RightHand == null ? InstrumentType.Lute : creature.RightHand.Data.InstrumentType);
		Send.PlayEffect(creature, type, PlayingQuality.VeryGood, MabiZip.Compress(mml), 0);
	}

	private void StopSong(Creature creature)
	{
		Send.Effect(creature, Effect.StopMusic);
	}

	private enum BattleResult
	{
		None,
		Won,
		Lost,
	}
}

public class RedTrainerScript : TrainerBaseScript
{
	protected override string Cheerleader { get { return "Red's Cheerleader"; } }

	public override void Load()
	{
		SetRace(10002);
		SetName("Trainer Red");
		SetFace(skinColor: 16, eyeType: 32, eyeColor: 76, mouthType: 2);
		SetLocation(1, 22600, 32748, 110);

		EquipItem(Pocket.Face, 4900, 0x000010, 0x000000, 0x000000);
		EquipItem(Pocket.Hair, 4089, 0x1000001c, 0x000000, 0x000000);
		EquipItem(Pocket.Head, 18156, 0xa43a2a, 0xfdfdfd, 0xfdfdfd);
		EquipItem(Pocket.Armor, 15153, 0xa43a2a, 0x000000, 0xffffff);
		EquipItem(Pocket.Shoe, 17060, 0x000000, 0x000000, 0x808080);
		EquipItem(Pocket.Glove, 16195, 0x000000, 0x000000, 0x808080);
		EquipItem(Pocket.RightHand1, 40004);

		SetAi("trainer");
	}

	protected override void AssembleChallengerTeam()
	{
		var mouse = new NPC(120009);
		mouse.Color1 = 0xffea00;

		_challengerMonsters.Add(mouse);
	}
}

public class RedTrainerSupportScript : NpcScript
{
	public override void Load()
	{
		SetRace(10001);
		SetName("Red's Cheerleader");
		SetBody(weight: 0.7f, upper: 0.7f, lower: 0.7f);
		SetFace(skinColor: 23, eyeType: 152, eyeColor: 38, mouthType: 48);
		SetLocation(1, 22795, 32947, 130);

		EquipItem(Pocket.Face, 3907, 0x00366969, 0x0094B330, 0x00737474);
		EquipItem(Pocket.Hair, 4933, 0x00E3CCBA, 0x00E3CCBA, 0x00E3CCBA);
		EquipItem(Pocket.Armor, 15946, 0x00EBD2D2, 0x00FFFFFF, 0x00C6794A);
		EquipItem(Pocket.RightHand1, 40861, 0xffffff, 0x000000, 0x000000);
	}

	protected override async Task Talk()
	{
		Close(Hide.None, "I dare you to challenge Red.");
	}
}

//public class BlueTrainerScript : TrainerBaseScript
//{
//	protected override string Cheerleader { get { return "Red's Cheerleader"; } }

//	public override void Load()
//	{
//		SetRace(10002);
//		SetName("Trainer Blue");
//		SetFace(skinColor: 16, eyeType: 32, eyeColor: 29, mouthType: 16);
//		SetLocation(1, 22700, 32583, 120);

//		EquipItem(Pocket.Face, 4900, 0x00000010, 0x00000000, 0x00000000);
//		EquipItem(Pocket.Hair, 4089, 0x10000025, 0x00000000, 0x00000000);
//		EquipItem(Pocket.Armor, 15406, 0x0069a481, 0x4b4645, 0x958baa);
//		EquipItem(Pocket.Shoe, 17012, 0x005f564d, 0x00000000, 0x00808080);
//		EquipItem(Pocket.RightHand1, 40018);

//		//SetAi("trainer");
//	}

//	protected override void AssembleChallengerTeam()
//	{
//		var mouse = new NPC(120009);
//		mouse.Color1 = 0xffea00;

//		_challengerMonsters.Add(mouse);
//	}
//}

//public class SyncPlayFoo : GeneralScript
//{
//	public override void Load()
//	{
//		AddCommand(99, -1, "playfoo", "", HandlePlayFoo);
//	}

//	public CommandResult HandlePlayFoo(ChannelClient client, Creature sender, Creature target, string message, IList<string> args)
//	{
//		var blue = sender.Region.GetCreature("Trainer Blue");
//		var red = sender.Region.GetCreature("Trainer Red");
//		var cheer = sender.Region.GetCreature("Red's Cheerleader");

//		PlaySong(blue, "MML@t142r2l8gga+gggggffffffffgga+gggggffffffffgga+gggggffffffffgga+gggggfffffffffffggg>d<affffffffgga+gggggffffffffgga+gggggffffffffgga+gggggfffffffffffggg>d<affffffff<a+4a+a+4a+l4a+a+a+8a+a+8a+a+a+8a+a+8a+>fc8fc8cd+<a+8a+a+8a+b+a8aa8aa+a+8al8b+a2.&aa+>dd+<a+4a+a+4>d+l4d+<aa8a>c8c<a+a+8>d+d+8d+cc8cc8cd+d+8n46d+8d+<a>d8dd8d<a+a+8al8>cd2<d>dfgdg4.ddfg4<a+g4dfg2;");
//		PlaySong(red, "MML@t142l1.rrrrrrrl4ggggffffggggffffggggffffl8f>dd<eee>ddd<ffafdfg>fn58d+f<a+d+>fn58fn46dfn58dfn58fn58d+fn58d+fcacfacfaca+dga+dga+dacfacfac<dddcccaddddf+fdfg>fn58d+f<a+d+>fn58acfacfaca+dga+dga+db+fab+fab+fgn58d+gn58d+gn58fdn58f<da+>fd<dddcccaddddf+dddgd+4.c4.<g2>dd1&d;");
//		PlaySong(cheer, "MML@t142r2o2g1&g2l8&gdfg1&g2.fd<g1&g2>f4c4d+d+1<a+1a+4.>c4.d2.&ddfg2&gl4>>ddfdffdddddfdd<a+a+a+a+a+>dd<a+l8a+>dfceg<a2.&a.>dfl4g&g16<a+ggffffgggga>ccf8g8n46dgg8f.<aaaa+.>c.d2.&d8.d8f8g&g16<a+ggffffgggga>ccccd+ccfffd<a+.>c.d2l8ddfg4g<g4.f4.d2dfg2.&g;");
//		PlaySong(sender, "MML@t142r1r1.l8r>dddd4.dc4<a+f2a+>d4d4cn58c1&c4.n58d+d+d+4fd4c4<a+2a+>d4dc4n58d1&d2d<ggg4.f4a+f2&fgg4.f4d+f1&f4gggg4a+f4f4f2a+>dd4c4n58d2.&d<dfg4.>g2&ggfdcdc<a+2.>dgaa+a4agagffg4.g2&ggfd4c4.n58cd4dc4<<a+>d2.&d>dfg4.ddfg4gfdcd16c16<a+2.&a+>d+gaa+a4g4f2&f<d+1f1a+4.a+4.a2ddfg4>g4.<ddfg4n70g4dfg2&g;");

//		Task.Delay(35000).ContinueWith(_ =>
//		{
//			var mouse = new NPC(120009);
//			mouse.Color1 = 0xffea00;
//			mouse.Warp(1, 22808, 33605);
//			mouse.Move(new Position(22298, 31873), true);
//		});

//		return CommandResult.Okay;
//	}

//	private void PlaySong(Creature creature, string mml)
//	{
//		var type = (creature.RightHand == null ? InstrumentType.Lute : creature.RightHand.Data.InstrumentType);
//		Send.PlayEffect(creature, type, PlayingQuality.VeryGood, MabiZip.Compress(mml), 0);
//	}
//}

//Msg("The foe's {0} became<br/>confused!");
//Msg("The foe's {0} is<br/>confused!");
//Msg("It hurt itself in its<br/>confusion!");
//Msg("{0}'s Defense<br/>fell!");
//Msg("{0} gained<br/>{1} Exp. Points!");
//Msg("{0} got {1}g<br/>for winning!");
//Msg("It's not very effective...");
