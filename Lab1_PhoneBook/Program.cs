namespace Lab1_PhoneBook
{
    public static class Program {
        static void Main(string[] args)
        {
            UserDialog.DisplayGreetings("Welcome to my App");
            PhoneBook pb = new PhoneBook();
            RequestHandler rh = new RequestHandler(pb);
            while (rh.ActionChooser()) ;
        }
    }

}
