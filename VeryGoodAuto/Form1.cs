using System;
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
        List<BalanceItem> changeOutStandingList;

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

        Thread startThead; // 무한 while 하면서 삭제할 종목이 있다면 계속 삭제하는 일만 한다.
        Thread orderClassManagerThread; // 매수와 매도가 orderClassList에 있다면 무조건 실행해라. 제한 넘지 않게.

        public List<string> deleteCode;// 삭제할 종목
        public List<OrderClass> orderClassList; // 매수와 매도를 함깨 동기화하여 시간제한 1초/4회 를 넘지 안게 관리한다.
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

            conditionList = new List<ConditionObject>();
            itemInfoList = new List<ItemInfo>(); // 주식명 검색을 위한 리스트.
            balanceItemList = new List<BalanceItem>();// 잔고와 미체결 관리를 한다.
            changeOutStandingList = new List<BalanceItem>();


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
            outStandingDataGridView.SelectionChanged += DataGridView_SelectedChanged;
            balanceDataGridView.SelectionChanged += DataGridView_SelectedChanged;
            #endregion 수동모드
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
        /// 미체결 리스트트 클릭시 시행 함수.
        /// </summary>     
        private void DataGridView_SelectedChanged(object sender, EventArgs e)
        {
            if (sender.Equals(outStandingDataGridView))
            {
                try
                {                   
                    if (outStandingDataGridView.SelectedCells.Count > 0)// 클릭시.
                    {                       
                        int selectedRowIndex = outStandingDataGridView.SelectedCells[0].RowIndex;
                        if (outStandingDataGridView["미체결_주문번호", selectedRowIndex].Value != null)
                        {
                            string 주문번호 = outStandingDataGridView["미체결_주문번호", selectedRowIndex].Value.ToString();
                            string 종목코드 = outStandingDataGridView["미체결_종목코드", selectedRowIndex].Value.ToString();
                            string 종목명 = outStandingDataGridView["미체결_종목명", selectedRowIndex].Value.ToString();
                            string 미체결수량 = outStandingDataGridView["미체결_미체결수량", selectedRowIndex].Value.ToString();
                            string 주문가격 = outStandingDataGridView["미체결_주문가격", selectedRowIndex].Value.ToString();
                            string 주문구분 = outStandingDataGridView["미체결_주문구분", selectedRowIndex].Value.ToString();

                            주문구분 = 주문구분.Replace("+", "").Replace("-", "");
                            if (주문구분.Length >= 2)
                                tradeOptionComboBox.Text = 주문구분.Substring(0, 2);

                            originOrderNumtextBox.Text = 주문번호;
                            itemCodeTextBox.Text = 종목코드;
                            itemNameLabel.Text = 종목명;
                            amountManulNumericUpDown.Value = int.Parse(미체결수량);
                            priceManualNumericUpDown.Value = int.Parse(주문가격);
                        }
                    }
                }catch(Exception exception)
                {
                    Console.WriteLine(exception.Message.ToString()+"201");
                }
            }
            else if (sender.Equals(balanceDataGridView)) //잔고 리스트 클릭시 매도할수 있도록.
            {                
                try
                {
                    if (balanceDataGridView.SelectedCells.Count > 0)// 클릭시.
                    {
                        int selectedRowIndex = balanceDataGridView.SelectedCells[0].RowIndex;
                      
                        if (balanceDataGridView["잔고_종목코드", selectedRowIndex].Value != null) // 시작과 동시에 호출 되서 null 을 인식한다.
                        {
                            string 종목코드 = balanceDataGridView["잔고_종목코드", selectedRowIndex].Value.ToString();
                            string 종목명 = balanceDataGridView["잔고_종목명", selectedRowIndex].Value.ToString();
                            string 보유수량 = balanceDataGridView["잔고_보유수량", selectedRowIndex].Value.ToString();
                            string 현재가격 = balanceDataGridView["잔고_현재가", selectedRowIndex].Value.ToString();
                            tradeOptionComboBox.Text = "매도";

                            itemCodeTextBox.Text = 종목코드;
                            itemNameLabel.Text = 종목명;
                            amountManulNumericUpDown.Value = decimal.Parse(보유수량.Trim());
                            priceManualNumericUpDown.Value = decimal.Parse(현재가격.Trim());
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message.ToString() +"229");
                }
                balanceDataGridView.ClearSelection(); // 첫 line index 되는것을 막는다.
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
            //List에 키값을 찾아 항목의 값을 변경해라. 
            //string 종목코드 = e.sRealKey;     
            //예수금951/손익율8019/당일실현손익990/당일신현손익율991/
            // [ opw00018 : 계좌평가잔고내역요청 ]
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

               
                try
                {
                    long 현재가 = 0;
                    for (int i = 0; i < balanceDataGridView.RowCount; i++)
                    {
                        if ( balanceDataGridView["잔고_종목코드", i].Value.ToString().Equals(종목코드))
                        {
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
                                    double 최고치 = 0;
                                    if (highestRate.ContainsKey(종목코드))
                                         최고치 = highestRate[종목코드];

                                     if (2 < 최고치)
                                    {

                                        if (손익율 < (최고치 - 1))
                                        {
                                            orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                                                          accountComboBox.Text, 2, 종목코드.Trim(), 보유수량, 0, "03", ""));

                                            insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동
                                        }
                                    }
                                    else if (( 0.5 <= 최고치) && (최고치 < 2))
                                    {
                                        if (손익율 < (최고치 - 0.5f))
                                        {
                                            orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                                                          accountComboBox.Text, 2, 종목코드.Trim(), 보유수량, 0, "03", ""));

                                            insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동
                                        }
                                    }                                    
                                }
                                //수익매매 처리
                                else if (takeProfitCheckBox.Checked)
                                {
                                    double takeProfit = (double)takeProfitNumericUpDown.Value;
                                    if (손익율 >= takeProfit)
                                    {
                                        orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                                                           accountComboBox.Text, 2, 종목코드.Trim(), 보유수량, 0, "03", ""));

                                        insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동
                                    }
                                }
                                //손절 처리
                                if (stopLossChheckBox.Checked)
                                {
                                    double stopLoss = (double)stopLossNumericUpDown.Value;
                                    if (손익율 <= stopLoss)
                                    {
                                        orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                                                             accountComboBox.Text, 2, 종목코드.Trim(), 보유수량, 0, "03", ""));
                                        insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동
                                    }
                                }
                            }
                            return;
                        }
                    }

                   현재가 = 0;
                    for (int i = 0; i < condigionItemDataGridView.RowCount; i++)
                    {

                        if (condigionItemDataGridView["조건종목_종목코드", i].Value.ToString().Equals(종목코드.Trim()))
                        {
                            // condigionItemDataGridView["조건종목_종목코드", i].Value = 종목코드.Trim();
                            // condigionItemDataGridView["조건종목_종목명", i].Value = 종목명.Trim();

                            if (현재가1 < 0)// - 부호를 없애기 위해 -현재가를 곱해  양수로 만든다.                           
                                현재가 = -현재가1;                            
                            else                         
                                현재가 = 현재가1;

                            condigionItemDataGridView["조건종목_현재가", i].Style.ForeColor = Color.Green;
                        
                            condigionItemDataGridView["조건종목_현재가", i].Value = String.Format("{0:###,#}", 현재가);
                            condigionItemDataGridView["조건종목_전일대비", i].Value = String.Format("{0:###,#}", 전일대비);
                            condigionItemDataGridView["조건종목_등락율", i].Value = String.Format("{0:F2}", 등락율) + " %";
                            condigionItemDataGridView["조건종목_거래량", i].Value = String.Format("{0:###,#}", 거래량);
                            condigionItemDataGridView["조건종목_거래금", i].Value = String.Format("{0:###,#}", 거래금);

                            condigionItemDataGridView.ClearSelection(); // 첫 line index 되는것을 막는다.
                            return;
                        }
                    }

                  

                   
                    if (현재가1 < 0)// - 부호를 없애기 위해 -현재가를 곱해  양수로 만든다.                   
                        현재가 = -현재가1;
                   else       
                        현재가 = 현재가1;

                    condigionItemDataGridView.Rows.Insert(0);                   

                    condigionItemDataGridView["조건종목_종목코드", 0].Value = 종목코드.Trim();
                    condigionItemDataGridView["조건종목_종목명", 0].Value = 종목명.Trim();
                    condigionItemDataGridView["조건종목_현재가", 0].Value = String.Format("{0:###,#}", 현재가);
                    condigionItemDataGridView["조건종목_전일대비", 0].Value = String.Format("{0:###,#}", 전일대비);
                    condigionItemDataGridView["조건종목_등락율", 0].Value = String.Format("{0:F2}", 등락율) + " %";
                    condigionItemDataGridView["조건종목_거래량", 0].Value = String.Format("{0:###,#}", 거래량);
                    condigionItemDataGridView["조건종목_거래금", 0].Value = String.Format("{0:###,#}", 거래금);

                    condigionItemDataGridView["조건종목_현재가", 0].Style.ForeColor = Color.Green;

                    if (전일대비 > 0)
                    {
                        condigionItemDataGridView["조건종목_전일대비", 0].Style.ForeColor = Color.Red;
                        condigionItemDataGridView["조건종목_등락율", 0].Style.ForeColor = Color.Red;
                    }
                    else if (전일대비 < 0)
                    {
                        condigionItemDataGridView["조건종목_전일대비", 0].Style.ForeColor = Color.Blue;
                        condigionItemDataGridView["조건종목_등락율", 0].Style.ForeColor = Color.Blue;
                    }
                    else
                    {
                        condigionItemDataGridView["조건종목_전일대비", 0].Style.ForeColor = Color.Black;
                        condigionItemDataGridView["조건종목_등락율", 0].Style.ForeColor = Color.Black;
                    }

                    condigionItemDataGridView.ClearSelection(); // 첫 line index 되는것을 막는다.

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
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message.ToString() + "554");
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
                    #region  수동주문
                    if (axKHOpenAPI1.GetChejanData(905).Equals("+매수"))
                    {
                        string 주문번호 = axKHOpenAPI1.GetChejanData(9203).Trim();
                        string 종목코드 = axKHOpenAPI1.GetChejanData(9001).Replace("A","").Trim();
                        string 종목명 = axKHOpenAPI1.GetChejanData(302).Trim();
                        int 주문수량 = int.Parse(axKHOpenAPI1.GetChejanData(900).Trim());
                        int 미체결수량 = int.Parse(axKHOpenAPI1.GetChejanData(902).Trim());
                        int 주문가격 = int.Parse(axKHOpenAPI1.GetChejanData(901).Trim());
                        int 현재가 = int.Parse(axKHOpenAPI1.GetChejanData(10).Trim());
                        int 체결가 = int.Parse(axKHOpenAPI1.GetChejanData(910).Trim());
                        string 주문구분 = axKHOpenAPI1.GetChejanData(905).Trim();
                        string 시간 = axKHOpenAPI1.GetChejanData(908).Trim();
                        long 원주문번호 = long.Parse(axKHOpenAPI1.GetChejanData(904));
                       
                        for(int i=0; i< balanceDataGridView.RowCount; i++)
                        {
                            if(balanceDataGridView["잔고_종목코드", i].Value.ToString().Equals(종목코드))
                            {
                                BalanceItem bi = balanceItemList.Find(o => o.종목코드 == 종목코드);
                                //변화된 값을 입력한다.
                                if (bi != null)
                                {
                                    bi.미체결수량 = 미체결수량;
                                    bi.현재가 = 현재가;
                                    bi.시간 = 시간;                                    
                                }
                                return;
                            }
                        }

                        //balanceDataGridView 에 없다면.
                        balanceItemList.Add(new BalanceItem(주문번호, 종목코드, 종목명, 주문수량, 주문가격, 미체결수량, 주문구분, 체결가, 현재가, 시간));

                        balanceDataGridView.Rows.Clear();

                        for(int i =0; i < balanceItemList.Count; i++)
                        {                           
                            BalanceItem bi = balanceItemList[i];
                            balanceDataGridView.Rows.Add();

                            balanceDataGridView["잔고_종목코드", i].Value = bi.주문번호;
                            // outStandingDataGridView["잔고_평가금액", i].Value = osdi.평가금액;
                            balanceDataGridView["잔고_종목명", i].Value = bi.종목명;
                            balanceDataGridView["잔고_보유수량", i].Value = bi.주문수량;
                            balanceDataGridView["잔고_평균단가", i].Value = bi.체결가;
                            balanceDataGridView["잔고_현재가", i].Value = bi.현재가;
                            balanceDataGridView["잔고_손익금액", i].Value = 0;
                            balanceDataGridView["잔고_손익율", i].Value = 0;
                            balanceDataGridView["잔고_최고율", i].Value = 0;
                            balanceDataGridView["잔고_매입금액", i].Value = bi.주문가격;

                            if (bi.미체결수량 > 0)
                            {
                                BalanceItem changeBi = changeOutStandingList.Find(o => o.종목코드 == bi.종목코드);
                                if (changeBi != null)
                                {
                                    changeOutStandingList.Remove(changeBi);
                                    changeOutStandingList.Add(bi);
                                }
                                else
                                    changeOutStandingList.Add(bi);
                            }
                                
                        }

                        // 미체결 뷰에서 보여주기.
                        outStandingDataGridView.Rows.Clear();
                        outStandingDataGridView.DataSource = changeOutStandingList;
                    }
                    #endregion 수동주문
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
                Console.WriteLine(exception.Message.ToString()+"597");
            }         
        }

       

        private void AxKHOpenAPI1_OnReceiveTrCondition(object sender, _DKHOpenAPIEvents_OnReceiveTrConditionEvent e)
        {
            //TODO 실시간 검색만 하기 때문에  / 없어도 잘 작동되네.    문자열 오류가 사라졌다.. 나중에 확인.
            /*
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
            */
        }

        public void DataGridView_SelectionChanged(object sender, EventArgs e)
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
                                Console.WriteLine("이전_스크린번호:" + 이전_스크린번호 + "/이전_조건식명:" + 이전_조건식명 + "/이전_조건식번호:" + 이전_조건식번호);
                                axKHOpenAPI1.SendConditionStop(이전_스크린번호, 이전_조건식명, 이전_조건식번호);// 조건식에대한 정지.
                                axKHOpenAPI1.SetRealRemove("ALL", "ALL"); //화면에 더이상 못나게 하고. 종목에 대한정지.                              
                              
                                deleteCode.Clear();
                                condigionItemDataGridView.Rows.Clear(); // 화면청소. 
                            }

                            현재_선택된_조건식번호 = int.Parse(conditionDataGridView["조건식_조건번호", rowIndex].Value.ToString());
                            현재_석택된_조건식명 = conditionDataGridView["조건식_조건식명", rowIndex].Value.ToString(); //OnReciveRealData 에서 매수에 사용하려고 저장용도.


                            이전_조건식번호 = 현재_선택된_조건식번호;                            
                            이전_조건식명 = 현재_석택된_조건식명;

                            현재_스크린번호 = (현재_선택된_조건식번호 + 실시간조건검색수신화면번호).ToString().Trim();
                            이전_스크린번호 =  현재_스크린번호;

                            //잔고요청.
                            RequestAccountEstimation();
                            //미체결 요청.
                            RequestOutStanding();
                            //일자별 수익율.
                            RequestRealizationProfit();
                          // axKHOpenAPI1.SetRealReg(현재_스크린번호,"990","990",)
                            int result = axKHOpenAPI1.SendCondition(화면번호_조건검색, 현재_석택된_조건식명, 현재_선택된_조건식번호, 1);                         

                            if (result == 1)//성공
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
                당일손익율Label.Text = String.Format("{0:F2}", 당일손익율) + "%";

                //잔고 datalist 추가
                int n = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                balanceDataGridView.Rows.Clear();
                for (int i = 0; i < n; i++)
                {
                    string 종목코드 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim().Replace("A", "");
                    string 종목명 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();
                    long 보유수량 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "보유수량"));
                    double 평균단가 = double.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "평균단가"));//-???/
                    long 현재가 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가"));
                    long 평가금액 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "평가금액"));
                    long 손익금액 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "손익금액"));
                    double 손익율 = double.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "손익율"));
                    long 매입금액 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "매입금액"));

                    balanceDataGridView.Rows.Add();//생성
                    balanceDataGridView["잔고_종목코드", i].Value = 종목코드;
                    balanceDataGridView["잔고_종목명", i].Value = 종목명;
                    balanceDataGridView["잔고_보유수량", i].Value = String.Format("{0:###,#}", 보유수량);
                    balanceDataGridView["잔고_평균단가", i].Value = String.Format("{0:###,#}", 평균단가);
                    balanceDataGridView["잔고_현재가", i].Value = String.Format("{0:###,#}", 현재가);
                    balanceDataGridView["잔고_평가금액", i].Value = String.Format("{0:###,#}", 평가금액);
                    balanceDataGridView["잔고_손익금액", i].Value = String.Format("{0:###,#}", 손익금액);
                    balanceDataGridView["잔고_손익율", i].Value = String.Format("{0:F2}", 손익율) + "%";
                    balanceDataGridView["잔고_매입금액", i].Value = String.Format("{0:###,#}", 매입금액);
                    /*
                    if (balanceDataGridView["잔고_최고율", i].Value == null) //최고율에 값이 없다면
                    {
                        if (!highestRate.ContainsKey(종목코드))
                        {
                            highestRate.Add(종목코드, 손익율);
                        }
                        // balanceDataGridView["잔고_최고율", i].Value = String.Format("{0:F2}", 손익율);
                    }
                    */
                   // if(balanceDataGridView["잔고_최고율", i].Value != null)
                  //  {
                        if (highestRate.ContainsKey(종목코드))
                        {
                            double 저장최고율 = highestRate[종목코드];
                            if (손익율 > 저장최고율)
                                highestRate[종목코드] = 손익율;

                        balanceDataGridView["잔고_최고율", i].Value = String.Format("{0:F2}", highestRate[종목코드]);
                        balanceDataGridView["잔고_최고율", i].Style.ForeColor = Color.DarkViolet;
                    }
                        //else
                          //  highestRate.Add(종목코드, 손익율);
                 //   }

                   

                    //색깔
                    if (손익금액 > 0)
                    {
                        balanceDataGridView["잔고_손익금액", i].Style.ForeColor = Color.Red;
                        balanceDataGridView["잔고_손익율", i].Style.ForeColor = Color.Red;
                    }
                    else if (손익금액 < 0)
                    {
                        balanceDataGridView["잔고_손익금액", i].Style.ForeColor = Color.Blue;
                        balanceDataGridView["잔고_손익율", i].Style.ForeColor = Color.Blue;
                    }
                    else
                    {
                        balanceDataGridView["잔고_손익금액", i].Style.ForeColor = Color.Black;
                        balanceDataGridView["잔고_손익율", i].Style.ForeColor = Color.Black;
                    }
                    //예수금951/손익율8019/당일실현손익990/당일신현손익율991/
                    // axKHOpenAPI1.SetRealReg(화면번호_실시간데이터요청, 종목코드, "10;11;12;15", "1"); //실시간 잔고 요청.      
                    axKHOpenAPI1.SetRealReg(화면번호_실시간데이터요청, 종목코드, "10;11;12;15;951;8019;990;991", "1"); //실시간 잔고 요청.     
                }
            }
            else if (e.sRQName.Equals("실시간미체결요청"))
            {
                int count = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                outStandingDataGridView.Rows.Clear();

                for (int i = 0; i < count; i++)
                {
                    string 주문번호 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "주문번호").Trim();
                    string 종목코드 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim();
                    string 종목명 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();
                    int 주문수량 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "주문수량"));
                    int 주문가격 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "주문가격"));
                    int 미체결수량 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "미체결수량"));
                    string 주문구분 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "주문구분").Trim(); ;
                    int 체결가 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "체결가"));
                    // int 체결량 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "체결량"));
                    int 현재가 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가"));
                    string 시간 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "시간").Trim(); ;

                    balanceItemList.Add(new BalanceItem(주문번호,종목코드,종목명,주문수량,주문가격,미체결수량, 주문구분,체결가,현재가,시간));
                   
                    outStandingDataGridView.Rows.Add();//생성
                    outStandingDataGridView["미체결_주문번호", i].Value = 주문번호;
                    outStandingDataGridView["미체결_종목코드", i].Value = 종목코드;
                    outStandingDataGridView["미체결_종목명", i].Value = 종목명;
                    outStandingDataGridView["미체결_주문수량", i].Value = String.Format("{0:###,#}", 주문수량);
                    outStandingDataGridView["미체결_주문가격", i].Value = String.Format("{0:###,#}", 주문가격);
                    outStandingDataGridView["미체결_미체결수량", i].Value = String.Format("{0:###,#}", 미체결수량);
                    outStandingDataGridView["미체결_주문구분", i].Value = 주문구분;
                    outStandingDataGridView["미체결_체결가", i].Value = String.Format("{0:###,#}", 체결가);
                    //  outStandingDataGridView["미체결_체결량", i].Value = String.Format("{0:###,#}" + 체결량);
                    outStandingDataGridView["미체결_현재가", i].Value = String.Format("{0:###,#}", 현재가);
                    outStandingDataGridView["미체결_시간", i].Value = 시간;  
                  
                }
                
            }
         /* 
            else if (e.sRQName.Contains("조건식종목요청"))
            {
                if (e.sRQName.Split(';').Length == 2)
                {
                    string[] 조건문배열 = e.sRQName.Split(';');
                    int 조건식번호 = int.Parse(조건문배열[1]);

                    //종목코드,종목명,현재가,전일대비,등락율,거래량
                    int n = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                    condigionItemDataGridView.Rows.Clear(); // 전 로우들 삭제.

                    for (int i = 0; i < n; i++)
                    {
                        string 종목코드 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드");
                        string 종목명 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목명");
                        int 현재가 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가"));
                        int 전일대비 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "전일대비"));
                        double 등락율 = double.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "등락율"));
                        long 거래량 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "거래량"));
                        long 거래금 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "거래금액"));

                        condigionItemDataGridView.Rows.Add();// 생성.
                        condigionItemDataGridView["조건종목_종목코드", i].Value = 종목코드.Trim();
                        condigionItemDataGridView["조건종목_종목명", i].Value = 종목명.Trim();
                        condigionItemDataGridView["조건종목_현재가", i].Value = String.Format("{0:###,#}", 현재가);
                        condigionItemDataGridView["조건종목_전일대비", i].Value = String.Format("{0:###,#}", 전일대비);
                        condigionItemDataGridView["조건종목_등락율", i].Value = String.Format("{0:F2}", 등락율) + "%";
                        condigionItemDataGridView["조건종목_거래량", i].Value = String.Format("{0:###,#}", 거래량);
                        condigionItemDataGridView["조건종목_거래량", i].Value = String.Format("{0:###,#}", 거래금);

                        //색깔
                        if (전일대비 > 0)
                        {
                            condigionItemDataGridView["조건종목_전일대비", i].Style.ForeColor = Color.Red;
                            condigionItemDataGridView["조건종목_등락율", i].Style.ForeColor = Color.Red;
                        }
                        else if (전일대비 < 0)
                        {
                            condigionItemDataGridView["조건종목_전일대비", i].Style.ForeColor = Color.Blue;
                            condigionItemDataGridView["조건종목_등락율", i].Style.ForeColor = Color.Blue;
                        }
                        else
                        {
                            condigionItemDataGridView["조건종목_전일대비", i].Style.ForeColor = Color.Black;
                            condigionItemDataGridView["조건종목_등락율", i].Style.ForeColor = Color.Black;
                        }

                        ConditionObject conditionObject = conditionList.Find(o => o.조건식번호 == 조건식번호);
                        if (conditionObject != null)
                        {
                            conditionObject.stockItemList.Add(new StockItemObject(종목코드, 종목명, 현재가, 전일대비, 등락율, 거래량,거래금));
                        }
                    }
                }
            }
         */
            /*
            else if (e.sRQName.Contains("편입종목정보요청"))
            {
                try
                {
                    if (e.sRQName.Split(';').Length == 2)
                    {
                        int 조건식번호 = int.Parse(e.sRQName.Split(';')[1]);
                        
                        ConditionObject conditionObject = conditionList.Find(o => o.조건식번호 == 조건식번호); 
                        
                        if (conditionObject != null) // 조건식에 항목이 있고
                        {                            
                            string 종목코드 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목코드").Trim();
                            string 종목명 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목명").Trim();
                            int 현재가 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "현재가"));
                            int 전일대비 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "전일대비"));
                            double 등락율 = double.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "등락율"));
                            long 거래량 = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "거래량"));

                            StockItemObject stockItem = new StockItemObject(종목코드, 종목명, 현재가, 전일대비, 등락율, 거래량);
                           
                            //-----------------  추가 사항 ( 리스트의 각 항목중 중첩되는 리스트의 값만 바꿔준다.  ( List class에만 적용)
                            // 이유 : 중첩으로 리스트가 계속만들어진다.
                            StockItemObject checkStockItem = conditionObject.stockItemList.Find(o => o.종목명 == 종목명);
                            if (checkStockItem != null) //조건식에 있고 -> 하위에 같은 종목명의 리스트가 있다면
                            {
                                //checkStockItem.종목코드 = 종목코드;
                                //checkStockItem.종목명 = 종목명;
                                checkStockItem.현재가 = 현재가;
                                checkStockItem.등락율 = 등락율;
                                checkStockItem.거래량 = 거래량;

                                insertListBox.Items.Add("[종목 중첩 편입] 조건명 : " + conditionObject.조건식명 + " 종목명 :" + 종목명);
                                insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동
                            }
                            else
                            {
                                conditionObject.stockItemList.Add(stockItem); //조건식 하위에 항목을 추가 한다.

                                insertListBox.Items.Add("[종목편입] 조건명 : " + conditionObject.조건식명 + " 종목명 :" + 종목명);
                                insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동

                                if (conditionDataGridView.SelectedCells.Count > 0) //조건식 셀이 선택됬다.
                                {
                                    int rowIndex = conditionDataGridView.SelectedCells[0].RowIndex;
                                    if (int.Parse(conditionDataGridView["조건식_조건번호", rowIndex].Value.ToString()) == 조건식번호)
                                    {
                                        condigionItemDataGridView.Rows.Insert(0);

                                        condigionItemDataGridView["조건종목_종목코드", 0].Value = 종목코드.Trim();
                                        condigionItemDataGridView["조건종목_종목명", 0].Value = 종목명.Trim();
                                        condigionItemDataGridView["조건종목_현재가", 0].Value = String.Format("{0:###,#}", 현재가);
                                        condigionItemDataGridView["조건종목_전일대비", 0].Value = String.Format("{0:###,#}", 전일대비);
                                        condigionItemDataGridView["조건종목_등락율", 0].Value = String.Format("{0:F2}", 등락율) + "%";
                                        condigionItemDataGridView["조건종목_거래량", 0].Value = String.Format("{0:###,#}", 거래량);
                                    }
                                }
                                if (isTrading)
                                {
                                    if (buyConditionCheckBox.Checked)
                                    {
                                        if (buyConditionComboBox.Text.Equals(conditionObject.조건식명))
                                        {
                                            //해당 종목코드 자동매수 요청.
                                            if (accountComboBox.Text.Length > 0)
                                            {
                                                int 총1회매수금액 = (int)totalAmountNumericUpDown.Value;

                                                if (총1회매수금액 > 0 && 현재가 != 0)
                                                {
                                                    int 매수량 = 총1회매수금액 / 현재가;
                                                    if (매수량 > 0)
                                                    {
                                                        //이미 매수한 같은 종목이 있는가 확인. -> balanceDataGridView
                                                        if (BalanceListCheck(종목코드.Trim()))
                                                        {
                                                            insertListBox.Items.Add("=보유중인종목= 조건식명 : " + conditionObject.조건식명 + " 종목명 :" + 종목명);
                                                            insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동
                                                        }
                                                        else
                                                        {
                                                            int orderResult = -1; //임의로 정했다.

                                                          //  lock (lockThis)
                                                          //  {
                                                          //      var t = Task.Run(async delegate
                                                           //     {
                                                                    orderResult = axKHOpenAPI1.SendOrder("주식매수요청", 화면번호_주식매수요청, accountComboBox.Text, 1, 종목코드.Trim(), 매수량, 0, "03", "");

                                                           //         await Task.Delay(250);
                                                             //       return 0;
                                                            //    });
                                                            //    t.Wait();
                                                              //  orderResult = t.Result;
                                                          //  }
                                                            Console.WriteLine("주식매수요청 orderResult : " + orderResult);

                                                            if (orderResult == 0)
                                                                insertListBox.Items.Add("[*주문요청 성공*] 조건식명 : " + conditionObject.조건식명 + " 종목명 :" + 종목명);
                                                            else
                                                                insertListBox.Items.Add("[*주문요청 실패*] 조건식명 : " + conditionObject.조건식명 + " 종목명 :" + 종목명);

                                                            insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // --------------   추가사항 끝                        
                         
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message.ToString());
                }
            }*/
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
                    Console.WriteLine(exeption.Message.ToString()+"1109");
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
                    Console.WriteLine(exception.Message.ToString()+"1129");
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

        bool BalanceListCheck(string kindName)
        {
            bool b = false;
            for (int i = 0; i < balanceDataGridView.RowCount; i++)
            {
                if (balanceDataGridView["잔고_종목코드", i].Value != null && balanceDataGridView["잔고_종목코드", i].Value.ToString().Trim().Equals(kindName))
                {
                    b = true;
                    break;
                }                
            }
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
           
            for (int i = 0; i < condigionItemDataGridView.RowCount; i++)
            {
                if (condigionItemDataGridView["조건종목_종목코드", i].Value.ToString().Trim() == 저장삭제코드.Trim())
                {
                    condigionItemDataGridView.Rows.RemoveAt(i);                    
                    break;
                }
            }
            
            if (deleteCode.Count > 0)
            {
                string 삭제 = deleteCode.Find(o => o.Trim() == 저장삭제코드.Trim());
                deleteCode.Remove(삭제);
            }
              // if (deleteCode.Find(o => o.Trim() == 저장삭제코드.Trim()) != null)//.RemoveAt(0);

            return DateTime.Now;
        }

        public DateTime DelayOrder(int MS, OrderClass orderClass)
        {
            //먼저 일하고 쉬었다 다음일 진행.
            
                int orderResult = axKHOpenAPI1.SendOrder(orderClass.주문형태, orderClass.화면번호, orderClass.계정,
                                                                            orderClass.타입, orderClass.종목코드, orderClass.보유수량,
                                                                            orderClass.가격, orderClass.호가구분, orderClass.원래주문번호);
            if (orderClass.주문형태 == "주문매도")
            {
                for (int i = 0; i < balanceDataGridView.RowCount; i++)
                {
                    if (balanceDataGridView["잔고_종목코드", i].Value.ToString().Equals(orderClass.종목코드))
                        balanceDataGridView.Rows.RemoveAt(i);
                }

                highestRate.Remove(orderClass.종목코드);
            }

                if (orderResult == 0)
                    insertListBox.Items.Add("[*" + orderClass.주문형태 + "=성공*]  종목명 :" + axKHOpenAPI1.GetMasterCodeName(orderClass.종목코드));
                else
                    insertListBox.Items.Add("[*" + orderClass.주문형태 + "=실패*]  종목명 :" + axKHOpenAPI1.GetMasterCodeName(orderClass.종목코드));
                            
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
        public string 종목코드;
        public string 종목명;
        public long 현재가;
        public long 전일대비;
        public double 등락율;
        public long 거래량;
        public long 거래금;
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
    /// 미체결 잔고 묶음.
    /// </summary>
    public class BalanceItem
    {
        public string 주문번호;
        public string 종목코드;
        public string 종목명;
        public int 주문수량;
        public int 주문가격;
        public int 미체결수량;
        public string 주문구분;
        public int 체결가;      
        public int 현재가;
        public string 시간;

        public BalanceItem(string 주문번호, string 종목코드, string 종목명, int 주문수량, int 주문가격,
            int 미체결수량, string 주문구분, int 체결가, int 현재가, string 시간)
        {
          this. 주문번호= 주문번호;
          this. 종목코드 = 종목코드;
          this.종목명 = 종목명;
          this.주문수량 = 주문수량;
          this.주문가격 = 주문가격;
          this.미체결수량 = 미체결수량;
          this.주문구분= 주문구분;
          this.체결가 = 체결가;         
          this. 현재가 = 현재가;
          this. 시간 = 시간;
        }
    }

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