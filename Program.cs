using Microsoft.EntityFrameworkCore;

namespace LogSuite
{
    internal class Program
    {
        private static string command = "";

        public static string HelpMessage = @"
1 - Show all transactions
2 - Add a new transaction
3 - Remove a transaction
4 - Edit a transaction
0 - Go back";

        private static void Main()
        {
            Console.WriteLine(HelpMessage);

            var actions = new Dictionary<int, Action>
            {
              { 1, ShowAll },
              { 2, Add },
              { 3, Remove },
              { 4, Edit }
            };

            while (true)
            {
                command = Console.ReadLine()!.ToLower().Trim();

                if (command == "exit" || command == "0") break;

                else if (command == "help") Console.WriteLine(HelpMessage);

                else if (string.IsNullOrWhiteSpace(command)) continue;

                else
                {
                    try
                    {
                        actions[Convert.ToInt32(command)]();
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Console.WriteLine("Not a command. Use 'help' if required. ");
                    }
                }
            }



            //using (var db = new TransactionContext())
            //{
            //    Console.WriteLine($"Database path {db.DbPath}");

            //    // Create
            //    Console.WriteLine("Inserting a new transaction");
            //    db.Add(new Transaction { CustomerName = "Kevin" });
            //    db.SaveChanges();

            //    // Read
            //    Console.WriteLine("Querying for a transaction");
            //    var transaction = db.transactions.OrderBy(t => t.Id).First();
            //    Console.WriteLine(transaction!.CustomerName);

            //    // Update
            //    Console.WriteLine("Updating a transaction by adding a product");
            //    transaction.Items?.Add(new Product { Name = "Pen", Price = 69 });
            //    db.SaveChanges();
            //    Console.WriteLine($"{transaction.Items?.First().Name} {transaction.Items?.First().Price}");

            //    // Delete
            //    Console.WriteLine("Delete the transaction");
            //    db.Remove(transaction);
            //    db.SaveChanges();
            //}
        }

        private static void Edit()
        {
            throw new NotImplementedException();
        }

        private static void Remove()
        {
            throw new NotImplementedException();
        }

        private static void Add()
        {
            using (var db = new TransactionContext())
            {
                try
                {
                    var transactionData = AcceptInput("Customer Name", "Total Price");
                    db.Add(new Transaction { CustomerName = transactionData[0], TotalPrice = float.Parse(transactionData[1]) });
                    db.SaveChanges();
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Make sure you are entering valid values");
                }
            }
        }

        private static void ShowAll()
        {
            using (var db = new TransactionContext())
            {
                foreach (var item in db.transactions)
                {
                    Console.WriteLine($"{item.Id}  {item.CustomerName}  {item.TotalPrice}   {item.CreatedOn}");
                }
            }
        }

        private static string[] AcceptInput(params string[] fields)
        {
            var data = new string[fields.Length];

            for (int i = 0; i < data.Length; i++)
            {
                Console.Write($"{fields[i]}: ");
                data[i] = Console.ReadLine()!.Trim();
            }

            return data;
        }

    }

    public class TransactionContext : DbContext
    {
        public DbSet<Transaction> transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source=./transactions.db");
    }

    public class Transaction
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public float TotalPrice { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}