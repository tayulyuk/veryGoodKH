﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxKHOpenAPILib;

namespace VeryGoodAuto
{
    public partial class Form1 : Form
    {       
        public const string 화면번호_계좌평가현황요청 = "5001";
        public const string 화면번호_실시간미체결요청 = "5002";
        public const string 화면번호_조건검색 = "5003";
        public const string 화면번호_조건식종목요청 = "5004";
        public const string 화면번호_조건식종목추가요청 = "6004";
        public const string 화면번호_기본주식정보요청 = "5005";
        public const string 화면번호_주식매수요청 = "5006";
        public const string 화면번호_주식매도요청 = "5007";
        public const string 화면번호_실시간데이터요청 = "5008";
        public const string 화면번호_일자별실현손익 = "5009";

        public  int 실시간조건검색수신화면번호 = 8000;

        public readonly object lockThis;
        public readonly object lockRemove;

        #region 수동
        public const string 주문종목정보 = "7000";
        public const string 주식주문 = "6000";
        #endregion

        /// <summary>
        /// 조건검색식의 이름과 인덱스를 저장한 클래스 모음.    
        /// </summary>
        public List<ConditionObject> conditionList;
        /// <summary>
        /// 실시간 저장하고 있는 조건검색식 번호를 임시 저장 공유
        /// </summary>
        public int currentConditionCode;
        /// <summary>
        /// 바로 직적의 조건검색식 번호.
        /// </summary>
        public int beforeConditionCode;

        /// <summary>
        /// 잔고와 미체결 관리를 한다.
        /// </summary>
        List<BalanceItem> balanceItemList;

        /// <summary>
        /// 잔고에서 미체결 리스트를 별도 관리 저장.
        /// </summary>
        List<OutStanding> outStandingList;

        /// <summary>
        /// Text  입력시 코드를 반환하기 위해 만든 list.
        /// </summary>
        List<ItemInfo> itemInfoList;
        bool isTrading = false; //시작버튼 첫 값.
        /// <summary>
        /// OnReciveRealData에서 이전 화면을 없애기 위해 이전 스크린번호를 저장한다.
        /// </summary>
        public string 이전_스크린번호 ="";
        public string 현재_스크린번호 = "";

        private int 이전_조건식번호 = -1;
        int 현재_선택된_조건식번호 = -1;
        private string 이전_조건식명 = "";
        string 현재_석택된_조건식명 = "";

        public string 삭제코드 = "";

        //Thread autoViews;
        Thread startThead; // 무한 while 하면서 삭제할 종목이 있다면 계속 삭제하는 일만 한다.
        Thread orderClassManagerThread; // 매수와 매도가 orderClassList에 있다면 무조건 실행해라. 제한 넘지 않게.

        public List<string> deleteCode;// 삭제할 종목
        public List<OrderClass> orderClassList; // 매수와 매도를 함깨 동기화하여 시간제한 1초/4회 를 넘지 안게 관리한다.
        public List<StockItemObject> conditionViewList; // 단순 컨디션 뷰 보여주기위한 목록,
        /// <summary>
        /// 실시간으로 받아오는 매수평균치 -> 매도시 언제라도 팔수 있는 량을 매수에 사용한다.
        /// </summary>
        private int 매수할종목평균수량;

        /// <summary>
        /// 매수종목의 최고 수익율 저장.
        /// </summary>
        Dictionary<string, double> highestRate;  
        
        public Form1()
        {
            InitializeComponent();

            conditionList = new List<ConditionObject>();// 조건검색식에 따른 항목리스트

            conditionViewList = new List<StockItemObject>(); //실시간창에 리스트를 보여준다.
            itemInfoList = new List<ItemInfo>(); // 주식명 검색을 위한 리스트.
            balanceItemList = new List<BalanceItem>();// 잔고와 미체결 관리를 한다.
            outStandingList = new List<OutStanding>();


            lockThis = new object(); // lock
            lockRemove = new object();// 삭제 종목명.

            highestRate = new Dictionary<string, double>();

            conditionDataGridView.SelectionChanged += DataGridView_SelectionChanged;
            tradingStartButton.Click += Button_Click;
            tradingStopButton.Click += Button_Click;

            axKHOpenAPI1.OnEventConnect += AxKHOpenAPI1_OnEventConnect;

            axKHOpenAPI1.OnReceiveChejanData += AxKHOpenAPI1_OnReceiveChejanData; //채결 .
            axKHOpenAPI1.OnReceiveRealCondition += AxKHOpenAPI1_OnReceiveRealCondition; //조건검색에 편입/퇴출 감지       
            
            axKHOpenAPI1.OnReceiveConditionVer += AxKHOpenAPI1_OnReceiveConditionVer;
            axKHOpenAPI1.OnReceiveTrCondition += AxKHOpenAPI1_OnReceiveTrCondition;

            axKHOpenAPI1.OnReceiveTrData += AxKHOpenAPI1_OnReceiveTrData;
            axKHOpenAPI1.OnReceiveRealData += AxKHOpenAPI1_OnReceiveRealData;

            axKHOpenAPI1.CommConnect();


            //----
            deleteCode = new List<string>();
            orderClassList = new List<OrderClass>();

            startThead = new Thread(new ThreadStart(UseDeleteMethod));
            startThead.IsBackground = true;
            startThead.Start();         

            orderClassManagerThread = new Thread(new ThreadStart(UseOrderClassMethod));
            orderClassManagerThread.IsBackground = true;
            orderClassManagerThread.Start();            
            //----
            #region 수동모드
            itemCodeTextBox.TextChanged += TextBox_TextChanged;
            buyButton.Click += ManualButton_Click;
            sellButton.Click += ManualButton_Click;
            changeButton.Click += ManualButton_Click;
            cancelButton.Click += ManualButton_Click;

            searchButton.Click += ManualButton_Click;

            outStandingDataGridView.CellContentClick += DataGridView_SelectedBalanceDataGridView;
            balanceDataGridView.CellContentClick += DataGridView_SelectedBalanceDataGridView;

            #endregion 수동모드
        }


