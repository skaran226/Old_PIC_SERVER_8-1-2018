using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Management;
using System.Data.OleDb;
using System.Drawing;
using System.Data.SqlClient;
using System.Data.Sql;

namespace FPS
{
    public partial class Transactions_View : Form
    {
        delegate void StringArgReturningVoidDelegate(Button btn, string lbl);

        public static Transactions_View tv;
       // System.Timers.Timer process = new System.Timers.Timer();
        public static Label[] lbl_arr;
        public Transactions_View()
        {


            tv = this;
            InitializeComponent();

            /*process.Interval = 10000; 
            process.Elapsed += new System.Timers.ElapsedEventHandler(UpdateTrans);
            process.Start();*/

            create_tran.Visible = false;
            Authorize.Visible = false;
            complet.Visible = false;
            lbl_arr = new Label[] { tv.btnId1, tv.btnId2, tv.btnId3, tv.btnId4, tv.btnId5, tv.btnId6 };

            
            

        }


     
        public static bool IsPrintable=false;

        static TimerCallback timer_updateTransaction = new TimerCallback(UpdateTrans);


        static System.Threading.Timer GenerateEOD_report_stateTimer = new System.Threading.Timer(timer_updateTransaction, null, 5000,5000);

        private static void UpdateTrans(object state)
        {
            if (DB.IsCompltedTransaction == true)
            {

                Debug.WriteLine("DB.IsCompltedTransaction:" + DB.IsCompltedTransaction);

                
                UpdateCompletedTransView();
                

                DB.IsCompltedTransaction = false;
            }
            Debug.WriteLine("DB.IsCompltedTransaction:" + DB.IsCompltedTransaction);
        }
        
       /* private void UpdateTrans(object sender, System.Timers.ElapsedEventArgs e)
        {

            if (DB.IsCompltedTransaction == true) {

                Debug.WriteLine("DB.IsCompltedTransaction:" + DB.IsCompltedTransaction);

                var timer = sender as System.Timers.Timer;

                timer.Stop();
                UpdateCompletedTransView();
                timer.Start();

                DB.IsCompltedTransaction = false;
            }

            
        }*/





        
        

        int PassIndex=1;//send data for print

      

         

        string date_formate = "";

        string lblMonth = "";
        string[] month_arr = new string[] {"Jan","Feb","March","Apirl","May","June","July","Aug","Sep","Oct","Nov","Dec"};
        int m_inc_dec = 0;
        private void previous_month_Click(object sender, EventArgs e)
        {
            
            lblMonth = month_year_lbl.Text.ToString().Split(',')[0];

            int m = VerifyMonth(lblMonth);
            m_inc_dec = m - 1;

            m_inc_dec--;

            if (m_inc_dec <= 0) {
                ButtonVisibility(previous_month, false);
            }

            if (m_inc_dec > 1) {
                ButtonVisibility(next_month, true);
            }

            month_year_lbl.Text = month_arr[m_inc_dec].ToString() + ",2018";



            if (m_inc_dec + 1 <= 9)
            {

                date_formate = (m_inc_dec + 1) + "/" + day_lbl.Text.ToString() + "/2018";
            }
            else
            {
                date_formate = (m_inc_dec + 1) + "/" + day_lbl.Text.ToString() + "/2018";
            }


            GetChooseTransations(date_formate);
            //MessageBox.Show(date_formate + "");
            
        }

        private void next_month_Click(object sender, EventArgs e)
        {
            lblMonth = month_year_lbl.Text.ToString().Split(',')[0];

            int m = VerifyMonth(lblMonth);
            m_inc_dec = m-1;
            m_inc_dec++;

            if (m_inc_dec == 11)
            {
                ButtonVisibility(next_month, false);
            }
            else {
                ButtonVisibility(next_month, true);
            }

            if (m_inc_dec >=1) {

                ButtonVisibility(previous_month, true);
            }

            month_year_lbl.Text = month_arr[m_inc_dec].ToString() + ",2018";

            if (m_inc_dec + 1 <= 9)
            {

                date_formate = (m_inc_dec + 1) + "/" + day_lbl.Text.ToString() + "/2018";
            }
            else {
                date_formate = (m_inc_dec + 1) + "/" + day_lbl.Text.ToString() + "/2018";
            }

            



           GetChooseTransations(date_formate);




           //MessageBox.Show(date_formate + "");
        }


