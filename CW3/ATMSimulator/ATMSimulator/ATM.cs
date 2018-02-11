using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ATMSimulator
{
    public partial class ATM : Form
    {
        /****************************************************************
         * Boolean used to simulate/fix concurrency problems
         * 
         * true to simulate concurrency problems due to data racing
         * false to enforce mutual exclusion to prevent race conditions
         */
        private Boolean dataRace = false;
        //****************************************************************

        // local reference to the array of accounts
        private Account[] ac;

        // local reference to the account being accessed
        private Account activeAccount = null;

        // keeps track of the current screen displayed on the ATM
        private string currentScreen = "ENTER_ACCOUNT";

        // stores the attempted pin entry when signing into an account
        private string pin = "";

        // constructor takes an array of bank accounts
        public ATM(Account[] ac)
        {
            this.ac = ac;
            InitializeComponent();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            // screen reached after the user has completed an action
            if (currentScreen == "POST_TRANSACTION")
            {
                // return to the main menu
                currentScreen = "MAIN_MENU";
                flipVisibleScreen();
                displayOptions();
            }
            // first screen that prompts user for an account number
            else if (currentScreen == "ENTER_ACCOUNT")
            {
                // attempts to find a matching account (null if no match)
                activeAccount = this.findAccount(lblScreen.Text);

                // valid account found
                if (activeAccount != null)
                {
                    // move to screen to prompt for PIN number
                    currentScreen = "ENTER_PIN";
                    lblMessage.Text = "Enter your pin number: ";
                    lblScreen.Text = "";
                }
                else
                {
                    lblMessage.Text = "Invalid account number. Please try again.";
                    lblScreen.Text = "";
                }
            }
            // second screen that prompts user for an account number
            else if (currentScreen == "ENTER_PIN")
            {
                // check if the pin matches the account
                if (activeAccount.checkPin(pin))
                {
                    // successfully accessed account
                    currentScreen = "MAIN_MENU";
                    flipVisibleScreen();
                    displayOptions();
                }
                else
                {
                    lblMessage.Text = "Invalid pin number. Please try again.";
                    pin = ""; // reset the string storing the pin attempt
                    lblScreen.Text = "";
                }
            }
            else if (currentScreen == "CHECK_BALANCE")
            {
                // return from the check balance screen to the main menu
                flipVisibleScreen();
                currentScreen = "MAIN_MENU";
                displayOptions();
            }
            else if (currentScreen == "WITHDRAW_CASH")
            {
                // return from the withdraw cash screen to the main menu
                currentScreen = "MAIN_MENU";
                flipVisibleScreen();
                displayOptions();
            }
        }

        private void displayOptions()
        {
            if (currentScreen == "POST_TRANSACTION")
            {
                // enable keypad buttons to let the user return to
                // the main menu or quit
                btnEnter.Enabled = true;
                btnClear.Enabled = true;
            }
            else
            {
                clearScreen();
                lockKeypad(); // keypad has no function after signing in to an account so disable keypad

                // display menu options depending on which screen the user is at
                if (currentScreen == "MAIN_MENU")
                {
                    lblMessage.Text = "Welcome, " + activeAccount.getAccountNum() + "!";
                    lblOption1.Text = "> Withdraw cash";
                    lblOption2.Text = "Check balance <";
                    lblOption3.Text = "> Exit";
                }
                else if (currentScreen == "WITHDRAW_CASH")
                {
                    // if mutual exclusion is being enforced, check if the account is unlocked before allowing access
                    if (activeAccount.getLocked() == true && dataRace == false)
                    {
                        flipVisibleScreen();
                        lblScreen.Text = "Error: another user is currently withdrawing cash from this account.\nPress Enter to make another transaction, press Clear to exit.";
                        currentScreen = "POST_TRANSACTION";
                        displayOptions();
                    }
                    else
                    {
                        activeAccount.setLocked(true);

                        // withdraw cash
                        lblMessage.Text = "How much do you want to withdraw?";
                        lblOption1.Text = "> £10";
                        lblOption2.Text = "£20 <";
                        lblOption3.Text = "> £40";
                        lblOption4.Text = "£100 <";
                        lblOption5.Text = "> £500";
                        lblOption6.Text = "Main Menu <";
                    }
                }
                else if (currentScreen == "CHECK_BALANCE")
                {
                    lblMessage.Text = "Current balance";

                    // display balance
                    if (this.activeAccount != null)
                    {
                        flipVisibleScreen();
                        lblScreen.Text = "Current balance: £" + activeAccount.getBalance() + ".\nPress Enter to make another transaction, press Clear to exit.";
                        currentScreen = "POST_TRANSACTION";
                        displayOptions();
                    }
                }
            }
        }

        /*
         * The Clear keypad button is only used to clear input on the 'enter account number'
         * and 'enter pin' screens. It acts as a quit button in all other cases
         */
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (currentScreen == "ENTER_PIN" || currentScreen == "ENTER_ACCOUNT")
            {
                lblScreen.Text = "";
                pin = ""; // reset the uncensored pin being stored
            }
            else
            {
                // exit
                ActiveForm.Close();
            }
        }

        private void btnOption1_Click(object sender, EventArgs e)
        {
            // if clicked in the main menu, the user wants to withdraw cash
            if (currentScreen == "MAIN_MENU")
            {
                currentScreen = "WITHDRAW_CASH";
                displayOptions();
            }
            // if clicked in the withdraw cash window, the user wants to withdraw £10 cash
            else if (currentScreen == "WITHDRAW_CASH")
            {
                flipVisibleScreen();

                // decrementBalance returns true if the transaction was successful
                // false if insufficient funds in the account
                if (activeAccount.decrementBalance(10))
                {
                    lblScreen.Text = "£10 withdrawn. New balance: £" + activeAccount.getBalance() + ".\nPress Enter to make another transaction, press Clear to exit.";
                }
                else
                {
                    lblScreen.Text = "Insufficient funds. Press Enter to make another transaction, press Clear to exit.";
                }

                // transaction has completed (successfully/unsuccessfully) so unlock the
                // active account to allow access by other threads
                activeAccount.setLocked(false);
                currentScreen = "POST_TRANSACTION";
                displayOptions();
            }
        }

        private void btnOption2_Click(object sender, EventArgs e)
        {
            // if clicked in the main menu, the user wants to check their balance
            if (currentScreen == "MAIN_MENU")
            {
                currentScreen = "CHECK_BALANCE";
                displayOptions();
            }
            else if (currentScreen == "WITHDRAW_CASH")
            {
                // if clicked in the withdraw cash window, the user wants to withdraw £20 cash
                flipVisibleScreen();

                // decrementBalance returns true if the transaction was successful
                // false if insufficient funds in the account
                if (activeAccount.decrementBalance(20))
                {
                    lblScreen.Text = "£20 withdrawn. New balance: £" + activeAccount.getBalance() + ".\nPress Enter to make another transaction, press Clear to exit.";
                }
                else
                {
                    lblScreen.Text = "Insufficient funds. Press Enter to make another transaction, press Clear to exit.";
                }

                // transaction has completed (successfully/unsuccessfully) so unlock the
                // active account to allow access by other threads
                activeAccount.setLocked(false);
                currentScreen = "POST_TRANSACTION";
                displayOptions();
            }
        }

        private void btnOption3_Click(object sender, EventArgs e)
        {
            // if clicked in the main menu, the user wants to exit the ATM
            if (currentScreen == "MAIN_MENU")
            {
                ActiveForm.Close();
            }
            // if clicked in the withdraw cash window, the user wants to withdraw £40 cash
            else if (currentScreen == "WITHDRAW_CASH")
            {
                flipVisibleScreen();

                // decrementBalance returns true if the transaction was successful
                // false if insufficient funds in the account
                if (activeAccount.decrementBalance(40))
                {
                    lblScreen.Text = "£40 withdrawn. New balance: £" + activeAccount.getBalance() + ".\nPress Enter to make another transaction, press Clear to exit.";
                }
                else
                {
                    lblScreen.Text = "Insufficient funds. Press Enter to make another transaction, press Clear to exit.";
                }

                // transaction has completed (successfully/unsuccessfully) so unlock the
                // active account to allow access by other threads
                activeAccount.setLocked(false);
                currentScreen = "POST_TRANSACTION";
                displayOptions();
            }
        }

        private void btnOption4_Click(object sender, EventArgs e)
        {
            // if clicked in the withdraw cash window, the user wants to withdraw £100 cash
            if (currentScreen == "WITHDRAW_CASH")
            {
                flipVisibleScreen();

                // decrementBalance returns true if the transaction was successful
                // false if insufficient funds in the account
                if (activeAccount.decrementBalance(100))
                {
                    lblScreen.Text = "£100 withdrawn. New balance: £" + activeAccount.getBalance() + ".\nPress Enter to make another transaction, press Clear to exit.";
                }
                else
                {
                    lblScreen.Text = "Insufficient funds. Press Enter to make another transaction, press Clear to exit.";
                }

                // transaction has completed (successfully/unsuccessfully) so unlock the
                // active account to allow access by other threads
                activeAccount.setLocked(false);
                currentScreen = "POST_TRANSACTION";
                displayOptions();
            }
        }

        private void btnOption5_Click(object sender, EventArgs e)
        {
            // if clicked in the withdraw cash window, the user wants to withdraw £500 cash
            if (currentScreen == "WITHDRAW_CASH")
            {
                flipVisibleScreen();

                // decrementBalance returns true if the transaction was successful
                // false if insufficient funds in the account
                if (activeAccount.decrementBalance(500))
                {
                    lblScreen.Text = "£500 withdrawn. New balance: £" + activeAccount.getBalance() + ".\nPress Enter to make another transaction, press Clear to exit.";
                }
                else
                {
                    lblScreen.Text = "Insufficient funds. Press Enter to make another transaction, press Clear to exit.";
                }

                // transaction has completed (successfully/unsuccessfully) so unlock the
                // active account to allow access by other threads
                activeAccount.setLocked(false);
                currentScreen = "POST_TRANSACTION";
                displayOptions();
            }
        }

        private void btnOption6_Click(object sender, EventArgs e)
        {
            // if clicked in the withdraw cash window, the user wants to return to the main menu
            if (currentScreen == "WITHDRAW_CASH")
            {
                // transaction has completed (successfully/unsuccessfully) so unlock the
                // active account to allow access by other threads
                activeAccount.setLocked(false);
                currentScreen = "MAIN_MENU";
                displayOptions();
            }
        }

        /*
         * Method checks the given input of an account number against
         * those held in the array of bank accounts
         * 
         * returns
         * the matching Account if found
         * null if no match is found
         */
        private Account findAccount(string input)
        {
            // user inputs account number as a string
            // convert string to int to compare to the account numbers
            int account_num = Convert.ToInt32(input);

            for (int i = 0; i < this.ac.Length; i++)
            {
                if (ac[i].getAccountNum() == account_num)
                {
                    return ac[i];
                }
            }

            return null;
        }

        /*
         * Method to change the visibility of labels on the form to
         * show full-screen messages or ATM options
         */
        private void flipVisibleScreen()
        {
            if (lblScreen.Visible == true)
            {
                lblScreen.Visible = false;
                lblOption1.Visible = true;
                lblOption2.Visible = true;
                lblOption3.Visible = true;
                lblOption4.Visible = true;
                lblOption5.Visible = true;
                lblOption6.Visible = true;
            }
            else
            {
                lblScreen.Visible = true;
                lblOption1.Visible = false;
                lblOption2.Visible = false;
                lblOption3.Visible = false;
                lblOption4.Visible = false;
                lblOption5.Visible = false;
                lblOption6.Visible = false;
            }
        }

        /*
         * Method to clear the text in all the on-screen labels
         * Prevents leftover options from previous screens being
         * displayed when the screen changes
         */
        private void clearScreen()
        {
            lblScreen.Text = "";
            lblMessage.Text = "";
            lblOption1.Text = "";
            lblOption2.Text = "";
            lblOption3.Text = "";
            lblOption4.Text = "";
            lblOption5.Text = "";
            lblOption6.Text = "";
        }

        /*
         * Method to lock the keypad buttons by changing their Enabled property
         */
        private void lockKeypad()
        {
            btnKey1.Enabled = false;
            btnKey2.Enabled = false;
            btnKey3.Enabled = false;
            btnKey4.Enabled = false;
            btnKey5.Enabled = false;
            btnKey6.Enabled = false;
            btnKey7.Enabled = false;
            btnKey8.Enabled = false;
            btnKey9.Enabled = false;
            btnKey0.Enabled = false;
            btnEnter.Enabled = false;
            btnClear.Enabled = false;
        }

        /*
         * Handlers for the numeric keypad buttons
         */
        private void btnKey1_Click(object sender, EventArgs e)
        {
            if (currentScreen == "ENTER_ACCOUNT")
            {
                lblScreen.Text += "1";
            }
            else if (currentScreen == "ENTER_PIN")
            {
                lblScreen.Text += "*";
                pin += "1";
            }
        }

        private void btnKey2_Click(object sender, EventArgs e)
        {
            if (currentScreen == "ENTER_ACCOUNT")
            {
                lblScreen.Text += "2";
            }
            else if (currentScreen == "ENTER_PIN")
            {
                lblScreen.Text += "*";
                pin += "2";
            }
        }

        private void btnKey3_Click(object sender, EventArgs e)
        {
            if (currentScreen == "ENTER_ACCOUNT")
            {
                lblScreen.Text += "3";
            }
            else if (currentScreen == "ENTER_PIN")
            {
                lblScreen.Text += "*";
                pin += "3";
            }
        }

        private void btnKey4_Click(object sender, EventArgs e)
        {
            if (currentScreen == "ENTER_ACCOUNT")
            {
                lblScreen.Text += "4";
            }
            else if (currentScreen == "ENTER_PIN")
            {
                lblScreen.Text += "*";
                pin += "4";
            }
        }

        private void btnKey5_Click(object sender, EventArgs e)
        {
            if (currentScreen == "ENTER_ACCOUNT")
            {
                lblScreen.Text += "5";
            }
            else if (currentScreen == "ENTER_PIN")
            {
                lblScreen.Text += "*";
                pin += "5";
            }
        }

        private void btnKey6_Click(object sender, EventArgs e)
        {
            if (currentScreen == "ENTER_ACCOUNT")
            {
                lblScreen.Text += "6";
            }
            else if (currentScreen == "ENTER_PIN")
            {
                lblScreen.Text += "*";
                pin += "6";
            }
        }

        private void btnKey7_Click(object sender, EventArgs e)
        {
            if (currentScreen == "ENTER_ACCOUNT")
            {
                lblScreen.Text += "7";
            }
            else if (currentScreen == "ENTER_PIN")
            {
                lblScreen.Text += "*";
                pin += "7";
            }
        }

        private void btnKey8_Click(object sender, EventArgs e)
        {
            if (currentScreen == "ENTER_ACCOUNT")
            {
                lblScreen.Text += "8";
            }
            else if (currentScreen == "ENTER_PIN")
            {
                lblScreen.Text += "*";
                pin += "8";
            }
        }

        private void btnKey9_Click(object sender, EventArgs e)
        {
            if (currentScreen == "ENTER_ACCOUNT")
            {
                lblScreen.Text += "9";
            }
            else if (currentScreen == "ENTER_PIN")
            {
                lblScreen.Text += "*";
                pin += "9";
            }
        }

        private void btnKey0_Click(object sender, EventArgs e)
        {
            if (currentScreen == "ENTER_ACCOUNT")
            {
                lblScreen.Text += "0";
            }
            else if (currentScreen == "ENTER_PIN")
            {
                lblScreen.Text += "*";
                pin += "0";
            }
        }
    }
}