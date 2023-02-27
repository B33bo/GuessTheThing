using System.Text;

namespace GuessTheThing;

public class BinaryTree
{
    public BinaryTree Yes { get; set; }
    public BinaryTree No { get; set; }
    public string Question { get; set; }

    public BinaryTree()
    {
        Yes = null;
        No = null;
        Question = default;
    }

    public BinaryTree(BinaryTree yes, BinaryTree no, string question)
    {
        Yes = yes;
        No = no;
        Question = question;
    }

    public BinaryTree(string question)
    {
        Question = question.Trim();
    }

    public BinaryTree(string question, string yes, string no)
    {
        Question = question;
        Yes = new BinaryTree(yes);
        No = new BinaryTree(no);
    }

    public override string ToString()
    {
        if (Yes == null || No == null)
            return Question;

        StringBuilder s = new StringBuilder();

        s.Append(Question + "\n");
        var lines = GetLines();

        for (int i = 0; i < lines.Length; i++)
            s.Append("|" + lines[i] + "\n");
        return s.ToString();
    }

    private string[] GetLines()
    {
        if (Yes == null)
            return Array.Empty<string>();

        List<string> lines = new();

        var yes = Yes.GetLines();
        var no = No.GetLines();

        lines.Add(Yes.Question);

        for (int i = 0; i < yes.Length; i++)
            lines.Add("|" + yes[i]);

        lines.Add(No.Question);

        for (int i = 0; i < no.Length; i++)
            lines.Add("|" + no[i]);

        return lines.ToArray();
    }

    public static BinaryTree Parse(string s)
    {
        string[] lines = s.Split('\n');
        BinaryTree tree = new(lines[0]);

        Stack<BinaryTree> branches = new();
        branches.Push(tree);
        BinaryTree last = tree;

        int indentLevel = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim();

            if (string.IsNullOrEmpty(lines[i]))
                continue;

            int newIndentLevel = IndentLevel(lines[i]);

            if (indentLevel == newIndentLevel)
            {
                branches.Pop();
                last = branches.Peek();
                last.No = new(lines[i][newIndentLevel..]);
                last = last.No;

                branches.Push(last);

                indentLevel = newIndentLevel;
                continue;
            }

            if (newIndentLevel > indentLevel)
            {
                if (last.Yes == null)
                {
                    last.Yes = new BinaryTree(lines[i][newIndentLevel..]);
                    last = last.Yes;
                    branches.Push(last);
                }
                else
                {
                    last.No = new BinaryTree(lines[i][newIndentLevel..]);
                    last = last.No;
                    branches.Push(last);
                }

                indentLevel = newIndentLevel;
                continue;
            }

            if (newIndentLevel < indentLevel)
            {
                branches.Pop();
                last = branches.Peek();

                while (last.No != null)
                {
                    branches.Pop();
                    last = branches.Peek();
                }

                last.No = new BinaryTree(lines[i][newIndentLevel..]);
                last = last.No;
                branches.Push(last);

                indentLevel = newIndentLevel;
                continue;
            }
        }

        return tree;

        static int IndentLevel(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != '|')
                    return i;
            }

            return str.Length;
        }
    }
}