        int day_inc_dec;
        private void previous_day_Click(object sender, EventArgs e)
        {
            day_inc_dec = Convert.ToInt32(day_lbl.Text.ToString());
            day_inc_dec--;
            if (day_inc_dec <= 9)
            {
                day_lbl.Text =  day_inc_dec.ToString();
            }
            else {
                day_lbl.Text = day_inc_dec.ToString();
            }
            
            if (day_inc_dec <= 1) {

                ButtonVisibility(previous_day, false);
            }

            if (day_inc_dec <= 30) {
                ButtonVisibility(next_day, true);
            }



            


            lblMonth = month_year_lbl.Text.ToString().Split(',')[0];

            int m = VerifyMonth(lblMonth);

            if (m <= 9)
            {

                date_formate = m+"/" + day_lbl.Text.ToString() + "/2018";
            }
            else
            {
                date_formate = m + "/" + day_lbl.Text.ToString() + "/2018";
            }

            GetChooseTransations(date_formate);

           // MessageBox.Show(date_formate + "");
        }

        private void next_day_Click(object sender, EventArgs e)
        {
            day_inc_dec = Convert.ToInt32(day_lbl.Text.ToString());

            day_inc_dec++;

            if (day_inc_dec <= 9)
            {
                day_lbl.Text =   day_inc_dec.ToString();
            }
            else
            {
                day_lbl.Text = day_inc_dec.ToString();
            }
            

            if (day_inc_dec >= 31)
            {

                ButtonVisibility(next_day, false);
            }

            if (day_inc_dec > 1) {
                ButtonVisibility(previous_day, true);
            }


            lblMonth = month_year_lbl.Text.ToString().Split(',')[0];

            int m = VerifyMonth(lblMonth);

            if (m <= 9)
            {

                date_formate = m + "/" + day_lbl.Text.ToString() + "/2018";
            }
            else
            {
                date_formate = m + "/" + day_lbl.Text.ToString() + "/2018";
            }

            GetChooseTransations(date_formate);

            //GetChooseTransations(date_formate);
            //MessageBox.Show(date_formate + "");

        }

        int iPage = 1;
        private void previous_btn_Click(object sender, EventArgs e)
        {

            ClearSelection();
            ClearTransactionsDetails();
            int iButtonIndex;
            int iTranIndex;

            iPage--;

            iButtonIndex = 0;
            for (iTranIndex = (6 * (iPage - 1)); iTranIndex < (6 * iPage); iTranIndex++)
            {
                if (iTranIndex < DB.lCompletedTrans.Count)
                {
                    iButtonIndex++;
                  Update_Transactions_ButtonText(iButtonIndex, "PUMP: " + DB.lCompletedTrans[iTranIndex].sPump + " @ " + DB.lCompletedTrans[iTranIndex].sShowTime + "\nPAID: $" + DB.lCompletedTrans[iTranIndex].sDeposit + "  CHANGE: $" + DB.lCompletedTrans[iTranIndex].sChange);
                  tv.UpdateTransactionId(lbl_arr[iButtonIndex - 1], DB.lCompletedTrans[iTranIndex].sTranId);
                }
            }

           

            if (iPage == 1)
            {
                ButtonVisibility(previous_btn, false);
            }

            if (DB.lCompletedTrans.Count >= 6 * iPage)
            {

                ButtonVisibility(next_btn, true);
            }


            
        }

        private void next_btn_Click(object sender, EventArgs e)
        {
            
            ClearButtonTexts();
            ClearSelection();
            ClearTransactionsDetails();
            //DB.lCompletedTrans.Clear();
            

            int iButtonIndex;
            int iTranIndex;

            iPage++;

            iButtonIndex = 0;
            for (iTranIndex = (6 * (iPage - 1)); iTranIndex < (6 * iPage); iTranIndex++)
            {
                if (iTranIndex < DB.lCompletedTrans.Count)
                {
                    iButtonIndex++;

                   Update_Transactions_ButtonText(iButtonIndex, "PUMP: " + DB.lCompletedTrans[iTranIndex].sPump + " @ " + DB.lCompletedTrans[iTranIndex].sShowTime + "\nPAID: $" + DB.lCompletedTrans[iTranIndex].sDeposit + "  CHANGE: $" + DB.lCompletedTrans[iTranIndex].sChange);
                   tv.UpdateTransactionId(lbl_arr[iButtonIndex-1], DB.lCompletedTrans[iTranIndex].sTranId);
                }
            }

            if (DB.lCompletedTrans.Count <= 6 * iPage)
            {
                
                ButtonVisibility(next_btn, false);
            }

            if (iPage == 2)
            {
                ButtonVisibility(previous_btn, true);
            }







        }

