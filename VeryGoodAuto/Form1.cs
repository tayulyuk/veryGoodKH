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
       // public int beforeConditionCode;

        /// <summary>
        /// 잔고와 미체결 관리를 한다.
        /// </summary>
        List<BalanceObject> balanceItemList;

        /// <summary>
        /// 미체결 리스트를 별도 관리 저장.
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

        //Thread autoViews;  CPU 부하를 줄이기 위해 test
      //  Thread startThead; // 무한 while 하면서 삭제할 종목이 있다면 계속 삭제하는 일만 한다.
       Thread orderClassManagerThread; // 매수와 매도가 orderClassList에 있다면 무조건 실행해라. 제한 넘지 않게.

        public List<string> deleteCode;// 삭제할 종목
        /// <summary>
        /// 매수와 매도를 함깨 동기화하여 시간제한 4회/1초 를 넘지 않게 관리한다.
        /// </summary>
        public List<OrderClass> orderClassList;
        /// <summary>
        /// 단순 컨디션 뷰 보여주기위한 목록,
        /// </summary>
        public List<ConditionViewObject> conditionViewList; 
        /// <summary>
        /// 실시간으로 받아오는 매수평균치 -> 매도시 언제라도 팔수 있는 량을 매수에 사용한다.
        /// </summary>
        private int 매수할종목평균수량;

        /// <summary>
        /// 매수 종목을 저장하고 사용후 삭제. [reaData  함수의 정보를 -> chaja함수 에서 사용할수 있도록 저장용도]
        /// </summary>
        List<BalanceObject> storeOrderList;

        /// <summary>
        /// 중첩 매도/매수 를 방지 관리 한다.
        /// 정보를 채잔 함수/ realData함수 /쓰래드/ 에서 값을 공유하기 위해.
        /// </summary>
        List<UseDeleteOrderClass> deletOrderManager;

        /// <summary>
        /// 1초/2회만 전송를 하기위해
        /// 시간지연후 다음 일이 있을경우를 알려주는 싸인.
        /// true : 다음 일이 있을경우    false: 다음 일이 없을 경우.
        /// </summary>
        private bool isSend = false;

        /// <summary>
        /// OrderClass 를 SendOrder 할때 받는 신호.
        /// </summary>
       // public event InfoEventHandler eventHandler;
        /// <summary>
        /// conditonView 항목 삭제  받는 신호.
        /// </summary>
      //  public event InfoEventHandler eventHandlerDeleteOrder;

        public Form1()
        {
            InitializeComponent();

            conditionList = new List<ConditionObject>();// 조건검색식에 따른 항목리스트

            conditionViewList = new List<ConditionViewObject>(); //실시간창에 리스트를 보여준다.
            itemInfoList = new List<ItemInfo>(); // 주식명 검색을 위한 리스트.
            balanceItemList = new List<BalanceObject>();// 잔고와 미체결 관리를 한다.
            outStandingList = new List<OutStanding>();
            storeOrderList = new List<BalanceObject>(); // 주문을 중복을 막기 위해.
            deletOrderManager = new List<UseDeleteOrderClass>();//중첩 매도를 방지 관리 한다.

            lockThis = new object(); // lock
            lockRemove = new object();// 삭제 종목명.
            

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

            //startThead = new Thread(new ThreadStart(UseDeleteMethod));
            // startThead.IsBackground = true;
            //startThead.Start();         

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
        /// <summary>
        /// SendOrder 작업의 새로운 Thread를 만든다.
        /// </summary>
        public void LetsSend3()
        {
            lock (lockThis)
            {
                try
                {
                    OrderClass oc = IsOrderClassListCheck();
                    if (oc != null)
                    {
                        Console.WriteLine("LetsSend :: =LetsSend 시작 =  orderClassList.Count ::" + orderClassList.Count);

                        DelayOrder(500, oc);

                        Console.WriteLine("LetsSend :: orderClassList  count   =LetsSend  끝=  작업끝난 수 2222 :;  " +
                                          orderClassList.Count);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message.ToString() + 201);
                }
            }
        }

        /// <summary>
        /// SendOrder 작업의 새로운 Thread를 만든다.
        /// </summary>
        public void LetsSend()
        {
            try
            {
                OrderClass oc = IsOrderClassListCheck();
                if (oc != null)
                {
                    //Console.WriteLine("LetsSend :: =LetsSend 시작 =  orderClassList.Count ::" + orderClassList.Count);

                    DelayOrder(500, oc);

                    // Console.WriteLine("LetsSend :: orderClassList  count   =LetsSend  끝=  작업끝난 수  :  " +
                    //                  orderClassList.Count);
                }
                else Console.WriteLine("LetsSend  order 없다.");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message.ToString() + 233);
            }
         
        }

        /// <summary>
        /// conditionView 항목에 일정시간 지연후 삭제하는 일.
        /// </summary>
        public void DeleteOrderCode(string 삭제대상)
        {
            try
            {
                DelayDeleteCode(500, 삭제대상);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message.ToString() + 250);
            }
        }

        private void DataGridView_SelectedBalanceDataGridView(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (sender.Equals(balanceDataGridView))
                {
                    string code = this.balanceDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
                    BalanceObject bi = balanceItemList.Find(o => o.종목코드 == code.Trim());
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
                        //Console.WriteLine("삭제 deleteCode  cout :: " + deleteCode.Count);
                        string 삭제대상 = deleteCode.First().Trim();
                        if (!삭제대상.Equals(String.Empty))
                        {
                            Thread t = new Thread(delegate()
                            {
                                this.Invoke(new Action(delegate() { Delay(1, 삭제대상); }));
                            });

                            t.Start();
                            t.Join();
                        }
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
                    try
                    {
                        OrderClass oc = IsOrderClassListCheck();
                        if (oc != null)
                        {
                            Console.WriteLine("T :: =쓰래드 시작 =  orderClassList.Count ::" + orderClassList.Count);

                            Thread t = new Thread(delegate()
                            {
                                Console.WriteLine("T :: 쓰레드 진입..");

                                this.Invoke(new Action(delegate() { DelayOrder(500, oc); }));

                                Console.WriteLine("T :: orderClassList  count   =쓰래드 끝=  작업끝난 수 2222 :;  " +
                                                  orderClassList.Count);
                            });
                            t.Start();
                            t.Join();
                        }
                    }
                    catch { }

                    //else
                       // Console.WriteLine("T :: order class null   -- Thread  251");
                }
            }
        }

        /// <summary>
        /// 처음 시도하는 ->deletOrderManager 에서
        /// 매도/매수 종목의 (첫번째 class)를 찾는다.
        /// </summary>
        /// <returns></returns>
        public OrderClass IsOrderClassListCheck()
        {
            try
            {
                OrderClass oc = null;

                for (int i = 0; i < orderClassList.Count; i++)
                {
                    UseDeleteOrderClass udoc = deletOrderManager.Find(o =>
                        o.종목코드 == orderClassList[i].종목코드 && o.거래형태 == orderClassList[i].주문형태);
                    if (udoc != null)
                        continue;

                    oc = orderClassList[i];
                    if (oc != null)
                    {
                        return oc;
                    }

                    break;
                }
            }
            catch { }
            return null;
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
           
            if (sender.Equals(buyButton))
            {
                if (계좌번호.Length > 0 && 종목코드.Length == 6 && 수량 > 0 && 가격 > 0 && 거래구분.Length == 2)
                {
                    orderClassList.Add(new OrderClass("매수주문", 주식주문, 계좌번호, 1, 종목코드, 수량, 가격, 거래구분, ""));
                    
                    // 전송작업 실행.
                    if(!isSend)
                        LetsSend();
                }
            }
            else if (sender.Equals(sellButton))
            {
                if (계좌번호.Length > 0 && 종목코드.Length == 6 && 수량 > 0 && 가격 > 0 && 거래구분.Length == 2)
                {
                    orderClassList.Add(new OrderClass("매도주문", 주식주문,
                                                                                            계좌번호, 2, 종목코드, 수량, 가격, 거래구분, ""));
                    // 전송작업 실행.
                    if (!isSend)
                        LetsSend();
                }
            }
            else if (sender.Equals(changeButton))
            {
                string 매매구분 = tradeOptionComboBox.Text; // 매매 필수 구분.
                string 주문번호 = originOrderNumtextBox.Text;
                                  
                if (매매구분.Equals("매수"))
                {
                    orderClassList.Add(new OrderClass("매수주문변경", 주식주문, 계좌번호, 5, 종목코드, 수량, 가격, 거래구분, 주문번호));
                    // 전송작업 실행.
                    if (!isSend)
                        LetsSend();

                }
                else if (매매구분.Equals("매도"))
                {
                    orderClassList.Add(new OrderClass("매도주문변경", 주식주문, 계좌번호, 6, 종목코드, 수량, 가격, 거래구분, 주문번호));
                    // 전송작업 실행.
                    if (!isSend)
                        LetsSend();
                }
            }
            else if (sender.Equals(cancelButton))
            {
                string 매매구분 = tradeOptionComboBox.Text; // 매매 필수 구분.
                string 주문번호 = originOrderNumtextBox.Text;
              
                if (매매구분.Equals("매수"))
                {
                    orderClassList.Add(new OrderClass("매수취소", 주식주문,계좌번호, 3, 종목코드, 수량, 가격, 거래구분, 주문번호));
                    // 전송작업 실행.
                    if (!isSend)
                        LetsSend();
                }
                else if (매매구분.Equals("매도"))
                {
                    orderClassList.Add(new OrderClass("매도취소", 주식주문, 계좌번호, 4, 종목코드, 수량, 가격, 거래구분, 주문번호));
                    // 전송작업 실행.
                    if (!isSend)
                        LetsSend();
                }
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

        private void AxKHOpenAPI1_OnReceiveRealData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            //종목코드,종목명,현재가10,전일대비11,등락율12,거래량15
            //string 종목코드 = e.sRealKey;     
            //예수금951/손익율8019/당일실현손익990/당일신현손익율991/

            if (!저장타입.Equals(e.sRealType))
            {
                저장타입 = e.sRealType;
                //   Console.WriteLine(e.sRealType);
            }

            if (e.sRealType.Equals("주식시세"))
            {
                try
                {
                    Console.WriteLine("주식시세 / 등락율 ->" + double.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 8019)));
                }
                catch
                {
                }
            }

            if (e.sRealType.Equals("주식종목정보"))
            {
                Console.WriteLine("주식종목정보 ->" + e.sRealData);
            }

            if (e.sRealType.Equals("잔고"))
            {
                //,매수총잔량125,실현손익990;손익율8019,예수금951,
                int 실현손익 = int.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 990));
                double 손익율 = double.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 8019));
                int 예수금 = int.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 951));
                Console.WriteLine("잔고 _실현손익 : " + 실현손익 + "/ 잔고 _손익율: " + 손익율 + "/ 잔고 _예수금: " + 예수금);
            }

            if (e.sRealType.Equals("주식호가잔량"))
            {
                int 매수총잔량 = int.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 125));
                매수할종목평균수량 = 매수총잔량 / 10; //언제든지 받아줄수 있는 가격만 산다. 이유.
                // Console.WriteLine("종목명 : " + axKHOpenAPI1.GetMasterCodeName(e.sRealKey) + "/ 매수총잔량" + 매수총잔량);
            }

            if (e.sRealType.Equals("주식체결"))
            {
                string 종목코드 = e.sRealKey.Replace("A", "").Trim(); // test 해보고 값도 출력해봐라.   매도에서 왜 안팔리지는지.확인해라. 그리고 삭제.
                if (!종목코드.Equals(String.Empty))
                {

                    string 종목명 = axKHOpenAPI1.GetMasterCodeName(e.sRealKey).Trim();
                    long 현재가1 = long.Parse(axKHOpenAPI1.GetCommRealData(e.sRealKey, 10).Trim());
                    long 전일대비 = long.Parse(axKHOpenAPI1.GetCommRealData(e.sRealKey, 11).Trim());
                    double 등락율 = double.Parse(axKHOpenAPI1.GetCommRealData(e.sRealKey, 12).Trim());
                    long 거래량 = long.Parse(axKHOpenAPI1.GetCommRealData(e.sRealKey, 13).Trim());
                    long 거래금 = long.Parse(axKHOpenAPI1.GetCommRealData(e.sRealKey, 14).Trim());
                    //long 보유수량 = 0; // 처음 감시 되면 무조건 보유수량은 없으니까.

                    if (현재가1 < 0)
                        현재가1 = -현재가1;

                    BalanceObject bo = null;
                    try
                    {
                        bo = balanceItemList.Find(o => o.종목코드.Equals(종목코드));
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message.ToString() + 420);
                    }

                    if (bo != null)
                    {
                        try
                        {
                            bo.현재가 = 현재가1;

                            long 평가금액 = bo.보유수량 * 현재가1; //  -(세금 + 수수료)

                            bo.손익금액 = 평가금액 - bo.매수금;

                            double 손익비율 = 100 * (bo.손익금액) / (double) bo.매수금;

                            bo.손익율 = double.Parse(손익비율.ToString("N2").Trim());

                            if (bo.손익율 > bo.최고율)
                                bo.최고율 = bo.손익율;

                            //  보여줌.
                            balanceDataGridView.DataSource = null;
                            balanceDataGridView.DataSource = balanceItemList;
                            balanceDataGridView.CurrentRow.Selected = false;

                            //잔고에 있는 종목은 conditionViewList 에는 없어야 한다.
                            //   ConditionViewObject cvo = conditionViewList.Find(o => o.종목코드 == 종목코드);
                            //   if (cvo != null)
                            //      conditionViewList.Remove(cvo);
                            //에러가 나네.. 일단 주석.
                            /*
                              if (isTrading) //시작버튼 활성시.
                              {
                                  if (isTrailingStopCheckBox.Checked)
                                  {
                                      if (2 < bo.최고율)
                                      {
      
                                          if (bo.손익율 < (bo.최고율 - 1))
                                          {
                                              if (!IsOrderClass(종목코드, "매도주문"))
                                              {
                                                  if (!IsDeleteOrderManage(종목코드, "매도주문")) //중복생성 방지.
                                                  {
                                                     // deletOrderManager.Add(new UseDeleteOrderClass(종목코드, "매도주문")); // 중복방지.
      
                                                      orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                          accountComboBox.Text, 2, 종목코드.Trim(), (int) bo.보유수량, 0, "03", ""));
      
                                                      // 전송작업 실행.
                                                      if (!isSend)
                                                          LetsSend();
      
                                                      insertListBox.SelectedIndex =
                                                          insertListBox.Items.Count - 1; // 첫 line으로 항상 이동
                                                  }
                                              }
                                          }
                                      }
                                      else if ((0.5 <= bo.최고율) && (  2 > bo.최고율))
                                      {
                                          if (bo.손익율 < (bo.최고율 - 0.5f))
                                          {
                                              if (!IsOrderClass(종목코드, "매도주문"))
                                              {
                                                  if (!IsDeleteOrderManage(종목코드, "매도주문"))// 중복 매도 방지.
                                                  {
                                                      //deletOrderManager.Add(new UseDeleteOrderClass(종목코드, "매도주문")); // 중복 매도 방지.
      
                                                      orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                          accountComboBox.Text, 2, 종목코드.Trim(), (int) bo.보유수량, 0, "03", ""));
      
                                                      // 전송작업 실행.
                                                      if (!isSend)
                                                          LetsSend();
      
                                                      insertListBox.SelectedIndex =
                                                          insertListBox.Items.Count - 1; // 첫 line으로 항상 이동
                                                  }
                                              }
                                          }
                                      }
                                  }
                                  //수익매도 처리
                                  if (takeProfitCheckBox.Checked)
                                  {
                                      double takeProfit = (double) takeProfitNumericUpDown.Value;
                                      if (bo.손익율 >= takeProfit)
                                      {
                                          if (!IsOrderClass(종목코드, "매도주문"))
                                          {
                                              if (!IsDeleteOrderManage(종목코드, "매도주문")) // 중복 매도 방지.
                                              {
                                                  //deletOrderManager.Add(new UseDeleteOrderClass(종목코드, "매도주문")); // 중복 매도 방지.
      
                                                  orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                      accountComboBox.Text, 2, 종목코드.Trim(), (int) bo.보유수량, 0, "03", ""));
      
                                                  // 전송작업 실행.
                                                  if (!isSend)
                                                      LetsSend();
      
                                                  insertListBox.SelectedIndex =
                                                      insertListBox.Items.Count - 1; // 첫 line으로 항상 이동
                                              }
                                          }
                                      }
                                  }
      
                                  //손절 처리
                                  if (stopLossChheckBox.Checked)
                                  {
                                      double stopLoss = (double) stopLossNumericUpDown.Value;
                                      if (bo.손익율 < stopLoss)
                                      {
                                          if (!IsOrderClass(종목코드, "매도주문"))
                                          {
                                              if (!IsDeleteOrderManage(종목코드, "매도주문")) // 중복 매도 방지.
                                              {
                                                  //deletOrderManager.Add(new UseDeleteOrderClass(종목코드, "매도주문")); // 중복 매도 방지.
      
                                                  orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                      accountComboBox.Text, 2, 종목코드.Trim(), (int) bo.보유수량, 0, "03", ""));
                                                  // 전송작업 실행.
                                                  if (!isSend)
                                                      LetsSend();
      
                                                  insertListBox.SelectedIndex =
                                                      insertListBox.Items.Count - 1; // 첫 line으로 항상 이동
                                              }
                                          }
                                      }
                                  }
                              }
                              */

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
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message.ToString() + "628");
                        }
                    }
                    else // 잔고에 없으면    컨디션뷰에 보여주기.
                    {
                        try
                        {
                            ConditionViewObject cvo = null;
                            //조건식에 맞는    리스트를 
                            // conditionGridView 에 보여준다.
                            try
                            {
                                cvo = conditionViewList.Find(o => o.종목코드 == 종목코드);
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception.Message.ToString() + 743);
                            }

                            if (cvo != null) // 입력되고 있는 조건식번호 입력  
                            {
                                cvo.현재가 = 현재가1;
                                cvo.거래금 = 거래금;
                                cvo.거래량 = 거래량;
                                cvo.등락율 = 등락율;
                                cvo.전일대비 = 전일대비;
                            }
                            else
                            {
                                // if(! deleteCode.Contains(종목코드))
                                conditionViewList.Add(new ConditionViewObject(종목코드, 종목명, 현재가1, 전일대비, 등락율, 거래량, 거래금));
                            }

                            conditionItemDataGridView.DataSource = null;
                            conditionItemDataGridView.DataSource = conditionViewList;
                            if (conditionItemDataGridView.RowCount > 0)
                                conditionItemDataGridView.CurrentRow.Selected = false;

                            //Color
                            for (int i = 0; i < conditionItemDataGridView.RowCount; i++)
                            {
                                // 전일대비  값 비교.
                                int val = int.Parse(conditionItemDataGridView.Rows[i].Cells[3].Value.ToString());

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
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message.ToString() + 566);
                        }


                        //매수  시작--
                        if (isTrading)
                        {
                            if (buyConditionCheckBox.Checked) // 체크?
                            {
                                if (buyConditionComboBox.Text.Equals(현재_석택된_조건식명.Trim())) //조건식명.
                                {
                                    //해당 종목코드 자동매수 요청.
                                    if (accountComboBox.Text.Length > 0)
                                    {
                                        int 예탁금 = int.Parse(예탁자산평가액Label.Text.Replace(",", "").Trim());

                                        int 총1회매수금액 = (int) totalAmountNumericUpDown.Value;

                                        if ((총1회매수금액 > 0) && (현재가1 > 0) && (예탁금 > 총1회매수금액) && 예탁금 > 0)
                                        {
                                            int 매수량 = 총1회매수금액 / (int) 현재가1;

                                            if (매수량 > 0)
                                            {
                                                //이미 매수한 같은 종목이 있는가 확인. -> balanceItemList
                                                if (BalanceListCheck(종목코드.Trim()))
                                                {
                                                    insertListBox.Items.Add(
                                                        "=이미보유종목= 조건식명 : " + 현재_석택된_조건식명 + " 종목명 :" + 종목명);
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        int 적정매수량 = 0;
                                                        if (isAdjustQuantityCheckBox.Checked)
                                                        {
                                                            if (예탁금 > 0)
                                                            {
                                                                int 예탁자산총매수량 = 예탁금 / (int) 현재가1;
                                                                적정매수량 = AdjustQuantity(매수할종목평균수량, 예탁자산총매수량);

                                                                if (!IsOrderClass(종목코드, "매수주문")) //오더 중복 하지않게
                                                                {
                                                                    if (!IsDeleteOrderManage(종목코드, "매수주문")) // 중복 [매수] 방지.
                                                                    {
                                                                        //deletOrderManager.Add(new UseDeleteOrderClass(종목코드, "매수주문")); // 중복 [매수] 방지.

                                                                        // 전송작업 실행.
                                                                        //if (!isSend)
                                                                        //{
                                                                            orderClassList.Add(new OrderClass("매수주문",
                                                                                화면번호_주식매수요청,
                                                                                accountComboBox.Text, 1, 종목코드.Trim(), 적정매수량,
                                                                                0, "03", ""));

                                                                            //LetsSend();
                                                                        //}
                                                                        //else
                                                                        //{
                                                                        //    orderClassList.Add(new OrderClass("매수주문",
                                                                        //        화면번호_주식매수요청,
                                                                        //        accountComboBox.Text, 1, 종목코드.Trim(), 적정매수량,
                                                                        //        0, "03", ""));
                                                                        //}
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (예탁금 > 0)
                                                            {
                                                                if (!IsOrderClass(종목코드, "매수주문")) //오더 중복 하지않게
                                                                {
                                                                    if (!IsDeleteOrderManage(종목코드, "매수주문")) // 중복 [매수] 방지.
                                                                    {
                                                                        deletOrderManager.Add(new UseDeleteOrderClass(종목코드, "매수주문")); // 중복 [매수] 방지.

                                                                        // 전송작업 실행.
                                                                        //if (!isSend)
                                                                        //{
                                                                        orderClassList.Add(new OrderClass("매수주문",
                                                                            화면번호_주식매수요청,
                                                                            accountComboBox.Text, 1, 종목코드.Trim(), 적정매수량,
                                                                            0, "03", ""));

                                                                        //    LetsSend();
                                                                        //}
                                                                        //else
                                                                        //{
                                                                        //    orderClassList.Add(new OrderClass("매수주문",
                                                                        //        화면번호_주식매수요청,
                                                                        //        accountComboBox.Text, 1, 종목코드.Trim(), 적정매수량,
                                                                        //        0, "03", ""));
                                                                        //}
                                                                    }
                                                                }
                                                            }
                                                        }


                                                        //order정보를 저장하고 채잔함수에서 사용하고 삭제한다.
                                                        long 매수금 = 현재가1 * 매수량;
                                                        long 평가금액 = 매수량 * 현재가1; //  -(세금 + 수수료)

                                                        long 손익금액 = 평가금액 - 매수금;

                                                        double 손익비율 = 100 * (손익금액) / (double) 매수금;
                                                        //Console.WriteLine("리시브 손익비율 : " + 손익비율);
                                                        double 손익율 = double.Parse(손익비율.ToString("N2"));
                                                        double 최고율 = 0;

                                                        if (손익율 > 0)
                                                            최고율 = 손익율;
                                                        else if (손익율 < 0)
                                                            최고율 = 0;

                                                        // 매수할경우 balanceItemView에 보여주기 위해.
                                                        //storeOrderList -> 저장하면  chajaData에서 "체결" 될경우만 호출하여 balanceGridView에 보여주고.
                                                        //storeOrderList에 중복 체크후  체잔Data에서  balanceList 에 입력하자.
                                                        if (!IsStoreOrderClass(종목코드))
                                                            storeOrderList.Add(new BalanceObject(종목코드, 평가금액, 종목명, 매수량,
                                                                현재가1,
                                                                손익금액, 손익율, 최고율, 매수금));

                                                        //balanceItemList -> 저장하면  무조건 (체결됬다 또는 체결됬다고 가정하고 임의로 입력한다)  balanceGridView에 보여주고.
                                                        // balanceItemList.Add(new BalanceObject(종목코드, 평가금액, 종목명, 매수량, 현재가1,
                                                        //      손익금액, 손익율, 최고율, 매수금));
                                                    }
                                                    catch (Exception exception)
                                                    {
                                                        Console.WriteLine(exception.Message.ToString() + 633);
                                                    }
                                                }

                                                insertListBox.SelectedIndex =
                                                    insertListBox.Items.Count - 1; // 첫 line으로 항상 이동
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
                }

                //---- 수정 시작.
                    //실시간 매도 검색후 매도.
                    if (isTrading) //시작버튼 활성시.
                    {
                        for (int i = 0; i < balanceItemList.Count; i++)
                        {
                            if (isTrailingStopCheckBox.Checked)
                            {
                                if (2 < balanceItemList[i].최고율)
                                {
                                    double 최고율 = balanceItemList[i].최고율;
                                    if (balanceItemList[i].손익율 < (최고율 - 1))
                                    {
                                        if (!IsOrderClass(종목코드, "매도주문"))
                                        {
                                            if (!IsDeleteOrderManage(종목코드, "매도주문")) //중복생성 방지.
                                            {
                                            // deletOrderManager.Add(new UseDeleteOrderClass(종목코드, "매도주문")); // 중복방지.

                                            orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                accountComboBox.Text, 2, 종목코드.Trim(), (int)balanceItemList[i].보유수량,
                                                0, "03", ""));

                                            // 전송작업 실행.
                                            //if (isSend)
                                            //{
                                            //    orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                            //        accountComboBox.Text, 2, 종목코드.Trim(), (int)balanceItemList[i].보유수량, 0,
                                            //        "03", ""));
                                            //}
                                            //else
                                            //{
                                            //    orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                            //        accountComboBox.Text, 2, 종목코드.Trim(), (int)balanceItemList[i].보유수량, 0,
                                            //        "03", ""));

                                            //    LetsSend();
                                            //}

                                            insertListBox.SelectedIndex =
                                                    insertListBox.Items.Count - 1; // 첫 line으로 항상 이동
                                            }
                                        }
                                    }
                                }
                                else if ((0.5 <= balanceItemList[i].최고율) && (2 > balanceItemList[i].최고율))
                                {
                                    if (balanceItemList[i].손익율 < (balanceItemList[i].최고율 - 0.5f))
                                    {
                                        if (!IsOrderClass(종목코드, "매도주문"))
                                        {
                                            if (!IsDeleteOrderManage(종목코드, "매도주문")) // 중복 매도 방지.
                                            {
                                            //deletOrderManager.Add(new UseDeleteOrderClass(종목코드, "매도주문")); // 중복 매도 방지.

                                            orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                accountComboBox.Text, 2, 종목코드.Trim(), (int)balanceItemList[i].보유수량,
                                                0, "03", ""));

                                            // 전송작업 실행.
                                            //if (isSend)
                                            //{
                                            //    orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                            //        accountComboBox.Text, 2, 종목코드.Trim(), (int)balanceItemList[i].보유수량, 0,
                                            //        "03", ""));
                                            //}
                                            //else
                                            //{
                                            //    orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                            //        accountComboBox.Text, 2, 종목코드.Trim(), (int)balanceItemList[i].보유수량, 0,
                                            //        "03", ""));

                                            //    LetsSend();
                                            //}

                                            insertListBox.SelectedIndex =
                                                    insertListBox.Items.Count - 1; // 첫 line으로 항상 이동
                                            }
                                        }
                                    }
                                }
                            }

                            //수익매도 처리
                            if (takeProfitCheckBox.Checked)
                            {
                                double takeProfit = (double) takeProfitNumericUpDown.Value;
                                if (balanceItemList[i].손익율 >= takeProfit)
                                {
                                    if (!IsOrderClass(종목코드, "매도주문"))
                                    {
                                        if (!IsDeleteOrderManage(종목코드, "매도주문")) // 중복 매도 방지.
                                        {
                                        //deletOrderManager.Add(new UseDeleteOrderClass(종목코드, "매도주문")); // 중복 매도 방지.
                                        Console.WriteLine("takeProfit : " + takeProfit + "현재 손익율 : " + balanceItemList[i].손익율);
                                        orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                                accountComboBox.Text, 2, 종목코드.Trim(), (int)balanceItemList[i].보유수량, 0,
                                                "03", ""));

                                        // 전송작업 실행.
                                        //if (isSend)
                                        //{
                                        //    orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                        //        accountComboBox.Text, 2, 종목코드.Trim(), (int)balanceItemList[i].보유수량, 0,
                                        //        "03", ""));
                                        //}
                                        //else
                                        //{
                                        //    orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                        //        accountComboBox.Text, 2, 종목코드.Trim(), (int)balanceItemList[i].보유수량, 0,
                                        //        "03", ""));

                                        //    LetsSend();
                                        //}

                                        insertListBox.SelectedIndex =
                                                insertListBox.Items.Count - 1; // 첫 line으로 항상 이동
                                        }
                                    }
                                }
                            }

                            //손절 처리
                            if (stopLossChheckBox.Checked)
                            {
                                double stopLoss = (double) stopLossNumericUpDown.Value;
                                if (balanceItemList[i].손익율 < stopLoss)
                                {
                                    if (!IsOrderClass(종목코드, "매도주문"))
                                    {
                                        if (!IsDeleteOrderManage(종목코드, "매도주문")) // 중복 매도 방지.
                                        {
                                            //deletOrderManager.Add(new UseDeleteOrderClass(종목코드, "매도주문")); // 중복 매도 방지.
                                            Console.WriteLine("stopLoss : " + stopLoss + " / 현재 손익율 : " + balanceItemList[i].손익율 + " /종목 : "+ axKHOpenAPI1.GetMasterCodeName(종목코드));
                                        orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                            accountComboBox.Text, 2, 종목코드.Trim(), (int)balanceItemList[i].보유수량, 0,
                                            "03", ""));
                                        // 전송작업 실행.
                                        // if (!isSend)
                                        //     LetsSend();

                                        // 전송작업 실행.
                                        //if (isSend)
                                        //{
                                        //    orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                        //    accountComboBox.Text, 2, 종목코드.Trim(), (int)balanceItemList[i].보유수량, 0,
                                        //    "03", ""));
                                        //}
                                        //else
                                        //{
                                        //    orderClassList.Add(new OrderClass("매도주문", 화면번호_주식매도요청,
                                        //    accountComboBox.Text, 2, 종목코드.Trim(), (int)balanceItemList[i].보유수량, 0,
                                        //    "03", ""));

                                        //    LetsSend();
                                        //}

                                        insertListBox.SelectedIndex =
                                                insertListBox.Items.Count - 1; // 첫 line으로 항상 이동
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //----- 수정 끝
               
            }

            itmeCountlabel.Text = "종목수: " + conditionItemDataGridView.RowCount.ToString().Trim();
            잔고label.Text = "항목수 : " + balanceDataGridView.RowCount.ToString().Trim();
        }

        /// <summary>
        /// Order Class에 중복되는 명령이 있는가 확인. ->orderClassList   에 있으면 true 반환.
        /// </summary>
        /// <param name="종목코드"></param>
        /// <param name="주문형태"></param>
        /// <returns></returns>
        bool IsOrderClass(string 종목코드,string 주문형태)
        {
            bool b = false;
            try
            {
                OrderClass oc = orderClassList.Find(o => o.종목코드 == 종목코드 && o.주문형태 == 주문형태);
                if (oc != null)
                    b = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message.ToString() +" IsOrderClass" + 699);
            }

            return b;
        }

        /// <summary>
        /// deletOrderManager 에 있으면 true 반환.
        /// OrderClass 처리된  매수:1주라도 매수 /매도: 완전 매도거래 됬을 경우 보조하기 위한.
        /// </summary>
        bool IsDeleteOrderManage(string 종목코드, string 주문형태)
        {
            bool b = false;
            try
            {
                // UseDeleteOrderClass  udoc = deletOrderManager.Find(o => o.종목코드 == 종목코드 && o.거래형태 == 주문형태);
                //if (udoc != null)
                //    b = true;
                UseDeleteOrderClass udoc = deletOrderManager.Find(o => o.종목코드 == 종목코드);
                if (udoc != null)
                    b = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message.ToString() + " IsDeleteOrderManage" + 764);
            }

            return b;
        }

        /// <summary>
        /// storeOrderList 에 중복되는 [ 잔고저장]이 있는가 확인. ->storeOrderList   에 있으면 true 반환.
        /// </summary>
        /// <param name="종목코드"></param>
        /// <returns></returns>
        bool IsStoreOrderClass(string 종목코드)
        {
            bool b = false;
            try
            {
                BalanceObject oc = storeOrderList.Find(o => o.종목코드 == 종목코드);
                if (oc != null)
                    b = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message.ToString() + " IsOrderClass" + 699);
            }

            return b;
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

        private void AxKHOpenAPI1_OnReceiveChejanData(object sender,AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            string 주문번호 = "";
            string 종목코드 = ""; 
            string 종목명 = "";
            int 주문수량 = 0;
            int 미체결수량 = 0;
            int 주문가격 = 0;
            int 현재가 = 0;
            string 체결가 = ""; 
            string 주문구분 = ""; 
            string 시간 = "";

            Console.WriteLine("sGubun : " + e.sGubun);
            Console.WriteLine("sFIdList : " + e.sFIdList);
            Console.WriteLine("nItemCnt : " + e.nItemCnt);

            if (e.sGubun.Equals("0"))//접수 or 체결
            {

                try
                {
                    //911=체결량,914=단위체결가,915=단위체결량
                  
                     주문번호 = axKHOpenAPI1.GetChejanData(9203).Trim();
                     종목코드 = axKHOpenAPI1.GetChejanData(9001).Replace("A", "").Trim();
                     종목명 = axKHOpenAPI1.GetChejanData(302).Trim();
                     주문수량 = int.Parse(axKHOpenAPI1.GetChejanData(900).Trim()) == 0? 0 : int.Parse(axKHOpenAPI1.GetChejanData(900).Trim());
                     미체결수량 = int.Parse(axKHOpenAPI1.GetChejanData(902).Trim());
                     주문가격 = int.Parse(axKHOpenAPI1.GetChejanData(901).Trim());
                     현재가 = int.Parse(axKHOpenAPI1.GetChejanData(10).Trim());
                     체결가 = axKHOpenAPI1.GetChejanData(910).Trim();
                     주문구분 = axKHOpenAPI1.GetChejanData(905).Trim();
                     시간 = axKHOpenAPI1.GetChejanData(908).Trim();
                    //원주문번호 = long.Parse(axKHOpenAPI1.GetChejanData(904));
                
                   // long 체결량 = long.Parse(axKHOpenAPI1.GetChejanData(911));
                  //  Console.WriteLine("실시간 리시브 체결량: " + 체결량);
                  
                    if (axKHOpenAPI1.GetChejanData(913).Trim() == "접수")
                    {
                        //Console.WriteLine("접수 구분  :  " + 주문구분);
                        //체결이 0이라도 보여줘야 한다. 미체결창에
                        OutStanding os = outStandingList.Find(o => o.종목코드 == 종목코드);
                        if (os == null)
                        {
                            outStandingList.Add(new OutStanding(주문번호,종목코드,종목명,주문수량,주문가격,미체결수량,주문구분,체결가,현재가,시간));
                        }

                        //쓰래드 처리한 명령을 삭제 해줘야 1회만 실행된다.
                        //접수구분을 변환시켜보자.
                        string 변환된주문 =  ChangeOrderString(주문구분);
                        OrderClass oc = orderClassList.Find(o => o.종목코드 == 종목코드 && o.주문형태 == 변환된주문);
                        if (oc != null)
                        {
                            orderClassList.Remove(oc);
                        }
                        Console.WriteLine("--------------------------");
                        Console.WriteLine("접수 --: " + 주문구분);
                        Console.WriteLine(" orderClassList 삭제 남은수  : " + orderClassList.Count);
                    }

                    if (axKHOpenAPI1.GetChejanData(913).Trim() == "체결")
                    {
                        Console.WriteLine("--------------------------");
                        
                        // 체결 됬다면   매수 일경우에는 등록시키면 된다.  - 쓰레드에서 잘되면 결과는 나올것이고 안되면 패스 될것이다.
                        if (주문구분.Equals("+매수")) //915= 단위체결량,903=계결누계금액
                        {
                            Console.WriteLine("+매수 --");

                            //쓰래드의 결과만 오는것이 아니라 체결 될 때도 값은 들어온다.

                            if (BalanceListCheck(종목코드))
                            {
                                //잔고창 -> 값만 변경 시키고.
                                BalanceObject bi = balanceItemList.Find(o => o.종목코드 == 종목코드);
                                if (bi != null)
                                {
                                    bi.보유수량 = 주문수량 - 미체결수량;
                                    Console.WriteLine("채잔 매수 보유수량 업데이트-> 주문수량 - 미체결수량 : " + bi.보유수량);
                                }

                                //미체결 창 -> 미체결수량 수량  변경.
                                OutStanding os1 = outStandingList.Find(o => o.종목코드 == bi.종목코드);
                                if (os1 != null)
                                {
                                    os1.미체결수량 = 미체결수량;  //미체결 수량 업데이트  
                                    Console.WriteLine("채잔 -매수 -미체결 - 업데이트 os1.미체결수량 :" + os1.미체결수량);
                                }
                            }
                            else // 없으면 
                            {
                                //매수에 대한 쓰래드 오더 삭제.
                                OrderClass oc = orderClassList.Find(o => o.종목코드 == 종목코드 && o.주문형태 == "매수주문");
                                if (oc != null)
                                {
                                    orderClassList.Remove(oc);
                                }
                                Console.WriteLine(" chajan +매수  orderClassList count :  " + orderClassList.Count);

                                //1주라도 거래가 되면 잔고에 입력.   
                                BalanceObject sbo = storeOrderList.Find(o => o.종목코드 == 종목코드);
                                if (sbo != null)
                                {
                                    if ( 미체결수량 != sbo.보유수량) //1주 라도 거래가 됬다면.
                                    {
                                        //보유수량에  매수시 매수량이 들어간다.
                                        sbo.보유수량 = sbo.보유수량 - 미체결수량;

                                        balanceItemList.Add(sbo); // 잔고에 기입한다.
                                        storeOrderList.Remove(sbo); //저장 오더 정보 삭제.(realData에서 넘어온 정보)
                                        
                                        //잔고창에 기입하여 [생성 목적]을 달성했으니  저장용 클래스는 삭제.
                                        UseDeleteOrderClass udoc = deletOrderManager.Find(o => o.종목코드 == 종목코드
                                                                                               && o.거래형태 == "매수주문");
                                        if (udoc != null)
                                            deletOrderManager.Remove(udoc);  // 중복 매수 방지의 [해지]. -쓰래드가 일할수 있도록 해지.
                                    }
                                }
                            }
                        }
                        else if (주문구분.Equals("-매도"))// 915= 단위체결량,902=회원사코드(거래원) ??
                        {
                            Console.WriteLine("--------------------------");
                            Console.WriteLine("-매도 --");

                            //매도에 의한 쓰래드 오더 삭제.
                            OrderClass oc = orderClassList.Find(o => o.종목코드 == 종목코드 && o.주문형태 == "매도주문");
                            if (oc != null)
                                orderClassList.Remove(oc); // 매도 명령 1번 만 실행한다.
                            Console.WriteLine(" chajan -매도  orderClassList count 전 :  " + orderClassList.Count);//후  매도에 대한 명령 오더는 1회 로 제거 됬고.

                            //orderClassList 리스트는 1번 명령(쓰래드처리)하면  매도가 걸려 있는 상태기 때문에
                            //매도가 1주가 아니더라도  0주라도 실행되야 하되   
                            //balanceList 리스트에서는 미체결량이 0이 되야 삭제한다.

                            BalanceObject bo = balanceItemList.Find(o => o.종목코드 == 종목코드);
                            if (bo != null)
                            {
                                // 미체결수량을 [업로드] 해준다.  // 보유수량을 직접 넣줘서 test해보자.
                                bo.보유수량 = 미체결수량;

                                Console.WriteLine("bo.보유수량 :" + 미체결수량);
                            }

                            Console.WriteLine(" chajan -매도  orderClassList count 후 :  " + orderClassList.Count);

                        }

                        balanceDataGridView.DataSource = null;
                        balanceDataGridView.DataSource = balanceItemList;
                        if (balanceDataGridView.RowCount > 0)
                            balanceDataGridView.CurrentRow.Selected = false;

                        //Console.WriteLine("체결  항목"); //[ 미체결 항목 ]에 실시간으로 보여준다. //순환 사이클.
                        OutStanding os = outStandingList.Find(o => o.종목코드 == 종목코드);
                        if (os != null)
                        {
                            os.주문수량 = 주문수량;
                            os.미체결수량 = 미체결수량;
                            os.현재가 = 현재가;
                            os.체결가 = 체결가;
                            os.시간 = 시간;
                            os.주문구분 = 주문구분;
                        }
                        else
                        {
                            //매도가 되면 무조건 생성한다?    주문 미체결 수량이 0 보다 크면 ( 1번만 생성된다)  
                            //미체결수량이 0이라면 완료 됬다고 봐야된다.
                            if(미체결수량 > 0)
                                outStandingList.Add(new OutStanding(주문번호, 종목코드, 종목명, 주문수량, 주문가격,
                                미체결수량, 주문구분, 체결가, 현재가, 시간));
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message.ToString() + "935");
                }

            }
            else if (e.sGubun.Equals("1"))//잔고
            {
                Console.WriteLine("--------------------------");
                Console.WriteLine("잔고 --");
                try
                {
                     long 예수금 = long.Parse(axKHOpenAPI1.GetChejanData(951));//test
                    double 손익율 = double.Parse(axKHOpenAPI1.GetChejanData(8019));//test

                     Console.WriteLine("예수금 : "  + 예수금);
                    Console.WriteLine("손익율 : " + 손익율.ToString("N2"));

                    //930=보유수량 ,933=주문가능수량   .GetMarketType??
                    string 잔고종목명 = axKHOpenAPI1.GetChejanData(302).Trim();
                    Console.WriteLine("잔고종목명 : " + 잔고종목명);
                    long currentPrice = long.Parse(axKHOpenAPI1.GetChejanData(10).Replace("-", "").Trim());
                    Console.WriteLine("currentPrice : " + currentPrice);
                    string 당일손익율 = axKHOpenAPI1.GetChejanData(8019);
                    Console.WriteLine("당일손익율 : " + 당일손익율);
                    // string 당일실현손익률 = axKHOpenAPI1.GetChejanData(991);
                    long 총매입금액 = long.Parse(axKHOpenAPI1.GetChejanData(932));
                    Console.WriteLine("총매입금액 : " + 총매입금액);
                    long 당일총매도손일 = long.Parse(axKHOpenAPI1.GetChejanData(950));
                    Console.WriteLine("당일총매도손일 : " + 당일총매도손일);

                    long 당일실현손익 = long.Parse(axKHOpenAPI1.GetChejanData(990));
                    Console.WriteLine("당일실현손익 : " + 당일실현손익);
                    long 주문가능수량 = long.Parse(axKHOpenAPI1.GetChejanData(933));//test
                    long 보유수량 = long.Parse(axKHOpenAPI1.GetChejanData(930));//test

                  
                    //?? 보유수량 - 보유수량 - 주문가능수량???? 꼭 확인.!
                    //---------------------------------

                    //매도에 대한.
                    BalanceObject bo = balanceItemList.Find(o => o.종목명 == 잔고종목명);
                    if (bo != null)
                    {
                        if (bo.보유수량 == 0)
                        {
                            UseDeleteOrderClass udoc = deletOrderManager.Find(o => o.종목코드 == bo.종목코드);
                            if (udoc != null)
                            {
                                if (udoc.거래형태 == "매도주문")
                                    deletOrderManager.Remove(udoc); // 중복 매도 방지의 해지.
                                else if (udoc.거래형태 == "매수주문")
                                {
                                    Console.WriteLine("매수 주문으로 아무것도 안함");
                                }
                                else
                                    Console.WriteLine("거래형태 : " + udoc.거래형태);
                            }
                            else
                                Console.WriteLine("잔고 -- 이미 deletOrderManager 에서 삭제되었다");

                            balanceItemList.Remove(bo); //잔고 목록에서 빼주고.

                            balanceDataGridView.DataSource = null;
                            balanceDataGridView.DataSource = balanceItemList;
                            if (balanceDataGridView.RowCount > 0)
                                if (balanceDataGridView.CurrentRow != null)
                                   balanceDataGridView.CurrentRow.Selected = false;

                            //UseDeleteOrderClass udoc = deletOrderManager.Find(o => o.종목코드 == 종목코드 && o.거래형태 == "매도주문");
                            //if (udoc != null)
                            //    deletOrderManager.Remove(udoc);  // 중복 매도 방지의 해지.
                            //else
                            //    Console.WriteLine("잔고 -- 이미 deletOrderManager 중복에서 삭제되었다");

                           
                        }

                        // 매수
                        UseDeleteOrderClass udocr = deletOrderManager.Find(o => o.종목코드 == bo.종목코드 && o.거래형태 == "매수주문");
                        if (udocr != null)
                        {
                           // if (bo.보유수량 == 주문수량)
                                deletOrderManager.Remove(udocr); // 중복 매도 방지의 해지.

                            Console.WriteLine("매수 주문 .. ");
                        }
                    }
                    else
                    {
                        Console.WriteLine("????????");
                        //UseDeleteOrderClass udoc = deletOrderManager.Find(o => o.종목코드 == 종목코드);
                        //if (udoc != null)
                        //{
                        //    if (udoc.거래형태 == "매도주문")
                        //        deletOrderManager.Remove(udoc); // 중복 매도 방지의 해지.
                        //    else if (udoc.거래형태 == "매수주문")
                        //    {
                        //        Console.WriteLine("매수 주문으로 아무것도 안함");
                        //    }
                        //    else
                        //        Console.WriteLine("거래형태 : " + udoc.거래형태);
                        //}

                    }
                    /*
                    //매수시 ?
                    UseDeleteOrderClass udoc1 = deletOrderManager.Find(o => o.종목코드 == 종목코드 && o.거래형태 == "매수주문");
                    if (udoc1 != null)
                    {
                        deletOrderManager.Remove(udoc1);  // 중복 매수 방지의 [해지]. -쓰래드가 일할수 있도록 해지.

                        if (!BalanceListCheck(종목코드)) //balanceItemList 없을경우 1번만 입력할수 있도록 
                        {
                            BalanceObject sbo = storeOrderList.Find(o => o.종목코드 == udoc1.종목코드 );
                            if (sbo != null )
                            {
                                if (sbo.보유수량 != 보유수량) //sbo.보유수량 : 매수량 이 입력되있다.  보유수량에 업데이트가 안됬기때문에 같지 않다.(처음 1회만 동작)
                                {
                                    balanceItemList.Add(sbo);

                                    storeOrderList.Remove(sbo); // 저장용도로 사용하고 삭제 한다.
                                }
                            }
                        }
                    }
                    */
                    //----------------------------------------

                    if (outStandingList.Count > 0)  // outstanding 제거.
                    {
                        OutStanding os = null;
                        try
                        {
                            os = outStandingList.Find(o => o.미체결수량 == 0);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message.ToString() + 855);
                        }

                        if (os != null)
                        {
                            outStandingList.Remove(os);
                        }

                        outStandingDataGridView.DataSource = null;
                        outStandingDataGridView.DataSource = outStandingList;
                        if (outStandingDataGridView.RowCount > 0)
                            outStandingDataGridView.CurrentRow.Selected = false;
                    }

                    //axKHOpenAPI.SetRealReg  함수 호출 , "9001;10", "1");

                    Console.WriteLine("당일손익율 : " + 당일손익율);
                    Console.WriteLine("당일투자손익 : " + 당일총매도손일);

                    Console.WriteLine("종목명 : " + 잔고종목명 + " | 현재 종가 : " + currentPrice);
                    Console.WriteLine("보유수량 : " + 보유수량);
                    Console.WriteLine("주문가능수량 : " + 주문가능수량);
                    Console.WriteLine("매입주문금액 : " + 총매입금액 + " | 금일 실현손익 : " + 당일실현손익);
                    realizationProfitLabel.Text = 당일총매도손일 == 0? "0" : String.Format("{0:#,###}", 당일총매도손일);
                    당일손익율Label.Text = 당일손익율;
                    //당일손익율Label.Text = 당일실현손익률;
                    Console.WriteLine("----------------------------------------------------------");
                    //TODO 일단 주석처리
                    //RequestAccountEstimation(); //잔고 요청 함수. //예수금,총매입금액,예탁자산평가액,당일투자손익,당일손익율
                    //RequestOutStanding(); //미채결 요청 함수. -- ?? 필요 없다   실시간으로 남은수치를 보여주니까. 처음1회 만보여주고 나머진 x
                    // RequestRealizationProfit(); //일자별 수익율.-단순한 손익만 보여주는 것으로 처음1회만 보여주고 후엔 실시간 로딩.
                    Console.WriteLine("--체잔-------------deletOrderManager list---------------------");
                    for (int i = 0; i < deletOrderManager.Count; i++)
                    {
                        Console.WriteLine("종목명 : " +axKHOpenAPI1.GetMasterCodeName( deletOrderManager[i].종목코드) + " 거래형태 : " + deletOrderManager[i].거래형태);
                    }
                    Console.WriteLine("--체잔-------------남은 Order list ---------------------");
                    for (int i = 0; i < orderClassList.Count; i++)
                    {
                        Console.WriteLine("종목명 : " + axKHOpenAPI1.GetMasterCodeName(orderClassList[i].종목코드) );
                    }
                    Console.WriteLine("-- ---------------------");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message.ToString()+ 984);
                }
            }
        }

        private string ChangeOrderString(string 주문구분)
        {
            string order = "";
            if (주문구분.Equals("+매수"))
                order = "매수주문";
            else if (주문구분.Equals("-매도"))
                order = "매도주문";
            return order;
        }

        private void AxKHOpenAPI1_OnReceiveRealCondition(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealConditionEvent e)
        {
            try
            {
                string 종목코드 = e.sTrCode;
                int 조건식번호 = int.Parse(e.strConditionIndex);
               // string 조건식명 = e.strConditionName;
               // string 종목타입 = e.strType;
                string 종목명 = axKHOpenAPI1.GetMasterCodeName(e.sTrCode);

                if (e.strType.Equals("I"))//종목 편입
                {                    
                    ConditionObject conditionObject = conditionList.Find(o => o.조건식번호 == 조건식번호);
                    if (conditionObject != null) // 입력되고 있는 조건식번호 입력  
                        currentConditionCode = 조건식번호; // "D"항목에서 공용으로 사용해서 윗것 다시 만듬.

                    string 스크린번호 = (실시간조건검색수신화면번호 + 조건식번호).ToString().Trim();
                    //종목코드와 종목명을  List에 없을경우 올려라 

                    string stockName = axKHOpenAPI1.GetMasterCodeName(e.sTrCode);
                    insertListBox.Items.Add("------- [+편입] : " + e.strConditionIndex + " 코드 :  " + e.sTrCode + " :  " + stockName);
                    //현재가10,전일대비11,등락율12,누적거래량13,누적거래대금14,매수총잔량125,실현손익990;손익율8019,예수금951,매수호가총잔량125
                  
                    //axKHOpenAPI1.SetRealReg(스크린번호.Trim(), 종목코드, "10;11;12;13;14", "1");       
                    axKHOpenAPI1.SetRealReg(현재_스크린번호, 종목코드, "10;11;12;15;951;8019;990;991;125", "1"); //매수호가 총잔량 추가 /적정매수량 계산시필요
                  
                    //conditionViewList.Add(new ConditionViewObject(종목코드,종목명,0,0,0,0,0));
                    insertListBox.SelectedIndex = insertListBox.Items.Count - 1;// 첫 line으로 항상 이동  
                }
                else if (e.strType.Equals("D"))//종목 이탈
                {
                    
                        ConditionObject conditionObject = conditionList.Find(o => o.조건식번호 == 조건식번호);
                        if (conditionObject != null) // 입력되고 있는 조건식번호 입력
                            currentConditionCode = 조건식번호;

                        string 스크린번호 = (실시간조건검색수신화면번호 + 조건식번호).ToString().Trim();
                        deleteListBox.Items.Add("------ [-이탈]  : " + e.strConditionIndex + "  종목명 : " + 종목명);
                        deleteListBox.SelectedIndex = deleteListBox.Items.Count - 1; // 첫 line으로 항상 이동

                        삭제코드 = e.sTrCode.Trim();

                        //종목리스트에서 더이상 나머지 data가 나오지 못하게 차단한다.           
                        axKHOpenAPI1.SetRealRemove(스크린번호.Trim(), e.sTrCode); // 실시간 시세해지                   

                    // ConditionViewObject cvo = conditionViewList.Find(o => o.종목코드 == 종목코드);
                    //  if(cvo != null)
                    //     conditionViewList.Remove(cvo);
                    try
                    {
                        //deleteCode.Add(삭제코드); // 단순히 삭제코드만 입력한다. <수정 해봤음. test 해보고 삭제>
                            // 삭제 작업을 한다
                        DeleteOrderCode(삭제코드); // 
                        
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message.ToString() +1372);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message.ToString()+"1028");
            }         
        }

       

        private void AxKHOpenAPI1_OnReceiveTrCondition(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrConditionEvent e)
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
                            // string 조건명 = "";
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
                                if(balanceItemList.Count == 0)
                                    RequestAccountEstimation();
                                //미체결 요청.
                                 RequestOutStanding();
                                //일자별 수익율.
                                 RequestRealizationProfit();
                                // axKHOpenAPI1.SetRealReg(현재_스크린번호,"990","990",)
                                int result = axKHOpenAPI1.SendCondition(화면번호_조건검색, 현재_석택된_조건식명, 현재_선택된_조건식번호, 1);

                                if (result == 1) //성공
                                {
                                    Console.WriteLine("조건검색 성공");
                                }
                                else
                                {
                                    Console.WriteLine("조건검색 실패");
                                    MessageBox.Show("1분 후 다시 검색해주세요 \n 검색 실패");
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

                try
                {
                    balanceItemList.Clear();// 초기화.

                    예수금Label.Text = String.Format("{0:###,#}", 예수금);
                    총매입금액Label1.Text = ((총매입금액==0) ? "0" : String.Format("{0:###,#}", 총매입금액));
                    예탁자산평가액Label.Text = String.Format("{0:###,#}", 예탁자산평가액);
                    당일손익금액Label.Text = ((당일투자손익 == 0) ? "0" : String.Format("{0:###,#}", 당일투자손익));
                    당일손익율Label.Text = ((당일손익율 == 0) ? "0" : String.Format("{0:F2}", 당일손익율) + " %");
                   // Console.WriteLine("예수금 : " + 예수금);
                    //Console.WriteLine("총매입금액 : " + 총매입금액);
                  //  Console.WriteLine("예탁자산평가액 : " + 예탁자산평가액);
                   // Console.WriteLine("당일투자손익 : " + 당일투자손익);
                    //Console.WriteLine("당일손익율 : " + 당일손익율);

                    //잔고 datalist 추가
                    int n = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);
                    Console.WriteLine("N : " + n);
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

                        if (현재가 < 0)
                            현재가 = -현재가;

                        double 최고율 = 0;
                        if (손익율 > 0)
                            최고율 = 손익율;
                        else 
                            최고율 = 0;

                        if(최고율 > 0)
                            최고율 = double.Parse(최고율.ToString("N2"));

                        double 손익비율 = double.Parse(손익율.ToString("N2"));
                        balanceItemList.Add(new BalanceObject(종목코드, 평가금액, 종목명, 보유수량, 현재가, 손익금액, 
                            손익비율, 최고율, 매수금));
                  
                        //예수금951/손익율8019/당일실현손익990/당일신현손익율991/
                        // axKHOpenAPI1.SetRealReg(화면번호_실시간데이터요청, 종목코드, "10;11;12;15", "1"); //실시간 잔고 요청.      
                        axKHOpenAPI1.SetRealReg(화면번호_실시간데이터요청, 종목코드, "10;11;12;15;951;8019;990;991","1"); //실시간 잔고 요청.     
                    }
                    
                    if (n > 0)
                    {
                        balanceDataGridView.DataSource = null;
                        balanceDataGridView.DataSource = balanceItemList;
                        balanceDataGridView.CurrentRow.Selected = false;
                    }

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

                    if (현재가 < 0)
                        현재가 = -현재가;

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
                    if(outStandingDataGridView.RowCount > 0)
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

                        if (현재가 < 0)
                            현재가 = -현재가;

                        conditionViewList.Add(new ConditionViewObject(종목코드,종목명,현재가,전일대비,등락율,거래량,거래금));
                    }
                    //Console.WriteLine("리스트 : "+ conditionViewList.Count);
                    conditionItemDataGridView.DataSource = conditionViewList;
                    conditionItemDataGridView.CurrentRow.Selected = false;

                    for(int i =0; i < conditionItemDataGridView.RowCount;i++)
                    {
                      // 전일대비  값 비교.
                        int val = int.Parse(conditionItemDataGridView.Rows[i].Cells[3].Value.ToString());
                      
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

        /// <summary>
        /// balanceItemList  의 항목을 검사하고 있으면 true 반환.
        /// </summary>
        /// <param name="kindCode"></param>
        /// <returns></returns>
        bool BalanceListCheck(string kindCode)
        {
            bool b = false;
            BalanceObject bi = null;
            try
            {
                 bi = balanceItemList.Find(o => o.종목코드 == kindCode);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message.ToString() + 1616 + " BalanceListCheck method ");
            }

            if (bi != null)
                b = true;

            return b;
        }

        /// <summary>
        /// 지연시간 후 conditionViewList에서 삭제한다.
        /// </summary>
        /// <param name="MS">지연시간</param>
        /// <param name="삭제코드">삭제 할 코드</param>
        /// <returns></returns>
        public DateTime DelayDeleteCode(int MS, string 삭제코드)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            try
            {
                ConditionViewObject sio = conditionViewList.Find(o => o.종목코드 == 삭제코드);
                if (sio != null)
                {
                    conditionViewList.Remove(sio);
                    // real data 에서 보여주는 이유로 리스트에서 삭제만 해봤다.

                    //삭제와 상태를 보여주기.
                    // conditionItemDataGridView.DataSource = null;
                    // conditionItemDataGridView.DataSource = conditionViewList;
                    // if (conditionItemDataGridView.RowCount > 0)
                    //     conditionItemDataGridView.CurrentRow.Selected = false;
                }
                else
                {
                    Console.WriteLine("삭제코드가 없습니다. 종목명 : " + axKHOpenAPI1.GetMasterCodeName(삭제코드));
                }
            }catch(Exception exception)
            {
                Console.WriteLine(exception.Message.ToString() + "2193");
            }
            return DateTime.Now;
        }

        public   DateTime Delay(int MS,string 삭제코드)
        {
            
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
               string 삭제 = deleteCode.Find(o => o.Trim() == 삭제코드.Trim());
                deleteCode.Remove(삭제);

                ConditionViewObject sio = conditionViewList.Find(o => o.종목코드 == 삭제코드);
                if (sio != null)
                {
                    conditionViewList.Remove(sio);
                    //삭제와 상태를 보여주기.
                    conditionItemDataGridView.DataSource = null;
                    conditionItemDataGridView.DataSource = conditionViewList;
                    if(conditionItemDataGridView.RowCount > 0)
                        conditionItemDataGridView.CurrentRow.Selected = false;
                }

                 // Console.WriteLine("삭제 후 남음 코드 수 :: " + deleteCode.Count);
            }
            return DateTime.Now;
        }

        public DateTime DelayOrder111(int MS, OrderClass orderClass)
        {

            BalanceObject bi = null;
            //잔고창에 목록이 있는것은 재매수 하지 않는다.
            try
            {
                bi = balanceItemList.Find(o => o.종목코드 == orderClass.종목코드);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message.ToString() + 1376);
            }

            if ((bi == null && orderClass.주문형태 == "매수주문") || (bi != null && orderClass.주문형태 == "매도주문" ) ||
                (bi == null && orderClass.주문형태 == "매수주문변경") || (bi == null && orderClass.주문형태 == "매도주문변경") ||
                (bi == null && orderClass.주문형태 == "매수취소") || (bi == null && orderClass.주문형태 == "매도취소") )

            {
                Console.WriteLine("LetsSend :: 종목명: " + axKHOpenAPI1.GetMasterCodeName(orderClass.종목코드)+"/주문 형태  : " + orderClass.주문형태);
                //먼저 일하고 쉬었다 다음일 진행.
                int orderResult = axKHOpenAPI1.SendOrder(orderClass.주문형태, orderClass.화면번호, orderClass.계정,
                    orderClass.타입, orderClass.종목코드, orderClass.보유수량,
                    orderClass.가격, orderClass.호가구분, orderClass.원래주문번호);

                if (orderResult == 0)
                {
                    deletOrderManager.Add(new UseDeleteOrderClass(orderClass.종목코드, orderClass.주문형태)); // 중복방지.
                    RequestAccountEstimation();// 잔고 보여주기.
                    insertListBox.Items.Add("+ [*" + orderClass.주문형태 + "=성공*]  종목명 :" +
                                            axKHOpenAPI1.GetMasterCodeName(orderClass.종목코드));
                }
                else
                    insertListBox.Items.Add("- [*" + orderClass.주문형태 + "=실패*]  종목명 :" +
                                            axKHOpenAPI1.GetMasterCodeName(orderClass.종목코드));

                Console.WriteLine("LetsSend :: orderClassList  count :  쓰래드 (DelayOrder 함수 안에서) 작업끝난 수 :;  " + orderClassList.Count);
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

        public DateTime DelayOrder(int MS, OrderClass orderClass)
        {

            BalanceObject bi = null;
            //잔고창에 목록이 있는것은 재매수 하지 않는다.
            try
            {
                bi = balanceItemList.Find(o => o.종목코드 == orderClass.종목코드);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message.ToString() + 2289);
            }

            if ((bi == null && orderClass.주문형태 == "매수주문") || (bi != null && orderClass.주문형태 == "매도주문") ||
                (bi != null && orderClass.주문형태 == "매수주문변경") || (bi != null && orderClass.주문형태 == "매도주문변경") ||
                (bi != null && orderClass.주문형태 == "매수취소") || (bi != null && orderClass.주문형태 == "매도취소"))

            {
                Console.WriteLine("LetsSend :: 종목명: " + axKHOpenAPI1.GetMasterCodeName(orderClass.종목코드) + "/주문 형태  : " + orderClass.주문형태);
                //먼저 일하고 쉬었다 다음일 진행.
                int orderResult = axKHOpenAPI1.SendOrder(orderClass.주문형태, orderClass.화면번호, orderClass.계정,
                    orderClass.타입, orderClass.종목코드, orderClass.보유수량,
                    orderClass.가격, orderClass.호가구분, orderClass.원래주문번호);

                if (orderResult == 0)
                {
                    deletOrderManager.Add(new UseDeleteOrderClass(orderClass.종목코드, orderClass.주문형태)); // 중복방지.
                    RequestAccountEstimation();// 잔고 보여주기.
                    insertListBox.Items.Add("+ [*" + orderClass.주문형태 + "=성공*]  종목명 :" +
                                            axKHOpenAPI1.GetMasterCodeName(orderClass.종목코드));
                }
                else
                    insertListBox.Items.Add("- [*" + orderClass.주문형태 + "=실패*]  종목명 :" +
                                            axKHOpenAPI1.GetMasterCodeName(orderClass.종목코드));

                Console.WriteLine("LetsSend :: orderClassList  count :  쓰래드 (DelayOrder 함수 안에서) 작업끝난 수 :;  " + orderClassList.Count);

                //isSend = true;
               
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

            //OrderClass oc = IsOrderClassListCheck();
            //if (oc != null)
            //{
            //    LetsSend();
            //}
            //else
            //    isSend = false;

            Console.WriteLine("isSend :  " + isSend);
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
        public List<ConditionViewObject> stockItemList;

        public ConditionObject(int 조건식번호, string 조건식명)
        {
            this.조건식번호 = 조건식번호;
            this.조건식명 = 조건식명;
            stockItemList = new List<ConditionViewObject>();
        }
    }
    /// <summary>
    /// 조건식별 포함되는 종목들의 리스트 묶음
    /// </summary>
    public class ConditionViewObject
    {
        //종목코드,종목명,현재가,전일대비,등락율,거래량
        public string 종목코드 { get; set; }
        public string 종목명 { get; set; }
        public long 현재가 { get; set; }
        public long 전일대비 { get; set; }
        public double 등락율 { get; set; }
        public long 거래량 { get; set; }
        public long 거래금 { get; set; }
        public ConditionViewObject(string 종목코드, string 종목명, long 현재가, long 전일대비, double 등락율, long 거래량,long 거래금)
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
    public class BalanceObject
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

        public BalanceObject( string 종목코드, long 평가금액, string 종목명, long 보유수량,
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
    /// 매수/매도 관리 [쓰레드에서 저어하기 위한]
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

    /// <summary>
    /// 중첩 매도/매수 를 막기위해 (1번만 실행하기 위해 )
    /// 매도시 접수;매수시 접수; 체결   될수 있으므로 (미체결이 될경우 banlanceList에서 삭제 방지 위해)
    /// </summary>
    public class UseDeleteOrderClass
    {
        public string 종목코드;
        public string 거래형태;

        public UseDeleteOrderClass(string 종목코드,string 거래형태)
        {
            this.종목코드 = 종목코드;
            this.거래형태 = 거래형태;
        }
    }
}