        private void DataGridView_SelectedBalanceDataGridView(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (sender.Equals(balanceDataGridView))
                {
                    string code = this.balanceDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
                    BalanceItem bi = balanceItemList.Find(o => o.종목코드 == code.Trim());
                    if (bi != null)
                    {
                        tradeOptionComboBox.Text = "매도";

                        itemCodeTextBox.Text = bi.종목코드;
                        itemNameLabel.Text = bi.종목명;
                        amountManulNumericUpDown.Value = decimal.Parse(bi.보유수량.ToString());
                        priceManualNumericUpDown.Value = decimal.Parse(bi.현재가.ToString());
                    }
                }
                else if (sender.Equals(outStandingDataGridView))
                {
                    string code = this.outStandingDataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
                    OutStanding os = outStandingList.Find(o => o.종목코드 == code.Trim());
                    if (os != null)
                    {
                        string 주문구분 = os.주문구분.Replace("+", "").Replace("-", "");
                        if (주문구분.Length >= 2)
                            tradeOptionComboBox.Text = os.주문구분.Substring(0, 2);

                        originOrderNumtextBox.Text = os.주문번호;
                        itemCodeTextBox.Text = os.종목코드;
                        itemNameLabel.Text = os.종목명;
                        amountManulNumericUpDown.Value = int.Parse(os.미체결수량.ToString());
                        priceManualNumericUpDown.Value = int.Parse(os.주문가격.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message.ToString() + "199");
            }
        }
        /// <summary>
        /// 삭제 종목을 확인하여 계속 삭제하는 일만 한다.
        /// </summary>
        void UseDeleteMethod()
        {
            while (true)
            {
                lock (lockRemove)
                {
                    if (deleteCode.Count > 0)
                    {
                        Thread t = new Thread(delegate ()
                        {
                            this.Invoke(new Action(delegate ()
                            {
                                Delay(500, deleteCode.First().Trim());
                            }));
                        });

                        t.Start();
                        t.Join();
                    }
                }
            }
        }

        /// <summary>
        /// 삭제 종목을 확인하여 계속 삭제하는 일만 한다.
        /// </summary>
        void UseOrderClassMethod()
        {
            while (true)
            {
                lock (lockThis)
                {
                    if (orderClassList.Count > 0)
                    {
                        Thread t = new Thread(delegate ()
                        {
                            this.Invoke(new Action(delegate ()
                            {
                                DelayOrder(1000, orderClassList.First());
                            }));
                        });
                        t.Start();
                        t.Join();
                    }
                }
            }
        }

        /// <summary>
        /// 수동 매매 -- 버튼들
        /// </summary>      
        private void ManualButton_Click(object sender, EventArgs e)
        {
            string 계좌번호 = accountComboBox.Text;
            string 종목코드 = itemCodeTextBox.Text;
            int 수량 = (int)amountManulNumericUpDown.Value;
            int 가격 = (int)priceManualNumericUpDown.Value;
            string 거래구분 = guBunComboBox.Text;
            거래구분 = 거래구분.Substring(0, 2);

             // orderClass.주문형태, orderClass.화면번호, orderClass.계정,
            //  orderClass.타입, orderClass.종목코드, orderClass.보유수량,
            //  orderClass.가격, orderClass.호가구분, orderClass.원래주문번호);
            if (sender.Equals(buyButton))
            {
                if (계좌번호.Length > 0 && 종목코드.Length == 6 && 수량 > 0 && 가격 > 0 && 거래구분.Length == 2)
                {
                    orderClassList.Add(new OrderClass("매수주문", 주식주문, 계좌번호, 1, 종목코드, 수량, 가격, 거래구분, ""));
                }
                   // axKHOpenAPI1.SendOrder("주식주문", 주식주문, 계좌번호, 1, 종목코드, 수량, 가격, 거래구분, "");
            }
            else if (sender.Equals(sellButton))
            {
                if (계좌번호.Length > 0 && 종목코드.Length == 6 && 수량 > 0 && 가격 > 0 && 거래구분.Length == 2)
                {
                    orderClassList.Add(new OrderClass("매도주문", 주식주문,
                                                                                            계좌번호, 2, 종목코드, 수량, 가격, 거래구분, ""));
                }
                    //axKHOpenAPI1.SendOrder("주식주문", 주식주문, 계좌번호, 2, 종목코드, 수량, 가격, 거래구분, "");
            }
            else if (sender.Equals(changeButton))
            {
                string 매매구분 = tradeOptionComboBox.Text; // 매매 필수 구분.
                string 주문번호 = originOrderNumtextBox.Text;
                                  
                if (매매구분.Equals("매수"))
                {
                    orderClassList.Add(new OrderClass("매수주문 변경", 주식주문, 계좌번호, 5, 종목코드, 수량, 가격, 거래구분, 주문번호));
                }
                        //axKHOpenAPI1.SendOrder("주식주문", 주식주문, 계좌번호, 5, 종목코드, 수량, 가격, 거래구분, 주문번호);
                else if (매매구분.Equals("매도"))
                {
                    orderClassList.Add(new OrderClass("매도주문 변경", 주식주문, 계좌번호, 6, 종목코드, 수량, 가격, 거래구분, 주문번호));
                }
                        //axKHOpenAPI1.SendOrder("주식주문", 주식주문, 계좌번호, 6, 종목코드, 수량, 가격, 거래구분, 주문번호);               
            }
            else if (sender.Equals(cancelButton))
            {
                string 매매구분 = tradeOptionComboBox.Text; // 매매 필수 구분.
                string 주문번호 = originOrderNumtextBox.Text;
              
                if (매매구분.Equals("매수"))
                {
                    orderClassList.Add(new OrderClass("매수 취소", 주식주문,계좌번호, 3, 종목코드, 수량, 가격, 거래구분, 주문번호));
                }
                    //axKHOpenAPI1.SendOrder("주식주문", 주식주문, 계좌번호, 3, 종목코드, 수량, 가격, 거래구분, 주문번호);
                else if (매매구분.Equals("매도"))
                {
                    orderClassList.Add(new OrderClass("매도 취소", 주식주문, 계좌번호, 4, 종목코드, 수량, 가격, 거래구분, 주문번호));
                }
                    //axKHOpenAPI1.SendOrder("주식주문", 주식주문, 계좌번호, 4, 종목코드, 수량, 가격, 거래구분, 주문번호);
               
            }

            //종목검색용도.
            else if (sender.Equals(searchButton))
            {               
                string searchItemName = searchTextBox.Text;               
               
                for (int i=0;i < itemInfoList.Count; i++)
                {
                    if(itemInfoList[i].itemName.Trim() == searchItemName.Trim())
                    {                       
                        itemCodeTextBox.Text = itemInfoList[i].itemCode;
                        break;
                    }
                }
            }

        }


        /// <summary>
        /// 수동 모드 -- 종목코드 입력.
        /// </summary>     
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (sender.Equals(itemCodeTextBox))
                if (itemCodeTextBox.TextLength == 6)
                {
                    axKHOpenAPI1.SetInputValue("종목코드", itemCodeTextBox.Text);
                    axKHOpenAPI1.CommRqData("주문종목정보", "opt10001", 0, 주문종목정보);
                }
        }

        string 저장타입 = "";        

        private void AxKHOpenAPI1_OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            //종목코드,종목명,현재가10,전일대비11,등락율12,거래량15
            //string 종목코드 = e.sRealKey;     
            //예수금951/손익율8019/당일실현손익990/당일신현손익율991/
         
            if (!저장타입.Equals(e.sRealType))
            {
                저장타입 = e.sRealType;
                Console.WriteLine(e.sRealType);
            }

            if (e.sRealType.Equals("주식종목정보"))
            {
                Console.WriteLine("주식종목정보 ->" +e.sRealData);
            }

