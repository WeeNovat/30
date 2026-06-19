using System;

namespace BankSystem
{
    public class Logger
    {
        private static Logger _instance;
        private static readonly object _lock = new object();

        private Logger() { }

        public static Logger Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Logger();
                    }
                    return _instance;
                }
            }
        }

        public void Log(string message)
        {
            Console.WriteLine($"[LOG {DateTime.Now:yyyy-MM-dd HH:mm:ss}]: {message}");
        }
    }

    public class BankAccount
    {
        public string AccountNumber { get; private set; }
        public decimal Balance { get; private set; }

        public BankAccount(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
            Logger.Instance.Log($"Створено рахунок №{AccountNumber} із початковим балансом {Balance} грн.");
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                Logger.Instance.Log($"Помилка: Спроба поповнення рахунку №{AccountNumber} на некоректну суму {amount} грн.");
                return;
            }

            Balance += amount;
            Logger.Instance.Log($"Рахунок №{AccountNumber} поповнено на {amount} грн. Новий баланс: {Balance} грн.");
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Logger.Instance.Log($"Помилка: Спроба зняття некоректної суми {amount} грн. з рахунку №{AccountNumber}.");
                return;
            }

            if (amount > Balance)
            {
                Logger.Instance.Log($"КРИТИЧНО: Спроба зняття {amount} грн. з рахунку №{AccountNumber} відхилена (недостатньо коштів).");
                throw new InvalidOperationException($"Недостатньо коштів на рахунку для зняття {amount} грн. Поточний баланс: {Balance} грн.");
            }

            Balance -= amount;
            Logger.Instance.Log($"З рахунку №{AccountNumber} знято {amount} грн. Залишок: {Balance} грн.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                BankAccount myAccount = new BankAccount("UA123456789", 5000m);

                myAccount.Deposit(1500m);
                myAccount.Withdraw(2000m);

                Console.WriteLine("\n--- Спроба зняти більше, ніж є на балансі ---");
                myAccount.Withdraw(6000m); 
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"\n[Перехоплено виняток у Main]: {ex.Message}");
            }

            Console.ReadLine();
        }
    }
}