        private void print_transaction_Click(object sender, EventArgs e)
        {
            String trnasc_id = "";
            if (PassIndex == 1) {

                trnasc_id = tv.btnId1.Text;
            }
            if (PassIndex == 2)
            {

                trnasc_id = tv.btnId2.Text;
            }
            if (PassIndex == 3)
            {

                trnasc_id = tv.btnId3.Text;
            }
            if (PassIndex == 4)
            {

                trnasc_id = tv.btnId4.Text;
            }
            if (PassIndex == 5)
            {

                trnasc_id = tv.btnId5.Text;
            }
            if (PassIndex == 6)
            {

                trnasc_id = tv.btnId6.Text;
            }

            if (IsPrintable) {
               PrintReceipt(trnasc_id);
            }
        }

        private void PrintReceipt(string trnasc_id)
        {
            Debug.WriteLine("TRYING TO PRINT... STATUS + " + Printer.iStatus);

            if (Printer.iStatus == 1)
            {
                string sPIC = "";
                string sPump = "";
                string sGrade = "";
                string sVolume = "";
                string sPurchase = "";
                string sPrice = "";
                string sDeposit = "";
                string sChange = "";
                string sShowTime = "";
                string sTranId = "";
                string sReceipt = "";

                OleDbCommand dbCmd;
                OleDbDataReader drRecordSet;


                SQL_SERVER.Set_Sql_Server_Conn();
                SQL_SERVER.Open_Sql_Server_Conn();


                //sQuery = "SELECT COMPLETED_TIME, PIC, PUMP, DEPOSIT, PURCHASE, PRICE, CHANGE, GRADE, VOLUME, SHOW_TIME, TRAN_ID FROM TRANSACTIONS ORDER BY COMPLETED_TIME DESC";

                string sQuery = "SELECT COMPLETED_TIME, PIC, PUMP, DEPOSIT, PURCHASE, PRICE, GRADE, VOLUME, SHOW_TIME, TRAN_ID,CHANGE FROM TRANSACTIONS WHERE TRAN_ID LIKE '%" + trnasc_id + "%' ORDER BY COMPLETED_TIME DESC;";
                dbCmd = SQL_SERVER.Set_Sql_Server_Cmd(sQuery);

                drRecordSet = dbCmd.ExecuteReader();

                Debug.WriteLine(sQuery);
                Debug.WriteLine(drRecordSet.HasRows);


                if (drRecordSet.HasRows)
                {


                    while (drRecordSet.Read())
                    {

                        sPIC = drRecordSet["PIC"].ToString();
                        sPump = drRecordSet["PUMP"].ToString();
                        sGrade = drRecordSet["GRADE"].ToString();
                        sVolume = drRecordSet["VOLUME"].ToString();
                        sPurchase = drRecordSet["PURCHASE"].ToString();
                        sPrice = drRecordSet["PRICE"].ToString();
                        sDeposit = drRecordSet["DEPOSIT"].ToString();
                        sChange = drRecordSet["CHANGE"].ToString();
                        sShowTime = drRecordSet["SHOW_TIME"].ToString();
                        sTranId = drRecordSet["TRAN_ID"].ToString();

                    }





                    sReceipt = sReceipt + Printer.sHeader;
                    sReceipt = sReceipt + "\n\n\n";

                    sReceipt = sReceipt + "Pump: " + sPump + "\n";
                    sReceipt = sReceipt + "Terminal: " + sPIC + "\n\n";

                    sReceipt = sReceipt + "Descr.           qty               amount" + "\n";
                    sReceipt = sReceipt + "------           ---               ------" + "\n";
                    sReceipt = sReceipt + sGrade + "               " + sVolume + "               " + sPurchase + "\n";
                    sReceipt = sReceipt + "                @ " + sPrice + "/G" + "\n";
                    sReceipt = sReceipt + "                Prepay Fuel        -" + sDeposit + ".00" + "\n";
                    sReceipt = sReceipt + "                                 --------" + "\n";
                    sReceipt = sReceipt + "                    Subtotal        -" + sChange + "\n";
                    sReceipt = sReceipt + "                         Tax         0.00" + "\n";
                    sReceipt = sReceipt + "                       TOTAL        -" + sChange + "\n";
                    sReceipt = sReceipt + "                        CASH        -" + sChange + "\n\n";

                    sReceipt = sReceipt + "TRAN# " + sTranId + "\n";
                    sReceipt = sReceipt + sShowTime + "\n\n";
                    sReceipt = sReceipt + Printer.sFooter + "\n\n\n\n\n\n\n\n\n\n\n\n\n";

                    Printer.sReceipt = sReceipt;
                    CenCom.bPrintRequest = true;

                    Debug.WriteLine(sReceipt);

                    MessageBox.Show(sReceipt + "");

                    dbCmd.Dispose();
                    drRecordSet.Dispose();
                    SQL_SERVER.Close_Sql_Sever_Conn();
                }
                

            }
            else
            {
                Display.ShowMessageBox("Printer Offline", 3);
            }
        }

