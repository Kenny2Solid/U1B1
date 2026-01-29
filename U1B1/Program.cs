using System;

namespace _02_Assignment
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n=== Arcade Card Simulator Lab ===\n");

            // Run the tests. You should NOT edit the tests.
            // TODO: Make these tests pass by completing the classes below.
            RunAllTests();

            Console.WriteLine("\nAll tests ran. If you see no FAIL messages, you're done.\n");
        }

        // ============================================================
        // TESTS (DO NOT EDIT)
        // ============================================================

        static void RunAllTests()
        {
            Test_ArcadeCard_Constructor_And_Properties();
            Test_ArcadeCard_LoadCredits_And_AddTickets();
            Test_ArcadeCard_PlayGame_Success_And_Fail();
            Test_ArcadeCard_Deactivate_BlocksActions();
            Test_ArcadeGame_Basics();
            Test_Prize_Redemption_Success_And_Fail();
        }

        static void Test_ArcadeCard_Constructor_And_Properties()
        {
            Console.WriteLine("TEST: ArcadeCard constructor + properties");

            int before = ArcadeCard.TotalCardsIssued;

            ArcadeCard card = new ArcadeCard("A100");

            AssertEqual("A100", card.CardId, "CardId should match constructor input");
            AssertEqual(0, card.Credits, "New card Credits should start at 0");
            AssertEqual(0, card.Tickets, "New card Tickets should start at 0");
            AssertEqual(true, card.IsActive, "New card should be active");

            AssertEqual(before + 1, ArcadeCard.TotalCardsIssued, "TotalCardsIssued should increment");

            Console.WriteLine("  PASS\n");
        }

        static void Test_ArcadeCard_LoadCredits_And_AddTickets()
        {
            Console.WriteLine("TEST: ArcadeCard LoadCredits + AddTickets");

            ArcadeCard card = new ArcadeCard("A200");

            card.LoadCredits(20);
            AssertEqual(20, card.Credits, "Credits should increase after LoadCredits");

            card.AddTickets(50);
            AssertEqual(50, card.Tickets, "Tickets should increase after AddTickets");

            // Negative loads should not change state (and should not crash)
            card.LoadCredits(-10);
            AssertEqual(20, card.Credits, "Credits should not change for negative LoadCredits");

            card.AddTickets(-999);
            AssertEqual(50, card.Tickets, "Tickets should not change for negative AddTickets");

            Console.WriteLine("  PASS\n");
        }

        static void Test_ArcadeCard_PlayGame_Success_And_Fail()
        {
            Console.WriteLine("TEST: ArcadeCard PlayGame success/fail");

            ArcadeCard card = new ArcadeCard("A300");
            card.LoadCredits(10);

            ArcadeGame skeeBall = new ArcadeGame("Skee Ball", creditCost: 3, ticketPayout: 10);

            bool played1 = card.TryPlayGame(skeeBall);
            AssertEqual(true, played1, "TryPlayGame should return true when enough credits");
            AssertEqual(7, card.Credits, "Credits should decrease by game cost");
            AssertEqual(10, card.Tickets, "Tickets should increase by game payout");

            bool played2 = card.TryPlayGame(skeeBall);
            AssertEqual(true, played2, "TryPlayGame should return true on second play");
            AssertEqual(4, card.Credits, "Credits should be 4 after second play");
            AssertEqual(20, card.Tickets, "Tickets should be 20 after second play");

            bool played3 = card.TryPlayGame(skeeBall);
            AssertEqual(true, played3, "TryPlayGame should return true on third play");
            AssertEqual(1, card.Credits, "Credits should be 1 after third play");
            AssertEqual(30, card.Tickets, "Tickets should be 30 after third play");

            // Now should fail (only 1 credit left, cost is 3)
            bool played4 = card.TryPlayGame(skeeBall);
            AssertEqual(false, played4, "TryPlayGame should return false when NOT enough credits");
            AssertEqual(1, card.Credits, "Credits should NOT change on failed play");
            AssertEqual(30, card.Tickets, "Tickets should NOT change on failed play");

            Console.WriteLine("  PASS\n");
        }

        static void Test_ArcadeCard_Deactivate_BlocksActions()
        {
            Console.WriteLine("TEST: Deactivate blocks actions");

            ArcadeCard card = new ArcadeCard("A400");
            card.LoadCredits(10);
            card.AddTickets(10);

            card.Deactivate();

            // Should not change after deactivation
            card.LoadCredits(999);
            card.AddTickets(999);

            ArcadeGame game = new ArcadeGame("Dance Dance", creditCost: 2, ticketPayout: 5);
            bool played = card.TryPlayGame(game);

            AssertEqual(false, played, "TryPlayGame should return false when card is inactive");
            AssertEqual(10, card.Credits, "Credits should not change when inactive");
            AssertEqual(10, card.Tickets, "Tickets should not change when inactive");

            Console.WriteLine("  PASS\n");
        }

        static void Test_ArcadeGame_Basics()
        {
            Console.WriteLine("TEST: ArcadeGame basics");

            ArcadeGame game = new ArcadeGame("Claw Machine", creditCost: 4, ticketPayout: 1);

            AssertEqual("Claw Machine", game.Name, "Game name should match");
            AssertEqual(4, game.CreditCost, "CreditCost should match");
            AssertEqual(1, game.TicketPayout, "TicketPayout should match");

            Console.WriteLine("  PASS\n");
        }

        static void Test_Prize_Redemption_Success_And_Fail()
        {
            Console.WriteLine("TEST: Prize redemption success/fail");

            ArcadeCard card = new ArcadeCard("A500");
            card.AddTickets(60);

            Prize gummyWorms = new Prize("Gummy Worms", ticketCost: 25, quantity: 2);

            bool redeemed1 = gummyWorms.TryRedeem(card);
            AssertEqual(true, redeemed1, "First redeem should succeed");
            AssertEqual(35, card.Tickets, "Tickets should decrease by prize cost");
            AssertEqual(1, gummyWorms.Quantity, "Prize quantity should decrease");

            bool redeemed2 = gummyWorms.TryRedeem(card);
            AssertEqual(true, redeemed2, "Second redeem should succeed");
            AssertEqual(10, card.Tickets, "Tickets should be 10 after second redeem");
            AssertEqual(0, gummyWorms.Quantity, "Prize quantity should be 0 after second redeem");

            // Should fail: out of stock
            bool redeemed3 = gummyWorms.TryRedeem(card);
            AssertEqual(false, redeemed3, "Redeem should fail when prize is out of stock");
            AssertEqual(10, card.Tickets, "Tickets should not change on failed redeem");
            AssertEqual(0, gummyWorms.Quantity, "Quantity should remain 0");

            // Another prize should fail: not enough tickets
            Prize bigBear = new Prize("Big Teddy Bear", ticketCost: 200, quantity: 5);
            bool redeemed4 = bigBear.TryRedeem(card);
            AssertEqual(false, redeemed4, "Redeem should fail when not enough tickets");
            AssertEqual(10, card.Tickets, "Tickets should not change when not enough tickets");
            AssertEqual(5, bigBear.Quantity, "Quantity should not change on failed redeem");

            Console.WriteLine("  PASS\n");
        }

        // Small assert helpers
        static void AssertEqual<T>(T expected, T actual, string message)
        {
            if (!Equals(expected, actual))
            {
                Console.WriteLine($"  FAIL: {message}");
                Console.WriteLine($"        Expected: {expected}");
                Console.WriteLine($"        Actual:   {actual}\n");
            }
        }
    }

    // ============================================================
    // PART A: ArcadeCard (Fields, Constructor, Properties)
    // ============================================================
    public class ArcadeCard
    {
        // Private backing fields
        private readonly string _cardId;
        private int _credits;
        private int _tickets;
        private bool _isActive;

        // Public static property TotalCardsIssued (already declared earlier in template)
        public static int TotalCardsIssued { get; private set; } = 0;

        // Public read-only properties
        public string CardId
        {
            get { return _cardId; }
        }

        public int Credits
        {
            get { return _credits; }
        }

        public int Tickets
        {
            get { return _tickets; }
        }

        public bool IsActive
        {
            get { return _isActive; }
        }

        public ArcadeCard(string cardId)
        {
            _cardId = cardId;
            _credits = 0;
            _tickets = 0;
            _isActive = true;
            TotalCardsIssued++;
        }

        // ============================================================
        // PART B: ArcadeCard Behaviors (Load, Tickets, Deactivate)
        // ============================================================

        public void LoadCredits(int amount)
        {
            // - If card is NOT active, do nothing.
            // - If amount <= 0, do nothing.
            // - Otherwise add amount to credits.
            if (!_isActive) return;
            if (amount <= 0) return;
            _credits += amount;
        }

        public void AddTickets(int amount)
        {
            // - If card is NOT active, do nothing.
            // - If amount <= 0, do nothing.
            // - Otherwise add amount to tickets.
            if (!_isActive) return;
            if (amount <= 0) return;
            _tickets += amount;
        }

        public void Deactivate()
        {
            // - Set IsActive to false
            _isActive = false;
        }

        // ============================================================
        // PART C: ArcadeCard + ArcadeGame interaction
        // ============================================================

        public bool TryPlayGame(ArcadeGame game)
        {
            // - If card is NOT active, return false.
            // - If game is null, return false.
            // - If Credits < game.CreditCost, return false.
            // - Otherwise:
            //     - subtract game.CreditCost from Credits
            //     - add game.TicketPayout to Tickets
            //     - return true
            if (!_isActive) return false;
            if (game == null) return false;
            if (_credits < game.CreditCost) return false;

            _credits -= game.CreditCost;
            _tickets += game.TicketPayout;
            return true;
        }

        // Internal helper for Prize redemption to safely spend tickets.
        internal bool TrySpendTickets(int amount)
        {
            if (!_isActive) return false;
            if (amount <= 0) return false;
            if (_tickets < amount) return false;
            _tickets -= amount;
            return true;
        }

        // ============================================================
        // PART E: Polish (Optional but recommended)
        // ============================================================

        public override string ToString()
        {
            // "Card A100 | Credits: 12 | Tickets: 40 | Active: True"
            return $"Card {CardId} | Credits: {Credits} | Tickets: {Tickets} | Active: {IsActive}";
        }
    }

    // ============================================================
    // PART C: ArcadeGame (Simple class)
    // ============================================================
    public class ArcadeGame
    {
        // Private fields
        private readonly string _name;
        private readonly int _creditCost;
        private readonly int _ticketPayout;

        // Read-only public properties
        public string Name
        {
            get { return _name; }
        }

        public int CreditCost
        {
            get { return _creditCost; }
        }

        public int TicketPayout
        {
            get { return _ticketPayout; }
        }

        public ArcadeGame(string name, int creditCost, int ticketPayout)
        {
            // - If creditCost < 0, treat it as 0
            // - If ticketPayout < 0, treat it as 0
            // - If name is null/empty/whitespace, set it to "Unnamed Game"
            if (string.IsNullOrWhiteSpace(name))
            {
                _name = "Unnamed Game";
            }
            else
            {
                _name = name;
            }

            _creditCost = creditCost < 0 ? 0 : creditCost;
            _ticketPayout = ticketPayout < 0 ? 0 : ticketPayout;
        }

        public override string ToString()
        {
            // "Skee Ball (Cost: 3, Payout: 10)"
            return $"{Name} (Cost: {CreditCost}, Payout: {TicketPayout})";
        }
    }

    // ============================================================
    // PART D: Prize (Redeem tickets for prizes)
    // ============================================================
    public class Prize
    {
        // Private fields
        private readonly string _name;
        private readonly int _ticketCost;
        private int _quantity;

        // Public read-only properties
        public string Name
        {
            get { return _name; }
        }

        public int TicketCost
        {
            get { return _ticketCost; }
        }

        public int Quantity
        {
            get { return _quantity; }
        }

        public Prize(string name, int ticketCost, int quantity)
        {
            // - ticketCost < 0 => 0
            // - quantity < 0 => 0
            // - null/empty name => "Unnamed Prize"
            if (string.IsNullOrWhiteSpace(name))
            {
                _name = "Unnamed Prize";
            }
            else
            {
                _name = name;
            }

            _ticketCost = ticketCost < 0 ? 0 : ticketCost;
            _quantity = quantity < 0 ? 0 : quantity;
        }

        public bool TryRedeem(ArcadeCard card)
        {
            // - If card is null => false
            // - If prize Quantity <= 0 => false
            // - If card is NOT active => false
            // - If card.Tickets < TicketCost => false
            // - Otherwise:
            //    - subtract TicketCost from card's tickets
            //    - decrease Quantity by 1
            //    - return true
            if (card == null) return false;
            if (_quantity <= 0) return false;
            if (!card.IsActive) return false;
            // Use the internal helper to attempt spending tickets atomically
            bool spent = card.TrySpendTickets(_ticketCost);
            if (!spent) return false;

            _quantity--;
            return true;
        }

        public override string ToString()
        {
            // "Gummy Worms (Cost: 25 tickets, Qty: 2)"
            return $"{Name} (Cost: {TicketCost} tickets, Qty: {Quantity})";
        }
    }
}

/*
PART E: Reflection (answer in COMMENTS)
------------------------------------------------------------
1) What is one advantage of making fields private and using properties?

// Making fields private and using properties lets you control and validate access to the data.

2) In your own words: what does it mean for a method to have a "side effect"?

//  A side effect is when a method modifies state outside of its local scope or interacts with the outside world
//

3) Where did you do input validation in this lab (name one place)? Why?

// TODO: In the ArcadeGame and Prize constructors we validate negative numbers and empty names (e.g., creditCost < 0 => 0).
// This stops bad state and makes later logic easier (it can expect non‑negative costs/amounts).
*/ 