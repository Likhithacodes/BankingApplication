using System;
using System.Collections.Generic;

namespace BankingApp
{
    // User Class
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Account> Accounts { get; set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
            Accounts = new List<Account>();
        }
    }

    // Account Class
    public class Account
    {
        public string AccountNumber { get; private set; }
        public string AccountHolderName { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; private set; }
        public List<Transaction> Transactions { get; private set; }

        private static int accountNumberStartFrom = 1000;

        public Account(string accountHolderName, string accountType, decimal initialDeposit)
        {
            AccountHolderName = accountHolderName;
            AccountType = accountType;
            Balance = initialDeposit;
            AccountNumber = GenerateAccountNumber();
            Transactions = new List<Transaction>();
            Transactions.Add(new Transaction("Deposit", initialDeposit));
        }

        private string GenerateAccountNumber()
        {
            return (++accountNumberStartFrom).ToString();
        }

        public void Deposit(decimal amount)
        {
            Balance += amount;
            Transactions.Add(new Transaction("Deposit", amount));
        }

        public bool Withdraw(decimal amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                Transactions.Add(new Transaction("Withdrawal", amount));
                return true;
            }
            else
            {
                Console.WriteLine("Insufficient funds. Withdrawal failed.");
                return false;
            }
        }

        public void CalculateInterest(decimal interestRate)
        {
            if (AccountType.ToLower() == "savings")
            {
                decimal interest = Balance * interestRate;
                Balance += interest;
                Transactions.Add(new Transaction("Interest", interest));
                Console.WriteLine($"Interest of {interest} added to the account {AccountNumber}.");
            }
            else
            {
                Console.WriteLine("Interest can only be calculated for savings accounts.");
            }
        }
    }

    // Transaction Class
    public class Transaction
    {
        public string Id { get; private set; }
        public string Type { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }

        public Transaction(string type, decimal amount)
        {
            Id = Guid.NewGuid().ToString();
            Type = type;
            Amount = amount;
            Date = DateTime.Now;
        }
    }

    // Main Program Class
    class Program
    {
        private static List<User> users = new List<User>();
        private static User currentUser = null;

        static void Main()
        {
            bool exit = false;
            while (!exit)
            {
                if (currentUser == null)
                {
                    Console.WriteLine("\n---Banking Application ---");
                    Console.WriteLine("1. Register");
                    Console.WriteLine("2. Login");
                    Console.WriteLine("3. Exit");
                    Console.Write("Choose an option: ");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            Register();
                            break;
                        case "2":
                            Login();
                            break;
                        case "3":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\n---Banking Application Menu---");
                    Console.WriteLine($"\nWelcome, {currentUser.Username}");
                    Console.WriteLine("1. Open New Account");
                    Console.WriteLine("2. Deposit");
                    Console.WriteLine("3. Withdraw");
                    Console.WriteLine("4. Check Balance");
                    Console.WriteLine("5. Generate Statement");
                    Console.WriteLine("6. Calculate Interest");
                    Console.WriteLine("7. Logout");
                    Console.Write("Choose an option: ");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            OpenAccount();
                            break;
                        case "2":
                            Deposit();
                            break;
                        case "3":
                            Withdraw();
                            break;
                        case "4":
                            CheckBalance();
                            break;
                        case "5":
                            GenerateStatement();
                            break;
                        case "6":
                            CalculateInterest();
                            break;
                        case "7":
                            currentUser = null;
          
                            Console.WriteLine("Logged out successfully.");
                            Console.Clear();
                            break;
                        default:
                            Console.WriteLine("Invalid option. Try again.");
                            break;
                    }
                }
            }
        }

        static void Register()
        {
            Console.WriteLine("\n--- Register ---");
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            if (users.Exists(user => user.Username == username))
            {
                Console.WriteLine("User already exists. Please try logging in or use another username.");
                return;
            }

            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            User newUser = new User(username, password);
            users.Add(newUser);
            Console.WriteLine("User registered successfully. You are now logged in.");
        }

        static void Login()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            currentUser = users.Find(user => user.Username == username && user.Password == password);
            Console.WriteLine(currentUser != null ? "Login successful." : "Invalid credentials.");
        }

        static void OpenAccount()
        {
            Console.Clear();
            Console.WriteLine("\n--- Open Account ---");
            Console.Write("Enter account holder's name: ");
            string name = Console.ReadLine();
            Console.Write("Enter account type (savings/checking): ");
            string type = Console.ReadLine();

            while (type != "savings" && type != "checking")
            {
                Console.WriteLine("Invalid account type. Please enter 'savings' or 'checking'.");
                Console.Write("Enter account type (savings/checking): ");
                type = Console.ReadLine();
            }

            Console.Write("Enter initial deposit: ");
            decimal initialDeposit = decimal.Parse(Console.ReadLine());

            Account newAccount = new Account(name, type, initialDeposit);

            if (currentUser.Accounts.Count < 2)
            {
                currentUser.Accounts.Add(newAccount);
                Console.WriteLine($"Account created successfully. Account Number: {newAccount.AccountNumber}");
            }
            else
            {
                Console.WriteLine("Only two accounts per user are allowed.");
            }
        }

        static void Deposit()
        {
            Console.Clear();
            Console.WriteLine("\n--- Deposit ---");
            Account account = SelectAccount();
            if (account == null) return;

            Console.Write("Enter deposit amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());
            account.Deposit(amount);
            Console.WriteLine("Deposit successful.");
        }

        static void Withdraw()
        {
            Console.Clear();
            Console.WriteLine("\n--- Withdraw ---");
            Account account = SelectAccount();
            if (account == null) return;

            Console.Write("Enter withdrawal amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());
            if (!account.Withdraw(amount))
            {
                Console.WriteLine("Withdrawal failed due to insufficient funds.");
            }
        }

        static void CheckBalance()
        {
            Console.Clear();
            Console.WriteLine("\n--- Check Balance ---");
            Account account = SelectAccount();
            if (account == null) return;

            Console.WriteLine($"Current balance: {account.Balance}");
        }

        static void GenerateStatement()
        {
            Console.Clear();
            Console.WriteLine("\n--- Transaction History ---");
            Account account = SelectAccount();
            if (account == null) return;

            foreach (var transaction in account.Transactions)
            {
                Console.WriteLine($"{transaction.Date}: {transaction.Type} of {transaction.Amount}");
            }
        }

        static void CalculateInterest()
        {
            Console.Clear();
            Console.WriteLine("\n--- Calculate Interest ---");
            Account account = SelectAccount();
            if (account == null) return;

            Console.Write("Enter interest rate (e.g., 0.05 for 5%): ");
            decimal interestRate = decimal.Parse(Console.ReadLine());
            account.CalculateInterest(interestRate);
        }

        static Account SelectAccount()
        {
            if (currentUser.Accounts.Count == 0)
            {
                Console.WriteLine("No accounts found.");
                return null;
            }

            Console.WriteLine("Select an account by number:");
            foreach (var account in currentUser.Accounts)
            {
                Console.WriteLine($"- {account.AccountNumber}");
            }

            string selectedAccount = Console.ReadLine();
            return currentUser.Accounts.Find(a => a.AccountNumber == selectedAccount);
        }
    }
}
