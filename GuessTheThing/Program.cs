namespace GuessTheThing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string filePos;
            if (args.Length >= 1)
                filePos = GetFilePos(args[0]);
            else
            {
                if (!Directory.Exists("./Games/"))
                    Directory.CreateDirectory("./Games/");

                ListFiles();
                Console.WriteLine("Enter file pos");
                filePos = GetFilePos(Console.ReadLine());
            }

            BinaryTree questions;
            if (File.Exists(filePos))
                questions = BinaryTree.Parse(File.ReadAllText(filePos));
            else
            {
                Console.WriteLine("This game does not exist, do you want to create it? (Y/N)");
                if (Console.ReadKey().Key != ConsoleKey.Y)
                    return;
                questions = CreateNewFile(filePos);
            }

            BinaryTree current = questions;

            while (true)
            {
                PlayGame(current);
                current = questions;

                Console.WriteLine("Good game\n");
                File.WriteAllText(filePos, current.ToString());
            }
        }

        private static BinaryTree CreateNewFile(string filePos)
        {
            BinaryTree questions;
            Console.WriteLine("Enter a possible outcome:");
            using (FileStream fs = File.Create(filePos))
                fs.Dispose();
            questions = new BinaryTree(Console.ReadLine());
            Console.WriteLine("Great! More outcomes will be added when more people play.");
            Console.WriteLine("\n\n");
            return questions;
        }

        private static void ListFiles()
        {
            Console.WriteLine("Games:\n");
            var files = Directory.GetFiles("./Games/");
            for (int i = 0; i < files.Length; i++)
                Console.WriteLine(Path.GetFileNameWithoutExtension(files[i]));
            Console.WriteLine();
        }

        private static string GetFilePos(string input)
        {
            if (input.EndsWith(".txt"))
                return input;
            return $"./Games/{input}.txt";
        }

        private static BinaryTree PlayGame(BinaryTree current)
        {
            while (current.Yes != null || current.No != null)
            {
                current = Guess(current);
                Console.Clear();
                continue;
            }

            Console.WriteLine($"Is the answer '{current.Question}'? (y/n)");

            var isGuessed = Console.ReadKey();
            Console.Clear();

            if (isGuessed.Key == ConsoleKey.Y)
                Console.WriteLine("I win again :)");
            else if (isGuessed.Key == ConsoleKey.N)
                current = AddNewHint(current);
            else
                Console.WriteLine("uhhh.. ok");

            return current;
        }

        private static BinaryTree AddNewHint(BinaryTree current)
        {
            Console.WriteLine("Well, what is it?");
            string trueAnswer = Console.ReadLine();
            Console.WriteLine($"What question should I ask to distinguish between a {trueAnswer} and a {current.Question}?");
            string questionToAsk = Console.ReadLine().Trim().Replace("|", "");
            Console.WriteLine("Would that be true or false?");
            bool shouldBeTrue = Console.ReadLine().ToLower() != "false";
            string oldResponse = current.Question;
            current.Question = questionToAsk;

            if (shouldBeTrue)
            {
                current.No = new(oldResponse);
                current.Yes = new(trueAnswer);
            }
            else
            {
                current.No = new(trueAnswer);
                current.Yes = new(oldResponse);
            }

            return current;
        }

        private static BinaryTree Guess(BinaryTree current)
        {
            Console.WriteLine(current.Question + " (y/n)");
            var result = Console.ReadKey();

            if (result.Key == ConsoleKey.Y)
                return current.Yes;
            else if (result.Key == ConsoleKey.N)
                return current.No;
            return current;
        }
    }
}