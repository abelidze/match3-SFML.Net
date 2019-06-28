using System;

namespace Match3
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            GameManager.Instance.Start("Match3");
        }
    }
}