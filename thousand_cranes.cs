//--- Aura Script -----------------------------------------------------------
// Crane the Ferret
//--- Description -----------------------------------------------------------
// Appears when a players acquires their thousands paper crane, and grants
// them a wish.
// 
// This script was written for a video, and is not necessarily production
// ready. For example, two people calling Crane at the same time would
// cause problems, the stat boost is only temporary, and there is no
// overflow check for the wealth wish.
//
// https://www.youtube.com/watch?v=sBw987K_ovg
//---------------------------------------------------------------------------

using Aura.Channel.World.Entities.Creatures;

public class ThousandCranesScript : NpcScript
{
	public override void Load()
	{
		SetRace(230001);
		SetName("Crane");
		SetLocation(1001, 0, 0, 0);
	}

	protected override async Task Talk()
	{
		if (Player.Vars.Temp.Crane != null)
		{
			Msg("*sigh* Wait... you again? Are you kidding me!? No. NO. I'm out of here.<button title='End Conversation' keyword='@a' />");
			await Select();
			NPC.WarpFlash(1001, 0, 0);
			End();
			return;
		}

		await Intro("A clearly irritated ferret appeared right before your eyes, as you finished folding the thousands paper crane.");

		Msg("Ugh, not again, damn it!<button title='Uhm...' keyword='@a' />");
		await Select();
		Msg("Alright, let's get this over with, what do you want?<button title=\"Shouldn't you be...\" keyword='@a' />");
		await Select();
		Msg("*sigh* No, I'm *not* a crane, my *name* is Crane! You stupid humans got that mixed up.");
		Msg("A few hundred years ago I bet with a friend that nobody would be crazy enough to fold a THOUSAND freaking paper cranes.<br/>But he found someone, and now I'm forced to grant a wish to anyone who repeats that task.");
		Msg("Any more questions!?<button title='No...' keyword='@a' />");
		await Select();

	L_Choose:
		Msg("So: What. do. you. want? Don't make me ask again!<button title='Wealth' keyword='@wealth' /><button title='Immortality' keyword='@immortality' /><button title='Strength' keyword='@strength' />");

		switch (await Select())
		{
			case "@wealth":
				Msg("It's always the same with you humans. Okay, I've transfered 1,000,000,000 to your bank account. Happy?");
				Player.Client.Account.Bank.Gold += 1000000000;
				break;

			case "@immortality":
				Msg("That's a popular one. I'm just gonna... eh, what are you? A milletian?<br/>What am I supposed to do now, you're immortal already, stupid!<br/>Wanna try that again?");
				goto L_Choose;

			case "@strength":
				Msg("Strength? What do you need strength for? This isn't... wait, this *is* G1? Oh my, alright, here you go.");
				Player.StatMods.Add(Stat.StrMod, 1000, StatModSource.Skill, 100000);
				Player.StatMods.Add(Stat.IntMod, 1000, StatModSource.Skill, 100000);
				Player.StatMods.Add(Stat.DexMod, 1000, StatModSource.Skill, 100000);
				Player.StatMods.Add(Stat.WillMod, 1000, StatModSource.Skill, 100000);
				Player.StatMods.Add(Stat.LuckMod, 1000, StatModSource.Skill, 100000);
				Send.StatUpdateDefault(Player);
				break;
		}

		Msg("Okay, we're done here. And don't you dare to tell anyone about me! Bye.<button title='Thanks!' keyword='@a' />");
		await Select();

		Player.Vars.Temp.Crane = true;
		NPC.WarpFlash(1001, 0, 0);

		End();
	}

	[On("PlayerReceivesItem")]
	public void OnPlayerReceivesItem(Creature creature, int itemId, int amount)
	{
		if (itemId != 52039 || creature.Inventory.Count(52039) != 1000)
			return;

		var npc = ChannelServer.Instance.World.GetNpc("Crane");
		if (npc == null)
		{
			Log.Error("ThousandCraneScript.OnPlayerReceivesItem: NPC not found.");
			return;
		}

		var pos = creature.GetPosition().GetRandomInRange(200, 500, RandomProvider.Get());

		npc.WarpFlash(creature.RegionId, pos.X, pos.Y);

		Task.Delay(3000).ContinueWith(_ =>
		{
			Send.NpcInitiateDialog(creature, npc.EntityId, npc.Name, npc.Name);
			creature.Client.NpcSession.StartTalk(npc, creature);
		});
	}
}