        private void go_back_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void One_Click(object sender, EventArgs e)
        {
            if (One.Text.Trim() != "")
            {
                PassIndex = 1;
                SetButton(PassIndex);
                SetTransactionsDetails(lbl_arr[0].Text);
                
            }
        }

        private void Two_Click(object sender, EventArgs e)
        {
            if (Two.Text.Trim() != "")
            {
                PassIndex = 2;
                SetButton(PassIndex);
                SetTransactionsDetails(lbl_arr[1].Text);
                

            }
        }

        private void Three_Click(object sender, EventArgs e)
        {
            if (Three.Text.Trim() != "")
            {

                PassIndex = 3;
                SetButton(PassIndex);
                SetTransactionsDetails(lbl_arr[2].Text);
            }
        }

        private void Four_Click(object sender, EventArgs e)
        {
            if (Four.Text.Trim() != "")
            {

                PassIndex = 4;
                SetButton(PassIndex);
                SetTransactionsDetails(lbl_arr[3].Text);
            }
        }

        private void Five_Click(object sender, EventArgs e)
        {
            if (Five.Text.Trim() != "")
            {

                PassIndex = 5;
                SetButton(PassIndex);
                SetTransactionsDetails(lbl_arr[4].Text);
            }
        }

        private void Six_Click(object sender, EventArgs e)
        {
            if (Six.Text.Trim() != "")
            {

                PassIndex = 6;
                SetButton(PassIndex);
                SetTransactionsDetails(lbl_arr[5].Text);
            }
        }

        public void SetButton(int index)
        {

            Button[] btnarr = new Button[] { One, Two, Three, Four, Five, Six };

            foreach (Button btn in btnarr)
            {
                btn.BackColor = Color.White;

            }

            if (index == 1)
            {

                Transactions_View.SetButtonColor(One, Color.Yellow);
            }

            if (index == 2)
            {

                Transactions_View.SetButtonColor(Two, Color.Yellow);
            }

            if (index == 3)
            {

                Transactions_View.SetButtonColor(Three, Color.Yellow);
            }

            if (index == 4)
            {

                Transactions_View.SetButtonColor(Four, Color.Yellow);
            }

            if (index == 5)
            {

                Transactions_View.SetButtonColor(Five, Color.Yellow);
            }

            if (index == 6)
            {

                Transactions_View.SetButtonColor(Six, Color.Yellow);
            }

        }

        public static void SetButtonColor(Button btn, Color color)
        {
            btn.BackColor = color;
            btn.FlatAppearance.MouseOverBackColor = color;
        }




        
        public  void SetButtonText(Button btn, string lbl)
        {
            //btn.CheckForIllegalCrossThreadCalls = false;
            if (btn.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(SetButtonText);
                this.Invoke(d, new object[] { btn, lbl });
                
            }
            else
            {
                btn.Text = lbl;
                

            }
           
        }

