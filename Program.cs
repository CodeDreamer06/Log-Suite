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
              { 1, Read },
              { 2, Create },
              { 3, Delete },
              { 4, Update }
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

                    catch
                    {
                        Console.WriteLine("Not a command. Use 'help' if required. ");
                    }
                }
            }
        }

        private static void Create()
        {
            using (var db = new TransactionContext())
            {
                try
                {
                    var transactionData = AcceptInput("Customer Name", "Total Price");

                    db.Add(new Transaction { CustomerName = transactionData[0], TotalPrice = float.Parse(transactionData[1]) });
                    db.SaveChanges();

                    Console.WriteLine($"Successfuly added {transactionData[0]} with a bill of {transactionData[1]}");
                }

                catch
                {
                    Console.WriteLine("Make sure you are entering valid values");
                }
            }
        }

        private static void Read()
        {
            using (var db = new TransactionContext())
            {
                var transactions = db.transactions!.OrderBy(t => t.Id).ToList();
                for (int i = 0; i < transactions.Count; i++)
                {
                    var item = transactions[i];
                    Console.WriteLine($"{i+1}  {item.CustomerName}  {item.TotalPrice}   {item.CreatedOn}");
                }
            }
        }

        private static void Update()
        {
            using (var db = new TransactionContext())
            {
                var transactions = db.transactions!.OrderBy(t => t.Id).ToList();
                try
                {
                    var transactionData = AcceptInput("id", "new bill");
                    var absoluteId = transactions[Convert.ToInt32(transactionData[0]) - 1].Id;
                    var record = db.transactions!.Where(t => t.Id == absoluteId).First();

                    record.TotalPrice = Convert.ToInt32(transactionData[1]);
                    db.SaveChanges();

                    Console.WriteLine($"{record.CustomerName}'s bill has been changed to {record.TotalPrice} successfully");
                }

                catch
                {
                    Console.WriteLine("Make sure you are entering valid values");
                }
            }
        }

        private static void Delete()
        {
            using (var db = new TransactionContext())
            {
                var transactions = db.transactions!.OrderBy(t => t.Id).ToList();
                try
                {
                    var relativeId = AcceptInput("id")[0];
                    var absoluteId = transactions[Convert.ToInt32(relativeId) - 1].Id;
                    var record = db.transactions!.Where(t => t.Id == absoluteId).First();

                    db.Remove(record);
                    db.SaveChanges();

                    Console.WriteLine($"Successfully deleted the transaction of {record.CustomerName}");
                }

                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Make sure that the value you are entering is from existing logs");
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
        public TransactionContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Transaction>? transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source=transactions.db");
    }

    public class Transaction
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public float TotalPrice { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}