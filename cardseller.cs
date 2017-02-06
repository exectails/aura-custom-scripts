//--- Aura Script -----------------------------------------------------------
// Card Seller
//--- Description -----------------------------------------------------------
// Sells premium card boxes that should be customized to give cards instead
// of items.
//---------------------------------------------------------------------------

public class CardSellerScript : NpcScript
{
	public override void Load()
	{
		SetRace(10002);
		SetName("Card Seller");
		SetFace(skinColor: 20, eyeColor: 27);
		SetLocation(1, 10613, 39427, 213);

		EquipItem(Pocket.Face, 4900, 0xF88B4A);
		EquipItem(Pocket.Hair, 4154, 0x4D4B53);
		EquipItem(Pocket.Robe, 95044, 0x00000000, 0x00FFFFFF, 0x00000000);
		EquipItem(Pocket.Shoe, 17012, 0x00000000, 0x00000000, 0x00000000);

		AddPhrase("Hey, bud. Come here.");
		AddPhrase("You look like someone interested in a bargain.");
		AddPhrase("Shhhh!");
		AddPhrase("You want to buy an eig- er, a card?");
	}

	protected override async Task Talk()
	{
		Msg("What can I do for you?");
		OpenShop("CardSellerShop");
	}
}

public class CardSellerShop : NpcShopScript
{
	public override void Setup()
	{
		Add("Character Cards", 92042); // Human
		Add("Character Cards", 92058); // Elf
		Add("Character Cards", 92072); // Giant
	}
}