        private void Transactions_View_Load(object sender, EventArgs e)
        {

            for (int i = 0; i < lbl_arr.Length; i++) {

                lbl_arr[i].Visible = false;
            }

           /* var dateAndTime = DateTime.Now;
            int yearint = dateAndTime.Year;
            int monthint = dateAndTime.Month;
            int dayint = dateAndTime.Day;*/

            //string dtformat=string.Format("{0}/{1}/{2}", monthint, dayint, yearint);

            //MessageBox.Show(dtformat + "");



            previous_btn.Visible = false;


            UpdateCompletedTransView();
           


            if (month_year_lbl.Text.ToString().Split(',')[0] == "Jan") {

                ButtonVisibility(previous_month, false);
            }

           


            string day = DateTime.Now.ToString("dd");
            string month = DateTime.Now.ToString("MM");
            string year = DateTime.Now.ToString("yyyy");

            month = getMonth(month);

            month_year_lbl.Text = month + "," + year;

            if (day == "01")
            {
                day = "1";
                ButtonVisibility(previous_day, false);
            }

         
            day_lbl.Text = day;

            





           /* if (DB.lCompletedTrans.Count <= 6 * iPage)
            {

                ButtonVisibility(next_btn, false);
            }*/

            
        }

       // public static Transactions_View tv1 = new Transactions_View();
       
        public  void Update_Transactions_ButtonText(int index, string lbl)
        {
            


            if (index == 1)
            {

                SetButtonText(tv.One, lbl);
            }

            if (index == 2)
            {
                SetButtonText(tv.Two, lbl);
            }

            if (index == 3)
            {
               SetButtonText(tv.Three, lbl);
            }

            if (index == 4)
            {
                SetButtonText(tv.Four, lbl);
            }

            if (index == 5)
            {
                SetButtonText(tv.Five, lbl);
            }

            if (index == 6)
            {
               SetButtonText(tv.Six, lbl);
            }
        }


        public static int iCount;
        
        public  static void UpdateCompletedTransView()
        {
            int iIndex;
            string sQuery;
            OleDbCommand dbCmd;
            OleDbDataReader drRecordSet;
            /* SqlCommand dbCmd;
             SqlDataReader drRecordSet;*/
            DB.TransStruct myTransStruct;





            Debug.WriteLine("UPDATE COMPLETE TRANSACTIONS VIEW");

            SQL_SERVER.Set_Sql_Server_Conn();
            SQL_SERVER.Open_Sql_Server_Conn();


            //sQuery = "SELECT COMPLETED_TIME, PIC, PUMP, DEPOSIT, PURCHASE, PRICE, CHANGE, GRADE, VOLUME, SHOW_TIME, TRAN_ID FROM TRANSACTIONS ORDER BY COMPLETED_TIME DESC";

            sQuery = "SELECT COMPLETED_TIME, PIC, PUMP, DEPOSIT, PURCHASE, PRICE, GRADE, VOLUME, SHOW_TIME, TRAN_ID,CHANGE FROM TRANSACTIONS ORDER BY COMPLETED_TIME DESC;";
            dbCmd = SQL_SERVER.Set_Sql_Server_Cmd(sQuery);

            drRecordSet = dbCmd.ExecuteReader();

            Debug.WriteLine(sQuery);
            Debug.WriteLine(drRecordSet.HasRows);

            iCount = 0;
            DB.lCompletedTrans.Clear();
            while (drRecordSet.Read())
            {
                myTransStruct.sPIC = drRecordSet["PIC"].ToString();
                myTransStruct.sPump = drRecordSet["PUMP"].ToString();
                myTransStruct.sDeposit = drRecordSet["DEPOSIT"].ToString();
                myTransStruct.sPurchase = drRecordSet["PURCHASE"].ToString();
                myTransStruct.sPrice = drRecordSet["PRICE"].ToString();
                myTransStruct.sChange = drRecordSet["CHANGE"].ToString();
                myTransStruct.sGrade = drRecordSet["GRADE"].ToString();
                myTransStruct.sVolume = drRecordSet["VOLUME"].ToString();
                myTransStruct.sShowTime = drRecordSet["SHOW_TIME"].ToString();
                myTransStruct.sTranId = drRecordSet["TRAN_ID"].ToString();

                DB.lCompletedTrans.Add(myTransStruct);
                iCount++;
            }

            for (iIndex = 0; iIndex < 6; iIndex++)
            {
                if (iIndex < iCount)
                {
                    //tv.pump_no.Text = DB.lCompletedTrans[iIndex].sPump.ToString();
                    tv.Update_Transactions_ButtonText(iIndex + 1, "PUMP: " + DB.lCompletedTrans[iIndex].sPump + " @ " + DB.lCompletedTrans[iIndex].sShowTime + " PAID: $" + DB.lCompletedTrans[iIndex].sDeposit + "  \nCHANGE: $" + DB.lCompletedTrans[iIndex].sChange);
                    tv.UpdateTransactionId(lbl_arr[iIndex], DB.lCompletedTrans[iIndex].sTranId);
                }
            }
            dbCmd.Dispose();
            drRecordSet.Dispose();
            SQL_SERVER.Close_Sql_Sever_Conn();
        }

