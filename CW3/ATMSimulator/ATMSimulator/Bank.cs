using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace ATMSimulator
{
    /*
     * Bank class simulates the bank main computer
     */
    class Bank
    {
        private Account[] ac = new Account[3]; // create an array of bank accounts
        private const int numATMs = 2; // number of ATMs to run simultaneously
        private Thread[] atm = new Thread[numATMs]; // create an array of threads to run the ATMs in

        /*
         * Constructor initialises the bank accounts and threads
         * to run the ATMs concurrently
         */
        public Bank()
        {
            // initialise 3 bank accounts
            ac[0] = new Account(300, 1111, 111111);
            ac[1] = new Account(750, 2222, 222222);
            ac[2] = new Account(3000, 3333, 333333);

            // initialise and start the parallel threads
            for (int i = 0; i < numATMs; i++)
            {
                atm[i] = new Thread(new ThreadStart(runATM));
                atm[i].Start();
            }
        }

        /*
         * Method to be threaded
         */
        private void runATM()
        {
            Application.Run(new ATM(ac)); // create and run a new instance of the ATM form in the thread
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new Bank(); // create a new bank
        }
    }
}