            if (e.sRealType.Equals("잔고"))
            {
                //,매수총잔량125,실현손익990;손익율8019,예수금951,
                int 실현손익 = int.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 990));
                double 손익율 = int.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 8019));
                int 예수금 = int.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 951));
                Console.WriteLine("실현손익 : " + 실현손익 + "/ 손익율" + 손익율 + "/ 예수금" + 예수금);
            }
            if (e.sRealType.Equals("주식호가잔량"))
            {
                int 매수총잔량 = int.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType,125));
                매수할종목평균수량 = 매수총잔량 / 10; //언제든지 받아줄수 있는 가격만 산다. 이유.
               // Console.WriteLine("종목명 : " + axKHOpenAPI1.GetMasterCodeName(e.sRealKey) + "/ 매수총잔량" + 매수총잔량);
            }

            if (e.sRealType.Equals("주식체결"))
            {
                string 종목코드 = e.sRealKey;
                string 종목명 = axKHOpenAPI1.GetMasterCodeName(e.sRealKey).Trim();
                long 현재가1 = long.Parse(axKHOpenAPI1.GetCommRealData(e.sRealKey, 10).Trim());
                long 전일대비 = long.Parse(axKHOpenAPI1.GetCommRealData(e.sRealKey, 11).Trim());
                double 등락율 = double.Parse(axKHOpenAPI1.GetCommRealData(e.sRealKey, 12).Trim());
                long 거래량 = long.Parse(axKHOpenAPI1.GetCommRealData(e.sRealKey, 13).Trim());
                long 거래금 = long.Parse(axKHOpenAPI1.GetCommRealData(e.sRealKey, 14).Trim());
                long 보유수량 = 0; // 처음 감시 되면 무조건 보유수량은 없으니까.

                try
                {
                    long 현재가 = 0;
                    if (현재가1 < 0)// - 부호를 없애기 위해 -현재가를 곱해  양수로 만든다.                          
                        현재가 = -현재가1;

                    BalanceItem bi = balanceItemList.Find(o => o.종목코드.Equals(종목코드));
                    if (bi != null)
                    {
                        Console.WriteLine("있다");
                        bi.현재가 = 현재가;

                        long 평가금액 = bi.보유수량 * 현재가; //  -(세금 + 수수료)

                        bi.손익금액 = 평가금액 - bi.매수금;
                        bi.손익율 = 100 * (평가금액 - bi.매수금) / (double) bi.매수금;

                        if (bi.손익율 > bi.최고율)
                            bi.최고율 = bi.손익율;

                        //  보여줌.
                        balanceDataGridView.DataSource = null;
                        balanceDataGridView.DataSource = balanceItemList;
                        balanceDataGridView.CurrentRow.Selected = false;

                        if (isTrading) //시작버튼 활성시.
                        {
                            if (isTrailingStopCheckBox.Checked)
                            {
                                // double 최고치 = 0;
                                // if (highestRate.ContainsKey(종목코드))
                                // 최고치 = highestRate[종목코드];

                                if (2 < bi.최고율)
                                {

                                    if (bi.손익율 < (bi.최고율 - 1))
                                    {
                                        orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                            accountComboBox.Text, 2, 종목코드.Trim(), (int) bi.보유수량, 0, "03", ""));

                                        insertListBox.SelectedIndex = insertListBox.Items.Count - 1; // 첫 line으로 항상 이동
                                    }
                                }
                                else if ((0.5 <= bi.최고율) && (bi.최고율 < 2))
                                {
                                    if (bi.손익율 < (bi.최고율 - 0.5f))
                                    {
                                        orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                            accountComboBox.Text, 2, 종목코드.Trim(), (int) bi.보유수량, 0, "03", ""));

                                        insertListBox.SelectedIndex = insertListBox.Items.Count - 1; // 첫 line으로 항상 이동
                                    }
                                }
                            }
                            //수익매매 처리
                            else if (takeProfitCheckBox.Checked)
                            {
                                double takeProfit = (double) takeProfitNumericUpDown.Value;
                                if (bi.손익율 >= takeProfit)
                                {
                                    orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                        accountComboBox.Text, 2, 종목코드.Trim(), (int) bi.보유수량, 0, "03", ""));

                                    insertListBox.SelectedIndex = insertListBox.Items.Count - 1; // 첫 line으로 항상 이동
                                }
                            }

                            //손절 처리
                            if (stopLossChheckBox.Checked)
                            {
                                double stopLoss = (double) stopLossNumericUpDown.Value;
                                if (bi.손익율 <= stopLoss)
                                {
                                    orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                        accountComboBox.Text, 2, 종목코드.Trim(), (int) bi.보유수량, 0, "03", ""));
                                    insertListBox.SelectedIndex = insertListBox.Items.Count - 1; // 첫 line으로 항상 이동
                                }
                            }
                        }

                        balanceDataGridView.CurrentRow.Selected = false; //cosor

                        //Color
                        for (int i = 0; i < balanceDataGridView.RowCount; i++)
                        {
                            // 손익금액  값 비교.
                            int val = int.Parse(balanceDataGridView.Rows[i].Cells[5].Value.ToString());
                            // Console.WriteLine("val :" + val);
                            if (val > 0)
                            {
                                balanceDataGridView.Rows[i].Cells[5].Style.ForeColor = Color.Red;
                                balanceDataGridView.Rows[i].Cells[6].Style.ForeColor = Color.Red;
                            }
                            else if (val < 0)
                            {
                                balanceDataGridView.Rows[i].Cells[5].Style.ForeColor = Color.Blue;
                                balanceDataGridView.Rows[i].Cells[6].Style.ForeColor = Color.Blue;
                            }
                            else
                            {
                                balanceDataGridView.Rows[i].Cells[5].Style.ForeColor = Color.Black;
                                balanceDataGridView.Rows[i].Cells[6].Style.ForeColor = Color.Black;
                            }

                            balanceDataGridView.Rows[i].Cells[7].Style.ForeColor = Color.DarkViolet; //최고율
                        }
                    }
                    else
                    {
                        //조건식에 맞는    리스트를 
                        // conditionGridView 에 보여준다.
                        StockItemObject sio = conditionViewList.Find(o => o.종목코드 == 종목코드);
                        if (sio != null) // 입력되고 있는 조건식번호 입력  
                        {
                            Console.WriteLine("없 다");

                            sio.현재가 = 현재가;
                            sio.거래금 = 거래금;
                            sio.거래량 = 거래량;
                            sio.등락율 = 등락율;
                            sio.전일대비 = 전일대비;
                            conditionItemDataGridView.DataSource = null;
                            conditionItemDataGridView.DataSource = conditionViewList;
                        }
                        else
                        {
                            Console.WriteLine("없 다   =============");
                            conditionViewList.Add(new StockItemObject(종목코드, 종목명, 현재가, 전일대비, 등락율, 거래량, 거래금));
                            conditionItemDataGridView.DataSource = null;
                            conditionItemDataGridView.DataSource = conditionViewList;
                        }

                        conditionItemDataGridView.CurrentRow.Selected = false;
                        //Color
                        for (int i = 0; i < conditionItemDataGridView.RowCount; i++)
                        {
                            // 전일대비  값 비교.
                            int val = int.Parse(conditionItemDataGridView.Rows[i].Cells[3].Value.ToString());
                            // Console.WriteLine("val :" + val);
                            if (val > 0)
                            {
                                conditionItemDataGridView.Rows[i].Cells[4].Style.ForeColor = Color.Red;
                                conditionItemDataGridView.Rows[i].Cells[3].Style.ForeColor = Color.Red;
                            }
                            else if (val < 0)
                            {
                                conditionItemDataGridView.Rows[i].Cells[4].Style.ForeColor = Color.Blue;
                                conditionItemDataGridView.Rows[i].Cells[3].Style.ForeColor = Color.Blue;
                            }
                            else
                            {
                                conditionItemDataGridView.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                                conditionItemDataGridView.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            }
                        }

                        //매수  시작--
                        if (isTrading)
                        {
                            if (buyConditionCheckBox.Checked)
                            {
                                if (buyConditionComboBox.Text.Equals(현재_석택된_조건식명))
                                {
                                    //해당 종목코드 자동매수 요청.
                                    if (accountComboBox.Text.Length > 0)
                                    {
                                        int 예탁금 = 0;
                                        int 총1회매수금액 = (int)totalAmountNumericUpDown.Value;

                                        if (int.Parse(예탁자산평가액Label.Text.Replace(",", "").Trim()) > 0)
                                            예탁금 = int.Parse(예탁자산평가액Label.Text.Replace(",", "").Trim());

                                        if ((총1회매수금액 > 0) && (현재가1 > 0) && (예탁금 > 총1회매수금액))
                                        {
                                            int 매수량 = 총1회매수금액 / (int)현재가1;

                                            if (매수량 > 0)
                                            {
                                                //이미 매수한 같은 종목이 있는가 확인. -> balanceDataGridView
                                                if (BalanceListCheck(종목코드.Trim()))
                                                {
                                                    insertListBox.Items.Add("=이미보유종목= 조건식명 : " + 현재_석택된_조건식명 + " 종목명 :" + 종목명);
                                                }
                                                else
                                                {
                                                    int 적정매수량 = 0;
                                                    if (isAdjustQuantityCheckBox.Checked)
                                                    {
                                                        int 예탁자산 = int.Parse(예탁자산평가액Label.Text.Replace(",", "").Trim());
                                                        int 예탁자산총매수량 = 예탁자산 / (int)현재가1;
                                                        적정매수량 = AdjustQuantity(매수할종목평균수량, 예탁자산총매수량);
                                                        orderClassList.Add(new OrderClass("주식매수요청", 화면번호_주식매수요청,
                                                                                                   accountComboBox.Text, 1, 종목코드.Trim(), 적정매수량, 0, "03", ""));
                                                    }
                                                    else
                                                        orderClassList.Add(new OrderClass("주식매수요청", 화면번호_주식매수요청,
                                                                                                accountComboBox.Text, 1, 종목코드.Trim(), 매수량, 0, "03", ""));
                                                }
                                                insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("1회 매수 비용이 작습니다");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    /* 
                                      // for (int i = 0; i < balanceDataGridView.RowCount; i++)
                                     // {
                                       //   if ( balanceDataGridView["잔고_종목코드", i].Value.ToString().Equals(종목코드))
                                      //    {
                                              // int 현재가 = int.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 10));
                                           if (현재가1 < 0)// - 부호를 없애기 위해 -현재가를 곱해  양수로 만든다.                          
                                                  현재가 = -현재가1;                            
                                              else                           
                                                  현재가 = 현재가1;

                                              balanceDataGridView["잔고_현재가", i].Style.ForeColor = Color.Green;                          

                                              int 보유수량 = int.Parse(balanceDataGridView["잔고_보유수량", i].Value.ToString().Replace(",", ""));
                                              long 매입금액 = long.Parse(balanceDataGridView["잔고_매입금액", i].Value.ToString().Replace(",", ""));

                                              long 평가금액 = 보유수량 * 현재가;//  -(세금 + 수수료)
                                              long 손익금액 = 평가금액 - 매입금액;
                                              double 손익율 = 100 * (평가금액 - 매입금액) / (double)매입금액; // long / long = 정수이므로 일부러 double 변환


                                              balanceDataGridView["잔고_현재가", i].Value = String.Format("{0:###,#}", 현재가);
                                              balanceDataGridView["잔고_평가금액", i].Value = String.Format("{0:###,#}", 평가금액);
                                              balanceDataGridView["잔고_손익금액", i].Value = String.Format("{0:###,#}", 손익금액);
                                              balanceDataGridView["잔고_손익율", i].Value = String.Format("{0:F2}", 손익율) + "%"; //소수 2자리.  flout?
                                             //???  미체결 리스트의 변경되는 값도 실시간으로 직접 바꿔줘야 하나? 내일 test

                                              // 최고율만 저장하는 딕셔너리를 만들자.
                                              if (balanceDataGridView["잔고_최고율", i].Value == null) //최고율에 값이 없다면
                                              {
                                                  if (!highestRate.ContainsKey(종목코드))
                                                  {
                                                      highestRate.Add(종목코드, 손익율);
                                                  }                              
                                                 // balanceDataGridView["잔고_최고율", i].Value = String.Format("{0:F2}", 손익율);
                                              }
                                              else //최고율에 값이 있다면.
                                              {
                                                  if (highestRate.ContainsKey(종목코드))
                                                  {
                                                      double 저장최고율 = highestRate[종목코드];
                                                      if (손익율 > 저장최고율)
                                                          highestRate[종목코드] = 손익율;
                                                  }
                                                  else                                 
                                                      highestRate.Add(종목코드, 손익율);                             
                                              }

                                              balanceDataGridView["잔고_최고율", i].Value = String.Format("{0:F2}", highestRate[종목코드]);
                                              balanceDataGridView["잔고_최고율", i].Style.ForeColor = Color.DarkViolet;

                                              if (isTrading) //시작버튼 활성시.
                                              {
                                                  if (isTrailingStopCheckBox.Checked)
                                                  {
                                                     // double 최고치 = 0;
                                                     // if (highestRate.ContainsKey(종목코드))
                                                          // 최고치 = highestRate[종목코드];

                                                       if (2 < bi.최고율)
                                                      {

                                                          if (bi.손익율 < (bi.최고율 - 1))
                                                          {
                                                              orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                                                                            accountComboBox.Text, 2, 종목코드.Trim(), (int)bi.보유수량, 0, "03", ""));

                                                              insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동
                                                          }
                                                      }
                                                      else if (( 0.5 <= bi.최고율) && (bi.최고율 < 2))
                                                      {
                                                          if (bi.손익율 < (bi.최고율 - 0.5f))
                                                          {
                                                              orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                                                                            accountComboBox.Text, 2, 종목코드.Trim(), (int)bi.보유수량, 0, "03", ""));

                                                              insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동
                                                          }
                                                      }                                    
                                                  }
                                                  //수익매매 처리
                                                  else if (takeProfitCheckBox.Checked)
                                                  {
                                                      double takeProfit = (double)takeProfitNumericUpDown.Value;
                                                      if (bi.손익율 >= takeProfit)
                                                      {
                                                          orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                                                                             accountComboBox.Text, 2, 종목코드.Trim(), (int)bi.보유수량, 0, "03", ""));

                                                          insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동
                                                      }
                                                  }
                                                  //손절 처리
                                                  if (stopLossChheckBox.Checked)
                                                  {
                                                      double stopLoss = (double)stopLossNumericUpDown.Value;
                                                      if (bi.손익율 <= stopLoss)
                                                      {
                                                          orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                                                                               accountComboBox.Text, 2, 종목코드.Trim(), (int)bi.보유수량, 0, "03", ""));
                                                          insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동
                                                      }
                                                  }
                                              }
                                            //  return;
                                         // }
                                     // }
                                        */
                    /*
                    현재가 = 0;
                    for (int i = 0; i < conditionItemDataGridView.RowCount; i++)
                    {

                        if (conditionItemDataGridView["조건종목_종목코드", i].Value.ToString().Equals(종목코드.Trim()))
                        {
                            // condigionItemDataGridView["조건종목_종목코드", i].Value = 종목코드.Trim();
                            // condigionItemDataGridView["조건종목_종목명", i].Value = 종목명.Trim();

                            if (현재가1 < 0)// - 부호를 없애기 위해 -현재가를 곱해  양수로 만든다.                           
                                현재가 = -현재가1;                            
                            else                         
                                현재가 = 현재가1;

                            conditionItemDataGridView["조건종목_현재가", i].Style.ForeColor = Color.Green;
                        
                            conditionItemDataGridView["조건종목_현재가", i].Value = String.Format("{0:###,#}", 현재가);
                            conditionItemDataGridView["조건종목_전일대비", i].Value = String.Format("{0:###,#}", 전일대비);
                            conditionItemDataGridView["조건종목_등락율", i].Value = String.Format("{0:F2}", 등락율) + " %";
                            conditionItemDataGridView["조건종목_거래량", i].Value = String.Format("{0:###,#}", 거래량);
                            conditionItemDataGridView["조건종목_거래금", i].Value = String.Format("{0:###,#}", 거래금);

                            conditionItemDataGridView.ClearSelection(); // 첫 line index 되는것을 막는다.
                            return;
                        }
                    }

                  

                   
                    if (현재가1 < 0)// - 부호를 없애기 위해 -현재가를 곱해  양수로 만든다.                   
                        현재가 = -현재가1;
                   else       
                        현재가 = 현재가1;

                    conditionItemDataGridView.Rows.Insert(0);                   

                    conditionItemDataGridView["조건종목_종목코드", 0].Value = 종목코드.Trim();
                    conditionItemDataGridView["조건종목_종목명", 0].Value = 종목명.Trim();
                    conditionItemDataGridView["조건종목_현재가", 0].Value = String.Format("{0:###,#}", 현재가);
                    conditionItemDataGridView["조건종목_전일대비", 0].Value = String.Format("{0:###,#}", 전일대비);
                    conditionItemDataGridView["조건종목_등락율", 0].Value = String.Format("{0:F2}", 등락율) + " %";
                    conditionItemDataGridView["조건종목_거래량", 0].Value = String.Format("{0:###,#}", 거래량);
                    conditionItemDataGridView["조건종목_거래금", 0].Value = String.Format("{0:###,#}", 거래금);

                    conditionItemDataGridView["조건종목_현재가", 0].Style.ForeColor = Color.Green;

                    if (전일대비 > 0)
                    {
                        conditionItemDataGridView["조건종목_전일대비", 0].Style.ForeColor = Color.Red;
                        conditionItemDataGridView["조건종목_등락율", 0].Style.ForeColor = Color.Red;
                    }
                    else if (전일대비 < 0)
                    {
                        conditionItemDataGridView["조건종목_전일대비", 0].Style.ForeColor = Color.Blue;
                        conditionItemDataGridView["조건종목_등락율", 0].Style.ForeColor = Color.Blue;
                    }
                    else
                    {
                        conditionItemDataGridView["조건종목_전일대비", 0].Style.ForeColor = Color.Black;
                        conditionItemDataGridView["조건종목_등락율", 0].Style.ForeColor = Color.Black;
                    }

                    conditionItemDataGridView.ClearSelection(); // 첫 line index 되는것을 막는다.

                    //매수  시작--
                    if (isTrading)
                    {                       
                        if (buyConditionCheckBox.Checked)
                        {                           
                            if (buyConditionComboBox.Text.Equals(현재_석택된_조건식명))
                            {                              
                                //해당 종목코드 자동매수 요청.
                                if (accountComboBox.Text.Length > 0)
                                {                                   
                                    int 예탁금 = 0;
                                    int 총1회매수금액 = (int)totalAmountNumericUpDown.Value;
                                   
                                    if (int.Parse(예탁자산평가액Label.Text.Replace(",", "").Trim()) > 0)
                                        예탁금 = int.Parse(예탁자산평가액Label.Text.Replace(",", "").Trim());                                
                                                                                                        
                                    if ((총1회매수금액 > 0) && (현재가1 > 0) && (예탁금 > 총1회매수금액))
                                    {                                       
                                        int 매수량 = 총1회매수금액 / (int)현재가1;
                                     
                                        if (매수량 > 0)
                                        {
                                            //이미 매수한 같은 종목이 있는가 확인. -> balanceDataGridView
                                            if (BalanceListCheck(종목코드.Trim()))
                                            {
                                                insertListBox.Items.Add("=보유중인종목= 조건식명 : " + 현재_석택된_조건식명 + " 종목명 :" + 종목명);
                                            }
                                            else
                                            {
                                                int 적정매수량 = 0;
                                                if (isAdjustQuantityCheckBox.Checked) {
                                                    int 예탁자산 = int.Parse(예탁자산평가액Label.Text.Replace(",","").Trim());
                                                    int 예탁자산총매수량 = 예탁자산 / (int)현재가1;
                                                    적정매수량 = AdjustQuantity(매수할종목평균수량, 예탁자산총매수량);
                                                    orderClassList.Add(new OrderClass("주식매수요청", 화면번호_주식매수요청,
                                                                                               accountComboBox.Text, 1, 종목코드.Trim(), 적정매수량, 0, "03", ""));
                                                }
                                                else
                                                    orderClassList.Add(new OrderClass("주식매수요청", 화면번호_주식매수요청,
                                                                                            accountComboBox.Text, 1, 종목코드.Trim(), 매수량, 0, "03", ""));
                                            }
                                            insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("1회 매수 비용이 작습니다");
                                    }
                                }
                            }
                        }
                    }
                    */
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message.ToString() + "717");
                }
            }
        }
        
        /// <summary>
        /// 언제나 팔수 있는 량을 매수 기준으로 한다.
        /// </summary>      
        private int AdjustQuantity(int 매수할종목평균수량,int 매수량)
        {
            int 적정매수량 = 0;
            if (매수할종목평균수량 > 매수량)
                적정매수량 = 매수량;
            else
                적정매수량 = 매수할종목평균수량;
            return 적정매수량;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (sender.Equals(tradingStartButton))
            {
                //트레일링스톱과 수익율체크가 동시에 되면 팝업창을 띠위서 한가지만 할수 있도록 유도한다.
                if (takeProfitCheckBox.Checked && isTrailingStopCheckBox.Checked)
                {
                    MessageBox.Show("수익율 체크와 트레일링스톱을 \n동시에 설정할수 없습니다. \n한가지만 설정하세요!");
                    return;
                }

                isTrading = true;
                tradingStopButton.Enabled = true;
                tradingStartButton.Enabled = false;
            }
            else if (sender.Equals(tradingStopButton))
            {
                isTrading = false;
                tradingStopButton.Enabled = false;
                tradingStartButton.Enabled = true;
            }
        }

        private void AxKHOpenAPI1_OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            if (e.sGubun.Equals("0"))//접수 or 체결
            {
                // Console.WriteLine("sGubun : " +e.sGubun);
                // Console.WriteLine("sFIdList : " + e.sFIdList);
                try
                {
                    string 주문번호 = axKHOpenAPI1.GetChejanData(9203).Trim();
                    string 종목코드 = axKHOpenAPI1.GetChejanData(9001).Replace("A", "").Trim();
                    string 종목명 = axKHOpenAPI1.GetChejanData(302).Trim();
                    int 주문수량 = int.Parse(axKHOpenAPI1.GetChejanData(900).Trim());
                    int 미체결수량 = int.Parse(axKHOpenAPI1.GetChejanData(902).Trim());
                    int 주문가격 = int.Parse(axKHOpenAPI1.GetChejanData(901).Trim());
                    int 현재가 = int.Parse(axKHOpenAPI1.GetChejanData(10).Trim());
                    string 체결가 = axKHOpenAPI1.GetChejanData(910).Trim();
                    string 주문구분 = axKHOpenAPI1.GetChejanData(905).Trim();
                    string 시간 = axKHOpenAPI1.GetChejanData(908).Trim();
                    long 원주문번호 = long.Parse(axKHOpenAPI1.GetChejanData(904));

                    if (axKHOpenAPI1.GetChejanData(913).Trim() == "접수")
                    {
                        Console.WriteLine("접수  항목");
                     //  balanceItemList.Add(new BalanceItem(주문번호, 종목코드, 종목명, 주문수량, 주문가격, 미체결수량, 주문구분, 체결가, 현재가, 시간));
                    }

                    if (axKHOpenAPI1.GetChejanData(913).Trim() == "체결")
                    {
                        //Console.WriteLine("체결  항목"); //[ 미체결 항목 ]에 실시간으로 보여준다.
                        OutStanding os = outStandingList.Find(o => o.종목코드 == 종목코드);
                        if (os != null)
                        {
                            os.주문수량 = 주문수량;
                            os.미체결수량 = 미체결수량;
                            os.현재가 = 현재가;
                            os.체결가 = 체결가;
                            os.시간 = 시간;
                            os.주문구분 = 주문구분;

                            // 0 이면 완료 된것으로 간주.
                            if (os.미체결수량 == 0)
                                outStandingList.Remove(os);
                        }
                        else
                        {
                            outStandingList.Add(new OutStanding(주문번호,종목코드,종목명,주문수량,주문가격,
                                미체결수량,주문구분,체결가,현재가,시간));
                        }

                        outStandingDataGridView.DataSource = null;
                        outStandingDataGridView.DataSource = outStandingList;
                        outStandingDataGridView.CurrentRow.Selected = false;

                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message.ToString()+"589");
                }

            }
            else if (e.sGubun.Equals("1"))//잔고
            {
                string stockName = axKHOpenAPI1.GetChejanData(302);
                long currentPrice = long.Parse(axKHOpenAPI1.GetChejanData(10).Replace("-", ""));

                string 당일손익율 = axKHOpenAPI1.GetChejanData(8019);
               // string 당일실현손익률 = axKHOpenAPI1.GetChejanData(991);
                long 총매입금액 = long.Parse(axKHOpenAPI1.GetChejanData(932));
                long 당일투자손익 = long.Parse(axKHOpenAPI1.GetChejanData(950));
               
                long 당일실현손익 = long.Parse(axKHOpenAPI1.GetChejanData(990));
                long 예수금 = long.Parse(axKHOpenAPI1.GetChejanData(951));

                //axKHOpenAPI.SetRealReg  함수 호출 , "9001;10", "1");

                Console.WriteLine("종목명 : " + stockName + " | 현재 종가 : " + String.Format("{0:#,###}", currentPrice));
              
                Console.WriteLine("매입주문금액 : " + String.Format("{0:#,###}", 총매입금액) + " | 금일 실현손익 : " + String.Format("{0:#,###}", 당일실현손익));
                realizationProfitLabel.Text = String.Format("{0:#,###}", 당일투자손익);
                당일손익율Label.Text = 당일손익율;
                //당일손익율Label.Text = 당일실현손익률;
                Console.WriteLine("----------------------------------------------------------");
                //TODO 일단 주석처리
                //RequestAccountEstimation(); //잔고 요청 함수. //예수금,총매입금액,예탁자산평가액,당일투자손익,당일손익율
               //RequestOutStanding(); //미채결 요청 함수. -- ?? 필요 없다   실시간으로 남은수치를 보여주니까. 처음1회 만보여주고 나머진 x
               // RequestRealizationProfit(); //일자별 수익율.-단순한 손익만 보여주는 것으로 처음1회만 보여주고 후엔 실시간 로딩.
            }
        }

        private void AxKHOpenAPI1_OnReceiveRealCondition(object sender, _DKHOpenAPIEvents_OnReceiveRealConditionEvent e)
        {
            try
            {
                string 종목코드 = e.sTrCode;
                int 조건식번호 = int.Parse(e.strConditionIndex);
                string 조건식명 = e.strConditionName;
                string 종목타입 = e.strType;
                string 종목명 = axKHOpenAPI1.GetMasterCodeName(e.sTrCode);

                if (e.strType.Equals("I"))//종목 편입
                {                    
                    ConditionObject conditionObject = conditionList.Find(o => o.조건식번호 == 조건식번호);
                    if (conditionObject != null) // 입력되고 있는 조건식번호 입력  
                        currentConditionCode = 조건식번호; // "D"항목에서 공용으로 사용해서 윗것 다시 만듬.

                    string 스크린번호 = (실시간조건검색수신화면번호 + 조건식번호).ToString().Trim();
                    //종목코드와 종목명을  List에 없을경우 올려라 

                    string stockName = axKHOpenAPI1.GetMasterCodeName(e.sTrCode);
                    insertListBox.Items.Add("편입| 조건식번호 : " + e.strConditionIndex + " |종목코드 : " + e.sTrCode + "|" + "종목명 : " + stockName);
                    //현재가10,전일대비11,등락율12,누적거래량13,누적거래대금14,매수총잔량125,실현손익990;손익율8019,예수금951,매수호가총잔량125
                  
                    //axKHOpenAPI1.SetRealReg(스크린번호.Trim(), 종목코드, "10;11;12;13;14", "1");       
                    axKHOpenAPI1.SetRealReg(현재_스크린번호, 종목코드, "10;11;12;15;951;8019;990;991;125", "1"); //매수호가 총잔량 추가 /적정매수량 계산시필요
                  
                    insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동  
                  
                }
                else if (e.strType.Equals("D"))//종목 이탈
                {                    
                    ConditionObject conditionObject = conditionList.Find(o => o.조건식번호 == 조건식번호);
                    if (conditionObject != null) // 입력되고 있는 조건식번호 입력
                        currentConditionCode = 조건식번호;

                    string 스크린번호 = (실시간조건검색수신화면번호 + 조건식번호).ToString().Trim();
                    deleteListBox.Items.Add("[이탈]  : 조건식번호 : " + e.strConditionIndex + "  종목명 : " + 종목명);
                    deleteListBox.SelectedIndex = deleteListBox.Items.Count - 1;// 첫 line으로 항상 이동

                    삭제코드 = e.sTrCode.Trim();
                    
                    //종목리스트에서 더이상 나머지 data가 나오지 못하게 차단한다.           
                    axKHOpenAPI1.SetRealRemove(스크린번호.Trim(), e.sTrCode);// 실시간 시세해지                   

                    deleteCode.Add(삭제코드); // 단순히 삭제코드만 입력한다.
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message.ToString()+"1131");
            }         
        }

       

        private void AxKHOpenAPI1_OnReceiveTrCondition(object sender, _DKHOpenAPIEvents_OnReceiveTrConditionEvent e)
        {
            //TODO 실시간 검색만 하기 때문에  / 없어도 잘 작동되네.    문자열 오류가 사라졌다.. 나중에 확인.
            //  e.sScrNo;
            int 조건번호 = e.nIndex;
            string codeList = e.strCodeList;
            if (codeList.Length > 0)
            {
                if (codeList[codeList.Length - 1] == ';') // 마직막에 ; <--삭제
                    codeList = codeList.Remove(codeList.Length - 1);

                int codeCount = codeList.Split(';').Length;

                if(codeCount <= 100)
                axKHOpenAPI1.CommKwRqData(codeList, 0, codeCount, 0, "조건식종목요청;" + 조건번호, 화면번호_조건식종목요청);
                if (e.nIndex != 0) // 더받을것이 있다면   +추가;
                    axKHOpenAPI1.SendCondition(화면번호_조건식종목추가요청, e.strConditionName, e.nIndex, 1);               
            }
        }

        public void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            try
            {

                if (sender.Equals(conditionDataGridView))
                {
                    if (conditionDataGridView.SelectedCells.Count > 0) //셀선택 된것!.
                    {
                        int rowIndex = conditionDataGridView.SelectedCells[0].RowIndex;

                        if (rowIndex > -1 && rowIndex < conditionDataGridView.Rows.Count)
                        {

                            string 조건명 = "";
                            if (conditionDataGridView["조건식_조건번호", rowIndex].Value != null)
                            {
                                if (이전_조건식번호 != -1 && !이전_조건식명.Equals(String.Empty))
                                {
                                    Console.WriteLine("이전_스크린번호:" + 이전_스크린번호 + "/이전_조건식명:" + 이전_조건식명 + "/이전_조건식번호:" +
                                                      이전_조건식번호);
                                    axKHOpenAPI1.SendConditionStop(이전_스크린번호, 이전_조건식명, 이전_조건식번호); // 조건식에대한 정지.
                                    axKHOpenAPI1.SetRealRemove("ALL", "ALL"); //화면에 더이상 못나게 하고. 종목에 대한정지.                              

                                    deleteCode.Clear();
                                    conditionViewList.Clear();
                                    conditionItemDataGridView.DataSource = null;
                                }

                                현재_선택된_조건식번호 = int.Parse(conditionDataGridView["조건식_조건번호", rowIndex].Value.ToString());
                                현재_석택된_조건식명 =
                                    conditionDataGridView["조건식_조건식명", rowIndex].Value.ToString(); //OnReciveRealData 에서 매수에 사용하려고 저장용도.


                                이전_조건식번호 = 현재_선택된_조건식번호;
                                이전_조건식명 = 현재_석택된_조건식명;

                                현재_스크린번호 = (현재_선택된_조건식번호 + 실시간조건검색수신화면번호).ToString().Trim();
                                이전_스크린번호 = 현재_스크린번호;

                                //잔고요청.
                                // RequestAccountEstimation();
                                //미체결 요청.
                                // RequestOutStanding();
                                //일자별 수익율.
                                // RequestRealizationProfit();
                                // axKHOpenAPI1.SetRealReg(현재_스크린번호,"990","990",)
                                int result = axKHOpenAPI1.SendCondition(화면번호_조건검색, 현재_석택된_조건식명, 현재_선택된_조건식번호, 1);

                                if (result == 1) //성공
                                {
                                    Console.WriteLine("조건검색 성공");
                                }
                                else
                                {
                                    Console.WriteLine("조건검색 실패");
                                    MessageBox.Show("1분 후 다시 검색해주세요 \n 검색식패");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message.ToString());
            }
        }

        public void AxKHOpenAPI1_OnReceiveConditionVer(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveConditionVerEvent e)
        {            

            string conditionNameList = axKHOpenAPI1.GetConditionNameList();
            string[] conditionArray = conditionNameList.Split(';');

            for (int i = 0; i < conditionArray.Length; i++)
            {
                string[] condition = conditionArray[i].Split('^');

                if (condition.Length == 2)
                {
                    //
                    int 조건식인덱스 = int.Parse(condition[0]);
                    string 조건식명 = condition[1]; //조건명. 

                    conditionDataGridView.Rows.Add();//추가.
                    conditionDataGridView["조건식_조건번호", i].Value = 조건식인덱스;
                    conditionDataGridView["조건식_조건식명", i].Value = 조건식명;

                    //리스트 객체에 조건 정보 객체 추가.
                    conditionList.Add(new ConditionObject(조건식인덱스, 조건식명));

                    //매수&매도 선택할수 있도록  콤보박스에  추가
                    buyConditionComboBox.Items.Add(조건식명);
                    sellConditionComboBox.Items.Add(조건식명);
                }
            }

            conditionDataGridView.ClearSelection(); // 첫 line index 되는것을 막는다.
        }

        public void AxKHOpenAPI1_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if (e.sRQName.Equals("계좌평가현황요청"))
            {
                long 예수금 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "예수금").Trim());
                long 총매입금액 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총매입금액").Trim());
                long 예탁자산평가액 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "예탁자산평가액").Trim());
                long 당일투자손익 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "당일투자손익").Trim());
                double 당일손익율 = double.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "당일손익율").Trim());

                예수금Label.Text = String.Format("{0:###,#}", 예수금);
                총매입금액Label1.Text = String.Format("{0:###,#}", 총매입금액);
                예탁자산평가액Label.Text = String.Format("{0:###,#}", 예탁자산평가액);
                당일손익금액Label.Text = String.Format("{0:###,#}", 당일투자손익);
                당일손익율Label.Text = String.Format("{0:F2}", 당일손익율) + " %";
                try
                {
                    //잔고 datalist 추가
                    int n = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                    // balanceDataGridView.Rows.Clear();
                    for (int i = 0; i < n; i++)
                    {
                        string 종목코드 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, 
                            "종목코드").Replace("A", "").Trim();
                        string 종목명 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();
                        long 보유수량 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "보유수량"));
                        long 현재가 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가"));
                        long 평가금액 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "평가금액"));
                        long 손익금액 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "손익금액"));
                        double 손익율 = double.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "손익율"));
                        long 매수금 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "매입금액"));

                        double 손익비율 = double.Parse(손익율.ToString("N2"));
                        balanceItemList.Add(new BalanceItem(종목코드, 평가금액, 종목명, 보유수량, 현재가, 손익금액, 
                            손익비율, 손익비율, 매수금));
                  
                        //예수금951/손익율8019/당일실현손익990/당일신현손익율991/
                        // axKHOpenAPI1.SetRealReg(화면번호_실시간데이터요청, 종목코드, "10;11;12;15", "1"); //실시간 잔고 요청.      
                        axKHOpenAPI1.SetRealReg(화면번호_실시간데이터요청, 종목코드, "10;11;12;15;951;8019;990;991",
                            "1"); //실시간 잔고 요청.     
                    }

                    balanceDataGridView.DataSource = balanceItemList;
                    balanceDataGridView.CurrentRow.Selected = false;

                    for (int i = 0; i < balanceDataGridView.RowCount; i++)
                    {
                        // 손익금액  값 비교.
                        int val = int.Parse(balanceDataGridView.Rows[i].Cells[5].Value.ToString());
                        // Console.WriteLine("val :" + val);
                        if (val > 0)
                        {
                            balanceDataGridView.Rows[i].Cells[5].Style.ForeColor = Color.Red;
                            balanceDataGridView.Rows[i].Cells[6].Style.ForeColor = Color.Red;
                        }
                        else if (val < 0)
                        {
                            balanceDataGridView.Rows[i].Cells[5].Style.ForeColor = Color.Blue;
                            balanceDataGridView.Rows[i].Cells[6].Style.ForeColor = Color.Blue;
                        }
                        else
                        {
                            balanceDataGridView.Rows[i].Cells[5].Style.ForeColor = Color.Black;
                            balanceDataGridView.Rows[i].Cells[6].Style.ForeColor = Color.Black;
                        }

                        balanceDataGridView.Rows[i].Cells[7].Style.ForeColor = Color.DarkViolet; //최고율
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message.ToString() + 1076);
                }
            }
            else if (e.sRQName.Equals("실시간미체결요청"))
            {
                int count = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                for (int i = 0; i < count; i++)
                {
                    string 주문번호 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "주문번호").Trim();
                    string 종목코드 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim();
                    string 종목명 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();
                    int 주문수량 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "주문수량"));
                    int 주문가격 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "주문가격"));
                    int 미체결수량 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "미체결수량"));
                    string 주문구분 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "주문구분").Trim(); ;
                    string 체결가 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "체결가");
                    // int 체결량 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "체결량"));
                    int 현재가 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가"));
                    string 시간 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "시간").Trim();
                   

                    OutStanding os = outStandingList.Find(o => o.종목코드 == 종목코드);
                    if (os != null)
                    {
                        os.주문번호 = 주문번호;
                        os.주문수량 = 주문수량;
                        os.주문가격 = 주문가격;
                        os.미체결수량 = 미체결수량;
                        os.주문구분 = 주문구분;
                        os.체결가 = 체결가;
                        os.현재가 = 현재가;
                        os.시간 = 시간;
                    }
                    else
                    {
                        outStandingList.Add(new OutStanding(주문번호,종목코드,종목명,주문수량,주문가격,
                            미체결수량,주문구분,체결가,현재가,시간));
                    }

                    outStandingDataGridView.DataSource = outStandingList;
                    outStandingDataGridView.CurrentRow.Selected = false;
                }
            }
            else if (e.sRQName.Contains("조건식종목요청"))
            {
                if (e.sRQName.Split(';').Length == 2)
                {
                    string[] 조건문배열 = e.sRQName.Split(';');
                    int 조건식번호 = int.Parse(조건문배열[1]);

                    //종목코드,종목명,현재가,전일대비,등락율,거래량
                    int n = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                    for (int i = 0; i < n; i++)
                    {
                        string 종목코드 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim();
                        string 종목명 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();
                        int 현재가 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim());
                        int 전일대비 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "전일대비"));
                        double 등락율 = double.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "등락율").Trim());
                        long 거래량 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "거래량").Trim());
                        long 거래금 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "거래대금").Trim());

                        conditionViewList.Add(new StockItemObject(종목코드,종목명,현재가,전일대비,등락율,거래량,거래금));
                    }
                    //Console.WriteLine("리스트 : "+ conditionViewList.Count);
                    conditionItemDataGridView.DataSource = conditionViewList;
                    conditionItemDataGridView.CurrentRow.Selected = false;

                    for(int i =0; i < conditionItemDataGridView.RowCount;i++)
                    {
                      // 전일대비  값 비교.
                        int val = int.Parse(conditionItemDataGridView.Rows[i].Cells[3].Value.ToString());
                       // Console.WriteLine("val :" + val);
                        if (val > 0)
                        {
                            conditionItemDataGridView.Rows[i].Cells[4].Style.ForeColor = Color.Red;
                            conditionItemDataGridView.Rows[i].Cells[3].Style.ForeColor = Color.Red;
                        }
                        else if (val < 0)
                        {
                            conditionItemDataGridView.Rows[i].Cells[4].Style.ForeColor = Color.Blue;
                            conditionItemDataGridView.Rows[i].Cells[3].Style.ForeColor = Color.Blue;
                        }
                        else
                        {
                            conditionItemDataGridView.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                            conditionItemDataGridView.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        }
                    }
                }
            }
            else if (e.sRQName.Equals("일자별실현손익"))
            {
                try
                {
                    int 실현손익 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "실현손익"));
                    if (실현손익 > 0)
                        realizationProfitLabel.Text = 실현손익.ToString().Trim();// String.Format("{0:###,#}", 실현손익);
                    if (실현손익 > 0)
                        realizationProfitLabel.ForeColor = Color.Red;
                    else if (실현손익 < 0)
                        realizationProfitLabel.ForeColor = Color.Blue;
                    else
                        realizationProfitLabel.ForeColor = Color.Black;
                }
                catch (Exception exeption)
                {
                    Console.WriteLine(exeption.Message.ToString()+"1400");
                }

            }
            //-------------수동 모드 
            else if (e.sRQName.Equals("주문종목정보"))
            {
                try
                {
                    string 종목명 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목명").Trim();
                    int 현재가 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "현재가"));

                    if (현재가 < 0)//음수로 나올경우 양수로 변경.
                        현재가 = -현재가;

                    itemNameLabel.Text = 종목명;
                    priceManualNumericUpDown.Value = 현재가;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message.ToString()+"1459");
                }
            }
        }
        public void AxKHOpenAPI1_OnEventConnect(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (e.nErrCode == 0)
            {            
                string[] accountArray = axKHOpenAPI1.GetLoginInfo("ACCNO").Split(';');
                for (int i = 0; i < accountArray.Length; i++)
                    accountComboBox.Items.Add(accountArray[i]);

                if (accountComboBox.Items.Count > 0)
                    accountComboBox.SelectedIndex = 0;

                userNameLabel.Text = axKHOpenAPI1.GetLoginInfo("USER_NAME");

                //전채주식 검색사용 이름 얻어 저장하기
                SetItemCodeList();

                //미체결 요청.
                RequestOutStanding();

                //잔고요청.
                RequestAccountEstimation();               
                //일자별 수익율.
                RequestRealizationProfit(); 

                //사용자 조건식 목록 요청.              
                axKHOpenAPI1.GetConditionLoad();
            }

            else if (e.nErrCode == 100)
                MessageBox.Show("사용자 정보교환 실패");
            else if (e.nErrCode == 101)
                MessageBox.Show("서버접속 실패");
            else if (e.nErrCode == 102)
                MessageBox.Show("버전처리 실패");
            else 
                MessageBox.Show("알 수 없는 오류 발생");
        }

        /// <summary>
        /// 전체 주식이름을 얻어와 검색에 사용한다.
        ///  [시장구분값]          0 : 장내    10 : 코스닥     3 : ELW      8 : ETF      50 : KONEX      4 :  뮤추얼펀드     5 : 신주인수권
        ///                             6 : 리츠       9 : 하이얼펀드       30 : K-OTC
        /// </summary>
        private void SetItemCodeList()
        {
            string itemCodeList = axKHOpenAPI1.GetCodeListByMarket("10");
            string[] itemCodeArrary = itemCodeList.Split(';');

            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection(); // 자동검색 

            for (int i=0; i < itemCodeArrary.Length; i++)
            {
                string itemName = axKHOpenAPI1.GetMasterCodeName(itemCodeArrary[i]);
                ItemInfo itemInfo = new ItemInfo(itemCodeArrary[i], itemName);
                itemInfoList.Add(itemInfo);     
                
                acsc.Add(itemName); // 자동검색
            }
            searchTextBox.AutoCompleteCustomSource = acsc;//자동검색
        }

        /// <summary>
        /// 일자별 수익율.
        /// </summary>
        public void RequestRealizationProfit()
        {
            if (accountComboBox.Text.Length > 0)
            {
                axKHOpenAPI1.SetInputValue("계좌번호", accountComboBox.Text);
                axKHOpenAPI1.SetInputValue("시작일자", DateTime.Now.ToString("yyyyMMdd"));
                axKHOpenAPI1.SetInputValue("종료일자", DateTime.Now.ToString("yyyyMMdd"));

                axKHOpenAPI1.CommRqData("일자별실현손익", "opt10074", 0, 화면번호_일자별실현손익);
                //axKHOpenAPI1.CommRqData("일자별실현손익", "opt10074", 2, 화면번호_일자별실현손익);
            }
        }
        /// <summary>
        /// 잔고 요청.
        /// </summary>
        public void RequestAccountEstimation()
        {
            if (accountComboBox.Text.Length > 0)
            {
                axKHOpenAPI1.SetInputValue("계좌번호", accountComboBox.Text);
                axKHOpenAPI1.SetInputValue("비밀번호", "");
                axKHOpenAPI1.SetInputValue("상장폐지조회구분", "0");
                axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");

                axKHOpenAPI1.CommRqData("계좌평가현황요청", "OPW00004", 0, 화면번호_계좌평가현황요청);
                //axKHOpenAPI1.CommRqData("계좌평가현황요청", "OPW00004", 2, 화면번호_계좌평가현황요청);
            }
        }
        //https://goni9071.tistory.com/262
        //CommRqData
        // 살펴보자.
        /// <summary>
        /// 실시간 미체결
        /// </summary>
        public void RequestOutStanding()
        {
            if (accountComboBox.Text.Length > 0)
            {
                axKHOpenAPI1.SetInputValue("계좌번호", accountComboBox.Text.Trim());
                axKHOpenAPI1.SetInputValue("체결구분", "1"); //체결2 미체결:1
                axKHOpenAPI1.SetInputValue("매매구분", "0");//전체:0  매도:1 매수:2

                axKHOpenAPI1.CommRqData("실시간미체결요청", "opt10075", 0, 화면번호_실시간미체결요청);
                //axKHOpenAPI1.CommRqData("실시간미체결요청", "opt10075", 2, 화면번호_실시간미체결요청);
            }
        }

        bool BalanceListCheck(string kindCode)
        {
            bool b = false;

            BalanceItem bi = balanceItemList.Find(o => o.종목코드 == kindCode);
            if (bi != null)
                b = true;

            return b;
        }


        public   DateTime Delay(int MS,string 삭제코드)
        {
            string 저장삭제코드 = 삭제코드;
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);
         
            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            if (deleteCode.Count > 0)
            {
                string 삭제 = deleteCode.Find(o => o.Trim() == 저장삭제코드.Trim());
                deleteCode.Remove(삭제);

                StockItemObject sio = conditionViewList.Find(o => o.종목코드 == 저장삭제코드);
                if (sio != null)
                {
                    conditionViewList.Remove(sio);
                    //삭제와 상태를 보여주기.
                    conditionItemDataGridView.DataSource = null;
                    conditionItemDataGridView.DataSource = conditionViewList;
                    conditionItemDataGridView.CurrentRow.Selected = false;
                }

                //  Console.WriteLine("삭제 코드 수 :: " + co.stockItemList.Count);
            }
            return DateTime.Now;
        }

        public DateTime DelayOrder(int MS, OrderClass orderClass)
        {
            //잔고창에 목록이 있는것은 재매수 하지 않는다.
            BalanceItem bi = balanceItemList.Find(o => o.종목코드 == orderClass.종목코드);

            if ((bi == null && orderClass.주문형태 == "주문매수") || (bi != null && orderClass.주문형태 == "주문매도"))
            {

                //먼저 일하고 쉬었다 다음일 진행.
                int orderResult = axKHOpenAPI1.SendOrder(orderClass.주문형태, orderClass.화면번호, orderClass.계정,
                    orderClass.타입, orderClass.종목코드, orderClass.보유수량,
                    orderClass.가격, orderClass.호가구분, orderClass.원래주문번호);

                if (orderClass.주문형태 == "주문매도")
                {
                    balanceItemList.Remove(bi); // 잔고창에서 삭제.

                    balanceDataGridView.DataSource = null;
                    balanceDataGridView.DataSource = balanceItemList;
                    balanceDataGridView.CurrentRow.Selected = false;
                }

                if (orderResult == 0)
                    insertListBox.Items.Add("[*" + orderClass.주문형태 + "=성공*]  종목명 :" +
                                            axKHOpenAPI1.GetMasterCodeName(orderClass.종목코드));
                else
                    insertListBox.Items.Add("[*" + orderClass.주문형태 + "=실패*]  종목명 :" +
                                            axKHOpenAPI1.GetMasterCodeName(orderClass.종목코드));

                OrderClass order = orderClassList.Find(o => o.종목코드 == orderClass.종목코드);
                orderClassList.Remove(order); // 앞에 있는 오더 삭제.

                // 먼저 일 끝.

                DateTime ThisMoment = DateTime.Now;
                TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
                DateTime AfterWards = ThisMoment.Add(duration);

                while (AfterWards >= ThisMoment)
                {
                    System.Windows.Forms.Application.DoEvents();
                    ThisMoment = DateTime.Now;
                }
            }

            return DateTime.Now;
        }       
    }
    /// <summary>
    /// 전체 종목 각각 담을 묶음.
    /// </summary>
    public class ItemInfo
    {
        public string itemCode;
        public string itemName;
        public ItemInfo(string itemCode,string itemName)
        {
            this.itemCode = itemCode;
            this.itemName = itemName;
        }
    }

    /// <summary>
    /// 조건식 이름 객체
    /// </summary>
    public class ConditionObject
    {
        public int 조건식번호;
        public string 조건식명;
        public List<StockItemObject> stockItemList;

        public ConditionObject(int 조건식번호, string 조건식명)
        {
            this.조건식번호 = 조건식번호;
            this.조건식명 = 조건식명;
            stockItemList = new List<StockItemObject>();
        }
    }
    /// <summary>
    /// 조건식별 포함되는 종목들의 리스트 묶음
    /// </summary>
    public class StockItemObject
    {
        //종목코드,종목명,현재가,전일대비,등락율,거래량
        public string 종목코드 { get; set; }
        public string 종목명 { get; set; }
        public long 현재가 { get; set; }
        public long 전일대비 { get; set; }
        public double 등락율 { get; set; }
        public long 거래량 { get; set; }
        public long 거래금 { get; set; }
        public StockItemObject(string 종목코드, string 종목명, long 현재가, long 전일대비, double 등락율, long 거래량,long 거래금)
        {
            this.종목코드 = 종목코드;
            this.종목명 = 종목명;
            this.현재가 = 현재가;
            this.전일대비 = 전일대비;
            this.등락율 = 등락율;
            this.거래량 = 거래량;
            this.거래금 = 거래금;
        }
    }

    /// <summary>
    /// 잔고 관리
    /// </summary>
    public class BalanceItem
    {
        public string 종목코드 { get; set; }
        public long 평가금액 { get; set; }
        public string 종목명 { get; set; }
        public long 보유수량 { get; set; }
        public long 현재가 { get; set; }
        public long 손익금액 { get; set; }
        public double 손익율 { get; set; }
        public double 최고율 { get; set; }
        public long 매수금 { get; set; }

        public BalanceItem( string 종목코드, long 평가금액, string 종목명, long 보유수량,
            long 현재가, long 손익금액,double 손익율,double 최고율, long 매수금)
        {
            this. 종목코드 = 종목코드;
          this.평가금액 = 평가금액;
          this.종목명 = 종목명;
          this.보유수량 = 보유수량;
          this.현재가 = 현재가;
          this.손익금액 = 손익금액;
          this.손익율 = 손익율;
          this.최고율 = 최고율;
          this.매수금 = 매수금;
        }
    }

    /// <summary>
    /// 미체결 관리
    /// </summary>
    public class OutStanding
    {
        public string 주문번호 { get; set; }
        public string 종목코드 { get; set; }
        public string 종목명 { get; set; }
        public int 주문수량 { get; set; }
        public int 주문가격 { get; set; }
        public int 미체결수량 { get; set; }
        public string 주문구분 { get; set; }
        public string 체결가 { get; set; }
        public int 현재가 { get; set; }
        public string 시간 { get; set; }

        public OutStanding(string 주문번호, string 종목코드, string 종목명, int 주문수량, int 주문가격,
            int 미체결수량, string 주문구분, string 체결가, int 현재가, string 시간)
        {
            this.주문번호 = 주문번호;
            this.종목코드 = 종목코드;
            this.종목명 = 종목명;
            this.주문수량 = 주문수량;
            this.주문가격 = 주문가격;
            this.미체결수량 = 미체결수량;
            this.주문구분 = 주문구분;
            this.체결가 = 체결가;
            this.현재가 = 현재가;
            this.시간 = 시간;
        }
    }

    /// <summary>
    /// 매수/매도 관리
    /// </summary>
    public class OrderClass
    {
        public string 주문형태;
        public string 화면번호;
        public string 계정;
        public int 타입;
        public string 종목코드;
        public int 보유수량;
        public int 가격;
        public string 호가구분;
        public string 원래주문번호;

        public OrderClass(string 주문형태,string 화면번호,string 계정,int 타입,string 종목코드, int 보유수량,int 가격,string 호가구분,string 원래주문번호)
        {
            this.주문형태 = 주문형태;
            this.화면번호 = 화면번호;
            this.계정 = 계정;
            this.타입 = 타입;
            this.종목코드 = 종목코드;
            this.보유수량 = 보유수량;
            this.가격 = 가격;
            this.호가구분 = 호가구분;
            this.원래주문번호 = 원래주문번호;
        }
    }
}