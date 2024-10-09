using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMProject
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            BankATM.Start();
        }
    }

    public static class BankATM
    {
        public struct Client
        {
            public string AcNumber;
            public string Name;
            public string Nip;
            public int Balance;
        }

        static Client[] tabClients;
        static int nbClients;
        static string account = "";

        public static void Start()
        {
            int choice = 0, PassAttempts = 0, attempts = 0, maxAttempts = 3;
            string pass;
            Single deposer, retirer = 0;
            // Find the client's index in the array
            int clientIndex = -1;

            ReadFileToArray();


            while (choice != 4)
            {
                Console.Clear();
                DisplayGradientTitle("BANQUE ROYALE");
                DisplaySubTitle("Guichet Automatique Bancaire");

                //Check account
                bool found = false;
                bool foundPass = false;

                while (!found && attempts < maxAttempts)
                {
                    do
                    {
                        Console.Write("Entrez votre numero de compte : ");
                        account = Console.ReadLine();
                    } while (account.Length == 0);

                    for (int i = 0; i < nbClients; i++)
                    {
                        if (tabClients[i].AcNumber.ToUpper() == account.ToUpper().Trim())
                        {
                            DisplaySubTitle("Bienvenue " + tabClients[i].Name);
                            found = true;
                            break; // Exit the loop if account is found
                        }
                    }

                    if (!found)
                    {
                        attempts++;
                        if (attempts < maxAttempts)
                        {
                            Console.WriteLine("Désolé!! Il n'y a pas de compte avec ce numéro " + account);
                            DisplayWrong("Vous avez encore " + (maxAttempts - attempts) + " tentative(s).");
                        }
                        else
                        {
                            DisplayWrong("Vous avez atteint le nombre maximal de tentatives. Au revoir!");
                            choice = 4;
                        }
                    }
                }

                // If the account was found, check the password
                if (found)
                {
                    while (!foundPass && PassAttempts < maxAttempts)
                    {
                        do
                        {
                            Console.Write("Entrez Votre Nip : ");
                            pass = Console.ReadLine();
                        } while (pass.Length == 0);

                        for (int i = 0; i < nbClients; i++)
                        {
                            if (tabClients[i].AcNumber.ToUpper() == account.ToUpper().Trim() &&
                                tabClients[i].Nip.ToUpper() == pass.ToUpper().Trim())

                            {
                                foundPass = true;
                                break; // Exit the loop if Nip is found
                            }
                        }

                        if (!foundPass)
                        {
                            PassAttempts++;
                            if (PassAttempts < maxAttempts)
                            {
                                Console.WriteLine("Désolé!! Votre Nip est erroné ");
                                DisplayWrong("Vous avez encore " + (maxAttempts - PassAttempts) + " tentative(s).");
                            }
                            else
                            {
                                DisplayWrong("Vous avez atteint le nombre maximal de tentatives. Au revoir!");
                                choice = 4;
                            }
                        }
                    }
                }

                //Menu
                if (foundPass)
                {
                    Console.Clear();
                    DisplayGradientTitle("BANQUE ROYALE");
                    if (clientIndex != -1)
                    {
                        DisplaySubTitle(tabClients[clientIndex].Name + ", s'il te plaît choisir votre Transaction");
                    }
                    Console.Write("\t1 - Pour Deposer\n\t2 - Pour Retirer\n\t3 - Pour Consulter\n\n\tEntrez votre choix <1 - 3> : ");
                    choice = Convert.ToInt16(Console.ReadLine());

                    //Deposit
                    if (choice == 1)
                    {
                        Console.Write("Entrez le montant a Deposer $ : ");
                        deposer = Convert.ToSingle(Console.ReadLine());
                        while (deposer < 20 || deposer > 20000)
                        {
                            DisplayWrong("Désolé, entrez un montant entre $ 20 $ et $20 000.");
                            Console.Write("Entrez le montant a Deposer $ : ");
                            deposer = Convert.ToSingle(Console.ReadLine());
                        }

                        // Update the balance in the tabClients array
                        if (clientIndex != -1)
                        {
                            tabClients[clientIndex].Balance += (int)deposer;
                        }

                        // Update the client.txt file with the updated balance
                        WriteArrayToFile(account);
                        DisplaySubTitle("La transaction a réussi");
                        Consulter(account);
                        break;

                    }

                    //Withdraw
                    else if (choice == 2)
                    {
                        Console.Write("Entrez le montant a Retirer $ : ");
                        retirer = Convert.ToSingle(Console.ReadLine());

                        while (retirer < 20 || retirer % 20 != 0 || retirer > 500 || retirer > tabClients[clientIndex].Balance)
                        {
                            DisplayWrong("Désolé, entrez un montant entre $20 et $500, multiple de $20, et ne dépassez pas votre solde $ " + tabClients[clientIndex].Balance);
                            Console.Write("Entrez le montant a Retirer $ : ");
                            retirer = Convert.ToSingle(Console.ReadLine());
                        }

                        // Update the balance after withdrawal
                        tabClients[clientIndex].Balance -= (int)retirer;

                        // Update the client.txt file with the updated balance
                        WriteArrayToFile(account);
                        DisplaySubTitle("La transaction a réussi");
                        Consulter(account);
                        break;
                    }

                    //Balance
                    else if (choice == 3)
                    {
                        Consulter(account);
                        break;
                    }
                }
            }
            WriteArrayToFile(account);
            // the quit choice
            DisplayBye("Merci d'avoir utilisé nos services.  =D");

        }


        //Save at TXT file
        private static void WriteArrayToFile(string account)
        {
            StreamWriter myfile = new StreamWriter("client.txt");
            for (int i = 0; i < nbClients; i++)
            {
                myfile.WriteLine(tabClients[i].AcNumber);
                myfile.WriteLine(tabClients[i].Name);
                myfile.WriteLine(tabClients[i].Nip);
                myfile.WriteLine(tabClients[i].Balance);
            }
            myfile.Close();
        }

        //Read the TXT file
        public static void ReadFileToArray()
        {
            // open the file to read the number of lines
            int nbLines = 0;
            StreamReader myfile = new StreamReader("client.txt");
            while (myfile.EndOfStream == false)
            {
                string tmp = myfile.ReadLine();
                nbLines++;
            }
            myfile.Close();

            int size = (nbLines / 4) + 4;
            tabClients = new Client[size]; // dynamic array with clients

            //Fill the tab with information
            myfile = new StreamReader("client.txt");
            int i = 0;
            while (myfile.EndOfStream == false)
            {
                tabClients[i].AcNumber = myfile.ReadLine();
                tabClients[i].Name = myfile.ReadLine();
                tabClients[i].Nip = myfile.ReadLine();
                tabClients[i].Balance = Convert.ToInt32(myfile.ReadLine());
                i++;
            }
            myfile.Close();
            nbClients = i;
        }

        //Titles
        public static void DisplayGradientTitle(string titleName)
        {
            int halfLength = titleName.Length / 2;

            Console.Write("\t\t");
            for (int i = 0; i < titleName.Length; i++)
            {
                Console.Write("-");
            }
            Console.Write("\n\n");
            Console.Write("\t\t");

            for (int i = 0; i < titleName.Length; i++)
            {
                if (i < halfLength)
                {
                    Console.Write("\u001b[33m"); // Yellow
                }
                else
                {
                    Console.Write("\u001b[32m"); // Green
                }

                Console.Write(titleName[i]);
                Console.Write("\u001b[0m"); // Reset color
            }

            Console.Write("\n\n\t\t");
            for (int i = 0; i < titleName.Length; i++)
            {
                Console.Write("-");
            }
            Console.Write("\n");
        }

        public static void DisplaySubTitle(string subtitleName)
        {
            Console.WriteLine("");
            Console.WriteLine("\t" + subtitleName);
            Console.Write("\t");
            for (int i = 0; i < subtitleName.Length; i++)
            {
                Console.Write("-");
            }
            Console.Write("\n\n");
        }

        public static void DisplayWrong(string wrongtitleName)
        {
            Console.WriteLine("\t  \u001b[31m" + wrongtitleName + "\u001b[0m"); // Red
        }

        public static void DisplayBye(string byetitleName)
        {
            Console.WriteLine("\t  \u001b[32m" + byetitleName + "\u001b[0m"); // Green
        }

        //Choice 3
        private static void Consulter(string account)
        {
            for (int i = 0; i < nbClients; i++)
            {
                if (tabClients[i].AcNumber.ToUpper() == account.ToUpper().Trim())
                {
                    Console.WriteLine("\t Numero : " + tabClients[i].AcNumber);
                    Console.WriteLine("\t Client : " + tabClients[i].Name);
                    Console.WriteLine("\t Nip : " + tabClients[i].Nip);
                    Console.WriteLine("\t Solde $ : " + tabClients[i].Balance);
                    Console.WriteLine("");
                }
            }
        }
    }
}