        public  void UpdateTransactionId(Label label_id, string id)
        {
            label_id.Text = id;
        }


        private void SetTransactionsDetails(string transac_id)
        {

            IsPrintable = true;
             OleDbCommand dbCmd;
            OleDbDataReader drRecordSet;


            SQL_SERVER.Set_Sql_Server_Conn();
            SQL_SERVER.Open_Sql_Server_Conn();


            //sQuery = "SELECT COMPLETED_TIME, PIC, PUMP, DEPOSIT, PURCHASE, PRICE, CHANGE, GRADE, VOLUME, SHOW_TIME, TRAN_ID FROM TRANSACTIONS ORDER BY COMPLETED_TIME DESC";

            string sQuery = "SELECT  PUMP, DEPOSIT, VOLUME, SHOW_TIME, TRAN_ID,CHANGE FROM TRANSACTIONS WHERE TRAN_ID LIKE '%" + transac_id + "%' ORDER BY COMPLETED_TIME DESC;";
            dbCmd = SQL_SERVER.Set_Sql_Server_Cmd(sQuery);

            drRecordSet = dbCmd.ExecuteReader();

            Debug.WriteLine(sQuery);
            Debug.WriteLine(drRecordSet.HasRows);


            if (drRecordSet.HasRows)
            {


                while (drRecordSet.Read())
                {

                    pump_no.Text = drRecordSet["PUMP"].ToString();
                    deposit.Text = drRecordSet["DEPOSIT"].ToString();

                    change.Text = drRecordSet["CHANGE"].ToString();

                    gal.Text = drRecordSet["VOLUME"].ToString();
                    date_time.Text = drRecordSet["SHOW_TIME"].ToString();



                }


                dbCmd.Dispose();
                drRecordSet.Dispose();
                SQL_SERVER.Close_Sql_Sever_Conn();

            }


        }



        private void ClearButtonTexts()
        {
            Button[] btnarr = new Button[] { One, Two, Three, Four, Five, Six };

            foreach (Button btn in btnarr)
            {
                btn.Text = "";
                btn.BackColor = Color.White;
                btn.FlatAppearance.MouseOverBackColor = Color.White;
            }
        }

        private void ClearSelection()
        {

            Button[] btnarr = new Button[] { One, Two, Three, Four, Five, Six };

            foreach (Button btn in btnarr)
            {

                btn.BackColor = Color.White;
                btn.FlatAppearance.MouseOverBackColor = Color.White;
            }
        }

        private void ClearTransactionsDetails()
        {

            pump_no.Text = "";
            deposit.Text = "";
            change.Text = "";
            total.Text = "";
            date_time.Text = "";
            gal.Text = "";
        }



        private void ButtonVisibility(Button btn,bool visiblity){

            btn.Visible = visiblity;
        }

        int monthNum;
        private int VerifyMonth(string month) {

            if (month == "Jan") {

                //ButtonVisibility(previous_month, false);
                monthNum = 1;
                
            }

           /* if (month != "Jan") {
                ButtonVisibility(previous_month, true);
            }*/

            if (month == "Feb") {
                monthNum = 2;
            }

            if (month == "March")
            {
                monthNum = 3;
            }

            if (month == "Apirl")
            {
                monthNum = 4;
            }

            if (month == "May")
            {
                monthNum = 5;
            }

            if (month == "June")
            {
                monthNum = 6;
            }

            if (month == "July")
            {
                monthNum = 7;
            }

            if (month == "Aug") {
                monthNum = 8;
            }

            if (month == "Sep")
            {
                monthNum = 9;
            }

            if (month == "Oct")
            {
                monthNum = 10;
            }

            if (month == "Nov")
            {
                monthNum = 11;
            }

            if (month == "Dec")
            {
                monthNum = 12;

                //ButtonVisibility(next_month, false);
            }

           /* if (month != "Dec")
            {
                ButtonVisibility(next_month, true);
            }*/
            return monthNum;
       }

