using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATMSimulator
{
    public class Account
    {
        // attributes of the account
        private int balance;
        private int pin;
        private int accountNum;
        private Boolean locked; // prevents data race when withdrawing money

        // constructor takes the initial values of the account's attributes
        public Account(int balance, int pin, int accountNum)
        {
            this.balance = balance;
            this.pin = pin;
            this.accountNum = accountNum;
            locked = false; // account initially unlocked
        }        

        /*
         * Checks the account's pin against the argument passed to it
         * 
         * returns:
         * true if the match
         * false if they don't match
         */
        public Boolean checkPin(string input)
        {
            // user inputs pin as a string
            // convert string to int to compare to the account's pin
            int pinEntered = Convert.ToInt32(input);

            if (pinEntered == pin)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /*
         * Decrements the account's balance by the amount given.
         * Checks that the balance is greater than the amount to be debited before
         * making the change
         * 
         * returns:
         * true if the account was debited by the amount given successfully
         * false if there are insufficient funds in the account
         */
        public Boolean decrementBalance(int amount)
        {
            if (this.balance >= amount)
            {
                balance -= amount;
                return true;
            }
            else
            {
                return false;
            }
        }

        // get and set methods for the account's attributes

        public int getBalance()
        {
            return balance;
        }

        public void setBalance(int newBalance)
        {
            this.balance = newBalance;
        }

        public int getAccountNum()
        {
            return accountNum;
        }

        public Boolean getLocked()
        {
            return locked;
        }

        public void setLocked(Boolean newLocked)
        {
            this.locked = newLocked;
        }
    }
}