       private string getMonth(string month){
           string ret_month = "";
           if (month == "01" || month == "1")
           {
               ret_month = "Jan";
           }


           if (month == "02" || month == "2")
           {
               ret_month = "Feb";
           }

           if (month == "03" || month == "3")
           {
               ret_month = "March";
           }

           if (month == "04" || month == "4")
           {
               ret_month = "Apirl";

           }

           if (month == "05" || month == "5")
           {
               ret_month = "May";
           }

           if (month == "06" || month == "6")
           {
               ret_month = "June";
           }

           if (month == "07" || month == "7")
           {
               ret_month = "July";
           }

           if (month == "08" || month == "8")
           {
               ret_month = "Aug";
           }

           if (month == "09" || month == "9")
           {
               ret_month = "Sep";
           }

           if (month == "10")
           {
               ret_month = "Oct";
           }

           if (month == "11")
           {
               ret_month = "Nov";
           }

           if (month == "12")
           {
               ret_month = "Dec";
           }



           return ret_month;
       }


        private void GetChooseTransations(string datetimeformate) {

            int iIndex;
            string sQuery;
            OleDbCommand dbCmd;
            OleDbDataReader drRecordSet;
            /* SqlCommand dbCmd;
             SqlDataReader drRecordSet;*/
            DB.TransStruct myTransStruct;





            Debug.WriteLine("UPDATE COMPLETE TRANSACTIONS VIEW");

            SQL_SERVER.Set_Sql_Server_Conn();
            SQL_SERVER.Open_Sql_Server_Conn();


            //sQuery = "SELECT COMPLETED_TIME, PIC, PUMP, DEPOSIT, PURCHASE, PRICE, CHANGE, GRADE, VOLUME, SHOW_TIME, TRAN_ID FROM TRANSACTIONS ORDER BY COMPLETED_TIME DESC";

            sQuery = "SELECT COMPLETED_TIME, PIC, PUMP, DEPOSIT, PURCHASE, PRICE, GRADE, VOLUME, SHOW_TIME, TRAN_ID,CHANGE FROM TRANSACTIONS WHERE SHOW_TIME LIKE '%" + datetimeformate + "%' ORDER BY COMPLETED_TIME DESC;";
            dbCmd = SQL_SERVER.Set_Sql_Server_Cmd(sQuery);

            drRecordSet = dbCmd.ExecuteReader();

            Debug.WriteLine(sQuery);
            Debug.WriteLine(drRecordSet.HasRows);

            
            if (drRecordSet.HasRows)
            {
                iCount = 0;
                DB.lCompletedTrans.Clear();
                ClearButtonTexts();
                ClearSelection();
                ClearTransactionsDetails();

                while (drRecordSet.Read())
                {
                    myTransStruct.sPIC = drRecordSet["PIC"].ToString();
                    myTransStruct.sPump = drRecordSet["PUMP"].ToString();
                    myTransStruct.sDeposit = drRecordSet["DEPOSIT"].ToString();
                    myTransStruct.sPurchase = drRecordSet["PURCHASE"].ToString();
                    myTransStruct.sPrice = drRecordSet["PRICE"].ToString();
                    myTransStruct.sChange = drRecordSet["CHANGE"].ToString();
                    myTransStruct.sGrade = drRecordSet["GRADE"].ToString();
                    myTransStruct.sVolume = drRecordSet["VOLUME"].ToString();
                    myTransStruct.sShowTime = drRecordSet["SHOW_TIME"].ToString();
                    myTransStruct.sTranId = drRecordSet["TRAN_ID"].ToString();

                    DB.lCompletedTrans.Add(myTransStruct);
                    iCount++;
                }

                for (iIndex = 0; iIndex < 6; iIndex++)
                {
                    if (iIndex < iCount)
                    {
                       Update_Transactions_ButtonText(iIndex + 1, "PUMP: " + DB.lCompletedTrans[iIndex].sPump + " @ " + DB.lCompletedTrans[iIndex].sShowTime + " PAID: $" + DB.lCompletedTrans[iIndex].sDeposit + "\nCHANGE: $" + DB.lCompletedTrans[iIndex].sChange);
                       tv.UpdateTransactionId(lbl_arr[iIndex], DB.lCompletedTrans[iIndex].sTranId);
                    }
                }


                iPage = 1;

                if (DB.lCompletedTrans.Count <= 6 * iPage)
                {

                    ButtonVisibility(next_btn, false);
                }
                if (DB.lCompletedTrans.Count >= 6 * iPage)
                {

                    ButtonVisibility(next_btn, true);
                }

                if(iPage==1){
                    ButtonVisibility(previous_btn, false);
                }

            }
            else {
               /* Display.ShowMessageBox("Not Available Selected date Transations", 4);*/
                DB.lCompletedTrans.Clear();
                ClearButtonTexts();
                ClearSelection();
                ClearTransactionsDetails();
                iPage = 1;

                if (DB.lCompletedTrans.Count <= 6 * iPage)
                {

                    ButtonVisibility(next_btn, false);
                }
                if (DB.lCompletedTrans.Count >= 6 * iPage)
                {

                    ButtonVisibility(next_btn, true);
                }

                if (iPage == 1)
                {
                    ButtonVisibility(previous_btn, false);
                }


            }
            dbCmd.Dispose();
            drRecordSet.Dispose();
            SQL_SERVER.Close_Sql_Sever_Conn();

        }

        private void Refresh_btn_Click(object sender, EventArgs e)
        {
            Display.ShowMessageBox("Updated Transactions Data", 3);
            UpdateCompletedTransView();
        }





        /*******************test transactions*************************/

        string sPassId = "";
        string sPassTime = "";
        int iPassPicNum;
        int iPassPumpNum;
        string sPassAmount = "";


        string sPurchage = "";
        string sPrice = "";
        string sVolume = "";
        string sPassGrade = "";
        string sPassChage = "";
        string sPassCompletedTime = "";
        string sPassShowTime = "";

        private void create_Click(object sender, EventArgs e)
        {
            SQL_SERVER.Open_Sql_Server_Conn();
            sPassId = DateTime.Now.ToString("yyMMddHHmmss") + new Random().Next(0, 10);
            sPassTime = DateTime.Now.ToString("yyMMddHHmmss");
            iPassPumpNum = new Random().Next(1, 8);
            iPassPicNum = new Random().Next(1, 8);

            DB.CreateTransaction(sPassId, sPassTime, iPassPicNum, iPassPumpNum);

            MessageBox.Show("Success Create transaction");
        }

        private void Authorize_Click(object sender, EventArgs e)
        {
            sPassAmount = new Random().Next(10, 100).ToString();
            DB.AuthorizeTransaction(sPassId, sPassAmount, sPassTime);

            MessageBox.Show("Success Authorization");

        }

        private void complet_Click(object sender, EventArgs e)
        {
            sPurchage = new Random().Next(20, 80).ToString();
            int amt = Convert.ToInt32(sPassAmount);
            int purchage = Convert.ToInt32(sPurchage);



            sPassChage = (amt - purchage).ToString();
            sPurchage = String.Format("{0:0.00}", sPurchage);
            sPassChage = String.Format("{0:0.00}", sPassChage);
            sPrice = "20";
            sVolume = (new Random().Next(7, 14) / 1000).ToString();
            sPassGrade = new Random().Next(1, 4).ToString();
            sPassCompletedTime = DateTime.Now.ToString("yyMMddHHmmss");
            sPassShowTime = DateTime.Now.ToString();

            //MessageBox.Show("Success Complete Transaction");

            if (purchage < amt)
            {
                DB.CompleteTransaction(sPassId, sPurchage, sPrice, sVolume, sPassGrade, sPassChage, sPassCompletedTime, sPassShowTime);
               // this.Hide();
               
                MessageBox.Show("Success ");

            }
            else
            {

                MessageBox.Show("error purchage amount greater then amt");
            }
        }


        



   
        }
    }

        

    

